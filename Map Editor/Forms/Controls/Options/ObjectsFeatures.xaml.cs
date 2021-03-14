using System.Windows;
using System.Windows.Controls;
using Map_Editor.Misc;

namespace Map_Editor.Forms.Controls.Options
{
    /// <summary>
    /// Interaction logic for ObjectsFeatures.xaml
    /// </summary>
    public partial class ObjectsFeatures : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectsFeatures"/> class.
        /// </summary>
        public ObjectsFeatures()
        {
            InitializeComponent();

            IsEnabled = false;

            Monsters.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Monsters");
            NPCs.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "NPCs");
            SpawnPoints.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "SpawnPoints");
            WarpGates.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "WarpGates");
            Effects.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Effects");
            Sounds.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Sounds");
            Sky.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Sky");
            Animation.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Animation");
            Collision.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Collision");
            Construction.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Construction");
            Decoration.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Decoration");
            EventTriggers.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "EventTriggers");
            Heightmaps.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Heightmaps");
            Water.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "Water");
            GridNumbers.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "GridNumbers");
            GridOutline.IsChecked = ConfigurationManager.GetValue<bool>("Draw", "GridOutline");

            IsEnabled = true;
        }

        /// <summary>
        /// Handles the CheckChanged event of the GridOutline control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void GridOutline_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!IsEnabled)
                return;

            ConfigurationManager.SetValue("Draw", "Monsters", Monsters.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "NPCs", NPCs.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "SpawnPoints", SpawnPoints.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "WarpGates", WarpGates.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "Effects", Effects.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "Sounds", Sounds.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "Sky", Sky.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "Animation", Animation.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "Collision", Collision.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "Construction", Construction.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "Decoration", Decoration.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "EventTriggers", EventTriggers.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "Heightmaps", Heightmaps.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "Water", Water.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "GridNumbers", GridNumbers.IsChecked ?? false);
            ConfigurationManager.SetValue("Draw", "GridOutline", GridOutline.IsChecked ?? false);

            ConfigurationManager.SaveConfig();
        }
    }
}