using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.WarpGates
{
    /// <summary>
    /// Removed class.
    /// </summary>
    public class Removed : WarpGate, ICommand
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
            Object.Entry.Parent.WarpGates.Add(Entry);
            MapManager.WarpGates.Add(Entry);

            int objectID = MapManager.WarpGates.WorldObjects.Count - 1;
            Object = MapManager.WarpGates.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.WarpGates.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.WarpGates.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.WarpGates.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.WarpGates.Removed)) && ((Commands.WarpGates.WarpGate)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.WarpGates.WarpGate)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.WarpGates.WarpGate)UndoManager.Commands[i]).Object = MapManager.WarpGates.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.WarpGates && MapManager.WarpGates.Tool.SelectedObject == ObjectID)
                MapManager.WarpGates.Tool.Remove(true);
            else
                MapManager.WarpGates.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Warp Gate - Removed";
        }
    }
}