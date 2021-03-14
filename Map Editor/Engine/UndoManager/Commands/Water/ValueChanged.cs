using System;
using Map_Editor.Engine.Commands.Interfaces;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.Water
{
    /// <summary>
    /// Value Changed class.
    /// </summary>
    public class ValueChanged : Water, ICommand
    {
        /// <summary>
        /// Object Value structure.
        /// </summary>
        public struct ObjectValue
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the minimum.
            /// </summary>
            /// <value>The minimum.</value>
            public Vector3 Minimum { get; set; }

            /// <summary>
            /// Gets or sets the maximum.
            /// </summary>
            /// <value>The maximum.</value>
            public Vector3 Maximum { get; set; }

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
            Object.Entry.Minimum = OldValue.Minimum;
            Object.Entry.Maximum = OldValue.Maximum;

            MapManager.Water.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Water && MapManager.Water.Tool.SelectedObject == ObjectID)
                MapManager.Water.Tool.Select(MapManager.Water.Tool.SelectedObject);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Minimum = NewValue.Minimum;
            Object.Entry.Maximum = NewValue.Maximum;

            MapManager.Water.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Water && MapManager.Water.Tool.SelectedObject == ObjectID)
                MapManager.Water.Tool.Select(MapManager.Water.Tool.SelectedObject);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Water - Value Changed";
        }
    }
}