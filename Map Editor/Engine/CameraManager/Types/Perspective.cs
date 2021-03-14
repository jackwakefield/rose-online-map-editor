using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Map_Editor.Engine.Types
{
    /// <summary>
    /// Perspective cass.
    /// </summary>
    public class Perspective
    {
        /// <summary>
        /// Speed of the camera.
        /// </summary>
        public const float CAMERA_SPEED = 0.833335f;

        #region Member Declarations

        /// <summary>
        /// Gets or sets the velocity position.
        /// </summary>
        /// <value>The velocity position.</value>
        private Vector3 velocityPosition { get;  set; }

        /// <summary>
        /// Gets or sets the mouse rotation X.
        /// </summary>
        /// <value>The mouse rotation X.</value>
        private float mouseRotationX { get;  set; }

        /// <summary>
        /// Gets or sets the mouse rotation Z.
        /// </summary>
        /// <value>The mouse rotation Z.</value>
        private float mouseRotationZ { get;  set; }

        /// <summary>
        /// Gets or sets the last mouse location.
        /// </summary>
        /// <value>The last mouse location.</value>
        private Vector2 lastMouseLocation { get;  set; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public Matrix View { get; private set; }

        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        /// <value>The projection.</value>
        public Matrix Projection { get; private set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Perspective"/> class.
        /// </summary>
        public Perspective()
        {
            View = Matrix.Identity;
            Projection = Matrix.Identity;

            mouseRotationX = 1.54f;
            mouseRotationZ = -3.150001f;

            Position = new Vector3(5650.0f, 5250.0f, 10.0f);

            velocityPosition = Vector3.Zero;
        }

        /// <summary>
        /// Rotates the camera.
        /// </summary>
        /// <param name="x">X angle.</param>
        /// <param name="z">Z angle.</param>
        private void Rotate(float x, float z)
        {
            mouseRotationX += x;
            mouseRotationZ += z;
        }

        /// <summary>
        /// Changes the position of the camera affected by the rotation.
        /// </summary>
        /// <param name="distanceChange">The change in distance.</param>
        private void Translate(Vector3 distanceChange)
        {
            Position += Vector3.Transform(distanceChange, Matrix.CreateRotationX(mouseRotationX) * Matrix.CreateRotationZ(mouseRotationZ));
        }

        /// <summary>
        /// Update the camera matrices each frame.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            Vector2 mouseMoved = new Vector2(lastMouseLocation.X - mouseState.X, lastMouseLocation.Y - mouseState.Y);
            lastMouseLocation = new Vector2(mouseState.X, mouseState.Y);

            if (!mouseState.Intersects(App.Engine.GraphicsDevice.Viewport))
            {
                Translate(velocityPosition *= CAMERA_SPEED);

                View = Matrix.Invert(Matrix.CreateRotationX(mouseRotationX) * Matrix.CreateRotationZ(mouseRotationZ) * Matrix.CreateTranslation(Position));

                return;
            }

            bool redeclare = false;
            Vector3 movement = Vector3.Zero;

            if (mouseState.RightButton == ButtonState.Pressed)
                Rotate((mouseMoved.Y * CAMERA_SPEED) / 200.0f, (mouseMoved.X * CAMERA_SPEED) / 200.0f);

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
            {
                movement += new Vector3(0, 0, -CAMERA_SPEED);

                if (keyboardState.IsKeyDown(Keys.LeftShift))
                    movement += new Vector3(0, 0, -CAMERA_SPEED);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                movement += new Vector3(0, 0, CAMERA_SPEED);

                if (keyboardState.IsKeyDown(Keys.LeftShift))
                    movement += new Vector3(0, 0, CAMERA_SPEED);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement += new Vector3(-CAMERA_SPEED, 0, 0);

                if (keyboardState.IsKeyDown(Keys.LeftShift))
                    movement += new Vector3(-CAMERA_SPEED, 0, 0);
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                movement += new Vector3(CAMERA_SPEED, 0, 0);

                if (keyboardState.IsKeyDown(Keys.LeftShift))
                    movement += new Vector3(CAMERA_SPEED, 0, 0);
            }

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.D))
            {
                redeclare = true;

                velocityPosition = movement;
            }

            if (!redeclare)
                Translate(velocityPosition *= (CAMERA_SPEED / 1.0f));

            Translate(movement);

            View = Matrix.Invert(Matrix.CreateRotationX(mouseRotationX) * Matrix.CreateRotationZ(mouseRotationZ) * Matrix.CreateTranslation(Position));
        }

        /// <summary>
        /// Updates the viewport.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void UpdateViewport(int width, int height)
        {
            Viewport viewport = App.Engine.GraphicsDevice.Viewport;
            viewport.MinDepth = 1.0f;
            viewport.MaxDepth = 50000.0f;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, viewport.AspectRatio, viewport.MinDepth, viewport.MaxDepth);
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetPosition(Vector3 position)
        {
            Position = position;

            View = Matrix.Invert(Matrix.CreateRotationX(mouseRotationX) * Matrix.CreateRotationZ(mouseRotationZ) * Matrix.CreateTranslation(Position));
        }
    }
}
