using System.Text;

namespace Map_Editor.Engine.Map
{
    /// <summary>
    /// MOV class..
    /// </summary>
    public class MOV
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the is walkable.
        /// </summary>
        /// <value>The is walkable.</value>
        public byte[,] IsWalkable { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MOV"/> class.
        /// </summary>
        public MOV()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MOV"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public MOV(string filePath)
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

            IsWalkable = new byte[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    IsWalkable[y, x] = fh.Read<byte>();
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

            fh.Write<int>(32);
            fh.Write<int>(32);

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                   fh.Write<byte>(IsWalkable[y, x]);
            }

            fh.Close();
        }
    }
}