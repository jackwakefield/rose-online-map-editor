namespace Map_Editor.Engine.Data
{
    public class TileSet
    {
        public struct Brush
        {
            public byte MinimumBrush { get; set; }
            public byte MaximumBrush { get; set; }
            public int TileNumber0 { get; set; }
            public byte TileCount0 { get; set; }
            public int TileNumberF { get; set; }
            public byte TileCountF { get; set; }
            public int TileNumber { get; set; }
            public byte TileCount { get; set; }
            public int Direction { get; set; }
        };

        public Brush[] Brushes;
        public byte[,] Chains;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileSet"/> class.
        /// </summary>
        public TileSet()
        {
            Brushes = new Brush[0];
            Chains = new byte[0, 0]; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileSet"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public TileSet(string filePath)
        {
            Load(filePath);
        }

        /// <summary>
        /// Loads the file at the file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Load(string filePath)
        {
            int i = 0;
            STB stb = new STB(filePath);

            int brushCount = int.Parse(stb.Cells[i++][2]);
            Brushes = new Brush[brushCount];

            for (; i < brushCount + 1; i++)
            {
                Brushes[i - 1] = new Brush()
                {
                    MinimumBrush = byte.Parse(stb.Cells[i][2]),
                    MaximumBrush = byte.Parse(stb.Cells[i][3]),
                    TileNumber0 = int.Parse(stb.Cells[i][4]),
                    TileCount0 = byte.Parse(stb.Cells[i][5]),
                    TileNumberF = int.Parse(stb.Cells[i][6]),
                    TileCountF = byte.Parse(stb.Cells[i][7]),
                    TileNumber = int.Parse(stb.Cells[i][8]),
                    TileCount = byte.Parse(stb.Cells[i][9]),
                    Direction = int.Parse(stb.Cells[i][10])
                };
            }

            int maxBrushCount = int.Parse(stb.Cells[i++][2]);
            Chains = new byte[maxBrushCount, maxBrushCount];

            for (int x = 0; x < maxBrushCount; x++)
            {
                for (int y = 0; y < maxBrushCount; y++)
                    Chains[x, y] = byte.Parse(stb.Cells[i + x][y + 2]);
            }
        }
    }
}