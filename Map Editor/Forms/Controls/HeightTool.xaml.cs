using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Map_Editor.Engine;

namespace Map_Editor.Forms.Controls
{
    /// <summary>
    /// Interaction logic for HeightTool.xaml
    /// </summary>
    public partial class HeightTool : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeightTool"/> class.
        /// </summary>
        public HeightTool()
        {
            InitializeComponent();

            InnerRadius.Value = 3;
            OutterRadius.Value = 7;

            Power.Value = 5.0f;

            Square_Checked(null, null);
        }

        /// <summary>
        /// Handles the ValueChanged event of the InnerRadius control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs&lt;System.Double&gt;"/> instance containing the event data.</param>
        private void InnerRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MapManager.Heightmaps.HeightTool.InnerRadius = (int)InnerRadius.Value;
        }

        /// <summary>
        /// Handles the ValueChanged event of the OutterRadius control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs&lt;System.Double&gt;"/> instance containing the event data.</param>
        private void OutterRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MapManager.Heightmaps.HeightTool.OutterRadius = (int)OutterRadius.Value;
        }

        /// <summary>
        /// Handles the ValueChanged event of the Power control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs&lt;System.Double&gt;"/> instance containing the event data.</param>
        private void Power_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MapManager.Heightmaps.HeightTool.Power = (float)Power.Value / 10.0f;
        }

        /// <summary>
        /// Handles the Checked event of the Square control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Square_Checked(object sender, RoutedEventArgs e)
        {
            MapManager.Heightmaps.HeightTool.HeightShape = Engine.Tools.Height.HeightShapeMode.Square;
        }

        /// <summary>
        /// Handles the Checked event of the Circle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Circle_Checked(object sender, RoutedEventArgs e)
        {
            MapManager.Heightmaps.HeightTool.HeightShape = Engine.Tools.Height.HeightShapeMode.Circle;
        }

        /// <summary>
        /// Handles the Click event of the Tool control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Tool_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < HeightOptions.Items.Count; i++)
                ((ToggleButton)HeightOptions.Items[i]).IsChecked = false;

            switch (((ToggleButton)sender).Name)
            {
                case "Raise":
                    MapManager.Heightmaps.HeightTool.HeightTool = Engine.Tools.Height.HeightToolMode.Raise;
                    break;
                case "Lower":
                    MapManager.Heightmaps.HeightTool.HeightTool = Engine.Tools.Height.HeightToolMode.Lower;
                    break;
                case "Flatten":
                    MapManager.Heightmaps.HeightTool.HeightTool = Engine.Tools.Height.HeightToolMode.Flatten;
                    break;
                case "Smooth":
                    MapManager.Heightmaps.HeightTool.HeightTool = Engine.Tools.Height.HeightToolMode.Smooth;
                    break;
            }

            ((ToggleButton)sender).IsChecked = true;
        }
    }
}