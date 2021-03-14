using System.ComponentModel;
using System.Drawing.Design;
using System.Windows;
using Map_Editor.Engine;
using Map_Editor.Misc.Properties.Editors;
using Microsoft.Xna.Framework;

namespace Map_Editor.Misc.Properties
{
    /// <summary>
    /// Monster Spawn Property class.
    /// </summary>
    public class MonsterSpawnProperty
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DisplayName("\t\tDescription")]
        [Category("\t\t\tObject")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the event ID.
        /// </summary>
        /// <value>The event ID.</value>
        [DisplayName("\tEvent ID")]
        [Category("\t\t\tObject")]
        public short EventID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DisplayName("\t\t\t\t\tName")]
        [Category("\t\tSpawn")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        [DisplayName("\t\t\t\tInterval")]
        [Category("\t\tSpawn")]
        public int Interval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        /// <value>The limit.</value>
        [DisplayName("\t\t\tLimit")]
        [Category("\t\tSpawn")]
        public int Limit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        [DisplayName("\t\tRange")]
        [Category("\t\tSpawn")]
        public int Range
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tactic points.
        /// </summary>
        /// <value>The tactic points.</value>
        [DisplayName("\tTactic Points")]
        [Category("\t\tSpawn")]
        public int TacticPoints
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [DisplayName("\t\tPosition")]
        [Category("\tWorld")]
        public Vector3 Position
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Monster Property class.
    /// </summary>
    public class MonsterProperty
    {
        #region Delegates

        /// <summary>
        /// ObjectID delegate.
        /// </summary>
        public delegate void ObjectID(int value);

        #endregion

        #region Member Declarations

        /// <summary>
        /// Object ID.
        /// </summary>
        private int objectID;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [object ID changed].
        /// </summary>
        public event ObjectID ObjectIDChanged;

        #endregion

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DisplayName("\t\t\tDescription")]
        [Category("\tMonster")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DisplayName("\t\tID")]
        [Category("\tMonster")]
        [Editor(typeof(CharacterEditor), typeof(UITypeEditor))]
        public int ID
        {
            get { return objectID; }
            set
            {
                if (value >= FileManager.STBs["LIST_NPC"].Cells.Count)
                {
                    MessageBox.Show("Invalid value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                objectID = value;

                if (ObjectIDChanged != null)
                    ObjectIDChanged(objectID);
            }
        }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        [DisplayName("\tCount")]
        [Category("\tMonster")]
        public int Count
        {
            get;
            set;
        }
    }
}