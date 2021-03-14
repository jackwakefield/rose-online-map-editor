namespace Map_Editor.Engine.Commands.Collision
{
    /// <summary>
    /// Collision class.
    /// </summary>
    public class Collision
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
        public Objects.Collision.WorldObject Object { get; set; }

        #endregion
    }
}
