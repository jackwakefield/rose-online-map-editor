namespace Map_Editor.Engine.Commands.Animation
{
    /// <summary>
    /// Animation class.
    /// </summary>
    public class Animation
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
        public Objects.Animation.WorldObject Object { get; set; }

        #endregion
    }
}
