using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Forms;
using Map_Editor.Engine.Terrain;
using Map_Editor.Engine.Tools.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Map_Editor.Engine.Tools
{
    /// <summary>
    /// Height class.
    /// </summary>
    public class Height : ITool
    {
        /// <summary>
        /// Height Shape type.
        /// </summary>
        public enum HeightShapeMode
        {
            /// <summary>
            /// Square.
            /// </summary>
            Square = 0,

            /// <summary>
            /// Circle.
            /// </summary>
            Circle = 1
        }

        /// <summary>
        /// Height Tool type.
        /// </summary>
        public enum HeightToolMode
        {
            /// <summary>
            /// Raise.
            /// </summary>
            Raise,

            /// <summary>
            /// Lower.
            /// </summary>
            Lower,

            /// <summary>
            /// Flatten.
            /// </summary>
            Flatten,

            /// <summary>
            /// Smooth.
            /// </summary>
            Smooth
        }

        #region Member Declarations

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDevice device { get; set; }

        /// <summary>
        /// Gets or sets the mouse position.
        /// </summary>
        /// <value>The mouse position.</value>
        public Vector3 MousePosition { get; set; }

        /// <summary>
        /// Gets or sets the inner radius.
        /// </summary>
        /// <value>The inner radius.</value>
        public int InnerRadius { get; set; }

        /// <summary>
        /// Gets or sets the outter radius.
        /// </summary>
        /// <value>The outter radius.</value>
        public int OutterRadius { get; set; }

        /// <summary>
        /// Gets or sets the power.
        /// </summary>
        /// <value>The power.</value>
        public float Power { get; set; }

        /// <summary>
        /// Gets or sets the height shape.
        /// </summary>
        /// <value>The height shape.</value>
        public HeightShapeMode HeightShape { get; set; }

        /// <summary>
        /// Gets or sets the height tool.
        /// </summary>
        /// <value>The height tool.</value>
        public HeightToolMode HeightTool { get; set; }

        /// <summary>
        /// Gets or sets the height of the middle.
        /// </summary>
        /// <value>The height of the middle.</value>
        private float middleHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [mouse down].
        /// </summary>
        /// <value><c>true</c> if [mouse down]; otherwise, <c>false</c>.</value>
        private bool mouseDown { get; set; }

        /// <summary>
        /// Gets or sets the modify command.
        /// </summary>
        /// <value>The modify command.</value>
        private Commands.Height.Modify modifyCommand { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Height"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public Height(GraphicsDevice device)
        {
            this.device = device;

            HeightShape = HeightShapeMode.Square;
            HeightTool = HeightToolMode.Raise;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static float GetHeight(float x, float y)
        {
            bool unused = false;

            return GetHeight(x, y, ref unused);
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="foundHeight">if set to <c>true</c> [found height].</param>
        /// <returns></returns>
        public static float GetHeight(float x, float y, ref bool foundHeight)
        {
            int sectorY = (int)(x / 64.0f);
            int sectorX = (int)((4160.0 - y) / 64.0f);

            int blockX = (int)(x - (sectorY * 64));
            int blockY = (int)((4160 - y) - (sectorX * 64));

            if (MapManager.Heightmaps.Blocks[sectorY, sectorX] == null)
            {
                foundHeight = false;

                return 0.0f;
            }

            return MapManager.Heightmaps.Blocks[sectorY, sectorX].HeightFile.Position[blockY, blockX];
        }

        /// <summary>
        /// Sets the height.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="blocks">The blocks.</param>
        public static void SetHeight(float x, float y, float height, List<Heightmaps.Heightmap> blocks)
        {
            int sectorY = (int)(x / 64.0f);
            int sectorX = (int)((4160.0 - y) / 64.0f);

            int blockX = (int)(x - (sectorY * 64));
            int blockY = (int)((4160 - y) - (sectorX * 64));

            if (MapManager.Heightmaps.Blocks[sectorY, sectorX] != null)
            {
                MapManager.Heightmaps.Blocks[sectorY, sectorX].HeightFile.Position[blockY, blockX] = height;

                if (!blocks.Contains(MapManager.Heightmaps.Blocks[sectorY, sectorX]))
                    blocks.Add(MapManager.Heightmaps.Blocks[sectorY, sectorX]);
            }

            if (blockX == 0)
            {
                if (MapManager.Heightmaps.Blocks[sectorY - 1, sectorX] != null)
                {
                    MapManager.Heightmaps.Blocks[sectorY - 1, sectorX].HeightFile.Position[blockY, 64] = height;

                    if (!blocks.Contains(MapManager.Heightmaps.Blocks[sectorY - 1, sectorX]))
                        blocks.Add(MapManager.Heightmaps.Blocks[sectorY - 1, sectorX]);
                }
            }

            if (blockY == 0)
            {
                if (MapManager.Heightmaps.Blocks[sectorY, sectorX - 1] != null)
                {
                    MapManager.Heightmaps.Blocks[sectorY, sectorX - 1].HeightFile.Position[64, blockX] = height;

                    if (!blocks.Contains(MapManager.Heightmaps.Blocks[sectorY, sectorX - 1]))
                        blocks.Add(MapManager.Heightmaps.Blocks[sectorY, sectorX - 1]);
                }

                if (blockX == 0)
                {
                    if (MapManager.Heightmaps.Blocks[sectorY - 1, sectorX - 1] != null)
                    {
                        MapManager.Heightmaps.Blocks[sectorY - 1, sectorX - 1].HeightFile.Position[64, 64] = height;

                        if (!blocks.Contains(MapManager.Heightmaps.Blocks[sectorY - 1, sectorX - 1]))
                            blocks.Add(MapManager.Heightmaps.Blocks[sectorY - 1, sectorX - 1]);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the tool.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            Cursor.Current = ((MousePosition = MapManager.Heightmaps.PickPosition()) == Vector3.Zero) ? Cursors.Default : Cursors.Hand;

            if (!mouseState.Intersects(device.Viewport))
            {
                Cursor.Current = Cursors.Default;

                mouseDown = mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;

                if (modifyCommand != null)
                {
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            if (MapManager.Heightmaps.Blocks[y, x] == null)
                                continue;

                            modifyCommand.Blocks[y, x].NewHeight = (float[,])MapManager.Heightmaps.Blocks[y, x].HeightFile.Position.Clone();
                        }
                    }

                    modifyCommand = null;
                }

                return;
            }

            if (MousePosition == Vector3.Zero || mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                mouseDown = mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;

                if (modifyCommand != null)
                {
                    for (int y = 0; y < 100; y++)
                    {
                        for (int x = 0; x < 100; x++)
                        {
                            if (MapManager.Heightmaps.Blocks[y, x] == null)
                                continue;

                            modifyCommand.Blocks[y, x].NewHeight = (float[,])MapManager.Heightmaps.Blocks[y, x].HeightFile.Position.Clone();
                        }
                    }

                    modifyCommand = null;
                }

                return;
            }

            int cellX = (int)((MousePosition.X / 2.5f) + 0.5f);
            int cellY = (int)((MousePosition.Y / 2.5f) + 0.5f);

            List<Heightmaps.Heightmap> blocks = new List<Heightmaps.Heightmap>();

            if (!mouseDown)
                UndoManager.AddCommand(modifyCommand = new Commands.Height.Modify());

            switch (HeightShape)
            {
                case HeightShapeMode.Square:
                    {
                        switch (HeightTool)
                        {
                            case HeightToolMode.Raise:
                            case HeightToolMode.Lower:
                            case HeightToolMode.Flatten:
                                {
                                    if (!mouseDown)
                                        middleHeight = GetHeight(cellX, cellY);

                                    int originalY = cellY;
                                    int maximumX = cellX + OutterRadius;
                                    int maximumY = cellY + OutterRadius;

                                    int innerMinX = cellX - InnerRadius;
                                    int innerMinY = cellY - InnerRadius;

                                    int innerMaxX = cellX + InnerRadius;
                                    int innerMaxY = cellY + InnerRadius;

                                    for (cellX = cellX - OutterRadius; cellX <= maximumX; cellX++)
                                    {
                                        for (cellY = cellY - OutterRadius; cellY <= maximumY; cellY++)
                                        {
                                            float currentHeight = GetHeight(cellX, cellY);
                                            float raiseHeight = Power / 10.0f;

                                            if (cellX < innerMinX || cellX > innerMaxX || cellY < innerMinY || cellY > innerMaxY)
                                            {
                                                int distance = 0;
                                                int distance2 = 0;

                                                if (cellX <= innerMinX)
                                                    distance = innerMinX - cellX;
                                                else
                                                    distance = cellX - innerMaxX;

                                                if (cellY <= innerMinY)
                                                    distance2 = innerMinY - cellY;
                                                else
                                                    distance2 = cellY - innerMaxY;

                                                distance = (distance2 > distance) ? distance2 : distance;
                                                raiseHeight = (raiseHeight / (float)distance);
                                            }

                                            switch (HeightTool)
                                            {
                                                case HeightToolMode.Raise:
                                                    currentHeight += raiseHeight;
                                                    break;
                                                case HeightToolMode.Lower:
                                                    currentHeight -= raiseHeight;
                                                    break;
                                                case HeightToolMode.Flatten:
                                                    {
                                                        if (currentHeight > middleHeight)
                                                        {
                                                            currentHeight -= raiseHeight;

                                                            if (currentHeight < middleHeight)
                                                                currentHeight = middleHeight;
                                                        }
                                                        else
                                                        {
                                                            currentHeight += raiseHeight;

                                                            if (currentHeight > middleHeight)
                                                                currentHeight = middleHeight;
                                                        }
                                                    }
                                                    break;
                                            }

                                            SetHeight(cellX, cellY, currentHeight, blocks);
                                        }
                                        cellY = originalY;
                                    }
                                }
                                break;
                            case HeightToolMode.Smooth:
                                {
                                    int xStart = cellX;
                                    int yStart = cellY;

                                    int xEnd = cellX + OutterRadius;
                                    int yEnd = cellY + OutterRadius;

                                    for (cellX = xStart; cellX <= xEnd; cellX++)
                                    {
                                        for (cellY = yStart; cellY <= yEnd; cellY++)
                                        {
                                            float valueCount = 0;
                                            float value = 0;

                                            int filterX = 0;
                                            int filterY = 0;

                                            for (int myX = cellX - 1; myX <= cellX + 1; myX++)
                                            {
                                                for (int myY = cellY - 1; myY <= cellY + 1; myY++)
                                                {
                                                    bool isFactor = true;

                                                    value += GetHeight(myX, myY, ref isFactor);

                                                    if (isFactor)
                                                        valueCount += 1.0f;

                                                    filterY++;
                                                }

                                                filterY = 0;

                                                filterX++;
                                            }

                                            if (valueCount < 1.0f)
                                                valueCount = 1.0f;

                                            if (value != 0)
                                                value /= valueCount;

                                            SetHeight(cellX, cellY, value, blocks);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case HeightShapeMode.Circle:
                    {
                        switch (HeightTool)
                        {
                            case HeightToolMode.Raise:
                            case HeightToolMode.Lower:
                            case HeightToolMode.Flatten:
                                {
                                    int centerX = cellX;
                                    int centerY = cellY;

                                    int endX = cellX + OutterRadius;
                                    int endY = cellY + OutterRadius;

                                    cellX -= OutterRadius;
                                    cellY -= OutterRadius;

                                    int originalY = cellY;

                                    float radiusDifference = OutterRadius - InnerRadius;
                                    float rSquared = 0;

                                    if (radiusDifference != 0)
                                        rSquared = radiusDifference * radiusDifference;

                                    if (!mouseDown)
                                        middleHeight = GetHeight(centerX, centerY);

                                    for (; cellX <= endX; cellX++)
                                    {
                                        for (; cellY <= endY; cellY++)
                                        {
                                            float dX = cellX - centerX;
                                            dX *= dX;

                                            float dY = cellY - centerY;
                                            dY *= dY;

                                            float distance = (float)Math.Sqrt(dY + dX);

                                            if (distance > OutterRadius)
                                                continue;

                                            float currentHeight = GetHeight(cellX, cellY);
                                            float raiseHeight = Power / 10.0f;

                                            if (radiusDifference != 0 && distance > InnerRadius)
                                            {
                                                distance -= InnerRadius;
                                                distance *= distance;
                                                distance = (float)Math.Sqrt(rSquared - distance);

                                                raiseHeight *= (distance / radiusDifference);
                                            }

                                            switch (HeightTool)
                                            {
                                                case HeightToolMode.Raise:
                                                    currentHeight += raiseHeight;
                                                    break;
                                                case HeightToolMode.Lower:
                                                    currentHeight -= raiseHeight;
                                                    break;
                                                case HeightToolMode.Flatten:
                                                    {
                                                        if (currentHeight > middleHeight)
                                                        {
                                                            currentHeight -= raiseHeight;

                                                            if (currentHeight < middleHeight)
                                                                currentHeight = middleHeight;
                                                        }
                                                        else
                                                        {
                                                            currentHeight += raiseHeight;

                                                            if (currentHeight > middleHeight)
                                                                currentHeight = middleHeight;
                                                        }
                                                    }
                                                    break;
                                            }

                                            SetHeight(cellX, cellY, currentHeight, blocks);
                                        }

                                        cellY = originalY;
                                    }
                                }
                                break;
                            case HeightToolMode.Smooth:
                                {
                                    int xStart = cellX;
                                    int yStart = cellY;

                                    xStart -= OutterRadius;
                                    yStart -= OutterRadius;

                                    int xEnd = xStart + (OutterRadius * 2);
                                    int yEnd = yStart + (OutterRadius * 2);

                                    int xCenter = xStart + OutterRadius;
                                    int yCenter = yStart + OutterRadius;

                                    for (int cellXB = xStart; cellXB <= xEnd; cellXB++)
                                    {
                                        for (int cellYB = yStart; cellYB <= yEnd; cellYB++)
                                        {
                                            float dX = cellXB - xCenter;
                                            dX *= dX;

                                            float dY = cellYB - yCenter;
                                            dY *= dY;

                                            float distance = (float)Math.Sqrt(dY + dX);

                                            if (distance > OutterRadius)
                                                continue;

                                            float valueCount = 0;
                                            float value = 0;

                                            int filterX = 0;
                                            int filterY = 0;

                                            for (int myX = cellXB - 1; myX <= cellXB + 1; myX++)
                                            {
                                                for (int myY = cellYB - 1; myY <= cellYB + 1; myY++)
                                                {
                                                    bool isFactor = true;

                                                    value += GetHeight(myX, myY, ref isFactor);

                                                    if (isFactor)
                                                        valueCount += 1.0f;

                                                    filterY++;
                                                }

                                                filterY = 0;

                                                filterX++;
                                            }

                                            if (valueCount < 1.0f)
                                                valueCount = 1.0f;

                                            if (value != 0)
                                                value /= valueCount;

                                            SetHeight(cellXB, cellYB, value, blocks);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    }
            }

            blocks.ForEach(delegate(Heightmaps.Heightmap block)
            {
                block.BuildVertices();
                block.BuildBoundingBoxes();
            });

            mouseDown = true;
        }
    }
}