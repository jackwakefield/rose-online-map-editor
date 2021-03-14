using System;
using System.Windows;
using System.Windows.Controls;
using Map_Editor.Engine;
using Map_Editor.Engine.Map;
using Microsoft.Xna.Framework;

namespace Map_Editor.Forms.Controls
{
    /// <summary>
    /// Interaction logic for WaterTool.xaml
    /// </summary>
    public partial class WaterTool : UserControl
    {
        /// <summary>
        /// Gets the Z.
        /// </summary>
        /// <value>The Z.</value>
        public System.Windows.Forms.TextBox Z
        {
            get { return (System.Windows.Forms.TextBox)ZHost.Child; }
        }

        #region Member Declarations

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        private int selectedObject { get; set; }

        /// <summary>
        /// Gets or sets the value changed.
        /// </summary>
        /// <value>The value changed.</value>
        private Engine.Commands.Interfaces.ICommand valueChanged { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WaterTool"/> class.
        /// </summary>
        public WaterTool()
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

            IFO.WaterBlock waterBlock = MapManager.Water.WorldObjects[selectedObject].Entry;

            string[] position = (MapManager.Water.WorldObjects[selectedObject].Entry.Parent.FileName).Remove(5, 4).Split('_');
            Vector2 block = new Vector2(Convert.ToInt32(position[1]), Convert.ToInt32(position[0]));

            Vector2 blockPosition = new Vector2(block.Y * 160.0f, 10400.0f - ((block.X + 1) * 160.0f));

            X1.Value = MathHelper.Clamp(waterBlock.Minimum.X - blockPosition.X, 0.0f, 160.0f);
            Y1.Value = MathHelper.Clamp(waterBlock.Minimum.Y - blockPosition.Y, 0.0f, 160.0f);

            X2.Value = MathHelper.Clamp(waterBlock.Maximum.X - blockPosition.X, 0.0f, 160.0f);
            Y2.Value = MathHelper.Clamp(waterBlock.Maximum.Y - blockPosition.Y, 0.0f, 160.0f);

            Z.Text = waterBlock.Minimum.Z.ToString();

            Remove.IsEnabled = true;

            return index;
        }

        /// <summary>
        /// Cleans this instance.
        /// </summary>
        public void Clean()
        {
            Remove.IsEnabled = false;

            X1.Value = 0.0f;
            Y1.Value = 0.0f;

            X2.Value = 0.0f;
            Y2.Value = 0.0f;

            Z.Text = "0";
        }

        /// <summary>
        /// Handles the ValueChanged event of the size controls.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs&lt;System.Double&gt;"/> instance containing the event data.</param>
        private void Size_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (!Remove.IsEnabled)
                return;

            string[] position = (MapManager.Water.WorldObjects[selectedObject].Entry.Parent.FileName).Remove(5, 4).Split('_');
            Vector2 block = new Vector2(Convert.ToInt32(position[1]), Convert.ToInt32(position[0]));

            Vector2 blockPosition = new Vector2(block.Y * 160.0f, 10400.0f - ((block.X + 1) * 160.0f));

            Vector3 minimum = new Vector3()
            {
                X = blockPosition.X + (float)X1.Value,
                Y = blockPosition.Y + (float)Y1.Value,
                Z = Convert.ToSingle(Z.Text)
            };

            Vector3 maximum = new Vector3()
            {
                X = blockPosition.X + (float)X2.Value,
                Y = blockPosition.Y + (float)Y2.Value,
                Z = Convert.ToSingle(Z.Text)
            };

            if (valueChanged == null && sender != null)
            {
                valueChanged = new Engine.Commands.Water.ValueChanged()
                {
                    ObjectID = selectedObject,
                    Object = MapManager.Water.WorldObjects[selectedObject],
                    OldValue = new Engine.Commands.Water.ValueChanged.ObjectValue()
                    {
                        Minimum = MapManager.Water.WorldObjects[selectedObject].Entry.Minimum,
                        Maximum = MapManager.Water.WorldObjects[selectedObject].Entry.Maximum
                    }
                };
            }

            MapManager.Water.WorldObjects[selectedObject].Entry.Minimum = minimum;
            MapManager.Water.WorldObjects[selectedObject].Entry.Maximum = maximum;

            MapManager.Water.Add(selectedObject, MapManager.Water.WorldObjects[selectedObject].Entry, true);
        }

