using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Map
{
    /// <summary>
    /// HIM class.
    /// </summary>
    public class HIM
    {
        /// <summary>
        /// Patch Data structure.
        /// </summary>
        public struct PatchData
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the map position X.
            /// </summary>
            /// <value>The map position X.</value>
            public int MapPositionX { get; set; }

            /// <summary>
            /// Gets or sets the map position Y.
            /// </summary>
            /// <value>The map position Y.</value>
            public int MapPositionY { get; set; }

            /// <summary>
            /// Gets or sets the height.
            /// </summary>
            /// <value>The height.</value>
            public float[] Height { get; set; }

            /// <summary>
            /// Gets or sets the maximum height.
            /// </summary>
            /// <value>The maximum height.</value>
            public float MaximumHeight { get; set; }

            /// <summary>
            /// Gets or sets the minimum height.
            /// </summary>
            /// <value>The minimum height.</value>
            public float MinimumHeight { get; set; }

            #endregion
        };

        /// <summary>
        /// Quad Patch structure.
        /// </summary>
        public struct QuadPatch
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the size of the F quad.
            /// </summary>
            /// <value>The size of the F quad.</value>
            public float FQuadSize { get; set; }

            /// <summary>
            /// Gets or sets the maximum height.
            /// </summary>
            /// <value>The maximum height.</value>
            public float MaximumHeight { get; set; }

            /// <summary>
            /// Gets or sets the minimum height.
            /// </summary>
            /// <value>The minimum height.</value>
            public float MinimumHeight { get; set; }

            /// <summary>
            /// Gets or sets the map position X.
            /// </summary>
            /// <value>The map position X.</value>
            public int MapPositionX { get; set; }

            /// <summary>
            /// Gets or sets the map position Y.
            /// </summary>
            /// <value>The map position Y.</value>
            public int MapPositionY { get; set; }

            /// <summary>
            /// Gets or sets the index X.
            /// </summary>
            /// <value>The index X.</value>
            public int[] IndexX { get; set; }

            /// <summary>
            /// Gets or sets the index Y.
            /// </summary>
            /// <value>The index Y.</value>
            public int[] IndexY { get; set; }

            /// <summary>
            /// Gets or sets the size of the I quad.
            /// </summary>
            /// <value>The size of the I quad.</value>
            public int IQuadSize { get; set; }

            /// <summary>
            /// Gets or sets the level.
            /// </summary>
            /// <value>The level.</value>
            public int Level { get; set; }

            #endregion
        };

        /// <summary>
        /// Vertex structure
        /// </summary>
        public struct Vertex
        {
            /// <summary>
            /// Size of the Vertex
            /// </summary>
            public const int SIZE_IN_BYTES = sizeof(float) * 9;

            /// <summary>
            /// Vertex Elements
            /// </summary>
            public static VertexElement[] VertexElements =
            {
                new VertexElement( 0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
                new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0 ),
                new VertexElement( 0, sizeof(float) * 5, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1 ),
                new VertexElement( 0, sizeof(float) * 7, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2 ),
            };

            #region Member Declarations

            /// <summary>
            /// Gets or sets the position.
            /// </summary>
            /// <value>The position.</value>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Gets or sets the bottom texture.
            /// </summary>
            /// <value>The bottom texture.</value>
            public Vector2 BottomTexture { get; set; }

            /// <summary>
            /// Gets or sets the top texture.
            /// </summary>
            /// <value>The top texture.</value>
            public Vector2 TopTexture { get; set; }

            /// <summary>
            /// Gets or sets the shadow texture.
            /// </summary>
            /// <value>The shadow texture.</value>
            public Vector2 ShadowTexture { get; set; }

            #endregion
        };

        #region Member Declarations

        /// <summary>
        /// Gets or sets the grid count.
        /// </summary>
        /// <value>The grid count.</value>
        public int GridCount { get; set; }

        /// <summary>
        /// Gets or sets the size of the grid.
        /// </summary>
        /// <value>The size of the grid.</value>
        public float GridSize { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public float[,] Position { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="HIM"/> class.
        /// </summary>
        public HIM()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HIM"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public HIM(string filePath)
        {
            Load(filePath);
        }

        /// <summary>
        /// Loads the specified file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Load(string filePath)
        {
            FileHandler fh = new FileHandler(FilePath = filePath, FileHandler.FileOpenMode.Reading, Encoding.GetEncoding("EUC-KR"));

            int height = fh.Read<int>();
            int width = fh.Read<int>();

            fh.Seek(8, SeekOrigin.Current);

            Position = new float[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    Position[y, x] = fh.Read<float>() / 100.0f;
            }

            fh.Close();
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        public void Save()
        {
            Save(FilePath);
        }

        /// <summary>
        /// Saves the specified file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Save(string filePath)
        {
            FileHandler fh = new FileHandler(FilePath = filePath, FileHandler.FileOpenMode.Writing, Encoding.GetEncoding("EUC-KR"));

            fh.Write<int>(65);
            fh.Write<int>(65);
            fh.Write<int>(4);
            fh.Write<float>(250.0f);

            float[,] mapHeight = new float[65, 65];

            for (int y = 0; y < 65; y++)
            {
                for (int x = 0; x < 65; x++)
                {
                    fh.Write<float>(Position[y, x] * 100.0f);

                    mapHeight[65 - (y + 1), x] = Position[y, x] * 100.0f;
                }
            }

            QuadPatch[] quadPatch = new QuadPatch[85];

            for (int i = 0; i < 85; i++)
            {
                quadPatch[i] = new QuadPatch()
                {
                    IndexX = new int[2],
                    IndexY = new int[2]
                };
            }

            PatchData[,] patch = new PatchData[16, 16];

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    patch[i, j] = new PatchData()
                    {
                        Height = new float[25]
                    };
                }
            }

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        for (int l = 0; l < 5; l++)
                            patch[i, j].Height[((4 - k) * 5) + l] = mapHeight[(i * 4) + k, (j * 4) + l];
                    }

                    patch[i, j].MaximumHeight = float.MaxValue;
                    patch[i, j].MinimumHeight = float.MinValue;

                    for (int k = 0; k < 25; k++)
                    {
                        if (patch[i, j].MaximumHeight < patch[i, j].Height[k])
                            patch[i, j].MaximumHeight = patch[i, j].Height[k];

                        if (patch[i, j].MinimumHeight > patch[i, j].Height[k])
                            patch[i, j].MinimumHeight = patch[i, j].Height[k];
                    }
                }
            }

            for (int i = 0; i < 85; i++)
            {
                float minimum = -10.0f;
                float maximum = 10000.0f;

                for (int j = quadPatch[i].IndexY[0]; j <= quadPatch[i].IndexY[1]; j++)
                {
                    for (int k = quadPatch[i].IndexX[0]; k <= quadPatch[i].IndexX[1]; k++)
                    {
                        float currentMaximum = patch[j, k].MaximumHeight;

                        if (currentMaximum > minimum)
                            minimum = currentMaximum;

                        float currentMinimum = patch[j, k].MinimumHeight;

                        if (currentMinimum < maximum)
                            maximum = currentMinimum;
                    }
                }

                quadPatch[i].MaximumHeight = minimum;
                quadPatch[i].MinimumHeight = maximum;
            }

            fh.Write<BString>("quad");

            fh.Write<int>(256);

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    fh.Write<float>(patch[i, j].MaximumHeight);
                    fh.Write<float>(patch[i, j].MinimumHeight);
                }
            }

            fh.Write<int>(85);

            for (int i = 0; i < 85; i++)
            {
                fh.Write<float>(quadPatch[i].MaximumHeight);
                fh.Write<float>(quadPatch[i].MinimumHeight);
            }

            fh.Close();
        }
    }
}