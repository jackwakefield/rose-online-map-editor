using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Map_Editor.Engine;
using Map_Editor.Engine.Characters;
using Map_Editor.Engine.Models;
using Map_Editor.Engine.RenderManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Forms.Controls.NPC
{
    /// <summary>
    /// Preview Panel class.
    /// </summary>
    public partial class PreviewPanel : UserControl
    {
        /// <summary>
        /// Gets the render panel.
        /// </summary>
        /// <value>The render panel.</value>
        public System.Windows.Forms.Panel RenderPanel
        {
            get { return (System.Windows.Forms.Panel)RenderPanelHost.Child; }
        }

        #region Member Declarations

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDevice device { get; set; }

        /// <summary>
        /// Gets or sets the object manager.
        /// </summary>
        /// <value>The object manager.</value>
        private ObjectManager objectManager { get; set; }

        /// <summary>
        /// Gets or sets the texture manager.
        /// </summary>
        /// <value>The texture manager.</value>
        private TextureManager textureManager { get; set; }

        /// <summary>
        /// Gets or sets the selected NPC.
        /// </summary>
        /// <value>The selected NPC.</value>
        private NPCs.WorldObject selectedNPC { get; set; }

        #region Camera

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public Matrix View { get; set; }

        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        /// <value>The projection.</value>
        public Matrix Projection { get; set; }

        /// <summary>
        /// Gets or sets the angle.
        /// </summary>
        /// <value>The angle.</value>
        private float angle { get; set; }

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [on select].
        /// </summary>
        public event EventHandler OnSelect;

        /// <summary>
        /// Occurs when [on cancel].
        /// </summary>
        public event EventHandler OnCancel;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewPanel"/> class.
        /// </summary>
        public PreviewPanel()
        {
            InitializeComponent();

            angle = 0.0f;
        }

        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LeftColumn.Width = new GridLength(272);

            PopulateList();
        }

        /// <summary>
        /// Populates the list.
        /// </summary>
        public void PopulateList()
        {
            if (NPCList.Items.Count > 0)
                NPCList.Items.Clear();

            for (int i = 1; i < FileManager.STBs["LIST_NPC"].Cells.Count; i++)
                NPCList.Items.Add(string.Format("[{0}] {1}", i, FileManager.STLs["LIST_NPC_S"].Search(FileManager.STBs["LIST_NPC"].Cells[i][41])));
        }

        /// <summary>
        /// Handles the Click event of the Find control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            Search findNPC = new Search(Forms.Search.FindType.Find | Forms.Search.FindType.GoTo);

            findNPC.FindClicked += new Search.Find(delegate(string value)
            {
                List<string> searchResults = new List<string>();

                value = value.ToLower();

                for (int i = 0; i < FileManager.STBs["LIST_NPC"].Cells.Count; i++)
                {
                    string npcName = FileManager.STLs["LIST_NPC_S"].Search(FileManager.STBs["LIST_NPC"].Cells[i][41]);

                    if (npcName.ToLower().Contains(value))
                        searchResults.Add(string.Format("[{0}] {1}", i, npcName));
                }

                return searchResults.ToArray();
            });

            findNPC.SelectionClicked += new Search.Selection(delegate(string value)
            {
                if (MessageBox.Show("Do you wish to select the NPC?", "Select NPC", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;

                int endOfID;

                for (endOfID = 1; endOfID < value.Length; endOfID++)
                {
                    if (value[endOfID] == ']')
                        break;
                }

                int npcID = Convert.ToInt32(value.Substring(1, endOfID - 1));

                NPCList.SelectedIndex = npcID - 1;
                NPCList.ScrollIntoView(NPCList.Items[npcID - 1]);

                findNPC.Close();
            });

            findNPC.GoToClicked += new Search.GoTo(delegate(int value)
            {
                if (value > 0 && value < FileManager.STBs["LIST_NPC"].Cells.Count)
                {
                    if (MessageBox.Show("Do you wish to select the NPC found?", "Select NPC", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        NPCList.SelectedIndex = value - 1;
                        NPCList.ScrollIntoView(NPCList.Items[value - 1]);

                        findNPC.Close();
                    }

                    return true;
                }

                return false;
            });

            findNPC.ShowDialog();
        }

        /// <summary>
        /// Draws the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        public void Draw(GraphicsDevice device)
        {
            if (this.device == null)
            {
                objectManager = new ObjectManager(device, ZMS.Vertex.VertexElements, ZMS.Vertex.SIZE_IN_BYTES);
                textureManager = new TextureManager(device);

                this.device = device;

                NPCList.IsEnabled = true;
            }

            if (selectedNPC != null)
            {
                angle += 0.05f;

                objectManager.ClearBatch();

                selectedNPC.World = Matrix.CreateRotationZ(angle);
                selectedNPC.Add(objectManager);
            }

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)RenderPanel.Width / (float)RenderPanel.Height, 1.0f, 5000.0f);

            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(66, 89, 127), 1.0f, 0);

            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.NPC);

            shader.Start("NPC");

            objectManager.Draw(textureManager, shader, false, View, Projection);

            shader.Finish();

            device.Present(new Rectangle(0, 0, RenderPanel.Width, RenderPanel.Height), null, RenderPanel.Handle);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the NPCList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void NPCList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!NPCList.IsEnabled)
                return;

            objectManager.ClearBatch();
            objectManager.Clear();
            textureManager.Clear();

            int id = NPCList.SelectedIndex + 1;

            float npcScale = 0.0f;

            try
            {
                npcScale = Convert.ToInt32(FileManager.STBs["LIST_NPC"].Cells[id][5]);
            }
            catch
            {
                npcScale = 100.0f;
            }
            finally
            {
                npcScale /= 100.0f;
            }

            Matrix objectWorld = Matrix.CreateScale(npcScale);

            List<short> chrModels = FileManager.CHRs["LIST_NPC"].Characters[id].Models;

            selectedNPC = new NPCs.WorldObject()
            {
                Entry = null,
                World = objectWorld,
                Parts = new List<List<NPCs.WorldObject.Part>>(chrModels.Count)
            };

            for (int i = 0; i < chrModels.Count; i++)
            {
                ZSC.Object zscObject = FileManager.ZSCs["PART_NPC"].Objects[chrModels[i]];

                List<NPCs.WorldObject.Part> newObjectList = new List<NPCs.WorldObject.Part>(zscObject.Models.Count);

                for (int j = 0; j < zscObject.Models.Count; j++)
                {
                    int modelID = objectManager.Add(FileManager.ZSCs["PART_NPC"].Models[zscObject.Models[j].ModelID]);

                    if (modelID == -1)
                        return;

                    Matrix world = Matrix.CreateFromQuaternion(zscObject.Models[j].Rotation) *
                                   Matrix.CreateScale(zscObject.Models[j].Scale) *
                                   Matrix.CreateTranslation(zscObject.Models[j].Position);

                    int textureID = textureManager.Add(FileManager.ZSCs["PART_NPC"].Textures[zscObject.Models[j].TextureID]);

                    newObjectList.Add(new NPCs.WorldObject.Part()
                    {
                        TextureID = textureID,
                        ModelID = modelID,
                        Position = zscObject.Models[j].Position,
                        Scale = zscObject.Models[j].Scale,
                        Rotation = zscObject.Models[j].Rotation,
                        AlphaEnabled = textureManager[textureID].RenderState.AlphaEnabled,
                        BoundingBox = objectManager.CreateBox(modelID, world * objectWorld)
                    });

                    if (i == 0 && j == 0)
                        selectedNPC.BoundingBox = newObjectList[j].BoundingBox;
                    else
                        selectedNPC.BoundingBox = BoundingBox.CreateMerged(selectedNPC.BoundingBox, newObjectList[j].BoundingBox);
                }

                selectedNPC.Parts.Add(newObjectList);
            }

            BoundingSphere modelSphere = BoundingSphere.CreateFromBoundingBox(selectedNPC.BoundingBox);

            float distanceToCenter = modelSphere.Radius / (float)Math.Sin(MathHelper.PiOver4 / 2);

            Vector3 cameraPosition = Vector3.Zero;
            cameraPosition += (-Vector3.Forward * distanceToCenter);
            cameraPosition += new Vector3(10.0f, 0.0f, 0.0f);
            cameraPosition.Z = 0.0f;

            View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, new Vector3(0.0f, 0.0f, 1.0f));

            selectedNPC.Add(objectManager);
        }

        /// <summary>
        /// Handles the Click event of the Select control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (OnSelect != null)
                OnSelect(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (OnCancel != null)
                OnCancel(this, EventArgs.Empty);

            Hide();
        }

        /// <summary>
        /// Shows this instance.
        /// </summary>
        public void Show()
        {
            App.Form.RenderPanelHost.Visibility = Visibility.Collapsed;
            App.Form.PreviewPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        public void Hide()
        {
            App.Form.PreviewPanel.Visibility = Visibility.Collapsed;
            App.Form.RenderPanelHost.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            if (objectManager == null)
                return;

            OnSelect = null;
            OnCancel = null;

            objectManager.ClearBatch();
            objectManager.Clear();
            textureManager.Clear();

            PopulateList();

            selectedNPC = null;
        }
    }
}