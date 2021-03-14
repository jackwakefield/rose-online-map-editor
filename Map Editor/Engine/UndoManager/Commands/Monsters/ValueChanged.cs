using System;
using System.Collections.Generic;
using Map_Editor.Engine.Commands.Interfaces;
using Map_Editor.Engine.Map;
using Map_Editor.Forms.Controls;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Commands.Monsters
{
    /// <summary>
    /// Value Changed class.
    /// </summary>
    public class ValueChanged : Monster, ICommand
    {
        /// <summary>
        /// Object Value structure.
        /// </summary>
        public struct ObjectValue
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <value>The description.</value>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the event ID.
            /// </summary>
            /// <value>The event ID.</value>
            public short EventID { get; set; }

            /// <summary>
            /// Gets or sets the position.
            /// </summary>
            /// <value>The position.</value>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the interval.
            /// </summary>
            /// <value>The interval.</value>
            public int Interval { get; set; }

            /// <summary>
            /// Gets or sets the limit.
            /// </summary>
            /// <value>The limit.</value>
            public int Limit { get; set; }

            /// <summary>
            /// Gets or sets the range.
            /// </summary>
            /// <value>The range.</value>
            public int Range { get; set; }

            /// <summary>
            /// Gets or sets the tactic points.
            /// </summary>
            /// <value>The tactic points.</value>
            public int TacticPoints { get; set; }

            /// <summary>
            /// Gets or sets the basic.
            /// </summary>
            /// <value>The basic.</value>
            public List<IFO.MonsterSpawn.Monster> Basic { get; set; }

            /// <summary>
            /// Gets or sets the tactic.
            /// </summary>
            /// <value>The tactic.</value>
            public List<IFO.MonsterSpawn.Monster> Tactic { get; set; }

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
            Object.Entry.Description = OldValue.Description;
            Object.Entry.EventID = OldValue.EventID;
            Object.Entry.Position = OldValue.Position;
            Object.Entry.Name = OldValue.Name;
            Object.Entry.Interval = OldValue.Interval;
            Object.Entry.Limit = OldValue.Limit;
            Object.Entry.Range = OldValue.Range;
            Object.Entry.TacticPoints = OldValue.TacticPoints;
            Object.Entry.Basic = new List<IFO.MonsterSpawn.Monster>();
            Object.Entry.Tactic = new List<IFO.MonsterSpawn.Monster>();

            for (int i = 0; i < OldValue.Basic.Count; i++)
            {
                Object.Entry.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = OldValue.Basic[i].ID,
                    Count = OldValue.Basic[i].Count,
                    Description = OldValue.Basic[i].Description
                });
            }

            for (int i = 0; i < OldValue.Tactic.Count; i++)
            {
                Object.Entry.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = OldValue.Tactic[i].ID,
                    Count = OldValue.Tactic[i].Count,
                    Description = OldValue.Tactic[i].Description
                });
            }

            MapManager.Monsters.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Monsters && MapManager.Monsters.Tool.SelectedObject == ObjectID)
                ((MonsterTool)App.Form.ToolHost.Content).Select(ObjectID);
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            Object.Entry.Description = NewValue.Description;
            Object.Entry.EventID = NewValue.EventID;
            Object.Entry.Position = NewValue.Position;
            Object.Entry.Name = NewValue.Name;
            Object.Entry.Interval = NewValue.Interval;
            Object.Entry.Limit = NewValue.Limit;
            Object.Entry.Range = NewValue.Range;
            Object.Entry.TacticPoints = NewValue.TacticPoints;
            Object.Entry.Basic = new List<IFO.MonsterSpawn.Monster>();
            Object.Entry.Tactic = new List<IFO.MonsterSpawn.Monster>();

            for (int i = 0; i < NewValue.Basic.Count; i++)
            {
                Object.Entry.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = NewValue.Basic[i].ID,
                    Count = NewValue.Basic[i].Count,
                    Description = NewValue.Basic[i].Description
                });
            }

            for (int i = 0; i < OldValue.Tactic.Count; i++)
            {
                Object.Entry.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = NewValue.Tactic[i].ID,
                    Count = NewValue.Tactic[i].Count,
                    Description = NewValue.Tactic[i].Description
                });
            }

            MapManager.Monsters.Add(ObjectID, Object.Entry, true);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Monsters && MapManager.Monsters.Tool.SelectedObject == ObjectID)
                ((MonsterTool)App.Form.ToolHost.Content).Select(ObjectID);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Monster - Value Changed";
        }
    }
}