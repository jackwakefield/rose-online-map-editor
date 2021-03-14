using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Map_Editor.Engine.SpriteManager
{
    public static class FontManager
    {
        public enum FontType
        {
            Verdana12 = 0
        }

        private static Game game { get; set; }

        private static List<SpriteFont> spriteFonts;

        public static void Initialize(Game game)
        {
            FontManager.game = game;

            spriteFonts = new List<SpriteFont>(Enum.GetNames(typeof(FontType)).Length);

            spriteFonts.Add(game.Content.Load<SpriteFont>(@"Fonts\Verdana12"));
        }

        public static SpriteFont GetFont(FontType fontType)
        {
            return spriteFonts[(int)fontType];
        }
    }
}