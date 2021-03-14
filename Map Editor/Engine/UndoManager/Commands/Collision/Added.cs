using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.Collision
{
    /// <summary>
    /// Added class.
    /// </summary>
    public class Added : Collision, ICommand
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
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Collision && MapManager.Collision.Tool.SelectedObject == ObjectID)
                MapManager.Collision.Tool.Remove(true);
            else
                MapManager.Collision.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Parent.Collision.Add(Entry);
            MapManager.Collision.Add(Entry);

            int objectID = MapManager.Collision.WorldObjects.Count - 1;
            Object = MapManager.Collision.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Collision.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Collision.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Collision.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Collision.Removed)) && ((Commands.Collision.Collision)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.Collision.Collision)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.Collision.Collision)UndoManager.Commands[i]).Object = MapManager.Collision.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Collision - Added";
        }
    }
}