using System;
using System.Windows.Forms;
using Map_Editor.Engine.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Map_Editor.Engine.Manipulation
{
    public class CursorTranslate
    {
        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDevice device { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        private Vector3 position;
        
        /// <summary>
        /// Occurs when [position changed].
        /// </summary>
        public event EventHandler PositionChanged;

        /// <summary>
        /// Occurs when [cancelled].
        /// </summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// Occurs when [finished].
        /// </summary>
        public event EventHandler Finished;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Translate"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public CursorTranslate(GraphicsDevice device)
        {
            this.device = device;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            if (Position == Vector3.Zero)
                return;

            MouseState mouseState = Mouse.GetState();

            if (!mouseState.Intersects(device.Viewport))
                return;

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (Cancelled != null)
                    Cancelled(null, EventArgs.Empty);

                App.Form.Manipulation_Click(App.Form.NoManipulation, null);

                return;
            }
            
            Vector3 pickedPosition = MapManager.Heightmaps.PickPosition();

            if (App.Form.SnapToGrid.SelectedIndex > 0)
            {
                float gridSize = 0.0625f * (float)Math.Pow(2, App.Form.SnapToGrid.SelectedIndex);

                pickedPosition.X = (float)Math.Round(pickedPosition.X / gridSize) * gridSize;
                pickedPosition.Y = (float)Math.Round(pickedPosition.Y / gridSize) * gridSize;
                pickedPosition.Z = Height.GetHeight((int)((pickedPosition.X / 2.5f) + 0.5f), (int)((pickedPosition.Y / 2.5f) + 0.5f));
            }

            if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                position = pickedPosition;

                if (Finished != null)
                    Finished(null, EventArgs.Empty);

                App.Form.Manipulation_Click(App.Form.NoManipulation, null);

                return;
            }

            if (pickedPosition != position)
            {
                position = pickedPosition;

                if (PositionChanged != null)
                    PositionChanged(null, EventArgs.Empty);
            }

            if (pickedPosition != Vector3.Zero)
                Cursor.Current = Cursors.Hand;
            else
                Cursor.Current = Cursors.Default;
        }
    }
}