using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Map_Editor.Misc.Properties
{
    /// <summary>
    /// Spawn Point Property class.
    /// </summary>
    public class SpawnPointProperty
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DisplayName("\t\tName")]
        [Category("\tSpawn Point")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [DisplayName("\tPosition")]
        [Category("\tSpawn Point")]
        public Vector3 Position 
        {
            get;
            set;
        }
    }
}