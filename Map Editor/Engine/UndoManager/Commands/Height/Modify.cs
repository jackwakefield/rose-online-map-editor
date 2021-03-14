using Map_Editor.Engine.Commands.Interfaces;

namespace Map_Editor.Engine.Commands.Height
{
    /// <summary>
    /// Modify class.
    /// </summary>
    public class Modify : ICommand
    {
        /// <summary>
        /// Block structure.
        /// </summary>
        public struct Block
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the height of the previous.
            /// </summary>
            /// <value>The height of the previous.</value>
            public float[,] PreviousHeight { get; set; }

            /// <summary>
            /// Gets or sets the new height.
            /// </summary>
            /// <value>The new height.</value>
            public float[,] NewHeight { get; set; }

            #endregion
        };

        #region Member Declarations

        /// <summary>
        /// Gets or sets the blocks.
        /// </summary>
        /// <value>The blocks.</value>
        public Block[,] Blocks { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Modify"/> class.
        /// </summary>
        public Modify()
        {
            Blocks = new Block[100, 100];

            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if (MapManager.Heightmaps.Blocks[y, x] == null)
                        continue;

                    Blocks[y, x] = new Block()
                    {
                        PreviousHeight = (float[,])MapManager.Heightmaps.Blocks[y, x].HeightFile.Position.Clone()
                    };
                }
            }
        }

        /// <summary>
        /// Undo command.
        /// </summary>
        public void Undo()
        {
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if (MapManager.Heightmaps.Blocks[y, x] == null)
                        continue;

                    MapManager.Heightmaps.Blocks[y, x].HeightFile.Position = (float[,])Blocks[y, x].PreviousHeight.Clone();

                    MapManager.Heightmaps.Blocks[y, x].BuildVertices();
                    MapManager.Heightmaps.Blocks[y, x].BuildBoundingBoxes();

                    if (ToolManager.GetToolMode() != ToolManager.ToolMode.Height)
                        MapManager.Heightmaps.Blocks[y, x].BuildBuffer();
                }
            }
        }

        /// <summary>
        /// Redo command.
        /// </summary>
        public void Redo()
        {
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    if (MapManager.Heightmaps.Blocks[y, x] == null)
                        continue;

                    MapManager.Heightmaps.Blocks[y, x].HeightFile.Position = (float[,])Blocks[y, x].NewHeight.Clone();

                    MapManager.Heightmaps.Blocks[y, x].BuildVertices();
                    MapManager.Heightmaps.Blocks[y, x].BuildBoundingBoxes();

                    if (ToolManager.GetToolMode() != ToolManager.ToolMode.Height)
                        MapManager.Heightmaps.Blocks[y, x].BuildBuffer();
                }
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetName()
        {
            return "Height Edit";
        }
    }
}