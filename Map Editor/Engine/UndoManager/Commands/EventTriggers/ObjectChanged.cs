using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Forms.Controls;
using Map_Editor.Misc.Properties;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.EventTriggers
{
    /// <summary>
    /// Object Changed class.
    /// </summary>
    public class ObjectChanged : EventTrigger, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the new object.
        /// </summary>
        /// <value>The new object.</value>
        public int NewObject { get; set; }

        /// <summary>
        /// Gets or sets the old object.
        /// </summary>
        /// <value>The old object.</value>
        public int OldObject { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            Object.Entry.ObjectID = OldObject;

            MapManager.EventTriggers.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers && MapManager.EventTriggers.Tool.SelectedObject == ObjectID)
                ((EventTriggerTool)App.Form.ToolHost.Content).SelectModel(OldObject);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.ObjectID = NewObject;

            MapManager.EventTriggers.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers && MapManager.EventTriggers.Tool.SelectedObject == ObjectID)
                ((EventTriggerTool)App.Form.ToolHost.Content).SelectModel(NewObject);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Event Trigger - Object Changed";
        }
    }
}