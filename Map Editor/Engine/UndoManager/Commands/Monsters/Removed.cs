using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.Monsters
{
    /// <summary>
    /// Removed class.
    /// </summary>
    public class Removed : Monster, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public IFO.MonsterSpawn Entry { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            Object.Entry.Parent.Monsters.Add(Entry);
            MapManager.Monsters.Add(Entry);

            int objectID = MapManager.Monsters.WorldObjects.Count - 1;
            Object = MapManager.Monsters.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Monsters.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Monsters.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Monsters.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.Monsters.Removed)) && ((Commands.Monsters.Monster)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.Monsters.Monster)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.Monsters.Monster)UndoManager.Commands[i]).Object = MapManager.Monsters.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Monsters && MapManager.Monsters.Tool.SelectedObject == ObjectID)
                MapManager.Monsters.Tool.Remove(true);
            else
                MapManager.Monsters.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Monster - Removed";
        }
    }
}