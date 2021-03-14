using System.Collections.Generic;
using System.Windows.Forms;
using Map_Editor.Engine.Tools.Interfaces;
using Map_Editor.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Map_Editor.Engine.Tools
{
    /// <summary>
    /// Tiles class.
    /// </summary>
    public class Tiles : ITool
    {
        /// <summary>
        /// Tile Tool type.
        /// </summary>
        public enum TileToolType
        {
            /// <summary>
            /// None.
            /// </summary>
            None,

            /// <summary>
            /// Pick.
            /// </summary>
            Pick,

            /// <summary>
            /// Modify.
            /// </summary>
            Modify
        }

        /// <summary>
        /// Tile structure.
        /// </summary>
        public struct Tile
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the X.
            /// </summary>
            /// <value>The X.</value>
            public int X { get; set; }

            /// <summary>
            /// Gets or sets the Y.
            /// </summary>
            /// <value>The Y.</value>
            public int Y { get; set; }

            /// <summary>
            /// Gets or sets the tile X.
            /// </summary>
            /// <value>The tile X.</value>
            public int TileX { get; set; }

            /// <summary>
            /// Gets or sets the tile Y.
            /// </summary>
            /// <value>The tile Y.</value>
            public int TileY { get; set; }

            #endregion
        };

        #region Member Declarations

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDevice device { get; set; }

        /// <summary>
        /// Gets or sets the hovered tile.
        /// </summary>
        /// <value>The hovered tile.</value>
        public Tile? HoveredTile { get; set; }

        /// <summary>
        /// Gets or sets the tool mode.
        /// </summary>
        /// <value>The tool mode.</value>
        public TileToolType ToolMode { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Tiles"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public Tiles(GraphicsDevice device)
        {
            this.device = device;

            ToolMode = TileToolType.None;
        }

        /// <summary>
        /// Updates the tool.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            if (ToolMode == TileToolType.None)
                return;

            MouseState mouseState = Mouse.GetState();

            Cursor.Current = ((HoveredTile = MapManager.Heightmaps.PickTile()) == null) ? Cursors.Default : Cursors.Hand;

            if (HoveredTile == null)
                return;

            if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                return;

            switch (ToolMode)
            {
                case TileToolType.Pick:
                    ((TileTool)App.Form.ToolHost.Content).SelectTile(MapManager.Heightmaps.Blocks[HoveredTile.Value.Y, HoveredTile.Value.X].TileFile.Tiles[HoveredTile.Value.TileY, HoveredTile.Value.TileX].TileID);
                    break;
                case TileToolType.Modify:
                    {
                        List<Commands.Tiles.Placement.Tile> modifiedTiles = new List<Commands.Tiles.Placement.Tile>();
                        int previousEdit = 0;

                        int gridSize = 1;

                        switch (((TileTool)App.Form.ToolHost.Content).MultipleSelection.SelectedIndex)
                        {
                            case 0:
                                gridSize = 3;
                                break;
                            case 1:
                                gridSize = 5;
                                break;
                            case 2:
                                gridSize = 7;
                                break;
                            case 3:
                                gridSize = 9;
                                break;
                        }

                        if (((TileTool)App.Form.ToolHost.Content).MultiplePlacement.IsChecked == true)
                        {
                            for (int ty = HoveredTile.Value.TileY - (gridSize / 2); ty <= HoveredTile.Value.TileY + (gridSize / 2); ty++)
                            {
                                for (int tx = HoveredTile.Value.TileX - (gridSize / 2); tx <= HoveredTile.Value.TileX + (gridSize / 2); tx++)
                                {
                                    int tileX = tx;
                                    int fileX = HoveredTile.Value.X;

                                    int tileY = ty;
                                    int fileY = HoveredTile.Value.Y;

                                    if (tileX > 15)
                                    {
                                        tileX -= 16;
                                        fileY += 1;
                                    }
                                    else if (tileX < 0)
                                    {
                                        tileX += 16;
                                        fileY -= 1;
                                    }

                                    if (tileY > 15)
                                    {
                                        tileY -= 16;
                                        fileX += 1;
                                    }
                                    else if (tileY < 0)
                                    {
                                        tileY += 16;
                                        fileX -= 1;
                                    }

                                    if (MapManager.Heightmaps.Blocks[fileY, fileX] == null)
                                    {
                                        previousEdit++;

                                        continue;
                                    }

                                    modifiedTiles.Add(new Commands.Tiles.Placement.Tile()
                                    {
                                        X = fileX,
                                        Y = fileY,
                                        TileX = tileX,
                                        TileY = tileY,
                                        OldID = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[tileY, tileX].TileID,
                                        NewID = ((TileTool)App.Form.ToolHost.Content).Tile
                                    });

                                    if (MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[tileY, tileX].TileID == ((TileTool)App.Form.ToolHost.Content).Tile)
                                        previousEdit++;

                                    MapManager.Heightmaps.Blocks[fileY, fileX].ChangeTile(tileX, tileY, ((TileTool)App.Form.ToolHost.Content).Tile);                               
                                }
                            }
                        }
                        else
                        {
                            modifiedTiles.Add(new Commands.Tiles.Placement.Tile()
                            {
                                X = HoveredTile.Value.X,
                                Y = HoveredTile.Value.Y,
                                TileX = HoveredTile.Value.TileX,
                                TileY = HoveredTile.Value.TileY,
                                OldID = MapManager.Heightmaps.Blocks[HoveredTile.Value.Y, HoveredTile.Value.X].TileFile.Tiles[HoveredTile.Value.TileY, HoveredTile.Value.TileX].TileID,
                                NewID = ((TileTool)App.Form.ToolHost.Content).Tile
                            });

                            if (modifiedTiles[0].NewID == modifiedTiles[0].OldID)
                                previousEdit++;

                            MapManager.Heightmaps.Blocks[HoveredTile.Value.Y, HoveredTile.Value.X].ChangeTile(HoveredTile.Value.TileX, HoveredTile.Value.TileY, ((TileTool)App.Form.ToolHost.Content).Tile);
                        }

                        if (previousEdit < gridSize * gridSize)
                        {
                            UndoManager.AddCommand(new Commands.Tiles.Placement()
                            {
                                Tiles = modifiedTiles.ToArray()
                            });
                        }
                    }
                    break;
            }
        }
    }
}