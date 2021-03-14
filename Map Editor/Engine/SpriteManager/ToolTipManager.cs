using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.SpriteManager
{
    public static class ToolTipManager
    {
        private static GraphicsDevice device { get; set; }

        private static SpriteBatch spriteBatch { get; set; }

        private static SpriteFont spriteFont
        {
            get { return FontManager.GetFont(FontManager.FontType.Verdana12); }
        }

        private static Texture2D[] tooltipTextures { get; set; }

        private static string setText { get; set; }
        private static Vector3 setPosition { get; set; }

        public static void Initialize(GraphicsDevice device)
        {
            spriteBatch = new SpriteBatch(ToolTipManager.device = device);

            tooltipTextures = new Texture2D[]
            {
                Texture2D.FromFile(device, @"Content\Images\ToolTip_Left.png"),
                Texture2D.FromFile(device, @"Content\Images\ToolTip_Middle.png"),
                Texture2D.FromFile(device, @"Content\Images\ToolTip_Right.png")
            };
        }

        public static void Set(string text, Vector3 position)
        {
            setText = text;
            setPosition = position;
        }

        public static void Draw(GameTime gameTime)
        {
            if (setPosition == Vector3.Zero || setText == string.Empty)
                return;

            Vector3 projectedCenterCoordinates = device.Viewport.Project(setPosition, CameraManager.Projection, CameraManager.View, Matrix.Identity);
            Vector2 screenCoordinates = new Vector2((int)projectedCenterCoordinates.X, (int)projectedCenterCoordinates.Y);

            int stringWidth = (int)spriteFont.MeasureString(setText).X;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

            spriteBatch.Draw(tooltipTextures[0], screenCoordinates, Color.White);

            if (stringWidth > 13)
                spriteBatch.Draw(tooltipTextures[1], new Rectangle((int)(screenCoordinates.X + 29.0f), (int)(screenCoordinates.Y + 12.0f), stringWidth - 13, 22), Color.White);

            spriteBatch.DrawString(spriteFont, setText, screenCoordinates + new Vector2(10.0f, 16.0f), Color.White);

            spriteBatch.Draw(tooltipTextures[2], screenCoordinates + new Vector2(29.0f + (stringWidth - 16.0f), 12), Color.White);

            spriteBatch.End();

            setPosition = Vector3.Zero;
            setText = string.Empty;
        }
    }
}