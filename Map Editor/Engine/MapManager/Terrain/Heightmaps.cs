using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Map_Editor.Engine.Map;
using Map_Editor.Engine.Tools;
using Map_Editor.Forms.Controls;
using Map_Editor.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Map_Editor.Engine.Terrain
{
    /// <summary>
    /// Heightmaps class.
    /// </summary>
    public class Heightmaps : DrawableGameComponent
    {
        #region Sub Classes

        /// <summary>
        /// Heightmap class.
        /// </summary>
        public class Heightmap
        {
            /// <summary>
            /// Tile structure.
            /// </summary>
            public struct Tile
            {
                /// <summary>
                /// Gets or sets the tile I d1.
                /// </summary>
                /// <value>The tile I d1.</value>
                public int TileID1 { get; set; }

                /// <summary>
                /// Gets or sets the tile I d2.
                /// </summary>
                /// <value>The tile I d2.</value>
                public int TileID2 { get; set; }

                /// <summary>
                /// Gets or sets the bounding box.
                /// </summary>
                /// <value>The bounding box.</value>
                public BoundingBox BoundingBox { get; set; }

                /// <summary>
                /// Gets or sets the bounding boxes.
                /// </summary>
                /// <value>The bounding boxes.</value>
                public BoundingBox[,] BoundingBoxes { get; set; }
            };

            #region Accessors

            /// <summary>
            /// Gets the tile tool.
            /// </summary>
            /// <value>The tile tool.</value>
            public Tools.Tiles TileTool
            {
                get { return (Tools.Tiles)ToolManager.Tool; }
            }

            /// <summary>
            /// Gets the brush tool.
            /// </summary>
            /// <value>The brush tool.</value>
            public Tools.Brush BrushTool
            {
                get { return (Tools.Brush)ToolManager.Tool; }
            }

            /// <summary>
            /// Gets the height tool.
            /// </summary>
            /// <value>The height tool.</value>
            public Tools.Height HeightTool
            {
                get { return (Tools.Height)ToolManager.Tool; }
            }

            #endregion

            #region Member Declarations

            /// <summary>
            /// Gets or sets the tile file.
            /// </summary>
            /// <value>The tile file.</value>
            public TIL TileFile { get; set; }

            /// <summary>
            /// Gets or sets the height file.
            /// </summary>
            /// <value>The height file.</value>
            public HIM HeightFile { get; set; }

            /// <summary>
            /// Gets or sets the shadow map.
            /// </summary>
            /// <value>The shadow map.</value>
            public Texture2D ShadowMap { get; set; }

            /// <summary>
            /// Gets or sets the shadow map raw.
            /// </summary>
            /// <value>The shadow map raw.</value>
            public Color[] ShadowMapRaw { get; set; }

            /// <summary>
            /// Gets or sets the bounding box.
            /// </summary>
            /// <value>The bounding box.</value>
            public BoundingBox BoundingBox { get; set; }

            /// <summary>
            /// Gets or sets the vertex buffer.
            /// </summary>
            /// <value>The vertex buffer.</value>
            public VertexBuffer VertexBuffer { get; set; }

            /// <summary>
            /// Gets or sets the tiles.
            /// </summary>
            /// <value>The tiles.</value>
            public Tile[,] Tiles { get; set; }

            /// <summary>
            /// Gets or sets the vertices.
            /// </summary>
            /// <value>The vertices.</value>
            public HIM.Vertex[] Vertices { get; set; }

            /// <summary>
            /// Gets or sets the device.
            /// </summary>
            /// <value>The device.</value>
            private GraphicsDevice device { get; set; }

            /// <summary>
            /// Gets or sets the map offset.
            /// </summary>
            /// <value>The map offset.</value>
            private Vector2 mapOffset { get; set; }

            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="Heightmap"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public Heightmap(GraphicsDevice device)
            {
                this.device = device;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Heightmap"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="filePath">The file path.</param>
            public Heightmap(GraphicsDevice device, string filePath)
                : this(device)
            {
                Load(filePath);
            }

            /// <summary>
            /// Loads the specified file.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            public void Load(string filePath)
            {
                string mapFolder = Path.GetDirectoryName(filePath);

                TileFile = new TIL(filePath.ToUpper().Replace(".HIM", ".TIL"));
                HeightFile = new HIM(filePath);

                string[] position = (Path.GetFileName(filePath)).Remove(5, 4).Split('_');
                mapOffset = new Vector2(Convert.ToInt32(position[1]), Convert.ToInt32(position[0]));

                const int IMAGE_DIMENSION = 512;

                try
                {
                    using (GraphicsDevice graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Hardware, new System.Windows.Forms.Panel().Handle, new PresentationParameters()
                    {
                        AutoDepthStencilFormat = DepthFormat.Depth24,
                        BackBufferCount = 1,
                        BackBufferFormat = SurfaceFormat.Color,
                        BackBufferHeight = IMAGE_DIMENSION,
                        BackBufferWidth = IMAGE_DIMENSION,
                        EnableAutoDepthStencil = true,
                        FullScreenRefreshRateInHz = 0,
                        IsFullScreen = false,
                        MultiSampleQuality = 0,
                        MultiSampleType = MultiSampleType.NonMaskable,
                        PresentationInterval = PresentInterval.One,
                        PresentOptions = PresentOptions.None,
                        RenderTargetUsage = RenderTargetUsage.DiscardContents,
                        SwapEffect = SwapEffect.Discard
                    }))
                    {
                        Texture2D shadowMap = Texture2D.FromFile(graphicsDevice, string.Format(@"{0}\{1}_{2}\{1}_{2}_PLANELIGHTINGMAP.DDS", mapFolder, mapOffset.Y, mapOffset.X));

                        SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);
                        RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, IMAGE_DIMENSION, IMAGE_DIMENSION, 1, SurfaceFormat.Color, MultiSampleType.NonMaskable, 0);

                        graphicsDevice.SetRenderTarget(0, renderTarget);

                        spriteBatch.Begin();
                        spriteBatch.Draw(shadowMap, new Rectangle(0, 0, IMAGE_DIMENSION, IMAGE_DIMENSION), Color.White);
                        spriteBatch.End();

                        graphicsDevice.SetRenderTarget(0, null);

                        Texture2D resizedTexture = renderTarget.GetTexture();

                        ShadowMapRaw = new Color[IMAGE_DIMENSION * IMAGE_DIMENSION];
                        resizedTexture.GetData<Color>(ShadowMapRaw);

                        resizedTexture.Dispose();
                        spriteBatch.Dispose();
                        shadowMap.Dispose();
                        renderTarget.Dispose();
                    }

                    ShadowMap = new Texture2D(device, IMAGE_DIMENSION, IMAGE_DIMENSION);
                    ShadowMap.SetData<Color>(ShadowMapRaw);
                }
                catch
                {
                    Output.WriteLine(Output.MessageType.Error, string.Format("Error Loading Texture: {0}", string.Format(@"{0}\{1}_{2}\{1}_{2}_PLANELIGHTINGMAP.DDS", mapFolder, mapOffset.Y, mapOffset.X)));

                    ShadowMap = new Texture2D(device, IMAGE_DIMENSION, IMAGE_DIMENSION);

                    for (int x = 0; x < IMAGE_DIMENSION; x++)
                    {
                        for (int y = 0; y < IMAGE_DIMENSION; y++)
                            ShadowMapRaw[(x * IMAGE_DIMENSION) + y] = new Color(127, 127, 127);
                    }

                    ShadowMap.SetData<Color>(ShadowMapRaw);
                }

                Vertices = new HIM.Vertex[6400];
                Tiles = new Tile[16, 16];

                BuildVertices();
                BuildBoundingBoxes();
                BuildBuffer();

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        if (FileManager.ZON.Textures[Tiles[y, x].TileID1].Tile == null)
                            FileManager.ZON.Textures[Tiles[y, x].TileID1].Tile = Texture2D.FromFile(device, FileManager.ZON.Textures[Tiles[y, x].TileID1].Path);

                        if (FileManager.ZON.Textures[Tiles[y, x].TileID2].Tile == null)
                            FileManager.ZON.Textures[Tiles[y, x].TileID2].Tile = Texture2D.FromFile(device, FileManager.ZON.Textures[Tiles[y, x].TileID2].Path);
                    }
                }
            }

            /// <summary>
            /// Builds the vertices.
            /// </summary>
            public void BuildVertices()
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        ZON.RotationType textureRotation = FileManager.ZON.Tiles[TileFile.Tiles[y, x].TileID].Rotation;

                        for (int vy = 0; vy < 5; vy++)
                        {
                            for (int vx = 0; vx < 5; vx++)
                            {
                                int dx4 = (y * 4) + vx;
                                int dy4 = (x * 4) + vy;

                                int vertex = (((y * 16) + x) * 25) + (vy * 5) + vx;

                                Vertices[vertex] = new HIM.Vertex()
                                {
                                    Position = new Vector3((dy4 * 2.5f) + (mapOffset.Y * 160.0f), 10400.0f - ((dx4 * 2.5f) + (mapOffset.X * 160.0f)), HeightFile.Position[dx4, dy4]),
                                    BottomTexture = new Vector2(vy / 4.0f, vx / 4.0f),
                                    TopTexture = RotationToCoordinates(textureRotation, new Vector2(vy / 4.0f, vx / 4.0f)),
                                    ShadowTexture = new Vector2(dy4 / 64.0f, dx4 / 64.0f)
                                };
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Builds the bounding boxes.
            /// </summary>
            public void BuildBoundingBoxes()
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        int[] numbers = new int[25];

                        for (int j = 0; j < 25; j++)
                            numbers[j] = (((y * 16) + x) * 25) + j;

                        var tileVertices = from n in numbers
                                           select Vertices[n].Position;
                        
                        Tiles[y, x] = new Tile()
                        {
                            BoundingBox = BoundingBox.CreateFromPoints(tileVertices),
                            TileID1 = FileManager.ZON.Tiles[TileFile.Tiles[y, x].TileID].ID1,
                            TileID2 = FileManager.ZON.Tiles[TileFile.Tiles[y, x].TileID].ID2,
                            BoundingBoxes = new BoundingBox[4, 4]
                        };

                        for (int by = 0; by < 4; by++)
                        {
                            for (int bx = 0; bx < 4; bx++)
                            {
                                int dx4a = (y * 4) + bx;
                                int dy4a = (x * 4) + by;

                                int dx4b = (y * 4) + bx + 1;
                                int dy4b = (x * 4) + by + 1;

                                Tiles[y, x].BoundingBoxes[by, bx] = new BoundingBox()
                                {
                                    Min = new Vector3()
                                    {
                                        X = (dy4a * 2.5f) + (mapOffset.Y * 160.0f),
                                        Y = 10400.0f - ((dx4a * 2.5f) + (mapOffset.X * 160.0f)),
                                        Z = HeightFile.Position[dx4a, dy4a]
                                    },
                                    Max = new Vector3()
                                    {
                                        X = (dy4b * 2.5f) + (mapOffset.Y * 160.0f),
                                        Y = 10400.0f - ((dx4b * 2.5f) + (mapOffset.X * 160.0f)),
                                        Z = HeightFile.Position[dx4b, dy4b]
                                    }
                                };
                            }
                        }
                    }
                }

                BoundingBox = BoundingBox.CreateFromPoints(Vertices.Select(item => item.Position));
            }

            public void ChangeTile(int x, int y, int tileID)
            {
                if (TileFile.Tiles[y, x].TileID == tileID)
                    return;

                ChangeTile(x, y, 0, 0, 0, tileID);
            }

            /// <summary>
            /// Changes the tile.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="tile">The tile.</param>
            public void ChangeTile(int x, int y, int brush, int tileSet, int tileIndex, int tileID)
            {
                TileFile.Tiles[y, x].BrushID = (byte)brush;
                TileFile.Tiles[y, x].TileSetNumber = (byte)tileSet;
                TileFile.Tiles[y, x].TileIndex = (byte)tileIndex;
                TileFile.Tiles[y, x].TileID = tileID;

                ZON.RotationType textureRotation = FileManager.ZON.Tiles[TileFile.Tiles[y, x].TileID].Rotation;

                for (int vy = 0; vy < 5; vy++)
                {
                    for (int vx = 0; vx < 5; vx++)
                    {
                        int vertex = (((y * 16) + x) * 25) + (vy * 5) + vx;

                        Vertices[vertex].BottomTexture = new Vector2(vy / 4.0f, vx / 4.0f);
                        Vertices[vertex].TopTexture = RotationToCoordinates(textureRotation, new Vector2(vy / 4.0f, vx / 4.0f));
                    }
                }

                Tiles[y, x].TileID1 = FileManager.ZON.Tiles[TileFile.Tiles[y, x].TileID].ID1;
                Tiles[y, x].TileID2 = FileManager.ZON.Tiles[TileFile.Tiles[y, x].TileID].ID2;

                if (FileManager.ZON.Textures[Tiles[y, x].TileID1].Tile == null)
                    FileManager.ZON.Textures[Tiles[y, x].TileID1].Tile = Texture2D.FromFile(device, FileManager.ZON.Textures[Tiles[y, x].TileID1].Path);

                if (FileManager.ZON.Textures[Tiles[y, x].TileID2].Tile == null)
                    FileManager.ZON.Textures[Tiles[y, x].TileID2].Tile = Texture2D.FromFile(device, FileManager.ZON.Textures[Tiles[y, x].TileID2].Path);
            }

            /// <summary>
            /// Builds the buffer.
            /// </summary>
            public void BuildBuffer()
            {
                if (VertexBuffer != null)
                    VertexBuffer.Dispose();

                VertexBuffer = new VertexBuffer(device, HIM.Vertex.SIZE_IN_BYTES * Vertices.Length, BufferUsage.WriteOnly);
                VertexBuffer.SetData(Vertices);
            }

            /// <summary>
            /// Picks the terrain and returns a position.
            /// </summary>
            /// <param name="boundingFrustum">The bounding frustum.</param>
            /// <param name="ray">The ray.</param>
            /// <returns>The positions</returns>
            public Vector3[] PickPosition(BoundingFrustum boundingFrustum, Ray ray)
            {
                if (!boundingFrustum.Intersects(BoundingBox) || !ray.Intersects(BoundingBox).HasValue)
                    return new Vector3[0];

                MouseState mouseState = Mouse.GetState();

                Vector3 nearPoint = device.Viewport.Unproject(new Vector3(mouseState.X, mouseState.Y, 0.0f), CameraManager.Projection, CameraManager.View, Matrix.Identity);
                Vector3 farPoint = device.Viewport.Unproject(new Vector3(mouseState.X, mouseState.Y, 1.0f), CameraManager.Projection, CameraManager.View, Matrix.Identity);

                List<Vector3> vertexPositions = new List<Vector3>();

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        if (!ray.Intersects(Tiles[y, x].BoundingBox).HasValue)
                            continue;

                        for (int vy = 0; vy < 4; vy++)
                        {
                            for (int vx = 0; vx < 4; vx++)
                            {
                                if (!ray.Intersects(Tiles[y, x].BoundingBoxes[vy, vx]).HasValue)
                                    continue;

                                Vector3 vectorPosition = Vector3.Zero;

                                Vector3 p1 = new Vector3(Tiles[y, x].BoundingBoxes[vy, vx].Min.X, Tiles[y, x].BoundingBoxes[vy, vx].Min.Y, Tiles[y, x].BoundingBoxes[vy, vx].Max.Z);
                                Vector3 p2 = new Vector3(Tiles[y, x].BoundingBoxes[vy, vx].Min.X, Tiles[y, x].BoundingBoxes[vy, vx].Max.Y, Tiles[y, x].BoundingBoxes[vy, vx].Max.Z);
                                Vector3 p3 = new Vector3(Tiles[y, x].BoundingBoxes[vy, vx].Max.X, Tiles[y, x].BoundingBoxes[vy, vx].Max.Y, Tiles[y, x].BoundingBoxes[vy, vx].Max.Z);

                                if ((vectorPosition = CheckLineTri(nearPoint, farPoint, p1, p2, p3)) != Vector3.Zero)
                                    vertexPositions.Add(vectorPosition);

                                p1 = new Vector3(Tiles[y, x].BoundingBoxes[vy, vx].Min.X, Tiles[y, x].BoundingBoxes[vy, vx].Min.Y, Tiles[y, x].BoundingBoxes[vy, vx].Max.Z);
                                p2 = new Vector3(Tiles[y, x].BoundingBoxes[vy, vx].Max.X, Tiles[y, x].BoundingBoxes[vy, vx].Min.Y, Tiles[y, x].BoundingBoxes[vy, vx].Max.Z);
                                p3 = new Vector3(Tiles[y, x].BoundingBoxes[vy, vx].Max.X, Tiles[y, x].BoundingBoxes[vy, vx].Max.Y, Tiles[y, x].BoundingBoxes[vy, vx].Max.Z);

                                if ((vectorPosition = CheckLineTri(nearPoint, farPoint, p1, p2, p3)) != Vector3.Zero)
                                    vertexPositions.Add(vectorPosition);
                            }
                        }
                    }
                }

                return vertexPositions.ToArray();
            }

            /// <summary>
            /// Gets a position to use from 3 vertex position.
            /// Credits go to ExJam, ported to C# by xadet.
            /// </summary>
            /// <param name="L1">The l1.</param>
            /// <param name="L2">The l2.</param>
            /// <param name="PV1">The P v1.</param>
            /// <param name="PV2">The P v2.</param>
            /// <param name="PV3">The P v3.</param>
            /// <returns>The position.</returns>
            public Vector3 CheckLineTri(Vector3 L1, Vector3 L2, Vector3 PV1, Vector3 PV2, Vector3 PV3)
            {
                Vector3 normalVector = Vector3.Cross(PV2 - PV1, PV3 - PV1);
                normalVector.Normalize();

                float distanceA = Vector3.Dot(L1 - PV1, normalVector);
                float distanceB = Vector3.Dot(L2 - PV1, normalVector);

                if ((distanceA * distanceB) >= 0.0f || distanceA == distanceB)
                    return Vector3.Zero;

                Vector3 intersectVector = L1 + (L2 - L1) * (-distanceA / (distanceB - distanceA));

                Vector3 testVector = Vector3.Cross(normalVector, PV2 - PV1);

                if (Vector3.Dot(testVector, intersectVector - PV1) < 0.0f)
                    return Vector3.Zero;

                testVector = Vector3.Cross(normalVector, PV3 - PV2);

                if (Vector3.Dot(testVector, intersectVector - PV2) < 0.0f)
                    return Vector3.Zero;

                testVector = Vector3.Cross(normalVector, PV1 - PV3);

                if (Vector3.Dot(testVector, intersectVector - PV1) < 0.0f)
                    return Vector3.Zero;

                return intersectVector;
            }

            /// <summary>
            /// Picks the tiles.
            /// </summary>
            /// <param name="boundingFrustum">The bounding frustum.</param>
            /// <param name="ray">The ray.</param>
            /// <returns>The tile index.</returns>
            public Vector2? PickTile(BoundingFrustum boundingFrustum, Ray ray)
            {
                if (!boundingFrustum.Intersects(BoundingBox) || !ray.Intersects(BoundingBox).HasValue)
                    return null;

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        if (ray.Intersects(Tiles[y, x].BoundingBox).HasValue)
                            return new Vector2(x, y);                        
                    }
                }

                return null;
            }

            /// <summary>
            /// Draw the heightmap.
            /// </summary>
            /// <param name="boundingFrustum">The bounding frustum.</param>
            /// <param name="shader">The shader.</param>
            public void Draw(BoundingFrustum boundingFrustum, Effect shader)
            {
                if (!boundingFrustum.OnScreen(BoundingBox))
                    return;

                if (ToolManager.GetToolMode() != ToolManager.ToolMode.Height && ToolManager.GetToolMode() != ToolManager.ToolMode.Tiles && ToolManager.GetToolMode() != ToolManager.ToolMode.Brush)
                    device.Vertices[0].SetSource(VertexBuffer, 0, HIM.Vertex.SIZE_IN_BYTES);

                device.RenderState.AlphaBlendEnable = true;

                shader.SetValue("ShadowTexture", ShadowMap);

                if (ToolManager.GetToolMode() == ToolManager.ToolMode.Height)
                {
                    shader.SetValue("SelectionPosition", HeightTool.MousePosition);
                    shader.SetValue("InnerRadius", HeightTool.InnerRadius * 2.5f);
                    shader.SetValue("OutterRadius", HeightTool.OutterRadius * 2.5f);
                    shader.SetValue("HeightMode", (int)HeightTool.HeightShape);
                }

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        if (!boundingFrustum.OnScreen(Tiles[y, x].BoundingBox))
                            continue;

                        if (ToolManager.GetToolMode() == ToolManager.ToolMode.Tiles)
                        {
                            bool modifyingCondition = TileTool.ToolMode == Tools.Tiles.TileToolType.Modify && ((TileTool.HoveredTile == null) ? false : (mapOffset.X == TileTool.HoveredTile.Value.X && mapOffset.Y == TileTool.HoveredTile.Value.Y && TileTool.HoveredTile.Value.TileX == x && TileTool.HoveredTile.Value.TileY == y));

                            if (((TileTool)App.Form.ToolHost.Content).MultiplePlacement.IsChecked == true)
                            {
                                if (TileTool.ToolMode != Tools.Tiles.TileToolType.Modify || TileTool.HoveredTile == null)
                                {
                                    modifyingCondition = false;

                                    goto ModifyingCondition;
                                }

                                int gridSize = 3 + (((TileTool)App.Form.ToolHost.Content).MultipleSelection.SelectedIndex * 2);

                                for (int ty = TileTool.HoveredTile.Value.TileY - (gridSize / 2); ty <= TileTool.HoveredTile.Value.TileY + (gridSize / 2); ty++)
                                {
                                    for (int tx = TileTool.HoveredTile.Value.TileX - (gridSize / 2); tx <= TileTool.HoveredTile.Value.TileX + (gridSize / 2); tx++)
                                    {
                                        int tileX = tx;
                                        int fileX = TileTool.HoveredTile.Value.X;

                                        int tileY = ty;
                                        int fileY = TileTool.HoveredTile.Value.Y;

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

                                        if (mapOffset.X == fileX && mapOffset.Y == fileY && tileX == x && tileY == y)
                                        {
                                            modifyingCondition = true;

                                            goto ModifyingCondition;
                                        }
                                    }
                                }
                            }

                        ModifyingCondition:
                            if (modifyingCondition)
                            {
                                shader.SetValue("BottomTexture", FileManager.ZON.Textures[FileManager.ZON.Tiles[((TileTool)App.Form.ToolHost.Content).Tile].ID1].Tile);
                                shader.SetValue("TopTexture", FileManager.ZON.Textures[FileManager.ZON.Tiles[((TileTool)App.Form.ToolHost.Content).Tile].ID2].Tile);
                                shader.SetValue("Rotation", (int)FileManager.ZON.Tiles[((TileTool)App.Form.ToolHost.Content).Tile].Rotation);
                                shader.SetValue("Highlight", true);
                                shader.SetValue("ModifyingTile", true);
                            }
                            else
                            {
                                shader.SetValue("BottomTexture", FileManager.ZON.Textures[Tiles[y, x].TileID1].Tile);
                                shader.SetValue("TopTexture", FileManager.ZON.Textures[Tiles[y, x].TileID2].Tile);
                                shader.SetValue("Highlight", TileTool.ToolMode == Tools.Tiles.TileToolType.Pick && ((TileTool.HoveredTile == null) ? false : (mapOffset.X == TileTool.HoveredTile.Value.X && mapOffset.Y == TileTool.HoveredTile.Value.Y && TileTool.HoveredTile.Value.TileX == x && TileTool.HoveredTile.Value.TileY == y)));
                                shader.SetValue("ModifyingTile", false);
                            }
                        }
                        else if (ToolManager.GetToolMode() == ToolManager.ToolMode.Brush)
                        {
                            bool modifyingCondition = BrushTool.HoveredTile != null;

                            if (modifyingCondition)
                            {
                                int tileX = BrushTool.HoveredTile.Value.TileX;
                                int fileX = BrushTool.HoveredTile.Value.X;

                                int tileY = BrushTool.HoveredTile.Value.TileY;
                                int fileY = BrushTool.HoveredTile.Value.Y;

                                modifyingCondition = mapOffset.X == fileX && mapOffset.Y == fileY && tileX == x && tileY == y;
                            }

                            if (modifyingCondition)
                            {
                                int tileID = FileManager.TileSet.Brushes[((BrushTool)App.Form.ToolHost.Content).Brush].TileNumberF;

                                shader.SetValue("BottomTexture", FileManager.ZON.Textures[FileManager.ZON.Tiles[tileID].ID1].Tile);
                                shader.SetValue("TopTexture", FileManager.ZON.Textures[FileManager.ZON.Tiles[tileID].ID2].Tile);
                                shader.SetValue("Rotation", (int)FileManager.ZON.Tiles[tileID].Rotation);
                                shader.SetValue("Highlight", true);
                                shader.SetValue("ModifyingTile", true);
                            }
                            else
                            {
                                shader.SetValue("BottomTexture", FileManager.ZON.Textures[Tiles[y, x].TileID1].Tile);
                                shader.SetValue("TopTexture", FileManager.ZON.Textures[Tiles[y, x].TileID2].Tile);
                                shader.SetValue("Highlight", false);
                                shader.SetValue("ModifyingTile", false);
                            }
                        }
                        else
                        {
                            shader.SetValue("BottomTexture", FileManager.ZON.Textures[Tiles[y, x].TileID1].Tile);
                            shader.SetValue("TopTexture", FileManager.ZON.Textures[Tiles[y, x].TileID2].Tile);

                            if (ToolManager.GetToolMode() != ToolManager.ToolMode.Height)
                                shader.SetValue("Highlight", false);
                        }

                        shader.CommitChanges();

                        if (ToolManager.GetToolMode() == ToolManager.ToolMode.Height || ToolManager.GetToolMode() == ToolManager.ToolMode.Tiles || ToolManager.GetToolMode() == ToolManager.ToolMode.Brush)
                            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, Vertices, ((y * 16) + x) * 25, 25, Heightmaps.Indices, 0, 44);
                        else
                            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, ((y * 16) + x) * 25, 0, 25, 0, 44);
                    }
                }

                device.RenderState.AlphaBlendEnable = false;
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the tile tool.
        /// </summary>
        /// <value>The tile tool.</value>
        public Tools.Tiles TileTool
        {
            get { return (Tools.Tiles)ToolManager.Tool; }
        }

        /// <summary>
        /// Gets the height tool.
        /// </summary>
        /// <value>The height tool.</value>
        public Tools.Height HeightTool
        {
            get { return (Tools.Height)ToolManager.Tool; }
        }

        #endregion

        #region Static Declarations

        /// <summary>
        /// Indices
        /// </summary>
        public static readonly short[] Indices = new short[]
        {
            5, 0, 6, 1, 7, 2, 8, 3, 9, 4, 4, 10,
            10, 5, 11, 6, 12, 7, 13, 8, 14, 9,
            9, 15, 15, 10, 16, 11, 17, 12, 18,
            13, 19, 14, 14, 20, 20, 15, 21, 16,
            22, 17, 23, 18, 24, 19
        };

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the blocks.
        /// </summary>
        /// <value>The blocks.</value>
        public Heightmap[,] Blocks { get; set; }

        /// <summary>
        /// Gets or sets the index buffer.
        /// </summary>
        /// <value>The index buffer.</value>
        private IndexBuffer indexBuffer { get; set; }

        /// <summary>
        /// Gets or sets the vertex declaration.
        /// </summary>
        /// <value>The vertex declaration.</value>
        private VertexDeclaration vertexDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the pick position.
        /// </summary>
        /// <value>The pick position.</value>
        private Vector3 pickPosition { get; set; }

        /// <summary>
        /// Gets or sets the picked tile.
        /// </summary>
        /// <value>The picked tile.</value>
        private Tiles.Tile pickedTile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Heightmaps"/> is loading.
        /// </summary>
        /// <value><c>true</c> if loading; otherwise, <c>false</c>.</value>
        public bool Loading { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Heightmaps"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public Heightmaps(Game game)
            : base(game)
        {
            DrawOrder = MapManager.DRAWORDER_TERRAIN;

            Blocks = new Heightmap[100, 100];

            indexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), sizeof(short) * Indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<short>(Indices);

            vertexDeclaration = new VertexDeclaration(game.GraphicsDevice, HIM.Vertex.VertexElements);
        }

        /// <summary>
        /// Adds the specified file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Add(string filePath)
        {
            string[] position = (Path.GetFileName(filePath)).Remove(5, 4).Split('_');

            int x = Convert.ToInt32(position[1]);
            int y = Convert.ToInt32(position[0]);

            Blocks[y, x] = new Heightmap(Game.GraphicsDevice, filePath);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if (Blocks[y, x] == null)
                        continue;

                    Blocks[y, x].VertexBuffer.Dispose();
                    Blocks[y, x].ShadowMap.Dispose();
                }
            }

            Blocks = new Heightmap[100, 100];
        }

        /// <summary>
        /// Picks the terrain.
        /// </summary>
        /// <returns>The position.</returns>
        public Vector3 PickPosition()
        {
            MouseState mouseState = Mouse.GetState();

            if (!mouseState.Intersects(Game.GraphicsDevice.Viewport))
                return pickPosition;

            Ray ray = new Ray().Create(Game.GraphicsDevice, Mouse.GetState(), CameraManager.View, CameraManager.Projection);
            BoundingFrustum boundingFrustum = new BoundingFrustum(CameraManager.View * CameraManager.Projection);

            List<Vector3> vertexPositions = new List<Vector3>();

            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if (Blocks[y, x] != null)
                        vertexPositions.AddRange(Blocks[y, x].PickPosition(boundingFrustum, ray));                        
                }
            }

            if (vertexPositions.Count == 0)
                return pickPosition;

            Vector2 cameraPosition = new Vector2(CameraManager.Position.X, CameraManager.Position.Y);

            Vector3 closestPosition = vertexPositions[0];
            float currentDistance = Vector2.Distance(cameraPosition, new Vector2(closestPosition.X, closestPosition.Y));

            for (int i = 0; i < vertexPositions.Count; i++)
            {
                float distance = Vector2.Distance(cameraPosition, new Vector2(vertexPositions[i].X, vertexPositions[i].Y));

                if (distance > currentDistance)
                    continue;

                closestPosition = vertexPositions[i];
                currentDistance = distance;
            }

            return pickPosition = closestPosition;
        }

        /// <summary>
        /// Picks the tiles.
        /// </summary>
        /// <returns>The tile.</returns>
        public Tiles.Tile? PickTile()
        {
            MouseState mouseState = Mouse.GetState();

            if (!mouseState.Intersects(Game.GraphicsDevice.Viewport))
                return null;

            Ray ray = new Ray().Create(Game.GraphicsDevice, Mouse.GetState(), CameraManager.View, CameraManager.Projection);
            BoundingFrustum boundingFrustum = new BoundingFrustum(CameraManager.View * CameraManager.Projection);

            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if (Blocks[y, x] == null)
                        continue;

                    Vector2? position;

                    if ((position = Blocks[y, x].PickTile(boundingFrustum, ray)) != null)
                    {
                        return pickedTile = new Tiles.Tile()
                        {
                            X = x,
                            Y = y,
                            TileX = (int)position.Value.X,
                            TileY = (int)position.Value.Y
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Draws the heightmaps.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || !ConfigurationManager.GetValue<bool>("Draw", "Heightmaps"))
                return;

            #region Drawing Heightmaps

            Game.GraphicsDevice.Indices = indexBuffer;
            Game.GraphicsDevice.VertexDeclaration = vertexDeclaration;

            Game.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            
            BoundingFrustum boundingFrustum = new BoundingFrustum(CameraManager.View * CameraManager.Projection);

            Effect shader = ShaderManager.GetShader((ToolManager.GetToolMode() == ToolManager.ToolMode.Height) ? ShaderManager.ShaderType.HeightEditing : ShaderManager.ShaderType.Height);

            shader.SetValue("WorldViewProjection", CameraManager.View * CameraManager.Projection);

            if (ToolManager.GetToolMode() != ToolManager.ToolMode.Height)
                shader.SetValue("GridOutlineEnabled", ConfigurationManager.GetValue<bool>("Draw", "GridOutline"));

            shader.Start("Ground");

            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if (Blocks[y, x] == null)
                        continue;

                    Blocks[y, x].Draw(boundingFrustum, shader);
                }
            }

            shader.Finish();

            Game.GraphicsDevice.RenderState.CullMode = CullMode.None;

            #endregion

            base.Draw(gameTime);
        }

        #region Static Functions

        /// <summary>
        /// Rotates texture coordinates using a rotation value.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        /// <param name="texCoord">The texture coordinates.</param>
        /// <returns>The rotated texture coordinates.</returns>
        public static Vector2 RotationToCoordinates(ZON.RotationType rotation, Vector2 texCoord)
        {
            switch (rotation)
            {
                case ZON.RotationType.LeftRight:
                    texCoord.X = 1.0f - texCoord.X;
                    break;
                case ZON.RotationType.TopBottom:
                    texCoord.Y = 1.0f - texCoord.Y;
                    break;
                case ZON.RotationType.LeftRightTopBottom:
                    {
                        texCoord.X = 1.0f - texCoord.X;
                        texCoord.Y = 1.0f - texCoord.Y;
                    }
                    break;
                case ZON.RotationType.Rotate90Clockwise:
                    {
                        float tempX = texCoord.X;
                        texCoord.X = texCoord.Y;
                        texCoord.Y = 1.0f - tempX;
                    }
                    break;
                case ZON.RotationType.Rotate90CounterClockwise:
                    {
                        float tempX = texCoord.X;
                        texCoord.X = texCoord.Y;
                        texCoord.Y = tempX;
                    }
                    break;
            }

            return texCoord;
        }

        #endregion
    }
}