using System;
using System.Windows.Forms;
using Map_Editor.Engine.RenderManager.Primitives;
using Map_Editor.Engine.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Map_Editor.Engine.Manipulation
{
    /// <summary>
    /// Translate Manipulation class.
    /// </summary>
    public class Translate
    {
        /// <summary>
        /// Selected axis type.
        /// </summary>
        public enum Axis
        {
            /// <summary>
            /// None.
            /// </summary>
            None,

            /// <summary>
            /// X.
            /// </summary>
            X,

            /// <summary>
            /// Y.
            /// </summary>
            Y,

            /// <summary>
            /// Z.
            /// </summary>
            Z
        }

        #region Member Declarations

        #region Mouse Related Declarations

        /// <summary>
        /// Gets or sets the last mouse location.
        /// </summary>
        /// <value>The last mouse location.</value>
        private Vector2 lastMouseLocation { get; set; }

        /// <summary>
        /// Gets or sets the mouse moved.
        /// </summary>
        /// <value>The mouse moved.</value>
        private Vector2 mouseMoved { get; set; }

        /// <summary>
        /// Gets or sets the selected axis.
        /// </summary>
        /// <value>The selected axis.</value>
        private Axis selectedAxis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [left button down].
        /// </summary>
        /// <value><c>true</c> if [left button down]; otherwise, <c>false</c>.</value>
        private bool leftButtonDown { get; set; }

        #endregion

        private bool upKeyDown;
        private bool downKeyDown;
        private bool leftKeyDown;
        private bool rightKeyDown;

        /// <summary>
        /// Position.
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// Gets or sets the real position.
        /// </summary>
        /// <value>The real position.</value>
        private Vector3 realPosition;

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDevice device { get; set; }

        /// <summary>
        /// Gets or sets the depth stencil.
        /// </summary>
        /// <value>The depth stencil.</value>
        private DepthStencilBuffer depthStencil { get; set; }

        /// <summary>
        /// Gets or sets the view scale.
        /// </summary>
        /// <value>The view scale.</value>
        private float viewScale { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position
        {
            get { return position; }
            set
            {
                heightDifference = position.Z - Height.GetHeight((int)((value.X / 2.5f) + 0.5f), (int)((value.Y / 2.5f) + 0.5f));

                position = value;

                CalculateScale();
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 RealPosition
        {
            set { realPosition = value; }
        }

        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>The bounding box.</value>
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [first time].
        /// </summary>
        /// <value><c>true</c> if [first time]; otherwise, <c>false</c>.</value>
        public bool FirstTime { get; set; }

        /// <summary>
        /// Gets or sets the height difference.
        /// </summary>
        /// <value>The height difference.</value>
        private float heightDifference { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [mouse released].
        /// </summary>
        public event EventHandler MouseReleased;

        /// <summary>
        /// Occurs when [position changed].
        /// </summary>
        public event EventHandler PositionChanged;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Translate"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public Translate(GraphicsDevice device)
        {
            leftButtonDown = false;
            FirstTime = true;

            this.device = device;

            depthStencil = new DepthStencilBuffer(device, device.Viewport.Width, device.Viewport.Height, device.PresentationParameters.AutoDepthStencilFormat, device.PresentationParameters.MultiSampleType, device.PresentationParameters.MultiSampleQuality);
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>Whether to update the base tool or not</returns>
        public bool Update()
        {
            MouseState mouseState = Mouse.GetState();

            mouseMoved = new Vector2(lastMouseLocation.X - mouseState.X, lastMouseLocation.Y - mouseState.Y);
            lastMouseLocation = new Vector2(mouseState.X, mouseState.Y);

            if (Position == Vector3.Zero)
                return true;

            if (FirstTime)
                return !(FirstTime = false);

            bool returnValue = true;
            bool mouseHovered = false;

            Ray pickRay = new Ray().Create(device, mouseState, CameraManager.View, CameraManager.Projection);

            Vector3 boundingBoxCenter = BoundingBox.GetCenter();

            Nullable<float> resultX1 = pickRay.Intersects(Cylinder.BoundingBox(new Vector3(boundingBoxCenter.X - (2.0f * (viewScale * 4.0f)), boundingBoxCenter.Y, boundingBoxCenter.Z), 10.0f, 1, 30, Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateScale(viewScale)));
            Nullable<float> resultY1 = pickRay.Intersects(Cylinder.BoundingBox(new Vector3(boundingBoxCenter.X, boundingBoxCenter.Y + (2.0f * (viewScale * 4.0f)), boundingBoxCenter.Z), 10.0f, 1, 30, Matrix.CreateScale(viewScale)));
            Nullable<float> resultZ1 = pickRay.Intersects(Cylinder.BoundingBox(new Vector3(boundingBoxCenter.X, boundingBoxCenter.Y, boundingBoxCenter.Z + (2.0f * (viewScale * 4.0f))), 10.0f, 1, 30, Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateScale(viewScale)));

            Nullable<float> resultX2 = pickRay.Intersects(Cone.BoundingBox(new Vector3(boundingBoxCenter.X - (4.0f * (viewScale * 4.0f)), boundingBoxCenter.Y, boundingBoxCenter.Z), 2.0f, 8, 30, Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateScale(viewScale)));
            Nullable<float> resultY2 = pickRay.Intersects(Cone.BoundingBox(new Vector3(boundingBoxCenter.X, boundingBoxCenter.Y + (4.0f * (viewScale * 4.0f)), boundingBoxCenter.Z), 2.0f, 8, 30, Matrix.CreateScale(viewScale)));
            Nullable<float> resultZ2 = pickRay.Intersects(Cone.BoundingBox(new Vector3(boundingBoxCenter.X, boundingBoxCenter.Y, boundingBoxCenter.Z + (4.0f * (viewScale * 4.0f))), 2.0f, 8, 30, Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateScale(viewScale)));

            if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && !leftButtonDown)
            {
                heightDifference = position.Z - Height.GetHeight((int)((position.X / 2.5f) + 0.5f), (int)((position.Y / 2.5f) + 0.5f));
                
                leftButtonDown = true;
            }
            else if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && leftButtonDown)
            {
                leftButtonDown = false;

                if (MouseReleased != null && mouseState.Intersects(device.Viewport))
                    MouseReleased(null, null);
            }

            if (((resultX1.HasValue || resultX2.HasValue) && selectedAxis == Axis.None) || selectedAxis == Axis.X)
            {
                selectedAxis = Axis.X;
                mouseHovered = true;

                if (leftButtonDown)
                {
                    if (App.Form.SnapToGrid.SelectedIndex > 0)
                    {
                        realPosition.X -= MouseDifference(Vector3.Normalize(Vector3.UnitX)).X;

                        float gridSize = 0.0625f * (float)Math.Pow(2, App.Form.SnapToGrid.SelectedIndex);
                        position.X = (float)Math.Round(realPosition.X / gridSize) * gridSize;
                    }
                    else
                        position.X -= MouseDifference(Vector3.Normalize(Vector3.UnitX)).X;

                    if (App.Form.FollowTerrain.IsChecked == true)
                        position.Z = Height.GetHeight((int)((position.X / 2.5f) + 0.5f), (int)((position.Y / 2.5f) + 0.5f)) + heightDifference;

                    if(PositionChanged != null)
                        PositionChanged(null, null);

                    returnValue = false;
                }
            }
            else if (((resultY1.HasValue || resultY2.HasValue) && selectedAxis == Axis.None) || selectedAxis == Axis.Y)
            {
                selectedAxis = Axis.Y;
                mouseHovered = true;

                if (leftButtonDown)
                {
                    if (App.Form.SnapToGrid.SelectedIndex > 0)
                    {
                        realPosition.Y -= MouseDifference(Vector3.Normalize(Vector3.UnitY)).Y;

                        float gridSize = 0.0625f * (float)Math.Pow(2, App.Form.SnapToGrid.SelectedIndex);
                        position.Y = (float)Math.Round(realPosition.Y / gridSize) * gridSize;
                    }
                    else
                        position.Y -= MouseDifference(Vector3.Normalize(Vector3.UnitY)).Y;

                    if (App.Form.FollowTerrain.IsChecked == true)
                        position.Z = Height.GetHeight((int)((position.X / 2.5f) + 0.5f), (int)((position.Y / 2.5f) + 0.5f)) + heightDifference;

                    if (PositionChanged != null)
                        PositionChanged(null, null);

                    returnValue = false;
                }
            }
            else if (((resultZ1.HasValue || resultZ2.HasValue) && selectedAxis == Axis.None) || selectedAxis == Axis.Z)
            {
                selectedAxis = Axis.Z;
                mouseHovered = true;

                if (leftButtonDown)
                {
                    position.Z -= MouseDifference(Vector3.Normalize(Vector3.UnitZ)).Z;

                    if (PositionChanged != null)
                        PositionChanged(null, null);

                    returnValue = false;
                }
            }

            KeyboardState keyboardState = Keyboard.GetState();

            Vector3 arrowPosition = Vector3.Zero;

            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && !upKeyDown)
                arrowPosition.Y += 0.1f;

            upKeyDown = keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up);

            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) && !downKeyDown)
                arrowPosition.Y -= 0.1f;

            downKeyDown = keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down);

            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) && !leftKeyDown)
                arrowPosition.X -= 0.1f;

            leftKeyDown = keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left);

            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) && !rightKeyDown)
                arrowPosition.X += 0.1f;

            rightKeyDown = keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right);

            if (arrowPosition != Vector3.Zero)
            {
                position += arrowPosition;

                if (App.Form.FollowTerrain.IsChecked == true)
                    position.Z = Height.GetHeight((int)((position.X / 2.5f) + 0.5f), (int)((position.Y / 2.5f) + 0.5f)) + heightDifference;

                if (PositionChanged != null)
                    PositionChanged(null, null);
            }

            if (mouseHovered)
                Cursor.Current = Cursors.Hand;
            else
                Cursor.Current = Cursors.Default;

            if (!leftButtonDown &&
                !((resultX1.HasValue || resultX2.HasValue) && selectedAxis == Axis.X) &&
                !((resultZ1.HasValue || resultZ2.HasValue) && selectedAxis == Axis.Z) &&
                !((resultY1.HasValue || resultY2.HasValue) && selectedAxis == Axis.Y))
                selectedAxis = Axis.None;

            return returnValue;
        }

        public void CalculateScale()
        {
            Matrix scaleView = Matrix.CreateLookAt(Vector3.UnitZ * Vector3.Distance(Matrix.Invert(CameraManager.View).Translation, Position), Vector3.Zero, Vector3.UnitY);

            Vector3 projectionStart = device.Viewport.Project(Vector3.Zero, CameraManager.Projection, scaleView, Matrix.Identity);
            Vector3 projectionEnd = device.Viewport.Project(Vector3.UnitX, CameraManager.Projection, scaleView, Matrix.Identity);

            viewScale = 5.0f / Vector3.Subtract(projectionEnd, projectionStart).Length();
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void Draw()
        {
            if (Position == Vector3.Zero)
                return;

            CalculateScale();

            DepthStencilBuffer depthBuffer = device.DepthStencilBuffer;
            device.DepthStencilBuffer = depthStencil;

            try
            {
                device.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            }
            catch
            {
                device.DepthStencilBuffer = depthBuffer;

                depthStencil = new DepthStencilBuffer(device, device.Viewport.Width, device.Viewport.Height, device.PresentationParameters.AutoDepthStencilFormat, device.PresentationParameters.MultiSampleType, device.PresentationParameters.MultiSampleQuality);

                return;
            }

            Vector3 boundingBoxCenter = BoundingBox.GetCenter();
            
            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleColour);

            shader.SetValue("WorldViewProjection", CameraManager.View * CameraManager.Projection);

            shader.Start("SimpleColour");

            shader.Parameters["Colour"].SetValue((selectedAxis == Axis.X) ? new Vector4(Color.Yellow.ToVector3(), 1.0f) : new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
            shader.CommitChanges();

            Cylinder.Draw(device, new Vector3(boundingBoxCenter.X - (2.0f * (viewScale * 4.0f)), boundingBoxCenter.Y, boundingBoxCenter.Z), 10.0f, 1, 30, Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateScale(viewScale));
            Cone.Draw(device, new Vector3(boundingBoxCenter.X - (4.0f * (viewScale * 4.0f)), boundingBoxCenter.Y, boundingBoxCenter.Z), 2.0f, 8, 30, Matrix.CreateRotationZ(MathHelper.PiOver2) * Matrix.CreateScale(viewScale));

            shader.SetValue("Colour", (selectedAxis == Axis.Y) ? new Vector4(Color.Yellow.ToVector3(), 1.0f) : new Vector4(0.0f, 1.0f, 0.0f, 1.0f));
            shader.CommitChanges();

            Cylinder.Draw(device, new Vector3(boundingBoxCenter.X, boundingBoxCenter.Y + (2.0f * (viewScale * 4.0f)), boundingBoxCenter.Z), 10.0f, 1, 30, Matrix.CreateScale(viewScale));
            Cone.Draw(device, new Vector3(boundingBoxCenter.X, boundingBoxCenter.Y + (4.0f * (viewScale * 4.0f)), boundingBoxCenter.Z), 2.0f, 8, 30, Matrix.CreateScale(viewScale));

            shader.SetValue("Colour", (selectedAxis == Axis.Z) ? new Vector4(Color.Yellow.ToVector3(), 1.0f) : new Vector4(0.0f, 0.0f, 1.0f, 1.0f));
            shader.CommitChanges();

            Cylinder.Draw(device, new Vector3(boundingBoxCenter.X, boundingBoxCenter.Y, boundingBoxCenter.Z + (2.0f * (viewScale * 4.0f))), 10.0f, 1, 30, Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateScale(viewScale));
            Cone.Draw(device, new Vector3(boundingBoxCenter.X, boundingBoxCenter.Y, boundingBoxCenter.Z + (4.0f * (viewScale * 4.0f))), 2.0f, 8, 30, Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateScale(viewScale));

            shader.SetValue("Colour", new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
            shader.CommitChanges();

            Sphere.Draw(device, boundingBoxCenter, 2.2f, 30, 30, Matrix.CreateScale(viewScale));

            shader.Finish();

            device.DepthStencilBuffer = depthBuffer;
        }

        /// <summary>
        /// Calculates the 3D position change from the mouse position.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>The difference.</returns>
        public Vector3 MouseDifference(Vector3 unit)
        {
            Matrix translation = Matrix.CreateTranslation(Position);

            Vector3 startPosition = device.Viewport.Project(Vector3.Zero, CameraManager.Projection, CameraManager.View, translation);
            Vector3 endPosition = device.Viewport.Project(unit, CameraManager.Projection, CameraManager.View, translation);

            Vector3 screenDirection = Vector3.Normalize(endPosition - startPosition);

            endPosition = startPosition + (screenDirection * (Vector3.Dot(new Vector3(mouseMoved, 0.0f), screenDirection)));

            startPosition = device.Viewport.Unproject(startPosition, CameraManager.Projection, CameraManager.View, translation);
            endPosition = device.Viewport.Unproject(endPosition, CameraManager.Projection, CameraManager.View, translation);

            return endPosition - startPosition;
        }
    }
}