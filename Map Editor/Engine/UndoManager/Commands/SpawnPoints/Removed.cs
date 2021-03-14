using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.SpawnPoints
{
    /// <summary>
    /// Removed class.
    /// </summary>
    public class Removed : SpawnPoint, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public ZON.SpawnPoint Entry { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            FileManager.ZON.SpawnPoints.Add(Entry);
            MapManager.SpawnPoints.Add(Entry);

            int objectID = MapManager.SpawnPoints.WorldObjects.Count - 1;
            Object = MapManager.SpawnPoints.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.SpawnPoints.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.SpawnPoints.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.SpawnPoints.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.SpawnPoints.Removed)) && ((Commands.SpawnPoints.SpawnPoint)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.SpawnPoints.SpawnPoint)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.SpawnPoints.SpawnPoint)UndoManager.Commands[i]).Object = MapManager.SpawnPoints.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.SpawnPoints && MapManager.SpawnPoints.Tool.SelectedObject == ObjectID)
                MapManager.SpawnPoints.Tool.Remove(true);
            else
                MapManager.SpawnPoints.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Spawn Point - Removed";
        }
    }
}