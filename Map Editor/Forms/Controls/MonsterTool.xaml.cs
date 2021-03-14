using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Map_Editor.Engine;
using Map_Editor.Engine.Map;
using Map_Editor.Engine.Models;
using Map_Editor.Engine.RenderManager;
using Map_Editor.Misc;
using Map_Editor.Misc.Properties;
using Microsoft.Xna.Framework;
using XNA = Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Forms.Controls
{
    /// <summary>
    /// Interaction logic for MonsterTool.xaml
    /// </summary>
    public partial class MonsterTool : UserControl
    {
        /// <summary>
        /// Monster type.
        /// </summary>
        public enum MonsterType
        {
            /// <summary>
            /// Basic.
            /// </summary>
            Basic,

            /// <summary>
            /// Tactical.
            /// </summary>
            Tactical
        }

        /// <summary>
        /// Width and height of the images.
        /// </summary>
        public const int IMAGE_DIMENSION = 120;

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public System.Windows.Forms.PropertyGrid Properties
        {
            get { return (System.Windows.Forms.PropertyGrid)PropertiesHost.Child; }
        }

        #region Static Members

        /// <summary>
        /// Gets or sets the copied object.
        /// </summary>
        /// <value>The copied object.</value>
        public static IFO.MonsterSpawn CopiedObject { get; set; }

        /// <summary>
        /// Gets or sets the cached images.
        /// </summary>
        /// <value>The cached images.</value>
        public static Dictionary<int, BitmapImage> CachedImages { get; set; }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        private int selectedObject { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterTool"/> class.
        /// </summary>
        public MonsterTool()
        {
            InitializeComponent();

            if (CachedImages == null)
                CachedImages = new Dictionary<int, BitmapImage>();

            DrawRadii.IsChecked = ConfigurationManager.GetValue<bool>("Monsters", "DrawRadii");
        }

        /// <summary>
        /// Handles the Click event of the DrawRadii control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DrawRadii_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.SetValue("Monsters", "DrawRadii", DrawRadii.IsChecked);
            ConfigurationManager.SaveConfig();
        }

        /// <summary>
        /// Selects the specified object.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int Select(int index)
        {
            selectedObject = index;

            Add.IsChecked = false;
            Remove.IsEnabled = false;
            Clone.IsEnabled = false;
            Clone.IsChecked = false;

            IFO.MonsterSpawn ifoEntry = MapManager.Monsters.WorldObjects[index].Entry;

            Properties.SelectedObject = new MonsterSpawnProperty()
            {
                Description = ifoEntry.Description,
                EventID = ifoEntry.EventID,
                Name = ifoEntry.Name,
                Interval = ifoEntry.Interval,
                Limit = ifoEntry.Limit,
                Range = ifoEntry.Range,
                TacticPoints = ifoEntry.TacticPoints,
                Position = ifoEntry.Position,
            };

            LoadMonsters(ifoEntry);

            Remove.IsEnabled = true;
            Clone.IsEnabled = true;

            App.Form.Cut.IsEnabled = true;
            App.Form.QuickCut.IsEnabled = true;

            App.Form.Copy.IsEnabled = true;
            App.Form.QuickCopy.IsEnabled = true;

            App.Form.Paste.IsEnabled = CopiedObject != null;
            App.Form.QuickPaste.IsEnabled = CopiedObject != null;

            return index;
        }

        /// <summary>
        /// Cleans this instance.
        /// </summary>
        public void Clean()
        {
            Remove.IsEnabled = false;
            Clone.IsEnabled = false;
            Clone.IsChecked = false;

            AddBasic.IsEnabled = false;
            AddBasic.IsChecked = false;
            RemoveBasic.IsEnabled = false;
            EditBasic.IsEnabled = false;
            EditBasic.IsChecked = false;
            CloneBasic.IsEnabled = false;

            AddTactical.IsEnabled = false;
            AddTactical.IsChecked = false;
            RemoveTactical.IsEnabled = false;
            EditTactical.IsEnabled = false;
            EditTactical.IsChecked = false;
            CloneTactical.IsEnabled = false;

            App.Form.Cut.IsEnabled = false;
            App.Form.QuickCut.IsEnabled = false;

            App.Form.Copy.IsEnabled = false;
            App.Form.QuickCopy.IsEnabled = false;

            App.Form.Paste.IsEnabled = false;
            App.Form.QuickPaste.IsEnabled = false;

            if (BasicList.Items.Count > 0)
                BasicList.Items.Clear();

            if (TacticalList.Items.Count > 0)
                TacticalList.Items.Clear();

            Properties.SelectedObject = null;

            selectedObject = -1;
        }

        /// <summary>
        /// Loads the monsters.
        /// </summary>
        public void LoadMonsters()
        {
            LoadMonsters(MapManager.Monsters.WorldObjects[selectedObject].Entry);
        }

        /// <summary>
        /// Loads the monsters.
        /// </summary>
        /// <param name="ifoEntry">The ifo entry.</param>
        public void LoadMonsters(IFO.MonsterSpawn ifoEntry)
        {
            if (BasicList.Items.Count > 0)
                BasicList.Items.Clear();

            for (int i = 0; i < ifoEntry.Basic.Count; i++)
            {
                ListBoxItem monsterItem = new ListBoxItem()
                {
                    Content = string.Format("[{0}] {1}; Count: {2}", i, FileManager.STLs["LIST_NPC_S"].Search(FileManager.STBs["LIST_NPC"].Cells[ifoEntry.Basic[i].ID][41]), ifoEntry.Basic[i].Count),
                    ToolTip = "Loading..."
                };

                BasicList.Items.Add(monsterItem);
            }

            AddBasic.IsEnabled = ifoEntry.Basic.Count < 5;
            RemoveBasic.IsEnabled = false;
            EditBasic.IsEnabled = false;
            EditBasic.IsChecked = false;
            CloneBasic.IsEnabled = false;

            if (TacticalList.Items.Count > 0)
                TacticalList.Items.Clear();

            for (int i = 0; i < ifoEntry.Tactic.Count; i++)
            {
                ListBoxItem monsterItem = new ListBoxItem()
                {
                    Content = string.Format("[{0}] {1}; Count: {2}", i, FileManager.STLs["LIST_NPC_S"].Search(FileManager.STBs["LIST_NPC"].Cells[ifoEntry.Tactic[i].ID][41]), ifoEntry.Tactic[i].Count),
                    ToolTip = "Loading..."
                };

                TacticalList.Items.Add(monsterItem);
            }

            new Thread(new ThreadStart(delegate
            {
                for (int i = 0; i < ifoEntry.Basic.Count; i++)
                {
                    int monsterID = i;

                    try
                    {
                        BitmapImage imageSource = CreateImage(ifoEntry.Basic[monsterID].ID);

                        Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                        {
                            ((ListBoxItem)BasicList.Items[monsterID]).ToolTip = new Image()
                            {
                                Source = imageSource
                            };

                            return null;

                        }, null);
                    }
                    catch { }
                }

                for (int i = 0; i < ifoEntry.Tactic.Count; i++)
                {
                    int monsterID = i;

                    try
                    {
                        BitmapImage imageSource = CreateImage(ifoEntry.Tactic[monsterID].ID);

                        Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                        {
                            ((ListBoxItem)TacticalList.Items[monsterID]).ToolTip = new Image()
                            {
                                Source = imageSource
                            };

                            return null;

                        }, null);
                    }
                    catch { }
                }
            }))
            {
                IsBackground = true
            }.Start();

            AddTactical.IsEnabled = ifoEntry.Tactic.Count < 5;
            RemoveTactical.IsEnabled = false;
            EditTactical.IsEnabled = false;
            EditTactical.IsChecked = false;
            CloneTactical.IsEnabled = false;
        }

        /// <summary>
        /// Creates the image.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public BitmapImage CreateImage(int id)
        {
            if (CachedImages.ContainsKey(id))
                return CachedImages[id];

            string fileName = string.Format("Monster_{0}.bmp", id);

            BitmapImage previewImage = new BitmapImage();

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

                ZSC zscFile = FileManager.ZSCs["PART_NPC"];

                using (XNA.BasicEffect shader = new XNA.BasicEffect(graphicsDevice, null))
                {
                    XNA.RenderTarget2D renderTarget = new XNA.RenderTarget2D(graphicsDevice, IMAGE_DIMENSION, IMAGE_DIMENSION, 1, graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.MultiSampleType, graphicsDevice.PresentationParameters.MultiSampleQuality, graphicsDevice.PresentationParameters.RenderTargetUsage);

                    graphicsDevice.SetRenderTarget(0, renderTarget);

                    graphicsDevice.Clear(XNA.ClearOptions.Target | XNA.ClearOptions.DepthBuffer, new XNA.Color(245, 247, 250), 1.0f, 0);

                    List<short> chrModels = FileManager.CHRs["LIST_NPC"].Characters[id].Models;
                    ZMS[][] models = new ZMS[chrModels.Count][];

                    BoundingBox boundingBox = new BoundingBox();

                    for (int i = 0; i < chrModels.Count; i++)
                    {
                        models[i] = new ZMS[zscFile.Objects[chrModels[i]].Models.Count];

                        for (int j = 0; j < zscFile.Objects[chrModels[i]].Models.Count; j++)
                        {
                            Microsoft.Xna.Framework.Matrix world = Microsoft.Xna.Framework.Matrix.Identity;
                            world *= Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(zscFile.Objects[chrModels[i]].Models[j].Rotation);
                            world *= Microsoft.Xna.Framework.Matrix.CreateScale(zscFile.Objects[chrModels[i]].Models[j].Scale);
                            world *= Microsoft.Xna.Framework.Matrix.CreateTranslation(zscFile.Objects[chrModels[i]].Models[j].Position);

                            models[i][j] = new ZMS();
                            models[i][j].Load(zscFile.Models[zscFile.Objects[chrModels[i]].Models[j].ModelID]);

                            Vector3[] modelPoints = new Vector3[models[i][j].VertexCount];

                            for (int k = 0; k < modelPoints.Length; k++)
                                modelPoints[k] = Vector3.Transform(models[i][j].Vertices[k].Position, world);

                            if (i == 0 && j == 0)
                                boundingBox = BoundingBox.CreateFromPoints(modelPoints);
                            else
                                boundingBox = BoundingBox.CreateMerged(boundingBox, BoundingBox.CreateFromPoints(modelPoints));
                        }
                    }

                    viewport.MinDepth = 1.0f;
                    viewport.MaxDepth = 50000.0f;

                    Microsoft.Xna.Framework.Matrix view = Microsoft.Xna.Framework.Matrix.Identity;
                    Microsoft.Xna.Framework.Matrix projection = Microsoft.Xna.Framework.Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, viewport.AspectRatio, viewport.MinDepth, viewport.MaxDepth);

                    BoundingSphere modelSphere = BoundingSphere.CreateFromBoundingBox(boundingBox);

                    Vector3 cameraPosition = Vector3.Transform(modelSphere.Center + (Vector3.Forward * ((modelSphere.Radius + 0.5f) / (float)Math.Sin(MathHelper.PiOver4 / 2))), Microsoft.Xna.Framework.Matrix.CreateRotationX(MathHelper.ToRadians(270)));

                    view = Microsoft.Xna.Framework.Matrix.CreateLookAt(cameraPosition, modelSphere.Center, Vector3.Down);

                    for (int i = 0; i < chrModels.Count; i++)
                    {
                        for (int j = 0; j < zscFile.Objects[chrModels[i]].Models.Count; j++)
                        {
                            Microsoft.Xna.Framework.Matrix world = Microsoft.Xna.Framework.Matrix.Identity;
                            world *= Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(zscFile.Objects[chrModels[i]].Models[j].Rotation);
                            world *= Microsoft.Xna.Framework.Matrix.CreateScale(zscFile.Objects[chrModels[i]].Models[j].Scale);
                            world *= Microsoft.Xna.Framework.Matrix.CreateTranslation(zscFile.Objects[chrModels[i]].Models[j].Position);

                            using (XNA.Texture2D texture = XNA.Texture2D.FromFile(graphicsDevice, zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].Path))
                            {
                                shader.World = world;
                                shader.View = view;
                                shader.Projection = projection;

                                shader.DiffuseColor = Microsoft.Xna.Framework.Graphics.Color.White.ToVector3();
                                shader.Texture = texture;
                                shader.TextureEnabled = true;

                                shader.Alpha = zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].Alpha;

                                shader.Begin();

                                graphicsDevice.VertexDeclaration = new XNA.VertexDeclaration(graphicsDevice, ZMS.Vertex.VertexElements);
                                graphicsDevice.Indices = models[i][j].CreateIndexBuffer(graphicsDevice);
                                graphicsDevice.Vertices[0].SetSource(models[i][j].CreateVertexBuffer(graphicsDevice), 0, ZMS.Vertex.SIZE_IN_BYTES);

                                graphicsDevice.RenderState.AlphaBlendEnable = zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].AlphaEnabled;

                                if (graphicsDevice.RenderState.AlphaBlendEnable)
                                {
                                    graphicsDevice.RenderState.AlphaTestEnable = zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].AlphaTestEnabled;
                                    graphicsDevice.RenderState.SourceBlend = TextureManager.GetSourceBlend(zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].BlendingMode);
                                    graphicsDevice.RenderState.DestinationBlend = TextureManager.GetDestinationBlend(zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].BlendingMode);
                                    graphicsDevice.RenderState.ReferenceAlpha = zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].AlphaReference;
                                    graphicsDevice.RenderState.AlphaFunction = XNA.CompareFunction.GreaterEqual;
                                    graphicsDevice.RenderState.BlendFunction = TextureManager.GetBlendFunction(zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].BlendingMode);
                                }
                                else
                                {
                                    graphicsDevice.RenderState.AlphaTestEnable = false;
                                    graphicsDevice.RenderState.SourceBlend = XNA.Blend.One;
                                    graphicsDevice.RenderState.DestinationBlend = XNA.Blend.Zero;
                                    graphicsDevice.RenderState.ReferenceAlpha = 0;
                                    graphicsDevice.RenderState.AlphaFunction = XNA.CompareFunction.Always;
                                    graphicsDevice.RenderState.BlendFunction = XNA.BlendFunction.Add;
                                }

                                graphicsDevice.RenderState.DepthBufferEnable = zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].ZTestEnabled;
                                graphicsDevice.RenderState.DepthBufferWriteEnable = zscFile.Textures[zscFile.Objects[chrModels[i]].Models[j].TextureID].ZWriteEnabled;

                                for (int k = 0; k < shader.CurrentTechnique.Passes.Count; k++)
                                {
                                    shader.CurrentTechnique.Passes[k].Begin();

                                    graphicsDevice.DrawIndexedPrimitives(XNA.PrimitiveType.TriangleList, 0, 0, models[i][j].VertexCount, 0, models[i][j].IndexCount);

                                    shader.CurrentTechnique.Passes[k].End();
                                }

                                shader.End();
                            }
                        }
                    }

                    graphicsDevice.SetRenderTarget(0, null);

                    renderTarget.GetTexture().Save(fileName, XNA.ImageFileFormat.Bmp);

                    byte[] buffer = File.ReadAllBytes(fileName);

                    previewImage.BeginInit();
                    previewImage.StreamSource = new MemoryStream(buffer);
                    previewImage.EndInit();
                    previewImage.Freeze();

                    if (CachedImages.ContainsKey(id))
                        return CachedImages[id];

                    CachedImages.Add(id, previewImage);

                    int currentID = id;

                    renderTarget.Dispose();
                    renderTarget = null;
                }
            }

            File.Delete(fileName);

            return previewImage;
        }

        #region Button Events

        /// <summary>
        /// Handles the Checked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Checked(object sender, RoutedEventArgs e)
        {
            if (Clone.IsChecked == true)
                Clone.IsChecked = false;

            MapManager.Monsters.Tool.StartAdding();
        }

        /// <summary>
        /// Handles the Unchecked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.Monsters.Tool.StopAdding();
        }

        /// <summary>
        /// Handles the Click event of the Remove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            MapManager.Monsters.Tool.Remove(false);
        }

        /// <summary>
        /// Handles the Checked event of the Clone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Clone_Checked(object sender, RoutedEventArgs e)
        {
            if (Add.IsChecked == true)
                Add.IsChecked = false;

            MapManager.Monsters.Tool.StartAdding();
        }

        /// <summary>
        /// Handles the Unchecked event of the Clone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Clone_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.Monsters.Tool.StopAdding();
        }

        /// <summary>
        /// Handles the Checked event of the AddMonster control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddMonster_Checked(object sender, RoutedEventArgs e)
        {
            MonsterType monsterType = ((ToggleButton)sender).Name.EndsWith("Basic") ? MonsterType.Basic : MonsterType.Tactical;

            App.Form.PreviewPanel.Reset();

            App.Form.PreviewPanel.OnSelect += new EventHandler(delegate
            {
                int monsterID = App.Form.PreviewPanel.NPCList.SelectedIndex + 1;

                App.Form.PreviewPanel.Hide();

                switch (monsterType)
                {
                    case MonsterType.Basic:
                        {
                            MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Add(new IFO.MonsterSpawn.Monster()
                            {
                                Description = "Untitled",
                                ID = monsterID,
                                Count = 1
                            });
                        }
                        break;
                    case MonsterType.Tactical:
                        {
                            MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Add(new IFO.MonsterSpawn.Monster()
                            {
                                Description = "Untitled",
                                ID = monsterID,
                                Count = 1
                            });
                        }
                        break;
                }

                LoadMonsters();

                ((ToggleButton)sender).IsChecked = false;

                MapManager.Monsters.Add(selectedObject, MapManager.Monsters.WorldObjects[selectedObject].Entry, true);
            });

            App.Form.PreviewPanel.OnCancel += new EventHandler(delegate
            {
                ((ToggleButton)sender).IsChecked = false;
            });

            switch (monsterType)
            {
                case MonsterType.Basic:
                    {
                        if (AddTactical.IsChecked == true)
                            AddTactical.IsChecked = false;
                    }
                    break;
                case MonsterType.Tactical:
                    {
                        if (AddBasic.IsChecked == true)
                            AddBasic.IsChecked = false;
                    }
                    break;
            }

            App.Form.PreviewPanel.Show();
        }

        /// <summary>
        /// Handles the Unchecked event of the AddMonster control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddMonster_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Form.PreviewPanel.Hide();
        }

        /// <summary>
        /// Handles the Click event of the RemoveMonster control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveMonster_Click(object sender, RoutedEventArgs e)
        {
            MonsterType monsterType = ((Button)sender).Name.EndsWith("Basic") ? MonsterType.Basic : MonsterType.Tactical;

            Engine.Commands.Monsters.ValueChanged.ObjectValue oldValue = new Engine.Commands.Monsters.ValueChanged.ObjectValue()
            {
                Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Description,
                EventID = MapManager.Monsters.WorldObjects[selectedObject].Entry.EventID,
                Position = MapManager.Monsters.WorldObjects[selectedObject].Entry.Position,
                Name = MapManager.Monsters.WorldObjects[selectedObject].Entry.Name,
                Interval = MapManager.Monsters.WorldObjects[selectedObject].Entry.Interval,
                Limit = MapManager.Monsters.WorldObjects[selectedObject].Entry.Limit,
                Range = MapManager.Monsters.WorldObjects[selectedObject].Entry.Range,
                TacticPoints = MapManager.Monsters.WorldObjects[selectedObject].Entry.TacticPoints,
                Basic = new List<IFO.MonsterSpawn.Monster>(),
                Tactic = new List<IFO.MonsterSpawn.Monster>()
            };

            Engine.Commands.Monsters.ValueChanged.ObjectValue newValue = new Engine.Commands.Monsters.ValueChanged.ObjectValue()
            {
                Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Description,
                EventID = MapManager.Monsters.WorldObjects[selectedObject].Entry.EventID,
                Position = MapManager.Monsters.WorldObjects[selectedObject].Entry.Position,
                Name = MapManager.Monsters.WorldObjects[selectedObject].Entry.Name,
                Interval = MapManager.Monsters.WorldObjects[selectedObject].Entry.Interval,
                Limit = MapManager.Monsters.WorldObjects[selectedObject].Entry.Limit,
                Range = MapManager.Monsters.WorldObjects[selectedObject].Entry.Range,
                TacticPoints = MapManager.Monsters.WorldObjects[selectedObject].Entry.TacticPoints,
                Basic = new List<IFO.MonsterSpawn.Monster>(),
                Tactic = new List<IFO.MonsterSpawn.Monster>()
            };

            for (int i = 0; i < MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Count; i++)
            {
                oldValue.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Description
                });

                newValue.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Description
                });
            }

            for (int i = 0; i < MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Count; i++)
            {
                oldValue.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Description
                });

                newValue.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Description
                });
            }

            switch (monsterType)
            {
                case MonsterType.Basic:
                    {
                        MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.RemoveAt(BasicList.SelectedIndex);
                        newValue.Basic.RemoveAt(BasicList.SelectedIndex);

                        RemoveBasic.IsEnabled = false;
                        EditBasic.IsEnabled = false;
                        EditBasic.IsChecked = false;
                        CloneBasic.IsEnabled = false;
                    }
                    break;
                case MonsterType.Tactical:
                    {
                        MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.RemoveAt(TacticalList.SelectedIndex);
                        newValue.Tactic.RemoveAt(BasicList.SelectedIndex);

                        RemoveTactical.IsEnabled = false;
                        EditTactical.IsEnabled = false;
                        EditTactical.IsChecked = false;
                        CloneTactical.IsEnabled = false;
                    }
                    break;
            }

            UndoManager.AddCommand(new Engine.Commands.Monsters.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.Monsters.WorldObjects[selectedObject],
                OldValue = oldValue,
                NewValue = newValue
            });

            LoadMonsters();

            MapManager.Monsters.Add(selectedObject, MapManager.Monsters.WorldObjects[selectedObject].Entry, true);
        }

        /// <summary>
        /// Handles the Checked event of the EditMonster control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void EditMonster_Checked(object sender, RoutedEventArgs e)
        {
            MonsterType monsterType = ((ToggleButton)sender).Name.EndsWith("Basic") ? MonsterType.Basic : MonsterType.Tactical;

            IFO.MonsterSpawn.Monster monsterEntry = new IFO.MonsterSpawn.Monster();

            switch (monsterType)
            {
                case MonsterType.Basic:
                    monsterEntry = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[BasicList.SelectedIndex];
                    break;
                case MonsterType.Tactical:
                    monsterEntry = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[TacticalList.SelectedIndex];
                    break;
            }

            Properties.SelectedObject = new MonsterProperty()
            {
                Description = monsterEntry.Description,
                ID = monsterEntry.ID,
                Count = monsterEntry.Count
            };

            ((MonsterProperty)Properties.SelectedObject).ObjectIDChanged += new MonsterProperty.ObjectID(MonsterTool_ObjectIDChanged);
        }

        /// <summary>
        /// Handles the Unchecked event of the EditMonster control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void EditMonster_Unchecked(object sender, RoutedEventArgs e)
        {
            IFO.MonsterSpawn ifoEntry = MapManager.Monsters.WorldObjects[selectedObject].Entry;

            Properties.SelectedObject = new MonsterSpawnProperty()
            {
                Description = ifoEntry.Description,
                EventID = ifoEntry.EventID,
                Name = ifoEntry.Name,
                Interval = ifoEntry.Interval,
                Limit = ifoEntry.Limit,
                Range = ifoEntry.Range,
                TacticPoints = ifoEntry.TacticPoints,
                Position = ifoEntry.Position,
            };
        }

        /// <summary>
        /// Handles the Click event of the CloneMonster control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloneMonster_Click(object sender, RoutedEventArgs e)
        {
            MonsterType monsterType = ((Button)sender).Name.EndsWith("Basic") ? MonsterType.Basic : MonsterType.Tactical;

            Engine.Commands.Monsters.ValueChanged.ObjectValue oldValue = new Engine.Commands.Monsters.ValueChanged.ObjectValue()
            {
                Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Description,
                EventID = MapManager.Monsters.WorldObjects[selectedObject].Entry.EventID,
                Position = MapManager.Monsters.WorldObjects[selectedObject].Entry.Position,
                Name = MapManager.Monsters.WorldObjects[selectedObject].Entry.Name,
                Interval = MapManager.Monsters.WorldObjects[selectedObject].Entry.Interval,
                Limit = MapManager.Monsters.WorldObjects[selectedObject].Entry.Limit,
                Range = MapManager.Monsters.WorldObjects[selectedObject].Entry.Range,
                TacticPoints = MapManager.Monsters.WorldObjects[selectedObject].Entry.TacticPoints,
                Basic = new List<IFO.MonsterSpawn.Monster>(),
                Tactic = new List<IFO.MonsterSpawn.Monster>()
            };

            Engine.Commands.Monsters.ValueChanged.ObjectValue newValue = new Engine.Commands.Monsters.ValueChanged.ObjectValue()
            {
                Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Description,
                EventID = MapManager.Monsters.WorldObjects[selectedObject].Entry.EventID,
                Position = MapManager.Monsters.WorldObjects[selectedObject].Entry.Position,
                Name = MapManager.Monsters.WorldObjects[selectedObject].Entry.Name,
                Interval = MapManager.Monsters.WorldObjects[selectedObject].Entry.Interval,
                Limit = MapManager.Monsters.WorldObjects[selectedObject].Entry.Limit,
                Range = MapManager.Monsters.WorldObjects[selectedObject].Entry.Range,
                TacticPoints = MapManager.Monsters.WorldObjects[selectedObject].Entry.TacticPoints,
                Basic = new List<IFO.MonsterSpawn.Monster>(),
                Tactic = new List<IFO.MonsterSpawn.Monster>()
            };

            for (int i = 0; i < MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Count; i++)
            {
                oldValue.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Description
                });

                newValue.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Description
                });
            }

            for (int i = 0; i < MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Count; i++)
            {
                oldValue.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Description
                });

                newValue.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Description
                });
            }

            switch (monsterType)
            {
                case MonsterType.Basic:
                    {
                        MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Add(new IFO.MonsterSpawn.Monster()
                        {
                            ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[BasicList.SelectedIndex].ID,
                            Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[BasicList.SelectedIndex].Count,
                            Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[BasicList.SelectedIndex].Description
                        });

                        newValue.Basic.Add(new IFO.MonsterSpawn.Monster()
                        {
                            ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Count - 1].ID,
                            Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Count - 1].Count,
                            Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Count - 1].Description
                        });
                    }
                    break;
                case MonsterType.Tactical:
                    {
                        MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Add(new IFO.MonsterSpawn.Monster()
                        {
                            ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[TacticalList.SelectedIndex].ID,
                            Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[TacticalList.SelectedIndex].Count,
                            Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[TacticalList.SelectedIndex].Description
                        });

                        newValue.Tactic.Add(new IFO.MonsterSpawn.Monster()
                        {
                            ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Count - 1].ID,
                            Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Count - 1].Count,
                            Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Count - 1].Description
                        });
                    }
                    break;
            }

            UndoManager.AddCommand(new Engine.Commands.Monsters.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.Monsters.WorldObjects[selectedObject],
                OldValue = oldValue,
                NewValue = newValue
            });

            LoadMonsters();
        }

        #endregion

        /// <summary>
        /// Handles the SelectionChanged event of the MonsterList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void MonsterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MonsterType monsterType = ((ListBox)sender).Name.StartsWith("Basic") ? MonsterType.Basic : MonsterType.Tactical;

            switch (monsterType)
            {
                case MonsterType.Basic:
                    {
                        AddBasic.IsChecked = false;
                        RemoveBasic.IsEnabled = true;
                        EditBasic.IsEnabled = true;
                        EditBasic.IsChecked = false;
                        CloneBasic.IsEnabled = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Count < 5;
                    }
                    break;
                case MonsterType.Tactical:
                    {
                        AddTactical.IsChecked = false;
                        RemoveTactical.IsEnabled = true;
                        EditTactical.IsEnabled = true;
                        EditTactical.IsChecked = false;
                        CloneTactical.IsEnabled = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Count < 5;
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles the PropertyValueChanged event of the PropertyGrid control.
        /// </summary>
        /// <param name="s">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PropertyValueChangedEventArgs"/> instance containing the event data.</param>
        private void PropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            Engine.Commands.Monsters.ValueChanged.ObjectValue oldValue = new Engine.Commands.Monsters.ValueChanged.ObjectValue()
            {
                Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Description,
                EventID = MapManager.Monsters.WorldObjects[selectedObject].Entry.EventID,
                Position = MapManager.Monsters.WorldObjects[selectedObject].Entry.Position,
                Name = MapManager.Monsters.WorldObjects[selectedObject].Entry.Name,
                Interval = MapManager.Monsters.WorldObjects[selectedObject].Entry.Interval,
                Limit = MapManager.Monsters.WorldObjects[selectedObject].Entry.Limit,
                Range = MapManager.Monsters.WorldObjects[selectedObject].Entry.Range,
                TacticPoints = MapManager.Monsters.WorldObjects[selectedObject].Entry.TacticPoints,
                Basic = new List<IFO.MonsterSpawn.Monster>(),
                Tactic = new List<IFO.MonsterSpawn.Monster>()
            };

            Engine.Commands.Monsters.ValueChanged.ObjectValue newValue = new Engine.Commands.Monsters.ValueChanged.ObjectValue()
            {
                Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Description,
                EventID = MapManager.Monsters.WorldObjects[selectedObject].Entry.EventID,
                Position = MapManager.Monsters.WorldObjects[selectedObject].Entry.Position,
                Name = MapManager.Monsters.WorldObjects[selectedObject].Entry.Name,
                Interval = MapManager.Monsters.WorldObjects[selectedObject].Entry.Interval,
                Limit = MapManager.Monsters.WorldObjects[selectedObject].Entry.Limit,
                Range = MapManager.Monsters.WorldObjects[selectedObject].Entry.Range,
                TacticPoints = MapManager.Monsters.WorldObjects[selectedObject].Entry.TacticPoints,
                Basic = new List<IFO.MonsterSpawn.Monster>(),
                Tactic = new List<IFO.MonsterSpawn.Monster>()
            };

            for (int i = 0; i < MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Count; i++)
            {
                oldValue.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Description
                });

                newValue.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Description
                });
            }

            for (int i = 0; i < MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Count; i++)
            {
                oldValue.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Description
                });

                newValue.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Description
                });
            }

            if (Properties.SelectedObject.GetType() == typeof(MonsterSpawnProperty))
            {
                MonsterSpawnProperty objectValues = (MonsterSpawnProperty)Properties.SelectedObject;

                IFO.MonsterSpawn ifoEntry = MapManager.Monsters.WorldObjects[selectedObject].Entry;
                newValue.Description = ifoEntry.Description = objectValues.Description;
                newValue.EventID = ifoEntry.EventID = objectValues.EventID;
                newValue.Name = ifoEntry.Name = objectValues.Name;
                newValue.Interval = ifoEntry.Interval = objectValues.Interval;
                newValue.Limit = ifoEntry.Limit = objectValues.Limit;
                newValue.TacticPoints = ifoEntry.TacticPoints = objectValues.TacticPoints;
                newValue.Range = ifoEntry.Range = objectValues.Range;
                newValue.Position = ifoEntry.Position = objectValues.Position;

                MapManager.Monsters.Tool.ChangeWorld(objectValues.Position);
            }
            else
            {
                MonsterType monsterType = (EditBasic.IsChecked == true) ? MonsterType.Basic : MonsterType.Tactical;

                MonsterProperty objectValues = (MonsterProperty)Properties.SelectedObject;

                switch (monsterType)
                {
                    case MonsterType.Basic:
                        {
                            int monsterID = BasicList.SelectedIndex;

                            IFO.MonsterSpawn.Monster monsterEntry = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[monsterID];

                            newValue.Basic[monsterID].Description = monsterEntry.Description = objectValues.Description;
                            newValue.Basic[monsterID].ID = monsterEntry.ID = objectValues.ID;
                            newValue.Basic[monsterID].Count = monsterEntry.Count = objectValues.Count;

                            ((ListBoxItem)BasicList.Items[monsterID]).Content = string.Format("[{0}] {1}; Count: {2}", monsterID, FileManager.STLs["LIST_NPC_S"].Search(FileManager.STBs["LIST_NPC"].Cells[monsterEntry.ID][41]), monsterEntry.Count);

                            new Thread(new ThreadStart(delegate
                            {
                                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                                {
                                    BitmapImage imageSource = CreateImage(monsterEntry.ID);

                                    ((ListBoxItem)BasicList.Items[monsterID]).ToolTip = new Image()
                                    {
                                        Source = imageSource
                                    };

                                    return null;

                                }, null);
                            })).Start();
                        }
                        break;
                    case MonsterType.Tactical:
                        {
                            int monsterID = TacticalList.SelectedIndex;

                            IFO.MonsterSpawn.Monster monsterEntry = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[monsterID];

                            newValue.Tactic[monsterID].Description = monsterEntry.Description = objectValues.Description;
                            newValue.Tactic[monsterID].ID = monsterEntry.ID = objectValues.ID;
                            newValue.Tactic[monsterID].Count = monsterEntry.Count = objectValues.Count;

                            ((ListBoxItem)TacticalList.Items[monsterID]).Content = string.Format("[{0}] {1}; Count: {2}", monsterID, FileManager.STLs["LIST_NPC_S"].Search(FileManager.STBs["LIST_NPC"].Cells[monsterEntry.ID][41]), monsterEntry.Count);

                            new Thread(new ThreadStart(delegate
                            {
                                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                                {
                                    BitmapImage imageSource = CreateImage(monsterEntry.ID);

                                    ((ListBoxItem)TacticalList.Items[monsterID]).ToolTip = new Image()
                                    {
                                        Source = imageSource
                                    };

                                    return null;

                                }, null);
                            })).Start();
                        }
                        break;
                }

                MapManager.Monsters.Add(selectedObject, MapManager.Monsters.WorldObjects[selectedObject].Entry, true);
            }

            UndoManager.AddCommand(new Engine.Commands.Monsters.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.Monsters.WorldObjects[selectedObject],
                OldValue = oldValue,
                NewValue = newValue
            });
        }

        /// <summary>
        /// Monsters the tool_ object ID changed.
        /// </summary>
        /// <param name="value">The value.</param>
        private void MonsterTool_ObjectIDChanged(int value)
        {
            MonsterType monsterType = (EditBasic.IsChecked == true) ? MonsterType.Basic : MonsterType.Tactical;

            Engine.Commands.Monsters.ValueChanged.ObjectValue oldValue = new Engine.Commands.Monsters.ValueChanged.ObjectValue()
            {
                Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Description,
                EventID = MapManager.Monsters.WorldObjects[selectedObject].Entry.EventID,
                Position = MapManager.Monsters.WorldObjects[selectedObject].Entry.Position,
                Name = MapManager.Monsters.WorldObjects[selectedObject].Entry.Name,
                Interval = MapManager.Monsters.WorldObjects[selectedObject].Entry.Interval,
                Limit = MapManager.Monsters.WorldObjects[selectedObject].Entry.Limit,
                Range = MapManager.Monsters.WorldObjects[selectedObject].Entry.Range,
                TacticPoints = MapManager.Monsters.WorldObjects[selectedObject].Entry.TacticPoints,
                Basic = new List<IFO.MonsterSpawn.Monster>(),
                Tactic = new List<IFO.MonsterSpawn.Monster>()
            };

            Engine.Commands.Monsters.ValueChanged.ObjectValue newValue = new Engine.Commands.Monsters.ValueChanged.ObjectValue()
            {
                Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Description,
                EventID = MapManager.Monsters.WorldObjects[selectedObject].Entry.EventID,
                Position = MapManager.Monsters.WorldObjects[selectedObject].Entry.Position,
                Name = MapManager.Monsters.WorldObjects[selectedObject].Entry.Name,
                Interval = MapManager.Monsters.WorldObjects[selectedObject].Entry.Interval,
                Limit = MapManager.Monsters.WorldObjects[selectedObject].Entry.Limit,
                Range = MapManager.Monsters.WorldObjects[selectedObject].Entry.Range,
                TacticPoints = MapManager.Monsters.WorldObjects[selectedObject].Entry.TacticPoints,
                Basic = new List<IFO.MonsterSpawn.Monster>(),
                Tactic = new List<IFO.MonsterSpawn.Monster>()
            };

            for (int i = 0; i < MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic.Count; i++)
            {
                oldValue.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Description
                });

                newValue.Basic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[i].Description
                });
            }

            for (int i = 0; i < MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic.Count; i++)
            {
                oldValue.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Description
                });

                newValue.Tactic.Add(new IFO.MonsterSpawn.Monster()
                {
                    ID = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].ID,
                    Count = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Count,
                    Description = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[i].Description
                });
            }

            switch (monsterType)
            {
                case MonsterType.Basic:
                    {
                        int monsterID = BasicList.SelectedIndex;

                        IFO.MonsterSpawn.Monster monsterEntry = MapManager.Monsters.WorldObjects[selectedObject].Entry.Basic[monsterID];

                        newValue.Basic[monsterID].ID = monsterEntry.ID = value;
                        MapManager.Monsters.Add(selectedObject, MapManager.Monsters.WorldObjects[selectedObject].Entry, true);

                        Properties.Refresh();

                        ((ListBoxItem)BasicList.Items[monsterID]).Content = string.Format("[{0}] {1}; Count: {2}", monsterID, FileManager.STLs["LIST_NPC_S"].Search(FileManager.STBs["LIST_NPC"].Cells[monsterEntry.ID][41]), monsterEntry.Count);

                        new Thread(new ThreadStart(delegate
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                            {
                                BitmapImage imageSource = CreateImage(monsterEntry.ID);

                                ((ListBoxItem)BasicList.Items[monsterID]).ToolTip = new Image()
                                {
                                    Source = imageSource
                                };

                                return null;

                            }, null);
                        })).Start();
                    }
                    break;
                case MonsterType.Tactical:
                    {
                        int monsterID = TacticalList.SelectedIndex;

                        IFO.MonsterSpawn.Monster monsterEntry = MapManager.Monsters.WorldObjects[selectedObject].Entry.Tactic[monsterID];

                        newValue.Tactic[monsterID].ID = monsterEntry.ID = value;
                        MapManager.Monsters.Add(selectedObject, MapManager.Monsters.WorldObjects[selectedObject].Entry, true);

                        Properties.Refresh();

                        ((ListBoxItem)TacticalList.Items[monsterID]).Content = string.Format("[{0}] {1}; Count: {2}", monsterID, FileManager.STLs["LIST_NPC_S"].Search(FileManager.STBs["LIST_NPC"].Cells[monsterEntry.ID][41]), monsterEntry.Count);

                        new Thread(new ThreadStart(delegate
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate
                            {
                                BitmapImage imageSource = CreateImage(monsterEntry.ID);

                                ((ListBoxItem)TacticalList.Items[monsterID]).ToolTip = new Image()
                                {
                                    Source = imageSource
                                };

                                return null;

                            }, null);
                        })).Start();
                    }
                    break;
            }

            UndoManager.AddCommand(new Engine.Commands.Monsters.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.Monsters.WorldObjects[selectedObject],
                OldValue = oldValue,
                NewValue = newValue
            });
        }
    }
}