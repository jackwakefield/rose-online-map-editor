using System;
using System.Collections.Generic;
using System.IO;
using Map_Editor.Engine.Character;
using Map_Editor.Engine.Data;
using Map_Editor.Engine.Map;
using Map_Editor.Engine.Models;
using Map_Editor.Misc;

namespace Map_Editor.Engine
{
    /// <summary>
    /// FileManager class.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// File type.
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// STB.
            /// </summary>
            STB,

            /// <summary>
            /// STL.
            /// </summary>
            STL,

            /// <summary>
            /// ZSC.
            /// </summary>
            ZSC,

            /// <summary>
            /// CHR.
            /// </summary>
            CHR,

            /// <summary>
            /// IFO.
            /// </summary>
            IFO,

            /// <summary>
            /// ZON.
            /// </summary>
            ZON,

            /// <summary>
            /// LIT.
            /// </summary>
            LIT
        }

        /// <summary>
        /// LIT file type.
        /// </summary>
        public enum LITType
        {
            /// <summary>
            /// Decoration.
            /// </summary>
            Decoration,

            /// <summary>
            /// Construction.
            /// </summary>
            Construction
        }

        #region Member Declarations

        /// <summary>
        /// Gets or sets the IFOs.
        /// </summary>
        /// <value>The IFOs.</value>
        public static List<IFO> IFOs { get; set; }

        /// <summary>
        /// Gets or sets the decoration LITs.
        /// </summary>
        /// <value>The decoration LITs.</value>
        public static List<LIT> DecorationLITs { get; set; }

        /// <summary>
        /// Gets or sets the construction LITs.
        /// </summary>
        /// <value>The construction LITs.</value>
        public static List<LIT> ConstructionLITs { get; set; }

        /// <summary>
        /// Gets or sets the ZSCs.
        /// </summary>
        /// <value>The ZSCs.</value>
        public static Dictionary<string, ZSC> ZSCs { get; set; }

        /// <summary>
        /// Gets or sets the CHRs.
        /// </summary>
        /// <value>The CHRs.</value>
        public static Dictionary<string, CHR> CHRs { get; set; }

        /// <summary>
        /// Gets or sets the STBs.
        /// </summary>
        /// <value>The STBs.</value>
        public static Dictionary<string, STB> STBs { get; set; }

        /// <summary>
        /// Gets or sets the STLs.
        /// </summary>
        /// <value>The STLs.</value>
        public static Dictionary<string, STL> STLs { get; set; }

        /// <summary>
        /// Gets or sets the ZON.
        /// </summary>
        /// <value>The ZON.</value>
        public static ZON ZON { get; set; }

        /// <summary>
        /// Gets or sets the tile set.
        /// </summary>
        /// <value>The tile set.</value>
        public static TileSet TileSet { get; set; }

        #endregion

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            Reset(true);

            Output.WriteLine(Output.MessageType.Event, "Loading STBs");

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\STB\\LIST_ZONE.STB [Key: LIST_ZONE]");
            Add("LIST_ZONE", "3DDATA\\STB\\LIST_ZONE.STB", FileType.STB);

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\STB\\LIST_SKY.STB [Key: LIST_SKY]");
            Add("LIST_SKY", "3DDATA\\STB\\LIST_SKY.STB", FileType.STB);

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\STB\\LIST_NPC.STB [Key: LIST_NPC]");
            Add("LIST_NPC", "3DDATA\\STB\\LIST_NPC.STB", FileType.STB);

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\STB\\FILE_EFFECT.STB [Key: FILE_EFFECT]");
            Add("FILE_EFFECT", "3DDATA\\STB\\FILE_EFFECT.STB", FileType.STB);

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\STB\\LIST_MORPH_OBJECT.STB [Key: LIST_MORPH_OBJECT]");
            Add("LIST_MORPH_OBJECT", "3DDATA\\STB\\LIST_MORPH_OBJECT.STB", FileType.STB);

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3Ddata\\TERRAIN\\TILES\\ZONETYPEINFO.STB [Key: ZONETYPEINFO]");
            Add("ZONETYPEINFO", "3Ddata\\TERRAIN\\TILES\\ZONETYPEINFO.STB", FileType.STB);

            Output.WriteLine(Output.MessageType.Event, "Loading STLs");

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\STB\\LIST_ZONE_S.STL [Key: LIST_ZONE_S]");
            Add("LIST_ZONE_S", "3DDATA\\STB\\LIST_ZONE_S.STL", FileType.STL);

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\STB\\LIST_NPC_S.STL [Key: LIST_NPC_S]");
            Add("LIST_NPC_S", "3DDATA\\STB\\LIST_NPC_S.STL", FileType.STL);

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\STB\\STR_PLANET.STL [Key: STR_PLANET]");
            Add("STR_PLANET", "3DDATA\\STB\\STR_PLANET.STL", FileType.STL);

            Output.WriteLine(Output.MessageType.Event, "Loading ZSCs");

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\NPC\\PART_NPC.ZSC [Key: PART_NPC]");
            Add("PART_NPC", "3DDATA\\NPC\\PART_NPC.ZSC", FileType.ZSC);

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\SPECIAL\\EVENT_OBJECT.ZSC [Key: EVENT_OBJECT]");
            Add("EVENT_OBJECT", "3DDATA\\SPECIAL\\EVENT_OBJECT.ZSC", FileType.ZSC);

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\SPECIAL\\LIST_DECO_SPECIAL.ZSC [Key: LIST_DECO_SPECIAL]");
            Add("LIST_DECO_SPECIAL", "3DDATA\\SPECIAL\\LIST_DECO_SPECIAL.ZSC", FileType.ZSC);

            Output.WriteLine(Output.MessageType.Event, "Loading CHRs");

            Output.WriteLine(Output.MessageType.Normal, "- Loading 3DDATA\\NPC\\LIST_NPC.CHR [Key: LIST_NPC]");
            Add("LIST_NPC", "3DDATA\\NPC\\LIST_NPC.CHR", FileType.CHR);
        }

        /// <summary>
        /// Adds the folder.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="fileType">Type of the file.</param>
        public static void AddFolder(string directory, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.IFO:
                    {
                        string[] files = Directory.GetFiles(directory, "*.IFO");

                        for (int i = 0; i < files.Length; i++)
                            Add(files[i], FileType.IFO);
                    }
                    break;
            }
        }

        /// <summary>
        /// Adds a keyless file
        /// </summary>
        /// <param name="filePath">File Path</param>
        /// <param name="fileType">File Type</param>
        public static void Add(string filePath, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.ZSC:
                case FileType.CHR:
                case FileType.STB:
                case FileType.STL:
                    throw new Exception(string.Format("You must specify a key for file type {0}", fileType));
                default:
                    Add(null, filePath, fileType);
                    break;
            }
        }

        /// <summary>
        /// Adds the specified file with the specified key key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileType">Type of the file.</param>
        public static void Add(string key, string filePath, FileType fileType)
        {
            if (!File.Exists(filePath))
                Main.Error(string.Format("Missing File:\n{0}", filePath));

            switch (fileType)
            {
                case FileType.STB:
                    {
                        STB newFile = new STB();
                        newFile.Load(filePath);

                        STBs.Add(key, newFile);
                    }
                    break;
                case FileType.STL:
                    {
                        STL newFile = new STL();
                        newFile.Load(filePath);

                        STLs.Add(key, newFile);
                    }
                    break;
                case FileType.ZSC:
                    {
                        ZSC newFile = new ZSC();
                        newFile.Load(filePath);

                        ZSCs.Add(key, newFile);
                    }
                    break;
                case FileType.CHR:
                    {
                        CHR newFile = new CHR();
                        newFile.Load(filePath);

                        CHRs.Add(key, newFile);
                    }
                    break;
                case FileType.IFO:
                    {
                        IFO newFile = new IFO();
                        newFile.Load(filePath);

                        IFOs.Add(newFile);
                    }
                    break;
                case FileType.ZON:
                    {
                        if (ZON != null)
                        {
                            ZON.Textures.ForEach(delegate(ZON.Texture texture)
                            {
                                if (texture.Tile != null)
                                    texture.Tile.Dispose();
                            });
                        }

                        ZON = new ZON();
                        ZON.Load(filePath);
                    }
                    break;
            }
        }

        /// <summary>
        /// Adds the specified file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="litType">Type of the lit.</param>
        public static void Add(string filePath, LITType litType)
        {
            switch (litType)
            {
                case LITType.Decoration:
                    {
                        LIT newFile = new LIT();
                        newFile.Load(filePath, App.Engine.GraphicsDevice);

                        DecorationLITs.Add(newFile);
                    }
                    break;
                case LITType.Construction:
                    {
                        LIT newFile = new LIT();
                        newFile.Load(filePath, App.Engine.GraphicsDevice);

                        ConstructionLITs.Add(newFile);
                    }
                    break;
            }
        }

        /// <summary>
        /// Resets the files.
        /// </summary>
        /// <param name="resetAll">if set to <c>true</c> [reset all].</param>
        public static void Reset(bool resetAll)
        {
            IFOs = new List<IFO>();

            DecorationLITs = new List<LIT>();
            ConstructionLITs = new List<LIT>();

            if (ZSCs != null)
            {
                if(ZSCs.ContainsKey("Construction"))
                    ZSCs.Remove("Construction");

                if (ZSCs.ContainsKey("Decoration"))
                    ZSCs.Remove("Decoration");
            }

            if (resetAll)
            {
                STBs = new Dictionary<string, STB>();
                STLs = new Dictionary<string, STL>();

                ZSCs = new Dictionary<string, ZSC>();
                CHRs = new Dictionary<string, CHR>();
            }
        }
    }
}