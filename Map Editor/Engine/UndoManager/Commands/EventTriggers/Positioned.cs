using System;
using Map_Editor.Engine.Commands.Interfaces;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.EventTriggers
{
    /// <summary>
    /// Positioned class.
    /// </summary>
    public class Positioned : EventTrigger, ICommand
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

            MapManager.EventTriggers.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers && MapManager.EventTriggers.Tool.SelectedObject == ObjectID)
                MapManager.EventTriggers.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Position = NewPosition;

            MapManager.EventTriggers.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers && MapManager.EventTriggers.Tool.SelectedObject == ObjectID)
                MapManager.EventTriggers.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Event Trigger - Position Changed";
        }
    }
}