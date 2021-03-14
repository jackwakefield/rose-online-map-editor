using System;
using Map_Editor.Engine.Commands.Interfaces;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.WarpGates
{
    /// <summary>
    /// Positioned class.
    /// </summary>
    public class Positioned : WarpGate, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the new position.
        /// </summary>
        /// <value>The new position.</value>
        public Vector3 NewPosition { get; set; }

        /// <summary>
        /// Gets or sets the old position.
        /// </summary>
        /// <value>The old position.</value>
        public Vector3 OldPosition { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            Object.Entry.Position = OldPosition;

            MapManager.WarpGates.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.WarpGates && MapManager.WarpGates.Tool.SelectedObject == ObjectID)
                MapManager.WarpGates.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Position = NewPosition;

            MapManager.WarpGates.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.WarpGates && MapManager.WarpGates.Tool.SelectedObject == ObjectID)
                MapManager.WarpGates.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Warp Gate - Position Changed";
        }
    }
}