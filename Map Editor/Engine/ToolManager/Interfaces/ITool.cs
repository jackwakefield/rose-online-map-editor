using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Tools.Interfaces
{
    /// <summary>
    /// ITool interface.
    /// </summary>
    public interface ITool
    {
        /// <summary>
        /// Updates the tool.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Update(GameTime gameTime);
    }
}