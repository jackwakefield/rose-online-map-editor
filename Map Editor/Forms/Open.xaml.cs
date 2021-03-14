using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Map_Editor.Engine;
using Map_Editor.Engine.Map;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Forms
{
    /// <summary>
    /// Interaction logic for Open.xaml
    /// </summary>
    public partial class Open : Window
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the selected map ID.
        /// </summary>
        /// <value>The selected map ID.</value>
        private int selectedMapID { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Open"/> class.
        /// </summary>
        public Open()
        {
            InitializeComponent();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ID");
            dataTable.Columns.Add("Name");

            for (int i = 1; i < FileManager.STBs["LIST_ZONE"].Cells.Count; i++)
            {
                if (!MapManager.IsValidMap(i))
                    continue;

                DataRow dataRow = dataTable.NewRow();

                dataRow[0] = i;
                dataRow[1] = FileManager.STLs["LIST_ZONE_S"].Search(FileManager.STBs["LIST_ZONE"].Cells[i][27]);

                dataTable.Rows.Add(dataRow);
            }

            MapList.SetBinding(ListView.ItemsSourceProperty, new Binding()
            {
                Source = dataTable,
            });
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the MapList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MapList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!MapManager.IsValidMap(selectedMapID))
                return;

            MapManager.Load(selectedMapID);

            this.Close();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the MapList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void MapList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedMapID = Convert.ToInt32((string)((DataRowView)MapList.SelectedItem).Row.ItemArray[0]);

            MinimapViewer.Children.Clear();

            Grid imageGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                SnapsToDevicePixels = true
            };

            if (!MapManager.IsValidMap(selectedMapID))
                return;

            new Thread(new ThreadStart(delegate
            {
                using (GraphicsDevice graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Hardware, new System.Windows.Forms.Panel().Handle, new PresentationParameters()
                {
                    AutoDepthStencilFormat = DepthFormat.Depth24,
                    BackBufferCount = 1,
                    BackBufferFormat = SurfaceFormat.Color,
                    BackBufferHeight = 1,
                    BackBufferWidth = 1,
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
                    try
                    {
                        if (!File.Exists(FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][9]))
                            return;

                        if (!File.Exists(string.Format(@"{0}\Minimap{1}.png", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), selectedMapID)))
                        {
                            Texture2D minimapTexture = Texture2D.FromFile(graphicsDevice, FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][9]);
                            minimapTexture.Save(string.Format(@"{0}\Minimap{1}.png", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), selectedMapID), ImageFileFormat.Png);
                            minimapTexture.Dispose();
                        }

                        byte[] buffer = File.ReadAllBytes(string.Format(@"{0}\Minimap{1}.png", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), selectedMapID));
                        File.Delete(string.Format(@"{0}\Minimap{1}.png", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), selectedMapID));

                        Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                        {
                            BitmapImage minimapImage = new BitmapImage();
                            minimapImage.BeginInit();
                            minimapImage.StreamSource = new MemoryStream(buffer);
                            minimapImage.EndInit();
                            minimapImage.Freeze();

                            imageGrid.Width = minimapImage.Width;
                            imageGrid.Height = minimapImage.Height;

                            imageGrid.Children.Add(new Image()
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Source = minimapImage,
                                Width = minimapImage.Width,
                                Height = minimapImage.Height,
                                SnapsToDevicePixels = true
                            });

                            return null;
                        }, null);
                    }
                    catch
                    {
                        MessageBox.Show(string.Format("An error occured while trying to load\n{0}", FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][9]), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    if (!Directory.Exists(System.IO.Path.GetDirectoryName(FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][2])))
                        return;

                    float startPositionX = (float)Convert.ToInt32(FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][10]) * 160.0f;
                    float startPositionY = 10400.0f - ((float)Convert.ToInt32(FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][11]) * 160.0f);

                    string[] ifoFiles = Directory.GetFiles(System.IO.Path.GetDirectoryName(FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][2]), "*.IFO");

                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                    {
                        for (int i = 0; i < ifoFiles.Length; i++)
                        {
                            IFO ifoFile = new IFO(ifoFiles[i]);

                            for (int j = 0; j < ifoFile.NPCs.Count; j++)
                            {
                                imageGrid.Children.Add(new Rectangle()
                                {
                                    Fill = System.Windows.Media.Brushes.LightGreen,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness((int)(64 - 2 + ((ifoFile.NPCs[j].Position.X - startPositionX) / 2.5f)), (int)(64 - 2 + ((startPositionY - ifoFile.NPCs[j].Position.Y) / 2.5f)), 0, 0),
                                    Width = 5,
                                    Height = 5,
                                    RadiusX = 1,
                                    RadiusY = 1,
                                    Stroke = System.Windows.Media.Brushes.Gray,
                                    StrokeThickness = 1,
                                    SnapsToDevicePixels = true
                                });
                            }

                            for (int j = 0; j < ifoFile.Monsters.Count; j++)
                            {
                                imageGrid.Children.Add(new Rectangle()
                                {
                                    Fill = System.Windows.Media.Brushes.Red,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness((int)(64 - 2 + ((ifoFile.Monsters[j].Position.X - startPositionX) / 2.5f)), (int)(64 - 2 + ((startPositionY - ifoFile.Monsters[j].Position.Y) / 2.5f)), 0, 0),
                                    Width = 5,
                                    Height = 5,
                                    RadiusX = 1,
                                    RadiusY = 1,
                                    Stroke = System.Windows.Media.Brushes.Gray,
                                    StrokeThickness = 1,
                                    SnapsToDevicePixels = true
                                });
                            }

                            for (int j = 0; j < ifoFile.EventTriggers.Count; j++)
                            {
                                imageGrid.Children.Add(new Rectangle()
                                {
                                    Fill = System.Windows.Media.Brushes.Yellow,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness((int)(64 - 2 + ((ifoFile.EventTriggers[j].Position.X - startPositionX) / 2.5f)), (int)(64 - 2 + ((startPositionY - ifoFile.EventTriggers[j].Position.Y) / 2.5f)), 0, 0),
                                    Width = 5,
                                    Height = 5,
                                    RadiusX = 1,
                                    RadiusY = 1,
                                    Stroke = System.Windows.Media.Brushes.Gray,
                                    StrokeThickness = 1,
                                    SnapsToDevicePixels = true
                                });
                            }

                            for (int j = 0; j < ifoFile.WarpGates.Count; j++)
                            {
                                imageGrid.Children.Add(new Rectangle()
                                {
                                    Fill = System.Windows.Media.Brushes.Orange,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness((int)(64 - 2 + ((ifoFile.WarpGates[j].Position.X - startPositionX) / 2.5f)), (int)(64 - 2 + ((startPositionY - ifoFile.WarpGates[j].Position.Y) / 2.5f)), 0, 0),
                                    Width = 5,
                                    Height = 5,
                                    RadiusX = 1,
                                    RadiusY = 1,
                                    Stroke = System.Windows.Media.Brushes.Gray,
                                    StrokeThickness = 1,
                                    SnapsToDevicePixels = true
                                });
                            }

                            for (int j = 0; j < ifoFile.Sounds.Count; j++)
                            {
                                imageGrid.Children.Add(new Rectangle()
                                {
                                    Fill = System.Windows.Media.Brushes.LightGray,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness((int)(64 - 2 + ((ifoFile.Sounds[j].Position.X - startPositionX) / 2.5f)), (int)(64 - 2 + ((startPositionY - ifoFile.Sounds[j].Position.Y) / 2.5f)), 0, 0),
                                    Width = 5,
                                    Height = 5,
                                    RadiusX = 1,
                                    RadiusY = 1,
                                    Stroke = System.Windows.Media.Brushes.Gray,
                                    StrokeThickness = 1,
                                    SnapsToDevicePixels = true
                                });
                            }

                            for (int j = 0; j < ifoFile.Effects.Count; j++)
                            {
                                imageGrid.Children.Add(new Rectangle()
                                {
                                    Fill = System.Windows.Media.Brushes.BlueViolet,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness((int)(64 - 2 + ((ifoFile.Effects[j].Position.X - startPositionX) / 2.5f)), (int)(64 - 2 + ((startPositionY - ifoFile.Effects[j].Position.Y) / 2.5f)), 0, 0),
                                    Width = 5,
                                    Height = 5,
                                    RadiusX = 1,
                                    RadiusY = 1,
                                    Stroke = System.Windows.Media.Brushes.Gray,
                                    StrokeThickness = 1,
                                    SnapsToDevicePixels = true
                                });
                            }

                            for (int j = 0; j < ifoFile.Animation.Count; j++)
                            {
                                imageGrid.Children.Add(new Rectangle()
                                {
                                    Fill = System.Windows.Media.Brushes.Plum,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness((int)(64 - 2 + ((ifoFile.Animation[j].Position.X - startPositionX) / 2.5f)), (int)(64 - 2 + ((startPositionY - ifoFile.Animation[j].Position.Y) / 2.5f)), 0, 0),
                                    Width = 5,
                                    Height = 5,
                                    RadiusX = 1,
                                    RadiusY = 1,
                                    Stroke = System.Windows.Media.Brushes.Gray,
                                    StrokeThickness = 1,
                                    SnapsToDevicePixels = true
                                });
                            }
                        }

                        MinimapViewer.Children.Add(imageGrid);

                        return null;
                    }, null);
                }
            }))
            {
                ApartmentState = ApartmentState.MTA,
                IsBackground = true
            }.Start();

            Planet.Text = FileManager.STLs["STR_PLANET"].Search(FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][20]);
            ZONFile.Text = FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][2];
            DecorationZSC.Text = FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][12];
            ConstructionZSC.Text = FileManager.STBs["LIST_ZONE"].Cells[selectedMapID][13];
        }
    }
}