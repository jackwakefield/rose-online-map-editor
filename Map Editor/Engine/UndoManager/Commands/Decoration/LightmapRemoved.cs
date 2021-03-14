using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.Decoration
{
    /// <summary>
    /// Lightmap Removed class.
    /// </summary>
    public class LightmapRemoved : Decoration, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the lightmap object.
        /// </summary>
        /// <value>The lightmap object.</value>
        public LIT.Object LightmapObject { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            FileManager.DecorationLITs[Object.FileID].Objects.Add(LightmapObject);

            MapManager.Decoration.Add(ObjectID, Object.Entry, Object.FileID, Object.EntryID, true);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            FileManager.DecorationLITs[Object.FileID].Objects.Remove(LightmapObject);

            MapManager.Decoration.Add(ObjectID, Object.Entry, Object.FileID, Object.EntryID, true);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Decoration - Lightmap Removed";
        }
    }
}