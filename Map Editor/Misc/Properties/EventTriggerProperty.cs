using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Map_Editor.Misc.Properties
{
    /// <summary>
    /// Event Trigger Property class.
    /// </summary>
    public class EventTriggerProperty
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
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [DisplayName("\t\t\tPosition")]
        [Category("\t\tWorld")]
        public Vector3 Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        [DisplayName("\t\tRotation")]
        [Category("\t\tWorld")]
        public Quaternion Rotation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        [DisplayName("\tScale")]
        [Category("\t\tWorld")]
        public Vector3 Scale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the QSD trigger.
        /// </summary>
        /// <value>The QSD trigger.</value>
        [DisplayName("\t\tQuest Trigger")]
        [Category("\tTriggers")]
        public string QSDTrigger
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the LUA trigger.
        /// </summary>
        /// <value>The LUA trigger.</value>
        [DisplayName("\tScript Trigger")]
        [Category("\tTriggers")]
        public string LUATrigger
        {
            get;
            set;
        }
    }
}