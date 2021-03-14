using System;
using Map_Editor.Engine.Commands.Interfaces;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.Decoration
{
    /// <summary>
    /// Positioned class.
    /// </summary>
    public class Positioned : Decoration, ICommand
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

            MapManager.Decoration.Add(ObjectID, Object.Entry, Object.FileID, Object.EntryID, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Decoration && MapManager.Decoration.Tool.SelectedObject == ObjectID)
                MapManager.Decoration.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Position = NewPosition;

            MapManager.Decoration.Add(ObjectID, Object.Entry, Object.FileID, Object.EntryID, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Decoration && MapManager.Decoration.Tool.SelectedObject == ObjectID)
                MapManager.Decoration.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Decoration - Position Changed";
        }
    }
}