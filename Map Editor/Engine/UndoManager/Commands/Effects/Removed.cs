using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.Effects
{
    /// <summary>
    /// Removed class.
    /// </summary>
    public class Removed : Effect, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public IFO.Effect Entry { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            Object.Entry.Parent.Effects.Add(Entry);
            MapManager.Effects.Add(Entry);

            int objectID = MapManager.Effects.WorldObjects.Count - 1;
            Object = MapManager.Effects.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Effects.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Effects.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Effects.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Effects.Removed)) && ((Commands.Effects.Effect)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.Effects.Effect)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.Effects.Effect)UndoManager.Commands[i]).Object = MapManager.Effects.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Effects && MapManager.Effects.Tool.SelectedObject == ObjectID)
                MapManager.Effects.Tool.Remove(true);
            else
                MapManager.Effects.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Effect - Removed";
        }
    }
}