        /// <summary>
        /// Handles the LostFocus event of the Z control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Z_LostFocus(object sender, EventArgs e)
        {
            if (!Remove.IsEnabled)
                return;

            float zValue;

            if (float.TryParse(Z.Text, out zValue))
            {
                ZChanged();

                Size_ValueChanged(null, null);
            }
            else
            {
                Z.Text = MapManager.Water.WorldObjects[selectedObject].Entry.Maximum.Z.ToString();

                MessageBox.Show("Invalid float value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the KeyUp event of the Z control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void Z_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!Remove.IsEnabled)
                e.Handled = true;

            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                Z_LostFocus(null, null);
        }

        /// <summary>
        /// Handles the Checked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Checked(object sender, RoutedEventArgs e)
        {
            MapManager.Water.Tool.StartAdding();
        }

        /// <summary>
        /// Handles the Unchecked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.Water.Tool.StopAdding();
        }

        /// <summary>
        /// Handles the Click event of the Remove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            MapManager.Water.Tool.Remove(false);
        }

        /// <summary>
        /// Handles the TextChanged event of the Z control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Z_TextChanged(object sender, EventArgs e)
        {
            if (!Remove.IsEnabled)
                return;

            float zValue;

            if (float.TryParse(Z.Text, out zValue))
            {
                ZChanged();

                Size_ValueChanged(null, null);
            }
        }

        /// <summary>
        /// Z changed event.
        /// </summary>
        private void ZChanged()
        {
            string[] position = (MapManager.Water.WorldObjects[selectedObject].Entry.Parent.FileName).Remove(5, 4).Split('_');
            Vector2 block = new Vector2(Convert.ToInt32(position[1]), Convert.ToInt32(position[0]));

            Vector2 blockPosition = new Vector2(block.Y * 160.0f, 10400.0f - ((block.X + 1) * 160.0f));

            Vector3 minimum = new Vector3()
            {
                X = blockPosition.X + (float)X1.Value,
                Y = blockPosition.Y + (float)Y1.Value,
                Z = Convert.ToSingle(Z.Text)
            };

            Vector3 maximum = new Vector3()
            {
                X = blockPosition.X + (float)X2.Value,
                Y = blockPosition.Y + (float)Y2.Value,
                Z = Convert.ToSingle(Z.Text)
            };

            if (minimum == MapManager.Water.WorldObjects[selectedObject].Entry.Minimum && maximum == MapManager.Water.WorldObjects[selectedObject].Entry.Maximum)
                return;

            UndoManager.AddCommand(new Engine.Commands.Water.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.Water.WorldObjects[selectedObject],
                OldValue = new Engine.Commands.Water.ValueChanged.ObjectValue()
                {
                    Minimum = MapManager.Water.WorldObjects[selectedObject].Entry.Minimum,
                    Maximum = MapManager.Water.WorldObjects[selectedObject].Entry.Maximum
                },
                NewValue = new Engine.Commands.Water.ValueChanged.ObjectValue()
                {
                    Minimum = minimum,
                    Maximum = maximum
                }
            });
        }

        /// <summary>
        /// Handles the PreviewMouseUp event of the Size control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Size_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (valueChanged == null)
                return;

            string[] position = (MapManager.Water.WorldObjects[selectedObject].Entry.Parent.FileName).Remove(5, 4).Split('_');
            Vector2 block = new Vector2(Convert.ToInt32(position[1]), Convert.ToInt32(position[0]));

            Vector2 blockPosition = new Vector2(block.Y * 160.0f, 10400.0f - ((block.X + 1) * 160.0f));

            Vector3 minimum = new Vector3()
            {
                X = blockPosition.X + (float)X1.Value,
                Y = blockPosition.Y + (float)Y1.Value,
                Z = Convert.ToSingle(Z.Text)
            };

            Vector3 maximum = new Vector3()
            {
                X = blockPosition.X + (float)X2.Value,
                Y = blockPosition.Y + (float)Y2.Value,
                Z = Convert.ToSingle(Z.Text)
            };

            ((Engine.Commands.Water.ValueChanged)valueChanged).NewValue = new Engine.Commands.Water.ValueChanged.ObjectValue()
            {
                Minimum = minimum,
                Maximum = maximum
            };

            UndoManager.AddCommand(valueChanged);

            valueChanged = null;
        }
    }
}