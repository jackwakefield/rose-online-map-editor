namespace Map_Editor.Engine.Commands.EventTriggers
{
    /// <summary>
    /// EventTrigger class.
    /// </summary>
    public class EventTrigger
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
        public Objects.EventTriggers.WorldObject Object { get; set; }

        #endregion
    }
}
