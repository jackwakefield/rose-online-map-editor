using System.Windows;
using System.Windows.Controls;
using Map_Editor.Engine;
using Map_Editor.Engine.Map;
using Map_Editor.Misc;
using Map_Editor.Misc.Properties;

namespace Map_Editor.Forms.Controls
{
    /// <summary>
    /// Interaction logic for SpawnPointTool.xaml
    /// </summary>
    public partial class SpawnPointTool : UserControl
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
        public static ZON.SpawnPoint CopiedObject { get; set; }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        private int selectedObject { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnPointTool"/> class.
        /// </summary>
        public SpawnPointTool()
        {
            InitializeComponent();

            DrawIndicators.IsChecked = ConfigurationManager.GetValue<bool>("SpawnPoints", "DrawIndicators");
        }

        /// <summary>
        /// Handles the Click event of the DrawIndicators control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DrawIndicators_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.SetValue("SpawnPoints", "DrawIndicators", DrawIndicators.IsChecked);
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

            ZON.SpawnPoint spawnPoint = MapManager.SpawnPoints.WorldObjects[selectedObject].Entry;

            Properties.SelectedObject = new SpawnPointProperty()
            {
                Name = spawnPoint.Name,
                Position = spawnPoint.Position
            };

            Remove.IsEnabled = true;

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

            App.Form.Cut.IsEnabled = false;
            App.Form.QuickCut.IsEnabled = false;

            App.Form.Copy.IsEnabled = false;
            App.Form.QuickCopy.IsEnabled = false;

            App.Form.Paste.IsEnabled = false;
            App.Form.QuickPaste.IsEnabled = false;

            Properties.SelectedObject = null;
        }

        #region Button Events

        /// <summary>
        /// Handles the Checked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Checked(object sender, RoutedEventArgs e)
        {
            MapManager.SpawnPoints.Tool.StartAdding();
        }

        /// <summary>
        /// Handles the Unchecked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.SpawnPoints.Tool.StopAdding();
        }

        /// <summary>
        /// Handles the Click event of the Remove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            MapManager.SpawnPoints.Tool.Remove(false);
        }

        #endregion

        /// <summary>
        /// Handles the PropertyValueChanged event of the PropertyGrid control.
        /// </summary>
        /// <param name="s">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PropertyValueChangedEventArgs"/> instance containing the event data.</param>
        private void PropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            SpawnPointProperty objectValues = (SpawnPointProperty)Properties.SelectedObject;

            ZON.SpawnPoint spawnPoint = MapManager.SpawnPoints.WorldObjects[selectedObject].Entry;

            UndoManager.AddCommand(new Engine.Commands.SpawnPoints.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.SpawnPoints.WorldObjects[selectedObject],
                OldValue = new Engine.Commands.SpawnPoints.ValueChanged.ObjectValue()
                {
                    Name = spawnPoint.Name,
                    Position = spawnPoint.Position
                },
                NewValue = new Engine.Commands.SpawnPoints.ValueChanged.ObjectValue()
                {
                    Name = objectValues.Name,
                    Position = objectValues.Position
                }
            });

            spawnPoint.Name = objectValues.Name;
            spawnPoint.Position = objectValues.Position;

            MapManager.SpawnPoints.Tool.ChangeWorld(objectValues.Position);
        }
    }
}