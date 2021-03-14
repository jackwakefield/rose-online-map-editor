using System;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;

namespace Map_Editor.Engine.Commands.EventTriggers
{
    /// <summary>
    /// Removed class.
    /// </summary>
    public class Removed : EventTrigger, ICommand
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public IFO.EventTrigger Entry { get; set; }

        #endregion

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            Object.Entry.Parent.EventTriggers.Add(Entry);
            MapManager.EventTriggers.Add(Entry);

            int objectID = MapManager.EventTriggers.WorldObjects.Count - 1;
            Object = MapManager.EventTriggers.WorldObjects[objectID];

            for (int i = 0; i < UndoManager.Commands.Length; i++)
            {
                if (UndoManager.Commands[i] == null)
                    continue;

                if ((UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.ValueChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.Positioned) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.Added) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.ObjectChanged) ||
                     UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.Removed)) && ((Commands.EventTriggers.EventTrigger)UndoManager.Commands[i]).ObjectID == ObjectID)
                {
                    ((Commands.EventTriggers.EventTrigger)UndoManager.Commands[i]).ObjectID = objectID;
                    ((Commands.EventTriggers.EventTrigger)UndoManager.Commands[i]).Object = MapManager.EventTriggers.WorldObjects[objectID];
                }
            }
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers && MapManager.EventTriggers.Tool.SelectedObject == ObjectID)
                MapManager.EventTriggers.Tool.Remove(true);
            else
                MapManager.EventTriggers.RemoveAt(ObjectID, true);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Event Trigger - Removed";
        }
    }
}