namespace Map_Editor.Engine.Commands.NPCs
{
    /// <summary>
    /// NPC class.
    /// </summary>
    public class NPC
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
        public Characters.NPCs.WorldObject Object { get; set; }

        #endregion
    }
}