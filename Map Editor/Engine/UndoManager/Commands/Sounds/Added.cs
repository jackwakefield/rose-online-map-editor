using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.Sounds
{
    /// <summary>
    /// Added class.
    /// </summary>
    public class Added : Sound, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public IFO.Sound Entry { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Sounds && MapManager.Sounds.Tool.SelectedObject == ObjectID)
                MapManager.Sounds.Tool.Remove(true);
            else
                MapManager.Sounds.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Parent.Sounds.Add(Entry);
            MapManager.Sounds.Add(Entry);

            int objectID = MapManager.Sounds.WorldObjects.Count - 1;
            Object = MapManager.Sounds.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Sounds.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Sounds.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Sounds.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Sounds.Removed)) && ((Commands.Sounds.Sound)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.Sounds.Sound)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.Sounds.Sound)UndoManager.Commands[i]).Object = MapManager.Sounds.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Sound - Added";
        }
    }
}