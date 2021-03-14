namespace Map_Editor.Engine.Commands.WarpGates
{
    /// <summary>
    /// WarpGate class.
    /// </summary>
    public class WarpGate
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
        public Events.WarpGates.WorldObject Object { get; set; }

        #endregion
    }
}
