using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.Construction
{
    /// <summary>
    /// Removed class.
    /// </summary>
    public class Removed : Construction, ICommand
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
            Object.Entry.Parent.Construction.Insert((Object.EntryID == 0) ? 0 : Object.EntryID - 1, Entry);
            MapManager.Construction.Add(Entry, File, Object.EntryID);

            int objectID = MapManager.Construction.WorldObjects.Count - 1;
            Object = MapManager.Construction.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Construction.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Construction.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Construction.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Construction.ObjectChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Construction.Removed) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.LightmapRemoved)) && ((Commands.Construction.Construction)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.Construction.Construction)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.Construction.Construction)UndoManager.Commands[i]).Object = MapManager.Construction.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Construction && MapManager.Construction.Tool.SelectedObject == ObjectID)
                MapManager.Construction.Tool.Remove(true);
            else
                MapManager.Construction.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Construction - Removed";
        }
    }
}