namespace Map_Editor.Engine.Commands.Effects
{
    /// <summary>
    /// Effect class.
    /// </summary>
    public class Effect
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
        public Misc.Effects.WorldObject Object { get; set; }

        #endregion
    }
}
