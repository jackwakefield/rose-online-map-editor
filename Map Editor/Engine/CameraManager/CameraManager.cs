using Map_Editor.Engine.Types;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine
{
    /// <summary>
    /// Camera Manager class.
    /// </summary>
    public static class CameraManager
    {
        /// <summary>
        /// Type of camera.
        /// </summary>
        public enum CameraType
        {
            /// <summary>
            /// Perspective camera.
            /// </summary>
            Perspective,

            /// <summary>
            /// Orthographic camera.
            /// </summary>
            Orthographic
        }

        #region Member Declarations

        /// <summary>
        /// Gets or sets the perspective camera.
        /// </summary>
        /// <value>The perspective camera.</value>
        public static Perspective PerspectiveCamera { get; set; }

        /// <summary>
        /// Gets or sets the orthographic camera.
        /// </summary>
        /// <value>The orthographic camera.</value>
        public static Orthographic OrthographicCamera { get; set; }

        /// <summary>
        /// Gets or sets the type of the current camera.
        /// </summary>
        /// <value>The type of the current camera.</value>
        public static CameraType CurrentCameraType { get; private set; }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        public static Matrix View
        {
            get
            {
                switch (CurrentCameraType)
                {
                    case CameraType.Perspective:
                        return PerspectiveCamera.View;
                    case CameraType.Orthographic:
                        return OrthographicCamera.View;
                    default:
                        return Matrix.Identity;
                }
            }
        }

        /// <summary>
        /// Gets the projection.
        /// </summary>
        /// <value>The projection.</value>
        public static Matrix Projection
        {
            get
            {
                switch (CurrentCameraType)
                {
                    case CameraType.Perspective:
                        return PerspectiveCamera.Projection;
                    case CameraType.Orthographic:
                        return OrthographicCamera.Projection;
                    default:
                        return Matrix.Identity;
                }
            }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public static Vector3 Position
        {
            get
            {
                switch (CurrentCameraType)
                {
                    case CameraType.Perspective:
                        return PerspectiveCamera.Position;
                    case CameraType.Orthographic:
                        return new Vector3(OrthographicCamera.Position, 0.0f);
                    default:
                        return Vector3.Zero;
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            PerspectiveCamera = new Perspective();
            OrthographicCamera = new Orthographic();

            SetCameraType(CameraType.Perspective);
        }

        /// <summary>
        /// Sets the type of the camera.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void SetCameraType(CameraType type)
        {
            CurrentCameraType = type;
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="position">The position.</param>
        public static void SetPosition(Vector3 position)
        {
            switch (CurrentCameraType)
            {
                case CameraType.Perspective:
                    PerspectiveCamera.SetPosition(position);
                    break;
                case CameraType.Orthographic:
                    break;
            }
        }

        /// <summary>
        /// Updates the selected camera.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public static void Update(GameTime gameTime)
        {
            switch (CurrentCameraType)
            {
                case CameraType.Perspective:
                    PerspectiveCamera.Update(gameTime);
                    break;
                case CameraType.Orthographic:
                    OrthographicCamera.Update(gameTime);
                    break;
            }
        }

        /// <summary>
        /// Updates the viewport.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void UpdateViewport(int width, int height)
        {
            PerspectiveCamera.UpdateViewport(width, height);
        }
    }
}