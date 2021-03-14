using System;
using System.Windows;
using System.Windows.Forms;
using Map_Editor.Engine.SpriteManager;
using Map_Editor.Forms.Controls.NPC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Map_Editor.Engine
{
    /// <summary>
    /// Main class.
    /// </summary>
    public class Main : Game
    {
        /// <summary>
        /// 
        /// </summary>
        public enum MainStateType
        {
            /// <summary>
            /// The application is initializing and XNA is not running.
            /// </summary>
            Initializing,

            /// <summary>
            /// The application has finished initializing. 
            /// </summary>
            Normal
        }

        /// <summary>
        /// How long the splash form will show.
        /// </summary>
        public const int SPLASH_TIME = 30;

        #region Member Declarations

        /// <summary>
        /// Gets or sets the state of the main.
        /// </summary>
        /// <value>The state of the main.</value>
        public MainStateType MainState { get; set; }

        #region Splash Form

        /// <summary>
        /// Gets or sets the splash form.
        /// </summary>
        /// <value>The splash form.</value>
        private Form splashForm { get; set; }

        /// <summary>
        /// Gets or sets the splash progress.
        /// </summary>
        /// <value>The splash progress.</value>
        private int splashProgress { get; set; }

        #endregion

        #region Graphics

        /// <summary>
        /// Gets or sets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        private GraphicsDeviceManager graphics { get; set; }

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDevice device { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1;
            graphics.PreferredBackBufferHeight = 1;
            graphics.PreferMultiSampling = true;
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(Graphics_PreparingDeviceSettings);
            graphics.ApplyChanges();
                        
            Content.RootDirectory = "Content";

            splashForm = (Form)Form.FromHandle(Window.Handle);
            splashForm.FormBorderStyle = FormBorderStyle.None;
            splashForm.AllowTransparency = true;

            Mouse.WindowHandle = App.Form.RenderPanel.Handle;

            MainState = MainStateType.Initializing;
        }

        /// <summary>
        /// Handles the PreparingDeviceSettings event of the Graphics control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Xna.Framework.PreparingDeviceSettingsEventArgs"/> instance containing the event data.</param>
        private void Graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = App.Form.RenderPanel.Handle;
        }

        /// <summary>
        /// Called after the Game and GraphicsDevice are created, but before LoadContent.  Reference page contains code sample.
        /// </summary>
        protected override void Initialize()
        {
            device = graphics.GraphicsDevice;

            UndoManager.Initialize();
            FileManager.Initialize();
            ShaderManager.Initialize(device);
            CameraManager.Initialize();
            ToolManager.Initialize(device);
            FontManager.Initialize(this);
            ToolTipManager.Initialize(device);
            MapManager.Initialize(this);

            App.Form.MiddleGrid.Children.Add(App.Form.PreviewPanel = new PreviewPanel()
            {
                Visibility = Visibility.Collapsed,
                Margin = new Thickness(5)
            });

            base.Initialize();
        }

        /// <summary>
        /// Errors with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Error(string message)
        {
            System.Windows.Forms.MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Environment.Exit(0);
        }

        /// <summary>
        /// Reference page contains links to related code samples.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        protected override void Update(GameTime gameTime)
        {
            CameraManager.Update(gameTime);
            ToolManager.Update(gameTime);

            switch (MainState)
            {
                case MainStateType.Normal:
                    {
                        if (graphics.PreferredBackBufferWidth != App.Form.RenderPanel.Width || graphics.PreferredBackBufferHeight != App.Form.RenderPanel.Height)
                        {
                            graphics.PreferredBackBufferWidth = App.Form.RenderPanel.Width;
                            graphics.PreferredBackBufferHeight = App.Form.RenderPanel.Height;
                            graphics.ApplyChanges();

                            CameraManager.UpdateViewport(App.Form.RenderPanel.Width, App.Form.RenderPanel.Height);
                        }
                    }
                    break;
                case MainStateType.Initializing:
                    {
                        splashProgress++;

                        if (splashProgress == SPLASH_TIME)
                        {
                            splashForm.Visible = false;

                            MainState = MainStateType.Normal;

                            App.Form.Topmost = true;
                            App.Form.WindowState = System.Windows.WindowState.Maximized;
                            App.Form.Topmost = false;

#if DEBUG
                            MapManager.Load(4);
#endif
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Reference page contains code sample.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (splashProgress < SPLASH_TIME)
                return;

            base.Draw(gameTime);

            ToolTipManager.Draw(gameTime);

            if (ToolManager.GetManipulationMode() == ToolManager.ManipulationMode.Position)
                ToolManager.Position.Draw();

            if (App.Form.PreviewPanel.IsVisible)
                App.Form.PreviewPanel.Draw(device);
        }
    }
}