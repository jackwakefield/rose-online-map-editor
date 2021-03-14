using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Forms.Controls;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.Effects
{
    /// <summary>
    /// Value Changed class.
    /// </summary>
    public class ValueChanged : Effect, ICommand
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
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            public string Path { get; set; }

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
            Object.Entry.Path = OldValue.Path;

            MapManager.Effects.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Effects && MapManager.Effects.Tool.SelectedObject == ObjectID)
                ((EffectTool)App.Form.ToolHost.Content).Select(ObjectID);
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
            Object.Entry.Path = NewValue.Path;

            MapManager.Effects.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Effects && MapManager.Effects.Tool.SelectedObject == ObjectID)
                ((EffectTool)App.Form.ToolHost.Content).Select(ObjectID);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Effect - Value Changed";
        }
    }
}