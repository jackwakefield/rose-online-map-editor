namespace Map_Editor.Engine.Commands.Monsters
{
    /// <summary>
    /// Monster class.
    /// </summary>
    public class Monster
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
        public Characters.Monsters.WorldObject Object { get; set; }

        #endregion
    }
}