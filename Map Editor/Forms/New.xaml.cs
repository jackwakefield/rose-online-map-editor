using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Map_Editor.Engine;
using Map_Editor.Engine.Data;
using Map_Editor.Engine.Map;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Forms
{
    /// <summary>
    /// Interaction logic for New.xaml
    /// </summary>
    public partial class New : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="New"/> class.
        /// </summary>
        public New()
        {
            InitializeComponent();

            SizeX.Value = 1;
            SizeY.Value = 1;

            for (int i = 1; i < FileManager.STBs["LIST_ZONE"].Cells.Count; i++)
            {
                if (!MapManager.IsValidMap(i))
                    ID.Items.Add(i);
            }

            ID.SelectedIndex = ID.Items.Add(FileManager.STBs["LIST_ZONE"].Cells.Count);

            for (int i = 1; i < FileManager.STLs["STR_PLANET"].Rows[1].Count; i++)
                Planet.Items.Add(FileManager.STLs["STR_PLANET"].Rows[1][i].Text);

            Planet.SelectedIndex = 0;

            for (int i = 1; i < FileManager.STBs["LIST_SKY"].Cells.Count; i++)
            {
                if (FileManager.STBs["LIST_SKY"].Cells[i][1].Length > 0)
                    Sky.Items.Add(i);
            }

            Sky.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the ValueChanged event of the SizeX control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs&lt;System.Double&gt;"/> instance containing the event data.</param>
        private void SizeX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SizeXValue.Text = string.Format("{0} [{1}]", (int)SizeX.Value, (int)SizeX.Value * 160.0f);
        }

        /// <summary>
        /// Handles the ValueChanged event of the SizeY control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs&lt;System.Double&gt;"/> instance containing the event data.</param>
        private void SizeY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SizeYValue.Text = string.Format("{0} [{1}]", (int)SizeY.Value, (int)SizeY.Value * 160.0f);
        }

        /// <summary>
        /// Handles the TextChanged event of the MapFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void MapFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FilePath.Text = string.Format(@"3Ddata\MAPS\{0}\{1}\", ((string)Planet.SelectedItem).ToUpper(), MapFolder.Text);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the Planet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void Planet_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilePath.Text = string.Format(@"3Ddata\MAPS\{0}\{1}\", ((string)Planet.SelectedItem).ToUpper(), MapFolder.Text);
        }

        /// <summary>
        /// Handles the Click event of the SelectDecoration control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectDecoration_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog selectFile = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "ZSC File (LIST_DECO_*.ZSC)|LIST_DECO_*.ZSC"
            };

            if (selectFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            int dataIndex = selectFile.FileName.ToLower().IndexOf("3ddata");

            Decoration.Text = selectFile.FileName.Substring(dataIndex, selectFile.FileName.Length - dataIndex);
        }

        /// <summary>
        /// Handles the Click event of the SelectContruction control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectContruction_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog selectFile = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "ZSC File (LIST_CNST_*.ZSC)|LIST_CNST_*.ZSC"
            };

            if (selectFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            int dataIndex = selectFile.FileName.ToLower().IndexOf("3ddata");

            Construction.Text = selectFile.FileName.Substring(dataIndex, selectFile.FileName.Length - dataIndex);
        }

        /// <summary>
        /// Handles the Click event of the SelectTileFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectTileFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog selectFile = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "ZON File (*.ZON)|*.ZON"
            };

            if (selectFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            int dataIndex = selectFile.FileName.ToLower().IndexOf("3ddata");

            TileFile.Text = selectFile.FileName.Substring(dataIndex, selectFile.FileName.Length - dataIndex);
        }

        /// <summary>
        /// Handles the Click event of the Create control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text.Length == 0)
            {
                MessageBox.Show("You must enter a map name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Name.Focus();

                return;
            }

            if (Decoration.Text.Length == 0)
            {
                MessageBox.Show("You must enter a decoration file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Decoration.Focus();

                return;
            }

            if (Construction.Text.Length == 0)
            {
                MessageBox.Show("You must enter a construction file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Construction.Focus();

                return;
            }

            if (ZoneFile.Text.Length == 0)
            {
                MessageBox.Show("You must enter a zone file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                ZoneFile.Focus();

                return;
            }

            if (string.Compare(Path.GetExtension(ZoneFile.Text), ".zon", true) != 0)
            {
                MessageBox.Show("Invalid file type for for zone file (must be .zon)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                ZoneFile.Focus();

                return;
            }

            if (MapFolder.Text.Length == 0)
            {
                MessageBox.Show("You must enter a map folder file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                ZoneFile.Focus();

                return;
            }

            if (Directory.Exists(FilePath.Text))
            {
                MessageBox.Show("Map folder already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                MapFolder.Focus();

                return;
            }

            if (TileFile.Text.Length == 0)
            {
                MessageBox.Show("You must enter a tile file file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                TileFile.Focus();

                return;
            }

            if (string.Compare(Path.GetExtension(TileFile.Text), ".zon", true) != 0)
            {
                MessageBox.Show("Invalid file type for for tile file (must be .zon)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                TileFile.Focus();

                return;
            }

            ZoneFile.Text = Path.GetFileName(ZoneFile.Text);

            Directory.CreateDirectory(FilePath.Text);

            #region Lightmap

            Texture2D emptyLightmap = new Texture2D(App.Engine.GraphicsDevice, 512, 512);

            Microsoft.Xna.Framework.Graphics.Color[] Lightmap = new Microsoft.Xna.Framework.Graphics.Color[512 * 512];

            for (int i = 0; i < 512 * 512; i++)
                Lightmap[i] = new Microsoft.Xna.Framework.Graphics.Color(136, 136, 153);

            emptyLightmap.SetData<Microsoft.Xna.Framework.Graphics.Color>(Lightmap);

            #endregion

            #region LIT

            LIT emptyLIT = new LIT()
            {
                DDSFiles = new List<LIT.DDS>(0),
                Objects = new List<LIT.Object>(0)
            };

            #endregion

            #region IFO

            IFO emptyIFO = new IFO()
            {
                MapInfo = new IFO.MapInformation(),
                Decoration = new List<IFO.BaseIFO>(0),
                NPCs = new List<IFO.NPC>(0),
                Construction = new List<IFO.BaseIFO>(0),
                Sounds = new List<IFO.Sound>(0),
                Effects = new List<IFO.Effect>(0),
                Animation = new List<IFO.BaseIFO>(0),
                WideWater = new IFO.UnusedWater()
                {
                    WaterBlocks = new IFO.UnusedWater.WaterBlock[16, 16],
                    X = 16,
                    Y = 16
                },
                Monsters = new List<IFO.MonsterSpawn>(0),
                Water = new List<IFO.WaterBlock>(0),
                WarpGates = new List<IFO.BaseIFO>(0),
                Collision = new List<IFO.BaseIFO>(0),
                EventTriggers = new List<IFO.EventTrigger>(0)
            };

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    emptyIFO.WideWater.WaterBlocks[x, y] = new IFO.UnusedWater.WaterBlock()
                    {
                        Use = 0,
                        Height = 0,
                        WaterType = 1,
                        WaterIndex = 0,
                        Reserved = 0
                    };
                }
            }

            #endregion

            #region TIL

            TIL emptyTIL = new TIL()
            {
                Tiles = new TIL.Tile[16, 16]
            };

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    emptyTIL.Tiles[y, x] = new TIL.Tile()
                    {
                        BrushID = 1,
                        TileIndex = 15,
                        TileSetNumber = 1,
                        TileID = 1
                    };
                }
            }

            #endregion

            #region MOV

            MOV emptyMOV = new MOV()
            {
                IsWalkable = new byte[32, 32]
            };

            #endregion

            #region HIM

            HIM emptyHIM = new HIM()
            {
                GridCount = 4,
                GridSize = 250,
                Position = new float[65, 65]
            };

            #endregion

            #region ZON

            ZON temporaryZON = new ZON()
            {
                Blocks = new List<ZON.Block>(5),
                ZoneInfo = new ZON.Block0()
                {
                    ZoneType = 0,
                    ZoneWidth = 64,
                    ZoneHeight = 64,
                    GridCount = 4,
                    GridSize = 250,
                    XCount = 30,
                    YCount = 30,
                    ZoneParts = new ZON.Block0.ZonePart[64, 64]
                },
                SpawnPoints = new List<ZON.SpawnPoint>(2)
            };

            for (int i = 0; i < 5; i++)
            {
                temporaryZON.Blocks.Add(new ZON.Block()
                {
                    Type = (ZON.BlockType)i,
                    Offset = 0
                });
            }

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    temporaryZON.ZoneInfo.ZoneParts[y, x] = new ZON.Block0.ZonePart()
                    {
                        UseMap = 0,
                        Position = new Microsoft.Xna.Framework.Vector2()
                        {
                            X = 0.0f,
                            Y = 0.0f
                        }
                    };
                }
            }

            temporaryZON.SpawnPoints.Add(new ZON.SpawnPoint()
            {
                Name = "start",
                Position = new Microsoft.Xna.Framework.Vector3(4930.0f, 5470.0f, 0.0f)
            });

            temporaryZON.SpawnPoints.Add(new ZON.SpawnPoint()
            {
                Name = "restore",
                Position = new Microsoft.Xna.Framework.Vector3(4910.0f, 5500.0f, 0.0f)
            });

            ZON copyingZON = new ZON();
            copyingZON.Load(TileFile.Text);

            temporaryZON.Textures = new List<ZON.Texture>(copyingZON.Textures.Count);

            copyingZON.Textures.ForEach(delegate(ZON.Texture texture)
            {
                temporaryZON.Textures.Add(new ZON.Texture()
                {
                    Path = texture.Path
                });
            });

            temporaryZON.Tiles = new List<ZON.Tile>(copyingZON.Tiles.Count);

            copyingZON.Tiles.ForEach(delegate(ZON.Tile tile)
            {
                temporaryZON.Tiles.Add(new ZON.Tile()
                {
                    BaseID1 = tile.BaseID1,
                    BaseID2 = tile.BaseID2,
                    Offset1 = tile.Offset1,
                    Offset2 = tile.Offset2,
                    IsBlending = tile.IsBlending,
                    Rotation = tile.Rotation,
                    TileType = tile.TileType
                });
            });

            temporaryZON.EconomyInfo = new ZON.Economy()
            {
                AreaName = "0",
                IsUnderground = 0,
                ButtonBGM = "button1",
                ButtonBack = "button2",
                CheckCount = 35,
                StandardPopulation = 500,
                StandardGrowthRate = 30,
                MetalConsumption = 10,
                StoneConsumption = 20,
                WoodConsumption = 20,
                LeatherConsumption = 10,
                ClothConsumption = 10,
                AlchemyConsumption = 10,
                ChemicalConsumption = 10,
                IndustrialConsumption = 10,
                MedicineConsumption = 30,
                FoodConsumption = 10
            };

            #endregion

            for (int y = 0; y < SizeY.Value; y++)
            {
                for (int x = 0; x < SizeX.Value; x++)
                {
                    Directory.CreateDirectory(string.Format(@"{0}\{1}_{2}", FilePath.Text, y + 30, x + 30));
                    emptyLightmap.Save(string.Format(@"{0}\{1}_{2}\{1}_{2}_PLANELIGHTINGMAP.DDS", FilePath.Text, y + 30, x + 30), ImageFileFormat.Dds);

                    Directory.CreateDirectory(string.Format(@"{0}\{1}_{2}\LIGHTMAP", FilePath.Text, y + 30, x + 30));
                    emptyLIT.Save(string.Format(@"{0}\{1}_{2}\LIGHTMAP\BUILDINGLIGHTMAPDATA.LIT", FilePath.Text, y + 30, x + 30));
                    emptyLIT.Save(string.Format(@"{0}\{1}_{2}\LIGHTMAP\OBJECTLIGHTMAPDATA.LIT", FilePath.Text, y + 30, x + 30));

                    emptyIFO.MapInfo.Width = 16;
                    emptyIFO.MapInfo.Height = 16;
                    emptyIFO.MapInfo.MapCellX = y + 30;
                    emptyIFO.MapInfo.MapCellY = x + 30;
                    emptyIFO.MapInfo.MapName = string.Format("{0}_{1}", y + 30, x + 30);

                    emptyIFO.Save(string.Format(@"{0}\{1}_{2}.IFO", FilePath.Text, y + 30, x + 30));
                    emptyTIL.Save(string.Format(@"{0}\{1}_{2}.TIL", FilePath.Text, y + 30, x + 30));
                    emptyMOV.Save(string.Format(@"{0}\{1}_{2}.MOV", FilePath.Text, y + 30, x + 30));
                    emptyHIM.Save(string.Format(@"{0}\{1}_{2}.HIM", FilePath.Text, y + 30, x + 30));
                }
            }

            temporaryZON.Save(string.Format(@"{0}\{1}", FilePath.Text, ZoneFile.Text));

            int mapID = Convert.ToInt32(ID.Text);

            if (mapID == FileManager.STBs["LIST_ZONE"].Cells.Count)
            {
                FileManager.STBs["LIST_ZONE"].Cells.Add(new List<string>());

                for (int i = 0; i < 38; i++)
                    FileManager.STBs["LIST_ZONE"].Cells[mapID].Add(string.Empty);
            }

            FileManager.STBs["LIST_ZONE"].Cells[mapID][0] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][1] = Name.Text;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][2] = string.Format(@"{0}{1}", FilePath.Text, ZoneFile.Text);
            FileManager.STBs["LIST_ZONE"].Cells[mapID][3] = "start";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][4] = "restore";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][5] = "0";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][6] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][7] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][8] = Sky.SelectedIndex.ToString();
            FileManager.STBs["LIST_ZONE"].Cells[mapID][9] = string.Format(@"{0}{1}.dds", FilePath.Text, ZoneFile.Text.Substring(0, ZoneFile.Text.Length - 4));
            FileManager.STBs["LIST_ZONE"].Cells[mapID][10] = "30";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][11] = "30";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][12] = Decoration.Text;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][13] = Construction.Text;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][14] = "160";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][15] = "0";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][16] = "11";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][17] = "112";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][18] = "128";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][19] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][20] = (Planet.SelectedIndex + 1).ToString();
            FileManager.STBs["LIST_ZONE"].Cells[mapID][21] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][22] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][23] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][24] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][25] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][26] = "4000";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][27] = string.Format("LZON{0:0000}", mapID);
            FileManager.STBs["LIST_ZONE"].Cells[mapID][28] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][29] = "4";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][30] = "4";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][31] = "0";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][32] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][33] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][34] = string.Empty;
            FileManager.STBs["LIST_ZONE"].Cells[mapID][35] = "0";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][36] = "25";
            FileManager.STBs["LIST_ZONE"].Cells[mapID][37] = mapID.ToString();

            int useRow = -1;

            for (int i = 0; i < FileManager.STLs["LIST_ZONE_S"].Entries.Count; i++)
            {
                if (string.Compare(FileManager.STLs["LIST_ZONE_S"].Entries[i].StringID, string.Format("LZON{0:0000}", mapID), false) != 0)
                    continue;

                useRow = i;

                break;
            }

            if (useRow >= 0)
            {
                for (int i = 0; i < FileManager.STLs["LIST_ZONE_S"].Rows.Count; i++)
                {
                    FileManager.STLs["LIST_ZONE_S"].Rows[i][useRow].Text = Name.Text;
                    FileManager.STLs["LIST_ZONE_S"].Rows[i][useRow].Comment = Name.Text;
                }
            }
            else
            {
                for (int i = 0; i < FileManager.STLs["LIST_ZONE_S"].Rows.Count; i++)
                {
                    FileManager.STLs["LIST_ZONE_S"].Rows[i].Add(new STL.Row()
                    {
                        Text = Name.Text,
                        Comment = Name.Text
                    });
                }

                FileManager.STLs["LIST_ZONE_S"].Entries.Add(new STL.Entry()
                {
                    ID = FileManager.STLs["LIST_ZONE_S"].Rows.Count - 1,
                    StringID = string.Format("LZON{0:0000}", mapID)
                });
            }

            FileManager.STBs["LIST_ZONE"].Save();
            FileManager.STLs["LIST_ZONE_S"].Save();

            if (MessageBox.Show("Do you wish to open the map?", "Open", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                MapManager.Load(mapID);

            Close();
        }

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to close the window?\nAll information will be lost.", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Close();
        }
    }
}