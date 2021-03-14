using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.NPCs
{
    /// <summary>
    /// Added class.
    /// </summary>
    public class Added : NPC, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public IFO.NPC Entry { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.NPCs && MapManager.NPCs.Tool.SelectedObject == ObjectID)
                MapManager.NPCs.Tool.Remove(true);
            else
                MapManager.NPCs.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Parent.NPCs.Add(Entry);
            MapManager.NPCs.Add(Entry);

            int objectID = MapManager.NPCs.WorldObjects.Count - 1;
            Object = MapManager.NPCs.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.NPCs.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.NPCs.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.NPCs.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.NPCs.CharacterChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.NPCs.Removed)) && ((Commands.NPCs.NPC)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.NPCs.NPC)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.NPCs.NPC)UndoManager.Commands[i]).Object = MapManager.NPCs.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "NPC - Added";
        }
    }
}