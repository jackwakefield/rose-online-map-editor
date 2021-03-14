namespace Map_Editor.Engine.Commands.Sounds
{
    /// <summary>
    /// Sound class.
    /// </summary>
    public class Sound
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
        public Misc.Sounds.WorldObject Object { get; set; }

        #endregion
    }
}
