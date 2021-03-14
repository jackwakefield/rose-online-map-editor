using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.Decoration
{
    /// <summary>
    /// Added class.
    /// </summary>
    public class Added : Decoration, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public IFO.BaseIFO Entry { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public int File { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Decoration && MapManager.Decoration.Tool.SelectedObject == ObjectID)
                MapManager.Decoration.Tool.Remove(true);
            else
                MapManager.Decoration.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Parent.Decoration.Add(Entry);
            MapManager.Decoration.Add(Entry, File, Object.Entry.Parent.Decoration.Count - 1);

            int objectID = MapManager.Decoration.WorldObjects.Count - 1;
            Object = MapManager.Decoration.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.ObjectChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.Removed) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.LightmapRemoved)) && ((Commands.Decoration.Decoration)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.Decoration.Decoration)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.Decoration.Decoration)UndoManager.Commands[i]).Object = MapManager.Decoration.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Decoration - Added";
        }
    }
}