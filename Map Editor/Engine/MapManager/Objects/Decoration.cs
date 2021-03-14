using Map_Editor.Misc;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Objects
{
    /// <summary>
    /// Decoration class.
    /// </summary>
    public class Decoration : Object
    {
        #region Accessors

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>The tool.</value>
        public Tools.Decoration Tool
        {
            get { return (Tools.Decoration)ToolManager.Tool; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Decoration"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public Decoration(Game game)
            : base(game, MapManager.DRAWORDER_DECORATION)
        {
            Type = ObjectType.Decoration;
        }

        /// <summary>
        /// Draws the objects.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Decoration)
                Tool.Draw(ConfigurationManager.GetValue<bool>("Decoration", "DrawBoundingBoxes"));
        }
    }
}
