namespace Map_Editor.Engine.Commands.Water
{
    /// <summary>
    /// Water class.
    /// </summary>
    public class Water
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the object ID.
        /// </summary>
        /// <value>The object ID.</value>
        public int ObjectID { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>The object.</value>
        public Terrain.Water.WorldObject Object { get; set; }

        #endregion
    }
}
