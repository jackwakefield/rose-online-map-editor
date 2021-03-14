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
    /// Interaction logic for WarpGateTool.xaml
    /// </summary>
    public partial class WarpGateTool : UserControl
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
        public static IFO.BaseIFO CopiedObject { get; set; }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        private int selectedObject { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WarpGateTool"/> class.
        /// </summary>
        public WarpGateTool()
        {
            InitializeComponent();

            DrawWarpAreas.IsChecked = ConfigurationManager.GetValue<bool>("WarpGates", "DrawWarpAreas");
        }

        /// <summary>
        /// Handles the Click event of the DrawWarpAreas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DrawWarpAreas_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.SetValue("WarpGates", "DrawWarpAreas", DrawWarpAreas.IsChecked);
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

            IFO.BaseIFO warpGate = MapManager.WarpGates.WorldObjects[selectedObject].Entry;

            Vector3 eularVector = warpGate.Rotation.ToEular();
            RotationYaw.Value = eularVector.X;
            RotationRoll.Value = eularVector.Y;
            RotationPitch.Value = eularVector.Z;

            Properties.SelectedObject = new WarpGateProperty()
            {
                Description = warpGate.Description,
                EventID = warpGate.EventID,
                WarpID = warpGate.WarpID,
                Position = warpGate.Position,
                Rotation = warpGate.Rotation,
                Scale = warpGate.Scale
            };

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

            MapManager.WarpGates.Tool.StartAdding();
        }

        /// <summary>
        /// Handles the Unchecked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.WarpGates.Tool.StopAdding();
        }

        /// <summary>
        /// Handles the Click event of the Remove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            MapManager.WarpGates.Tool.Remove(false);
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

            MapManager.WarpGates.Tool.StartAdding();
        }

        /// <summary>
        /// Handles the Unchecked event of the Clone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Clone_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.WarpGates.Tool.StopAdding();
        }

        #endregion

        /// <summary>
        /// Handles the PropertyValueChanged event of the PropertyGrid control.
        /// </summary>
        /// <param name="s">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PropertyValueChangedEventArgs"/> instance containing the event data.</param>
        private void PropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            WarpGateProperty objectValues = (WarpGateProperty)Properties.SelectedObject;

            IFO.BaseIFO ifoEntry = MapManager.WarpGates.WorldObjects[selectedObject].Entry;

            UndoManager.AddCommand(new Engine.Commands.WarpGates.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.WarpGates.WorldObjects[selectedObject],
                OldValue = new Engine.Commands.WarpGates.ValueChanged.ObjectValue()
                {
                    Description = ifoEntry.Description,
                    EventID = ifoEntry.EventID,
                    WarpID = ifoEntry.WarpID,
                    Position = ifoEntry.Position,
                    Rotation = ifoEntry.Rotation,
                    Scale = ifoEntry.Scale
                },
                NewValue = new Engine.Commands.WarpGates.ValueChanged.ObjectValue()
                {
                    Description = objectValues.Description,
                    EventID = objectValues.EventID,
                    WarpID = objectValues.WarpID,
                    Position = objectValues.Position,
                    Rotation = objectValues.Rotation,
                    Scale = objectValues.Scale
                }
            });

            ifoEntry.Description = objectValues.Description;
            ifoEntry.EventID = objectValues.EventID;
            ifoEntry.WarpID = objectValues.WarpID;
            ifoEntry.Position = objectValues.Position;
            ifoEntry.Rotation = objectValues.Rotation;
            ifoEntry.Scale = objectValues.Scale;

            MapManager.WarpGates.Tool.ChangeWorld(objectValues.Position, objectValues.Rotation, objectValues.Scale);
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

            IFO.BaseIFO ifoEntry = MapManager.WarpGates.WorldObjects[selectedObject].Entry;

            MapManager.WarpGates.Tool.ChangeWorld(ifoEntry.Position, Quaternion.CreateFromYawPitchRoll((float)RotationRoll.Value, (float)RotationYaw.Value, (float)RotationPitch.Value), ifoEntry.Scale);
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

            WarpGateProperty objectValues = (WarpGateProperty)Properties.SelectedObject;

            IFO.BaseIFO ifoEntry = MapManager.WarpGates.WorldObjects[selectedObject].Entry;

            UndoManager.AddCommand(new Engine.Commands.WarpGates.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.WarpGates.WorldObjects[selectedObject],
                OldValue = new Engine.Commands.WarpGates.ValueChanged.ObjectValue()
                {
                    Description = objectValues.Description,
                    EventID = objectValues.EventID,
                    WarpID = objectValues.WarpID,
                    Position = objectValues.Position,
                    Rotation = objectValues.Rotation,
                    Scale = objectValues.Scale
                },
                NewValue = new Engine.Commands.WarpGates.ValueChanged.ObjectValue()
                {
                    Description = ifoEntry.Description,
                    EventID = ifoEntry.EventID,
                    WarpID = objectValues.WarpID,
                    Position = ifoEntry.Position,
                    Rotation = ifoEntry.Rotation,
                    Scale = ifoEntry.Scale
                }
            });

            if (((WarpGateProperty)Properties.SelectedObject).Rotation != ifoEntry.Rotation)
            {
                ((WarpGateProperty)Properties.SelectedObject).Rotation = ifoEntry.Rotation;
                Properties.Refresh();
            }
        }
    }
}