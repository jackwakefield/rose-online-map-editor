using System;
using Map_Editor.Engine.Commands.Interfaces;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.Construction
{
    /// <summary>
    /// Positioned class.
    /// </summary>
    public class Positioned : Construction, ICommand
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

            MapManager.Construction.Add(ObjectID, Object.Entry, Object.FileID, Object.EntryID, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Construction && MapManager.Construction.Tool.SelectedObject == ObjectID)
                MapManager.Construction.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Position = NewPosition;

            MapManager.Construction.Add(ObjectID, Object.Entry, Object.FileID, Object.EntryID, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Construction && MapManager.Construction.Tool.SelectedObject == ObjectID)
                MapManager.Construction.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Construction - Position Changed";
        }
    }
}