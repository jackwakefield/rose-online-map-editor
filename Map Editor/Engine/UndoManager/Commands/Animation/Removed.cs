using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.Animation
{
    /// <summary>
    /// Removed class.
    /// </summary>
    public class Removed : Animation, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public IFO.BaseIFO Entry { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            Object.Entry.Parent.Animation.Add(Entry);
            MapManager.Animation.Add(Entry);

            int objectID = MapManager.Animation.WorldObjects.Count - 1;
            Object = MapManager.Animation.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Animation.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Animation.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Animation.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Animation.ObjectChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Animation.Removed)) && ((Commands.Animation.Animation)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.Animation.Animation)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.Animation.Animation)UndoManager.Commands[i]).Object = MapManager.Animation.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Animation && MapManager.Animation.Tool.SelectedObject == ObjectID)
                MapManager.Animation.Tool.Remove(true);
            else
                MapManager.Animation.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Animation - Removed";
        }
    }
}