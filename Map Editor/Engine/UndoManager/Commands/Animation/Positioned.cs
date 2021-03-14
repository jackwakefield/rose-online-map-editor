using System;
using Map_Editor.Engine.Commands.Interfaces;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.Animation
{
    /// <summary>
    /// Positioned class.
    /// </summary>
    public class Positioned : Animation, ICommand
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

            MapManager.Animation.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Animation && MapManager.Animation.Tool.SelectedObject == ObjectID)
                MapManager.Animation.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Position = NewPosition;

            MapManager.Animation.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Animation && MapManager.Animation.Tool.SelectedObject == ObjectID)
                MapManager.Animation.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Animation - Position Changed";
        }
    }
}