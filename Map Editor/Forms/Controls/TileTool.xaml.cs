using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Map_Editor.Engine;
using Map_Editor.Engine.Shaders;
using Map_Editor.Engine.Tools;
using Map_Editor.Misc;
using XNA = Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Forms.Controls
{
    /// <summary>
    /// Interaction logic for TileTool.xaml
    /// </summary>
    public partial class TileTool : UserControl
    {
        /// <summary>
        /// Width and height of the images.
        /// </summary>
        public const int IMAGE_DIMENSION = 120;

        #region Image Colours

        /// <summary>
        /// Normal colour.
        /// </summary>
        public readonly SolidColorBrush COLOUR_NORMAL = new SolidColorBrush(Color.FromRgb(201, 210, 225));

        /// <summary>
        /// Hover colour.
        /// </summary>
        public readonly SolidColorBrush COLOUR_HOVER = new SolidColorBrush(Color.FromRgb(225, 201, 201));

        /// <summary>
        /// Selected colour.
        /// </summary>
        public readonly SolidColorBrush COLOUR_SELECTED = new SolidColorBrush(Color.FromRgb(170, 54, 54));

        #endregion

        #region Static Members

        /// <summary>
        /// Gets or sets a value indicating whether [generating images].
        /// </summary>
        /// <value><c>true</c> if [generating images]; otherwise, <c>false</c>.</value>
        public static bool GeneratingImages { get; set; }

        /// <summary>
        /// Gets or sets the tile images.
        /// </summary>
        /// <value>The tile images.</value>
        private static List<BitmapImage> tileImages { get; set; }

        /// <summary>
        /// Gets or sets the image thread.
        /// </summary>
        /// <value>The image thread.</value>
        public static Thread ImageThread { get; set; }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the tile.
        /// </summary>
        /// <value>The tile.</value>
        public int Tile { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TileTool"/> class.
        /// </summary>
        public TileTool()
        {
            InitializeComponent();

            if (tileImages == null || tileImages.Count != FileManager.ZON.Tiles.Count)
                LoadImages(this);
            else
            {
                for (int i = 0; i < tileImages.Count; i++)
                    AddImage(i);
            }
        }

        /// <summary>
        /// Selects the tile.
        /// </summary>
        /// <param name="index">The index.</param>
        public void SelectTile(int index)
        {
            imageHost_MouseLeftButtonDown(((StackPanel)(((StackPanel)ImageView.Content).Children[(int)Math.Floor((float)index / 2.0f)])).Children[index % 2], null);

            ImageView.ScrollToVerticalOffset(ImageOffset(index));
        }

        /// <summary>
        /// Calculates the vertical offset of a tile image.
        /// </summary>
        /// <param name="index">The tile index.</param>
        /// <returns>The vertical offset.</returns>
        public int ImageOffset(int index)
        {
            return (int)Math.Floor((float)index / 2.0f) * (IMAGE_DIMENSION + 7);
        }

        #region Static Image Functions

        /// <summary>
        /// Loads the images.
        /// </summary>
        /// <param name="toolWindow">The tool window.</param>
        public static void LoadImages(TileTool toolWindow)
        {
            DateTime loadStart = DateTime.Now;
            Output.WriteLine(Output.MessageType.Event, "Generating Tile Images");

            tileImages = new List<BitmapImage>();

            ImageThread = new Thread(new ThreadStart(delegate
            {
                GeneratingImages = true;

                using (XNA.GraphicsDevice graphicsDevice = new XNA.GraphicsDevice(XNA.GraphicsAdapter.DefaultAdapter, XNA.DeviceType.Hardware, new System.Windows.Forms.Panel().Handle, new XNA.PresentationParameters()
                {
                    AutoDepthStencilFormat = XNA.DepthFormat.Depth24,
                    BackBufferCount = 1,
                    BackBufferFormat = XNA.SurfaceFormat.Color,
                    BackBufferHeight = IMAGE_DIMENSION,
                    BackBufferWidth = IMAGE_DIMENSION,
                    EnableAutoDepthStencil = true,
                    FullScreenRefreshRateInHz = 0,
                    IsFullScreen = false,
                    MultiSampleQuality = 0,
                    MultiSampleType = XNA.MultiSampleType.NonMaskable,
                    PresentationInterval = XNA.PresentInterval.One,
                    PresentOptions = XNA.PresentOptions.None,
                    RenderTargetUsage = XNA.RenderTargetUsage.DiscardContents,
                    SwapEffect = XNA.SwapEffect.Discard
                }))
                {
                    graphicsDevice.RenderState.CullMode = XNA.CullMode.None;

                    XNA.Viewport viewport = graphicsDevice.Viewport;

                    viewport.Width = IMAGE_DIMENSION;
                    viewport.Height = IMAGE_DIMENSION;

                    graphicsDevice.Viewport = viewport;

                    using (XNA.SpriteBatch spriteBatch = new XNA.SpriteBatch(graphicsDevice))
                    {
                        using (XNA.Effect shader = TileRotation.CreateEffect(graphicsDevice))
                        {
                            int id = 0;

                            for (int i = 0; i < FileManager.ZON.Tiles.Count; i++)
                            {
                                XNA.RenderTarget2D renderTarget = new XNA.RenderTarget2D(graphicsDevice, IMAGE_DIMENSION, IMAGE_DIMENSION, 1, graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.MultiSampleType, graphicsDevice.PresentationParameters.MultiSampleQuality, graphicsDevice.PresentationParameters.RenderTargetUsage);

                                graphicsDevice.SetRenderTarget(0, renderTarget);

                                graphicsDevice.Clear(XNA.ClearOptions.Target | XNA.ClearOptions.DepthBuffer, new XNA.Color(245, 247, 250), 1.0f, 0);

                                DrawTile(graphicsDevice, i, shader, spriteBatch);

                                graphicsDevice.SetRenderTarget(0, null);

                                renderTarget.GetTexture().Save("PreviewImage.bmp", XNA.ImageFileFormat.Bmp);

                                byte[] buffer = File.ReadAllBytes("PreviewImage.bmp");

                                BitmapImage previewImage = new BitmapImage();
                                previewImage.BeginInit();
                                previewImage.StreamSource = new MemoryStream(buffer);
                                previewImage.EndInit();
                                previewImage.Freeze();

                                tileImages.Add(previewImage);

                                int currentID = id;

                                toolWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                                {
                                    toolWindow.AddImage(currentID);

                                    return null;

                                }, null);

                                renderTarget.Dispose();
                                renderTarget = null;

                                id++;

                                if (id == (FileManager.ZON.Tiles.Count / 5))
                                    Output.WriteLine(Output.MessageType.Normal, "- 20%...");
                                else if (id == (FileManager.ZON.Tiles.Count / 5) * 2)
                                    Output.WriteLine(Output.MessageType.Normal, "- 40%...");
                                else if (id == (FileManager.ZON.Tiles.Count / 5) * 3)
                                    Output.WriteLine(Output.MessageType.Normal, "- 60%...");
                                else if (id == (FileManager.ZON.Tiles.Count / 5) * 4)
                                    Output.WriteLine(Output.MessageType.Normal, "- 80%...");
                            }
                        }
                    }

                    File.Delete("PreviewImage.bmp");

                    Output.WriteLine(Output.MessageType.Normal, "- 100%...");
                    Output.WriteLine(Output.MessageType.Event, string.Format("Created {0} Images in {1} Second(s)", FileManager.ZON.Tiles.Count, (DateTime.Now - loadStart).TotalSeconds));
                }

                GeneratingImages = false;

                toolWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                {
                    toolWindow.SelectTile(0);

                    return null;

                }, null);
            }));
            ImageThread.IsBackground = true;
            ImageThread.Start();
        }

        /// <summary>
        /// Draws the tile.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="index">The index.</param>
        /// <param name="shader">The shader.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public static void DrawTile(XNA.GraphicsDevice device, int index, XNA.Effect shader, XNA.SpriteBatch spriteBatch)
        {
            try
            {
                XNA.Texture2D bottomTexture = XNA.Texture2D.FromFile(device, FileManager.ZON.Textures[FileManager.ZON.Tiles[index].ID1].Path);
                XNA.Texture2D topTexture = XNA.Texture2D.FromFile(device, FileManager.ZON.Textures[FileManager.ZON.Tiles[index].ID2].Path);

                spriteBatch.Begin(XNA.SpriteBlendMode.None, XNA.SpriteSortMode.Deferred, XNA.SaveStateMode.SaveState);

                spriteBatch.Draw(bottomTexture, new Microsoft.Xna.Framework.Rectangle(0, 0, IMAGE_DIMENSION, IMAGE_DIMENSION), XNA.Color.White);

                spriteBatch.End();

                spriteBatch.Begin(XNA.SpriteBlendMode.AlphaBlend, XNA.SpriteSortMode.Deferred, XNA.SaveStateMode.SaveState);

                shader.SetValue("RotationValue", (int)FileManager.ZON.Tiles[index].Rotation);

                shader.Start("TileRotation");

                spriteBatch.Draw(topTexture, new Microsoft.Xna.Framework.Rectangle(0, 0, IMAGE_DIMENSION, IMAGE_DIMENSION), XNA.Color.White);

                shader.Finish();

                spriteBatch.End();

                bottomTexture.Dispose();
                topTexture.Dispose();
            }
            catch
            {
                Output.WriteLine(Output.MessageType.Error, string.Format("Error Drawing Tile: {0}", index));
            }
        }

        /// <summary>
        /// Clears the images.
        /// </summary>
        public static void ClearImages()
        {
            if (tileImages == null)
                return;

            if (ImageThread.IsAlive)
                ImageThread.Abort();

            tileImages.Clear();
            tileImages = null;
        }

        #endregion

        /// <summary>
        /// Adds the image.
        /// </summary>
        /// <param name="index">The index.</param>
        public void AddImage(int index)
        {
            if (index % 2 == 0)
            {
                ((StackPanel)ImageView.Content).Children.Add(new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 7, 0, 0)
                });
            }

            Border imageHost = new Border()
            {
                BorderBrush = COLOUR_NORMAL,
                BorderThickness = new Thickness(2),
                Margin = new Thickness(7, 0, 0, 0),
                Width = IMAGE_DIMENSION,
                Height = IMAGE_DIMENSION,
                SnapsToDevicePixels = true,
                Cursor = Cursors.Hand,
                Tag = index,
                ToolTip = string.Format("ID: {0}", FileManager.ZON.Tiles[index].ID2),
                Child = new Image()
                {
                    Margin = new Thickness(0),
                    Width = IMAGE_DIMENSION,
                    Height = IMAGE_DIMENSION,
                    Source = tileImages[index],
                    SnapsToDevicePixels = true
                }
            };

            imageHost.MouseEnter += new MouseEventHandler(imageHost_MouseEnter);
            imageHost.MouseLeave += new MouseEventHandler(imageHost_MouseLeave);
            imageHost.MouseLeftButtonDown += new MouseButtonEventHandler(imageHost_MouseLeftButtonDown);

            ((StackPanel)(((StackPanel)ImageView.Content).Children[((StackPanel)ImageView.Content).Children.Count - 1])).Children.Add(imageHost);
        }

        #region Image Events

        /// <summary>
        /// Handles the MouseEnter event of the imageHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void imageHost_MouseEnter(object sender, MouseEventArgs e)
        {
            Border imageHost = (Border)sender;

            if (imageHost.BorderBrush == COLOUR_SELECTED)
                return;

            imageHost.BorderBrush = COLOUR_HOVER;
        }

        /// <summary>
        /// Handles the MouseLeave event of the imageHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void imageHost_MouseLeave(object sender, MouseEventArgs e)
        {
            Border imageHost = (Border)sender;

            if (imageHost.BorderBrush == COLOUR_SELECTED)
                return;

            imageHost.BorderBrush = COLOUR_NORMAL;
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the imageHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void imageHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GeneratingImages)
            {
                MessageBox.Show("Please wait for all images to be generated", "Please wait...", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Border imageHost = (Border)sender;

            if (imageHost.BorderBrush == COLOUR_SELECTED)
                return;

            if (Tile >= 0)
            {
                Border previousBrush = (Border)((StackPanel)(((StackPanel)ImageView.Content).Children[(int)Math.Floor((float)Tile / 2.0f)])).Children[Tile % 2];

                previousBrush.BorderBrush = COLOUR_NORMAL;
                previousBrush.Cursor = Cursors.Hand;
            }

            imageHost.BorderBrush = COLOUR_SELECTED;
            imageHost.Cursor = Cursors.Arrow;

            Tile = (int)imageHost.Tag;

            if (FileManager.ZON.Textures[FileManager.ZON.Tiles[Tile].ID1].Tile == null)
                FileManager.ZON.Textures[FileManager.ZON.Tiles[Tile].ID1].Tile = XNA.Texture2D.FromFile(MapManager.Heightmaps.GraphicsDevice, FileManager.ZON.Textures[FileManager.ZON.Tiles[Tile].ID1].Path);

            if (FileManager.ZON.Textures[FileManager.ZON.Tiles[Tile].ID2].Tile == null)
                FileManager.ZON.Textures[FileManager.ZON.Tiles[Tile].ID2].Tile = XNA.Texture2D.FromFile(MapManager.Heightmaps.GraphicsDevice, FileManager.ZON.Textures[FileManager.ZON.Tiles[Tile].ID2].Path);

            if (MapManager.Heightmaps.TileTool.ToolMode == Tiles.TileToolType.None && MapManager.Heightmaps.TileTool.ToolMode != Tiles.TileToolType.Pick)
                MapManager.Heightmaps.TileTool.ToolMode = Tiles.TileToolType.Modify;
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the Find control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            if (GeneratingImages)
            {
                MessageBox.Show("Please wait for all images to be generated", "Please wait...", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Search findObject = new Search(Forms.Search.FindType.GoTo);

            findObject.GoToClicked += new Search.GoTo(delegate(int value)
            {
                if (value < FileManager.ZON.Tiles.Count)
                {
                    if (MessageBox.Show("Tile found, do you wish to select it?", "Select Tile", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        SelectTile(value);

                        findObject.Close();
                    }

                    return true;
                }

                return false;
            });

            findObject.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the SinglePlacement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SinglePlacement_Click(object sender, RoutedEventArgs e)
        {
            MultipleSelection.IsEnabled = MultiplePlacement.IsChecked ?? false;

            if (MultipleSelection.IsEnabled && MultipleSelection.SelectedIndex == -1)
                MultipleSelection.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Checked event of the EyeDropper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void EyeDropper_Checked(object sender, RoutedEventArgs e)
        {
            MapManager.Heightmaps.TileTool.ToolMode = Tiles.TileToolType.Pick;
        }

        /// <summary>
        /// Handles the Unchecked event of the EyeDropper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void EyeDropper_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.Heightmaps.TileTool.ToolMode = Tiles.TileToolType.Modify;
        }
    }
}