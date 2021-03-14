using System.Collections.Generic;
using System.Windows.Forms;
using Map_Editor.Engine.Tools.Interfaces;
using Map_Editor.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Map_Editor.Engine.Tools
{
    public class Brush : ITool
    {
        public struct TileSetInfo
        {
            /// <summary>
            /// Gets or sets the tile set.
            /// </summary>
            /// <value>The tile set.</value>
            public int TileSet { get; set; }

            /// <summary>
            /// Gets or sets the index of the tile.
            /// </summary>
            /// <value>The index of the tile.</value>
            public int TileIndex { get; set; }
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
        public Tiles.Tile? HoveredTile { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Tiles"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public Brush(GraphicsDevice device)
        {
            this.device = device;
        }

        /// <summary>
        /// Updates the tool.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            Cursor.Current = ((HoveredTile = MapManager.Heightmaps.PickTile()) == null) ? Cursors.Default : Cursors.Hand;

            if (HoveredTile == null)
                return;

            if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                return;

            List<Commands.Brush.Placement.Tile> modifiedTiles = new List<Commands.Brush.Placement.Tile>();

            int x = HoveredTile.Value.TileX;
            int y = HoveredTile.Value.TileY;
            int index = ((BrushTool)App.Form.ToolHost.Content).Brush;
            int brush = FileManager.TileSet.Brushes[((BrushTool)App.Form.ToolHost.Content).Brush].MinimumBrush;

            PutAdjacentTile(x - 1, y - 1, brush, 1, modifiedTiles);
            PutAdjacentTile(x, y - 1, brush, 3, modifiedTiles);
            PutAdjacentTile(x + 1, y - 1, brush, 2, modifiedTiles);
            PutAdjacentTile(x - 1, y, brush, 5, modifiedTiles);
            PutAdjacentTile(x + 1, y, brush, 10, modifiedTiles);
            PutAdjacentTile(x - 1, y + 1, brush, 4, modifiedTiles);
            PutAdjacentTile(x, y + 1, brush, 12, modifiedTiles);
            PutAdjacentTile(x + 1, y + 1, brush, 8, modifiedTiles);
            PutCurrentTile(x, y, index, brush, 15, modifiedTiles);

            if (modifiedTiles.Count > 0)
            {
                UndoManager.AddCommand(new Commands.Brush.Placement()
                {
                    Tiles = modifiedTiles.ToArray()
                });
            }
        }

        /// <summary>
        /// Puts the adjacent tile.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="modifiedTiles">The modified tiles.</param>
        private void PutAdjacentTile(int x, int y, int brush, int direction, List<Commands.Brush.Placement.Tile> modifiedTiles)
        {
            TileSetInfo tileSetInfo = CompoundTileFunction(x, y, brush, direction);

            if (tileSetInfo.TileIndex == 99)
            {
                tileSetInfo.TileSet = GetCompoundTile(x, y, brush);

                if (tileSetInfo.TileSet == 200)
                    return;

                tileSetInfo.TileIndex = direction;
                brush = FileManager.TileSet.Brushes[tileSetInfo.TileSet].MaximumBrush;

                switch (direction)
                {
                    case 1:
                        PutAdjacentTile(x - 1, y - 1, brush, 1, modifiedTiles);
                        PutAdjacentTile(x, y - 1, brush, 3, modifiedTiles);
                        PutAdjacentTile(x - 1, y, brush, 5, modifiedTiles);
                        break;

                    case 2:
                        PutAdjacentTile(x, y - 1, brush, 3, modifiedTiles);
                        PutAdjacentTile(x + 1, y - 1, brush, 2, modifiedTiles);
                        PutAdjacentTile(x + 1, y, brush, 10, modifiedTiles);
                        break;

                    case 3:
                        PutAdjacentTile(x, y - 1, brush, 3, modifiedTiles);
                        break;

                    case 4:
                        PutAdjacentTile(x - 1, y, brush, 5, modifiedTiles);
                        PutAdjacentTile(x - 1, y + 1, brush, 4, modifiedTiles);
                        PutAdjacentTile(x, y + 1, brush, 12, modifiedTiles);
                        break;

                    case 5:
                        PutAdjacentTile(x - 1, y, brush, 5, modifiedTiles);
                        break;

                    case 8:
                        PutAdjacentTile(x + 1, y, brush, 10, modifiedTiles);
                        PutAdjacentTile(x, y + 1, brush, 12, modifiedTiles);
                        PutAdjacentTile(x + 1, y + 1, brush, 8, modifiedTiles);
                        break;

                    case 10:
                        PutAdjacentTile(x + 1, y, brush, 10, modifiedTiles);
                        break;

                    case 12:
                        PutAdjacentTile(x, y + 1, brush, 12, modifiedTiles);
                        break;
                }
            }

            PutCurrentTile(x, y, tileSetInfo.TileSet, brush, tileSetInfo.TileIndex, modifiedTiles);
        }

        /// <summary>
        /// Puts the current tile.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="tileSet">The tile set.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="index">The index.</param>
        /// <param name="modifiedTiles">The modified tiles.</param>
        private void PutCurrentTile(int x, int y, int tileSet, int brush, int index, List<Commands.Brush.Placement.Tile> modifiedTiles)
        {
            int fileX = HoveredTile.Value.X;
            int fileY = HoveredTile.Value.Y;

            CheckValues(ref fileX, ref fileY, ref x, ref y);

            if (MapManager.Heightmaps.Blocks[fileY, fileX] == null)
                return;

            int previousBrush = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].BrushID;
            int previousTileSet = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileSetNumber;
            int previousTileIndex = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileIndex;
            int previousTileID = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileID;

            if (index == 15)
                MapManager.Heightmaps.Blocks[fileY, fileX].ChangeTile(x, y, brush, tileSet, index, FileManager.TileSet.Brushes[tileSet].TileNumberF);
            else
            {
                int tileOffset = 0;

                if (FileManager.TileSet.Brushes[tileSet].Direction < 0)
                    tileOffset = (15 - index) * FileManager.TileSet.Brushes[tileSet].TileCount;
                else
                    tileOffset = index * FileManager.TileSet.Brushes[tileSet].TileCount;

                MapManager.Heightmaps.Blocks[fileY, fileX].ChangeTile(x, y, brush, tileSet, index, FileManager.TileSet.Brushes[tileSet].TileNumber + tileOffset);
            }

            if (previousTileID != MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileID)
            {
                modifiedTiles.Add(new Commands.Brush.Placement.Tile()
                {
                    X = fileX,
                    Y = fileY,
                    TileX = x,
                    TileY = y,
                    OldID = previousTileID,
                    NewID = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileID,
                    OldBrush = previousBrush,
                    NewBrush = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].BrushID,
                    OldTileSet = previousTileSet,
                    NewTileSet = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileSetNumber,
                    OldTileIndex = previousTileIndex,
                    NewTileIndex = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileIndex
                });
            }
        }

        /// <summary>
        /// Gets the compound tile.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="brush">The brush.</param>
        /// <returns></returns>
        public int GetCompoundTile(int x, int y, int brush)
        {
            int fileX = HoveredTile.Value.X;
            int fileY = HoveredTile.Value.Y;

            CheckValues(ref fileX, ref fileY, ref x, ref y);

            if (MapManager.Heightmaps.Blocks[fileY, fileX] == null)
                return 200;

            int btTileIdx = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileIndex;
            int btBrushNO = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].BrushID;
            int btTileSetNO = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileSetNumber;

            for (int i = 0; i < FileManager.TileSet.Brushes.Length; i++)
            {
                int btMinBrush = FileManager.TileSet.Chains[brush, FileManager.TileSet.Brushes[btTileSetNO].MinimumBrush];

                if (btMinBrush == 99)
                    btMinBrush = FileManager.TileSet.Brushes[btTileSetNO].MinimumBrush;

                if ((FileManager.TileSet.Brushes[i].MaximumBrush == btMinBrush) && (FileManager.TileSet.Brushes[i].MinimumBrush == brush))
                    return i;
            }

            return 200;
        }

        /// <summary>
        /// Compounds the tile.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="tileIndex">Index of the tile.</param>
        /// <returns></returns>
        public TileSetInfo CompoundTileFunction(int x, int y, int brush, int tileIndex)
        {
            int fileX = HoveredTile.Value.X;
            int fileY = HoveredTile.Value.Y;

            CheckValues(ref fileX, ref fileY, ref x, ref y);

            TileSetInfo tileSetInfo = new TileSetInfo()
            {
                TileIndex = 99,
                TileSet = 200
            };

            if (MapManager.Heightmaps.Blocks[fileY, fileX] == null)
                return tileSetInfo;

            int tileNumber = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileIndex;
            int brushID = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].BrushID;
            int tileSet = MapManager.Heightmaps.Blocks[fileY, fileX].TileFile.Tiles[y, x].TileSetNumber;
            int minimumBrush = FileManager.TileSet.Chains[brush, FileManager.TileSet.Brushes[tileSet].MinimumBrush];

            if (minimumBrush == 99)
            {
                for (int i = 0; i < FileManager.TileSet.Brushes.Length; i++)
                {
                    minimumBrush = FileManager.TileSet.Brushes[tileSet].MinimumBrush;

                    if ((brush == FileManager.TileSet.Brushes[i].MinimumBrush) && (minimumBrush == FileManager.TileSet.Brushes[i].MaximumBrush))
                    {
                        if (((FileManager.TileSet.Brushes[i].MinimumBrush == FileManager.TileSet.Brushes[tileSet].MinimumBrush) && (FileManager.TileSet.Brushes[i].MaximumBrush == FileManager.TileSet.Brushes[tileSet].MaximumBrush)) || ((FileManager.TileSet.Brushes[i].MinimumBrush == FileManager.TileSet.Brushes[tileSet].MaximumBrush) && (FileManager.TileSet.Brushes[i].MaximumBrush == FileManager.TileSet.Brushes[tileSet].MinimumBrush)))
                        {
                            if (FileManager.TileSet.Brushes[i].MaximumBrush == FileManager.TileSet.Brushes[tileSet].MinimumBrush)
                                tileNumber = (byte)(15 - tileNumber);

                            tileSetInfo.TileIndex = (byte)(tileIndex | tileNumber);
                            tileSetInfo.TileSet = i;

                            return tileSetInfo;
                        }
                    }
                    else if ((i == tileSet) && (brush == minimumBrush))
                    {
                        tileSetInfo.TileIndex = (byte)(tileIndex | tileNumber);
                        tileSetInfo.TileSet = i;
                    }
                }
            }

            return tileSetInfo;
        }

        /// <summary>
        /// Checks the values.
        /// </summary>
        /// <param name="fileX">The file X.</param>
        /// <param name="fileY">The file Y.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public static void CheckValues(ref int fileX, ref int fileY, ref int x, ref int y)
        {
            if (x > 15)
            {
                x -= 16;
                fileY += 1;
            }
            else if (x < 0)
            {
                x += 16;
                fileY -= 1;
            }

            if (y > 15)
            {
                y -= 16;
                fileX += 1;
            }
            else if (y < 0)
            {
                y += 16;
                fileX -= 1;
            }
        }
    }
}