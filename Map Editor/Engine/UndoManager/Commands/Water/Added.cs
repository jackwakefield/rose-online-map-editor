using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.Water
{
    /// <summary>
    /// Added class.
    /// </summary>
    public class Added : Water, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the IFO entry.
        /// </summary>
        /// <value>The IFO entry.</value>
        public IFO.WaterBlock IFOEntry { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Water && MapManager.Water.Tool.SelectedObject == ObjectID)
                MapManager.Water.Tool.Remove(true);
            else
                MapManager.Water.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Parent.Water.Add(IFOEntry);
            MapManager.Water.Add(IFOEntry);

            int objectID = MapManager.Water.WorldObjects.Count - 1;
            Object = MapManager.Water.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Water.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Water.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Water.Removed)) && ((Commands.Water.Water)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.Water.Water)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.Water.Water)UndoManager.Commands[i]).Object = MapManager.Water.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Water - Added";
        }
    }
}