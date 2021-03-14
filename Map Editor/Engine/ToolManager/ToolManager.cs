using Map_Editor.Engine.Manipulation;
using Map_Editor.Engine.Tools;
using Map_Editor.Engine.Tools.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine
{
    /// <summary>
    /// Tool Manager class.
    /// </summary>
    public static class ToolManager
    {
        /// <summary>
        /// Tool Mode type.
        /// </summary>
        public enum ToolMode
        {
            /// <summary>
            /// None.
            /// </summary>
            None,

            /// <summary>
            /// Height.
            /// </summary>
            Height,

            /// <summary>
            /// Tiles.
            /// </summary>
            Tiles,

            /// <summary>
            /// Brush.
            /// </summary>
            Brush,

            /// <summary>
            /// Decoration.
            /// </summary>
            Decoration,

            /// <summary>
            /// Construction.
            /// </summary>
            Construction,

            /// <summary>
            /// Event Triggers
            /// </summary>
            EventTriggers,

            /// <summary>
            /// NPCs.
            /// </summary>
            NPCs,

            /// <summary>
            /// Monsters.
            /// </summary>
            Monsters,

            /// <summary>
            /// Spawn Points.
            /// </summary>
            SpawnPoints,

            /// <summary>
            /// Warp Gates.
            /// </summary>
            WarpGates,

            /// <summary>
            /// Sounds.
            /// </summary>
            Sounds,

            /// <summary>
            /// Effects.
            /// </summary>
            Effects,

            /// <summary>
            /// Collision. 
            /// </summary>
            Collision,

            /// <summary>
            /// Water.
            /// </summary>
            Water,

            /// <summary>
            /// Animation.
            /// </summary>
            Animation
        }

        /// <summary>
        /// Manipulation Mode type.
        /// </summary>
        public enum ManipulationMode
        {
            /// <summary>
            /// None.
            /// </summary>
            None,

            /// <summary>
            /// Cursor.
            /// </summary>
            Cursor,

            /// <summary>
            /// Position.
            /// </summary>
            Position
        }

        #region Member Declarations

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private static GraphicsDevice device { get; set; }

        /// <summary>
        /// Gets or sets the tool mode.
        /// </summary>
        /// <value>The tool mode.</value>
        private static ToolMode toolMode { get; set; }

        /// <summary>
        /// Gets or sets the manipulation mode.
        /// </summary>
        /// <value>The manipulation mode.</value>
        private static ManipulationMode manipulationMode { get; set; }

        /// <summary>
        /// Gets or sets the tool.
        /// </summary>
        /// <value>The tool.</value>
        public static ITool Tool { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public static Translate Position { get; set; }

        /// <summary>
        /// Gets or sets the cursor.
        /// </summary>
        /// <value>The cursor.</value>
        public static CursorTranslate Cursor { get; set; }

        #endregion

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="device">The device.</param>
        public static void Initialize(GraphicsDevice device)
        {
            ToolManager.device = device;

            toolMode = ToolMode.None;
            manipulationMode = ManipulationMode.None;

            Position = new Translate(device);
            Cursor = new CursorTranslate(device);
        }

        /// <summary>
        /// Gets the tool mode.
        /// </summary>
        /// <returns></returns>
        public static ToolMode GetToolMode()
        {
            return toolMode;
        }

        /// <summary>
        /// Sets the tool mode.
        /// </summary>
        /// <param name="toolMode">The tool mode.</param>
        public static void SetToolMode(ToolMode toolMode)
        {
            ToolManager.toolMode = toolMode;

            switch (toolMode)
            {
                case ToolMode.None:
                    Tool = null;
                    break;
                case ToolMode.Height:
                    Tool = new Height(device);
                    break;
                case ToolMode.Tiles:
                    Tool = new Tiles(device);
                    break;
                case ToolMode.Brush:
                    Tool = new Brush(device);
                    break;
                case ToolMode.Decoration:
                    Tool = new Decoration(device);
                    break;
                case ToolMode.Construction:
                    Tool = new Construction(device);
                    break;
                case ToolMode.EventTriggers:
                    Tool = new EventTriggers(device);
                    break;
                case ToolMode.NPCs:
                    Tool = new NPCs(device);
                    break;
                case ToolMode.Monsters:
                    Tool = new Monsters(device);
                    break;
                case ToolMode.SpawnPoints:
                    Tool = new SpawnPoints(device);
                    break;
                case ToolMode.WarpGates:
                    Tool = new WarpGates(device);
                    break;
                case ToolMode.Sounds:
                    Tool = new Sounds(device);
                    break;
                case ToolMode.Effects:
                    Tool = new Effects(device);
                    break;
                case ToolMode.Collision:
                    Tool = new Collision(device);
                    break;
                case ToolMode.Water:
                    Tool = new Water(device);
                    break;
                case ToolMode.Animation:
                    Tool = new Animation(device);
                    break;
            }
        }

        /// <summary>
        /// Gets the manipulation mode.
        /// </summary>
        /// <returns></returns>
        public static ManipulationMode GetManipulationMode()
        {
            return manipulationMode;
        }

        /// <summary>
        /// Sets the manipulation mode.
        /// </summary>
        /// <param name="manipulationMode">The manipulation mode.</param>
        public static void SetManipulationMode(ManipulationMode manipulationMode)
        {
            ToolManager.manipulationMode = manipulationMode;
        }

        /// <summary>
        /// Updates the selected tool.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public static void Update(GameTime gameTime)
        {
            if (Tool == null)
                return;

            Tool.Update(gameTime);
        } 
    }
}