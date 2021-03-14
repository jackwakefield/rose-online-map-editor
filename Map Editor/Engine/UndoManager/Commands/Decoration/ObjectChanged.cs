using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Forms.Controls;

namespace Map_Editor.Engine.Commands.Decoration
{
    /// <summary>
    /// Object Changed class.
    /// </summary>
    public class ObjectChanged : Decoration, ICommand
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

            MapManager.Decoration.Add(ObjectID, Object.Entry, Object.FileID, Object.EntryID, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Decoration && MapManager.Decoration.Tool.SelectedObject == ObjectID)
                ((DecorationTool)App.Form.ToolHost.Content).SelectModel(OldObject);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.ObjectID = NewObject;

            MapManager.Decoration.Add(ObjectID, Object.Entry, Object.FileID, Object.EntryID, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Decoration && MapManager.Decoration.Tool.SelectedObject == ObjectID)
                ((DecorationTool)App.Form.ToolHost.Content).SelectModel(NewObject);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Decoration - Object Changed";
        }
    }
}