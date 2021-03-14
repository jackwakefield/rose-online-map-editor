using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Map_Editor.Misc.Properties
{
    /// <summary>
    /// Sound Property class.
    /// </summary>
    public class SoundProperty
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
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        [DisplayName("\t\t\tPath")]
        [Category("\t\tSound")]
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        [DisplayName("\t\tRange")]
        [Category("\t\tSound")]
        public int Range
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        [DisplayName("\tInterval")]
        [Category("\t\tSound")]
        public int Interval
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
}