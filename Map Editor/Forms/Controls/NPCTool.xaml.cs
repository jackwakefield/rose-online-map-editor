using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Map_Editor.Engine;
using Map_Editor.Engine.Map;
using Map_Editor.Misc;
using Map_Editor.Misc.Properties;
using Microsoft.Xna.Framework;

namespace Map_Editor.Forms.Controls
{
    /// <summary>
    /// Interaction logic for NPCTool.xaml
    /// </summary>
    public partial class NPCTool : UserControl
    {
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
        public static IFO.NPC CopiedObject { get; set; }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public int Model { get; set; }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        private int selectedObject { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NPCTool"/> class.
        /// </summary>
        public NPCTool()
        {
            InitializeComponent();

            DrawBoundingBoxes.IsChecked = ConfigurationManager.GetValue<bool>("NPCs", "DrawBoundingBoxes");
        }

        /// <summary>
        /// Event for when the DrawBoundingBoxes checkbox has been clicked
        /// Changes the configuration and saves the newer version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawBoundingBoxes_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.SetValue("NPCs", "DrawBoundingBoxes", DrawBoundingBoxes.IsChecked);
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

            IFO.NPC ifoEntry = MapManager.NPCs.WorldObjects[index].Entry;

            Model = ifoEntry.ObjectID;

            Vector3 eularVector = ifoEntry.Rotation.ToEular();
            RotationYaw.Value = eularVector.X;
            RotationRoll.Value = eularVector.Y;
            RotationPitch.Value = eularVector.Z;

            Properties.SelectedObject = new NPCProperty()
            {
                CharacterID = ifoEntry.ObjectID,
                Description = ifoEntry.Description,
                EventID = ifoEntry.EventID,
                AIPatternID = ifoEntry.AIPatternIndex,
                Dialog = ifoEntry.Path,
                Position = ifoEntry.Position,
                Rotation = ifoEntry.Rotation,
            };

            ((NPCProperty)Properties.SelectedObject).ObjectIDChanged += new NPCProperty.ObjectID(delegate(int value)
            {
                MapManager.NPCs.Tool.ChangeModel(((NPCProperty)Properties.SelectedObject).CharacterID);

                Properties.Refresh();
            });

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

            App.Form.Cut.IsEnabled = false;
            App.Form.QuickCut.IsEnabled = false;

            App.Form.Copy.IsEnabled = false;
            App.Form.QuickCopy.IsEnabled = false;

            App.Form.Paste.IsEnabled = false;
            App.Form.QuickPaste.IsEnabled = false;

            Properties.SelectedObject = null;

            RotationYaw.Value = 0.0f;
            RotationPitch.Value = 0.0f;
            RotationRoll.Value = 0.0f;

            selectedObject = -1;
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

            App.Form.PreviewPanel.Reset();

            App.Form.PreviewPanel.OnSelect += new EventHandler(delegate
            {
                Model = App.Form.PreviewPanel.NPCList.SelectedIndex + 1;

                App.Form.PreviewPanel.Hide();

                MapManager.NPCs.Tool.StartAdding(Model);
            });

            App.Form.PreviewPanel.OnCancel += new EventHandler(delegate
            {
                Add.IsChecked = false;
            });

            App.Form.PreviewPanel.Show();
        }

        /// <summary>
        /// Handles the Unchecked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.NPCs.Tool.StopAdding();
        }

        /// <summary>
        /// Handles the Click event of the Remove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            MapManager.NPCs.Tool.Remove(false);
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

            MapManager.NPCs.Tool.StartAdding(Model);
        }

        /// <summary>
        /// Handles the Unchecked event of the Clone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Clone_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.NPCs.Tool.StopAdding();
        }

        #endregion

        /// <summary>
        /// Handles the PropertyValueChanged event of the Properties control.
        /// </summary>
        /// <param name="s">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PropertyValueChangedEventArgs"/> instance containing the event data.</param>
        private void Properties_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            NPCProperty objectValues = (NPCProperty)Properties.SelectedObject;

            IFO.NPC ifoEntry = MapManager.NPCs.WorldObjects[selectedObject].Entry;

            UndoManager.AddCommand(new Engine.Commands.NPCs.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.NPCs.WorldObjects[selectedObject],
                OldValue = new Engine.Commands.NPCs.ValueChanged.ObjectValue()
                {
                    Description = ifoEntry.Description,
                    EventID = ifoEntry.EventID,
                    Position = ifoEntry.Position,
                    Rotation = ifoEntry.Rotation,
                    AIPatternIndex = ifoEntry.AIPatternIndex,
                    Path = ifoEntry.Path
                },
                NewValue = new Engine.Commands.NPCs.ValueChanged.ObjectValue()
                {
                    Description = objectValues.Description,
                    EventID = objectValues.EventID,
                    Position = objectValues.Position,
                    Rotation = objectValues.Rotation,
                    AIPatternIndex = objectValues.AIPatternID,
                    Path = objectValues.Dialog
                }
            });

            ifoEntry.Description = objectValues.Description;
            ifoEntry.EventID = objectValues.EventID;
            ifoEntry.AIPatternIndex = objectValues.AIPatternID;
            ifoEntry.Path = objectValues.Dialog;
            ifoEntry.ObjectID = objectValues.CharacterID;

            MapManager.NPCs.Tool.ChangeWorld(objectValues.Position, objectValues.Rotation);
        }

        /// <summary>
        /// Handles the ValueChanged event of the Rotation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs&lt;System.Double&gt;"/> instance containing the event data.</param>
        private void Rotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!Remove.IsEnabled)
                return;

            IFO.NPC ifoEntry = MapManager.NPCs.WorldObjects[selectedObject].Entry;

            MapManager.NPCs.Tool.ChangeWorld(ifoEntry.Position, Quaternion.CreateFromYawPitchRoll((float)RotationRoll.Value, (float)RotationYaw.Value, (float)RotationPitch.Value));
        }

        /// <summary>
        /// Handles the PreviewMouseUp event of the RotationRoll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Rotation_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Properties.SelectedObject == null)
                return;

            NPCProperty objectValues = (NPCProperty)Properties.SelectedObject;

            IFO.NPC ifoEntry = MapManager.NPCs.WorldObjects[selectedObject].Entry;

            UndoManager.AddCommand(new Engine.Commands.NPCs.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.NPCs.WorldObjects[selectedObject],
                OldValue = new Engine.Commands.NPCs.ValueChanged.ObjectValue()
                {
                    Description = objectValues.Description,
                    EventID = objectValues.EventID,
                    Position = objectValues.Position,
                    Rotation = objectValues.Rotation,
                    AIPatternIndex = objectValues.AIPatternID,
                    Path = objectValues.Dialog
                },
                NewValue = new Engine.Commands.NPCs.ValueChanged.ObjectValue()
                {
                    Description = ifoEntry.Description,
                    EventID = ifoEntry.EventID,
                    Position = ifoEntry.Position,
                    Rotation = ifoEntry.Rotation,
                    AIPatternIndex = ifoEntry.AIPatternIndex,
                    Path = ifoEntry.Path
                }
            });

            if (((NPCProperty)Properties.SelectedObject).Rotation != ifoEntry.Rotation)
            {
                ((NPCProperty)Properties.SelectedObject).Rotation = ifoEntry.Rotation;
                Properties.Refresh();
            }
        }
    }
}