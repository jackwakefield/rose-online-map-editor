namespace Map_Editor.Engine.Commands.Construction
{
    /// <summary>
    /// Construction class.
    /// </summary>
    public class Construction
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
        public Objects.Object.WorldObject Object { get; set; }

        #endregion
    }
}
