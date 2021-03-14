using Map_Editor.Misc;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Objects
{
    /// <summary>
    /// Construction class.
    /// </summary>
    public class Construction : Object
    {
        #region Accessors

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>The tool.</value>
        public Tools.Construction Tool
        {
            get { return (Tools.Construction)ToolManager.Tool; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Construction"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public Construction(Game game)
            : base(game, MapManager.DRAWORDER_CONSTRUCTION)
        {
            Type = ObjectType.Construction;
        }

        /// <summary>
        /// Draws the objects.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Construction)
                Tool.Draw(ConfigurationManager.GetValue<bool>("Construction", "DrawBoundingBoxes"));
        }
    }
}
