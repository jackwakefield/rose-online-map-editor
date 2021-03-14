using System;
using System.Windows;
using System.Windows.Input;

namespace Map_Editor.Forms
{
    /// <summary>
    /// Search class.
    /// </summary>
    public partial class Search : Window
    {
        /// <summary>
        /// Find type.
        /// </summary>
        [Flags]
        public enum FindType
        {
            /// <summary>
            /// Enable the find option.
            /// </summary>
            Find = 1 << 0,

            /// <summary>
            /// Enable the goto option.
            /// </summary>
            GoTo = 1 << 1
        }

        #region Delegates

        /// <summary>
        /// Find delegate.
        /// </summary>
        public delegate string[] Find(string value);

        /// <summary>
        /// Selection delegate.
        /// </summary>
        public delegate void Selection(string value);

        /// <summary>
        /// GoTo delegate.
        /// </summary>
        public delegate bool GoTo(int value);

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [find clicked].
        /// </summary>
        public event Find FindClicked;

        /// <summary>
        /// Occurs when [selection clicked].
        /// </summary>
        public event Selection SelectionClicked;

        /// <summary>
        /// Occurs when [go to clicked].
        /// </summary>
        public event GoTo GoToClicked;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Search"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public Search(FindType type)
        {
            InitializeComponent();

            if ((type & FindType.Find) > 0)
            {
                FindValue.IsEnabled = true;
                FindButton.IsEnabled = true;
                FindResults.IsEnabled = true;

                FindExpander.IsExpanded = true;
            }

            if ((type & FindType.GoTo) > 0)
            {
                GoToValue.IsEnabled = true;
                GoToSearch.IsEnabled = true;

                DataObject.AddPastingHandler(GoToValue, TextBoxPastingEventHandler);

                if ((type & FindType.Find) == 0)
                    GoToExpander.IsExpanded = true;
            }
        }

        /// <summary>
        /// Handles the Expanded event of the FindExpander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void FindExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (GoToExpander.IsExpanded)
                GoToExpander.IsExpanded = false;
        }

        /// <summary>
        /// Handles the Collapsed event of the FindExpander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void FindExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (!GoToExpander.IsExpanded)
                GoToExpander.IsExpanded = true;
        }

        private void GoToExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (FindExpander.IsExpanded)
                FindExpander.IsExpanded = false;
        }

        /// <summary>
        /// Handles the Collapsed event of the GoToExpander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void GoToExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (!FindExpander.IsExpanded)
                FindExpander.IsExpanded = true;
        }

        /// <summary>
        /// Handles the PreviewTextInput event of the GoToValue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.TextCompositionEventArgs"/> instance containing the event data.</param>
        private void GoToValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Texts the box pasting event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DataObjectPastingEventArgs"/> instance containing the event data.</param>
        private void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.DataObject.GetData(typeof(string)));
            }
            catch
            {
                e.CancelCommand();

                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the GoToSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void GoToSearch_Click(object sender, RoutedEventArgs e)
        {
            if (GoToValue.Text.Length > 0 && GoToClicked != null)
            {
                int value = 0;

                try
                {
                    value = Convert.ToInt32(GoToValue.Text);
                }
                catch
                {
                    MessageBox.Show("Invalid value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                if (!GoToClicked(value))
                    MessageBox.Show(string.Format("Couldn't find value {0}", value), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Handles the Click event of the FindButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            string value = FindValue.Text;

            if (value.Trim().Length == 0)
                return;

            string[] values = FindClicked(value);

            FindResults.Items.Clear();

            for (int i = 0; i < values.Length; i++)
                FindResults.Items.Add(values[i]);
        }

        private void FindResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FindResults.SelectedIndex == -1)
                return;

            SelectionClicked((string)FindResults.SelectedItem);
        }
    }
}