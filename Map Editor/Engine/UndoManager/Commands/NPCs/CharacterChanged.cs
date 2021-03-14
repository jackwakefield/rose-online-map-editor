using System;
using Map_Editor.Engine.Commands.Interfaces;

namespace Map_Editor.Engine.Commands.NPCs
{
    /// <summary>
    /// Character Changed class.
    /// </summary>
    public class CharacterChanged : NPC, ICommand
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

            MapManager.NPCs.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.NPCs && MapManager.NPCs.Tool.SelectedObject == ObjectID)
                MapManager.NPCs.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.ObjectID = NewObject;

            MapManager.NPCs.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.NPCs && MapManager.NPCs.Tool.SelectedObject == ObjectID)
                MapManager.NPCs.Tool.Select(ObjectID);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "NPC - Character Changed";
        }
    }
}