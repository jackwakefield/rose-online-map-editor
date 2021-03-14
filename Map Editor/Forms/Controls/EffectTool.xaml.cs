using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Map_Editor.Engine;
using Map_Editor.Engine.Map;
using Map_Editor.Misc.Properties;
using Microsoft.Xna.Framework;

namespace Map_Editor.Forms.Controls
{
    /// <summary>
    /// Interaction logic for EffectTool.xaml
    /// </summary>
    public partial class EffectTool : UserControl
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
        public static IFO.Effect CopiedObject { get; set; }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        private int selectedObject { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectTool"/> class.
        /// </summary>
        public EffectTool()
        {
            InitializeComponent();
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

            IFO.Effect effect = MapManager.Effects.WorldObjects[selectedObject].Entry;

            Vector3 eularVector = effect.Rotation.ToEular();
            RotationYaw.Value = eularVector.X;
            RotationRoll.Value = eularVector.Y;
            RotationPitch.Value = eularVector.Z;

            Properties.SelectedObject = new EffectProperty()
            {
                Description = effect.Description,
                EventID = effect.EventID,
                Position = effect.Position,
                Rotation = effect.Rotation,
                Scale = effect.Scale,
                Path = effect.Path,
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

            MapManager.Effects.Tool.StartAdding();
        }

        /// <summary>
        /// Handles the Unchecked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.Effects.Tool.StopAdding();
        }

        /// <summary>
        /// Handles the Click event of the Remove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            MapManager.Effects.Tool.Remove(false);
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

            MapManager.Effects.Tool.StartAdding();
        }

        /// <summary>
        /// Handles the Unchecked event of the Clone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Clone_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.Effects.Tool.StopAdding();
        }

        #endregion

        /// <summary>
        /// Handles the PropertyValueChanged event of the PropertyGrid control.
        /// </summary>
        /// <param name="s">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PropertyValueChangedEventArgs"/> instance containing the event data.</param>
        private void PropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            EffectProperty objectValues = (EffectProperty)Properties.SelectedObject;

            IFO.Effect ifoEntry = MapManager.Effects.WorldObjects[selectedObject].Entry;

            UndoManager.AddCommand(new Engine.Commands.Effects.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.Effects.WorldObjects[selectedObject],
                OldValue = new Engine.Commands.Effects.ValueChanged.ObjectValue()
                {
                    Description = ifoEntry.Description,
                    EventID = ifoEntry.EventID,
                    Position = ifoEntry.Position,
                    Rotation = ifoEntry.Rotation,
                    Scale = ifoEntry.Scale,
                    Path = ifoEntry.Path
                },
                NewValue = new Engine.Commands.Effects.ValueChanged.ObjectValue()
                {
                    Description = objectValues.Description,
                    EventID = objectValues.EventID,
                    Position = objectValues.Position,
                    Rotation = objectValues.Rotation,
                    Scale = objectValues.Scale,
                    Path = objectValues.Path
                }
            });

            ifoEntry.Description = objectValues.Description;
            ifoEntry.EventID = objectValues.EventID;
            ifoEntry.Position = objectValues.Position;
            ifoEntry.Scale = objectValues.Scale;
            ifoEntry.Rotation = objectValues.Rotation;
            ifoEntry.Path = objectValues.Path;

            MapManager.Effects.Tool.ChangeWorld(objectValues.Position);
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

            IFO.Effect ifoEntry = MapManager.Effects.WorldObjects[selectedObject].Entry;
            ifoEntry.Rotation = Quaternion.CreateFromYawPitchRoll((float)RotationRoll.Value, (float)RotationYaw.Value, (float)RotationPitch.Value);
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

            EffectProperty objectValues = (EffectProperty)Properties.SelectedObject;

            IFO.Effect ifoEntry = MapManager.Effects.WorldObjects[selectedObject].Entry;

            UndoManager.AddCommand(new Engine.Commands.Effects.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.Effects.WorldObjects[selectedObject],
                OldValue = new Engine.Commands.Effects.ValueChanged.ObjectValue()
                {
                    Description = objectValues.Description,
                    EventID = objectValues.EventID,
                    Position = objectValues.Position,
                    Rotation = objectValues.Rotation,
                    Scale = objectValues.Scale,
                    Path = objectValues.Path
                },
                NewValue = new Engine.Commands.Effects.ValueChanged.ObjectValue()
                {
                    Description = ifoEntry.Description,
                    EventID = ifoEntry.EventID,
                    Position = ifoEntry.Position,
                    Rotation = ifoEntry.Rotation,
                    Scale = ifoEntry.Scale,
                    Path = ifoEntry.Path
                }
            });

            if (((EffectProperty)Properties.SelectedObject).Rotation != ifoEntry.Rotation)
            {
                ((EffectProperty)Properties.SelectedObject).Rotation = ifoEntry.Rotation;
                Properties.Refresh();
            }
        }
    }
}