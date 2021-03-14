namespace Map_Editor.Engine.Commands.SpawnPoints
{
    /// <summary>
    /// SpawnPoint class.
    /// </summary>
    public class SpawnPoint
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
        public Events.SpawnPoints.WorldObject Object { get; set; }

        #endregion
    }
}