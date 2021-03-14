using System.Windows;
using Map_Editor.Misc;

namespace Map_Editor
{
    /// <summary>
    /// App class.
    /// </summary>
    public partial class App : Application
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        /// <value>The form.</value>
        public static Main Form { get; set; }

        /// <summary>
        /// Gets or sets the engine.
        /// </summary>
        /// <value>The engine.</value>
        public static Engine.Main Engine { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.
        /// </exception>
        public App()
        {
            ConfigurationManager.LoadConfig();
            ConfigurationManager.CheckConfig();

            System.Windows.Forms.Application.EnableVisualStyles();

            Form = new Main();
            Form.WindowState = WindowState.Minimized;
            Form.Show();
        }
    }
}