using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Forms.Controls;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.EventTriggers
{
    /// <summary>
    /// Value Changed class.
    /// </summary>
    public class ValueChanged : EventTrigger, ICommand
    {
        /// <summary>
        /// Object Value structure.
        /// </summary>
        public struct ObjectValue
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <value>The description.</value>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the event ID.
            /// </summary>
            /// <value>The event ID.</value>
            public short EventID { get; set; }

            /// <summary>
            /// Gets or sets the position.
            /// </summary>
            /// <value>The position.</value>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Gets or sets the rotation.
            /// </summary>
            /// <value>The rotation.</value>
            public Quaternion Rotation { get; set; }

            /// <summary>
            /// Gets or sets the scale.
            /// </summary>
            /// <value>The scale.</value>
            public Vector3 Scale { get; set; }

            /// <summary>
            /// Gets or sets the LUA trigger.
            /// </summary>
            /// <value>The LUA trigger.</value>
            public string LUATrigger { get; set; }

            /// <summary>
            /// Gets or sets the QSD trigger.
            /// </summary>
            /// <value>The QSD trigger.</value>
            public string QSDTrigger { get; set; }

            #endregion
        };

        #region Member Declarations

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public ObjectValue NewValue { get; set; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public ObjectValue OldValue { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            Object.Entry.Description = OldValue.Description;
            Object.Entry.EventID = OldValue.EventID;
            Object.Entry.Position = OldValue.Position;
            Object.Entry.Rotation = OldValue.Rotation;
            Object.Entry.Scale = OldValue.Scale;
            Object.Entry.QSDTrigger = OldValue.QSDTrigger;
            Object.Entry.LUATrigger = OldValue.LUATrigger;

            MapManager.EventTriggers.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers && MapManager.EventTriggers.Tool.SelectedObject == ObjectID)
                ((EventTriggerTool)App.Form.ToolHost.Content).Select(ObjectID);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Description = NewValue.Description;
            Object.Entry.EventID = NewValue.EventID;
            Object.Entry.Position = NewValue.Position;
            Object.Entry.Rotation = NewValue.Rotation;
            Object.Entry.Scale = NewValue.Scale;
            Object.Entry.QSDTrigger = NewValue.QSDTrigger;
            Object.Entry.LUATrigger = NewValue.LUATrigger;

            MapManager.EventTriggers.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers && MapManager.EventTriggers.Tool.SelectedObject == ObjectID)
                ((EventTriggerTool)App.Form.ToolHost.Content).Select(ObjectID);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Event Trigger - Value Changed";
        }
    }
}