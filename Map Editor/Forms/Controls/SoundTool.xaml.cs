using System.Windows;
using System.Windows.Controls;
using IrrKlang;
using Map_Editor.Engine;
using Map_Editor.Engine.Map;
using Map_Editor.Misc;
using Map_Editor.Misc.Properties;

namespace Map_Editor.Forms.Controls
{
    /// <summary>
    /// Interaction logic for SoundTool.xaml
    /// </summary>
    public partial class SoundTool : UserControl
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
        public static IFO.Sound CopiedObject { get; set; }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the sound player.
        /// </summary>
        /// <value>The sound player.</value>
        private ISoundEngine soundPlayer { get; set; }

        /// <summary>
        /// Gets or sets the sound.
        /// </summary>
        /// <value>The sound.</value>
        private ISound sound { get; set; }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        private int selectedObject { get; set; }

        /// <summary>
        /// Gets or sets the sound timer.
        /// </summary>
        /// <value>The sound timer.</value>
        public System.Windows.Forms.Timer SoundTimer { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundTool"/> class.
        /// </summary>
        public SoundTool()
        {
            InitializeComponent();

            soundPlayer = new ISoundEngine();

            DrawRadii.IsChecked = ConfigurationManager.GetValue<bool>("Sounds", "DrawRadii");

            SoundTimer = new System.Windows.Forms.Timer()
            {
                Interval = 100,
                Enabled = false
            };

            SoundTimer.Tick += new System.EventHandler(SoundTimer_Tick);
        }

        /// <summary>
        /// Handles the Click event of the DrawRadii control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DrawRadii_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.SetValue("Sounds", "DrawRadii", DrawRadii.IsChecked);
            ConfigurationManager.SaveConfig();
        }

        /// <summary>
        /// Selects the specified objects.
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

            IFO.Sound sound = MapManager.Sounds.WorldObjects[selectedObject].Entry;

            Properties.SelectedObject = new SoundProperty()
            {
                Description = sound.Description,
                EventID = sound.EventID,
                Position = sound.Position,
                Path = sound.Path,
                Range = sound.Range,
                Interval = sound.Interval
            };

            Remove.IsEnabled = true;
            Clone.IsEnabled = true;

            if (this.sound != null)
                Stop();

            App.Form.PlaySound.IsEnabled = true;
            App.Form.PauseSound.IsEnabled = false;
            App.Form.StopSound.IsEnabled = false;

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

            App.Form.PlaySound.IsEnabled = false;
            App.Form.StopSound.IsEnabled = false;
            App.Form.PauseSound.IsEnabled = false;

            App.Form.PauseSound.IsChecked = false;

            Properties.SelectedObject = null;

            selectedObject = -1;

            if (sound != null)
                Stop();
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

            MapManager.Sounds.Tool.StartAdding();
        }

        /// <summary>
        /// Handles the Unchecked event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Add_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.Sounds.Tool.StopAdding();
        }

        /// <summary>
        /// Removes the selected object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            MapManager.Sounds.Tool.Remove(false);
        }

        /// <summary>
        /// Begins cloning the selected object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clone_Checked(object sender, RoutedEventArgs e)
        {
            if (Add.IsChecked == true)
                Add.IsChecked = false;

            MapManager.Sounds.Tool.StartAdding();
        }

        /// <summary>
        /// Stops the cloning the selected object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clone_Unchecked(object sender, RoutedEventArgs e)
        {
            MapManager.Sounds.Tool.StopAdding();
        }

        #endregion

        /// <summary>
        /// Handles the PropertyValueChanged event of the PropertyGrid control.
        /// </summary>
        /// <param name="s">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PropertyValueChangedEventArgs"/> instance containing the event data.</param>
        private void PropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            SoundProperty objectValues = (SoundProperty)Properties.SelectedObject;

            IFO.Sound ifoEntry = MapManager.Sounds.WorldObjects[selectedObject].Entry;

            UndoManager.AddCommand(new Engine.Commands.Sounds.ValueChanged()
            {
                ObjectID = selectedObject,
                Object = MapManager.Sounds.WorldObjects[selectedObject],
                OldValue = new Engine.Commands.Sounds.ValueChanged.ObjectValue()
                {
                    Description = ifoEntry.Description,
                    EventID = ifoEntry.EventID,
                    Position = ifoEntry.Position,
                    Path = ifoEntry.Path,
                    Interval = ifoEntry.Interval,
                    Range = ifoEntry.Range,
                },
                NewValue = new Engine.Commands.Sounds.ValueChanged.ObjectValue()
                {
                    Description = objectValues.Description,
                    EventID = objectValues.EventID,
                    Position = objectValues.Position,
                    Path = objectValues.Path,
                    Interval = objectValues.Interval,
                    Range = objectValues.Range,
                }
            });

            ifoEntry.Description = objectValues.Description;
            ifoEntry.EventID = objectValues.EventID;
            ifoEntry.Position = objectValues.Position;
            ifoEntry.Path = objectValues.Path;
            ifoEntry.Range = objectValues.Range;
            ifoEntry.Interval = objectValues.Interval;

            MapManager.Sounds.Tool.ChangeWorld(objectValues.Position);
        }

        #region Sound Functions

        /// <summary>
        /// Plays the sound.
        /// </summary>
        public void Play()
        {
            if (sound != null)
            {
                if (sound.Paused)
                {
                    Resume();

                    return;
                }

                Stop();
            }

            sound = soundPlayer.Play2D(MapManager.Sounds.WorldObjects[selectedObject].Entry.Path, false);

            App.Form.PlaySound.IsEnabled = false;
            App.Form.PauseSound.IsEnabled = true;
            App.Form.StopSound.IsEnabled = true;

            SoundTimer.Enabled = true;
        }

        /// <summary>
        /// Resumes the sound.
        /// </summary>
        public void Resume()
        {
            App.Form.PlaySound.IsEnabled = false;
            App.Form.PauseSound.IsChecked = false;

            sound.Paused = false;
        }

        /// <summary>
        /// Pauses the sound.
        /// </summary>
        public void Pause()
        {
            App.Form.PlaySound.IsEnabled = true;

            sound.Paused = true;
        }

        /// <summary>
        /// Stops the sound.
        /// </summary>
        public void Stop()
        {
            sound.Stop();

            sound.Paused = false;

            App.Form.PlaySound.IsEnabled = true;
            App.Form.PauseSound.IsChecked = false;
            App.Form.PauseSound.IsEnabled = false;
            App.Form.StopSound.IsEnabled = false;

            SoundTimer.Enabled = false;
        }

        #endregion

        /// <summary>
        /// Handles the Tick event of the SoundTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SoundTimer_Tick(object sender, System.EventArgs e)
        {
            if (sound.Finished)
                Stop();
        }
    }
}