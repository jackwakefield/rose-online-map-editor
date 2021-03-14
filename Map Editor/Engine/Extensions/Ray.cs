using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Ray Extensions class.
/// </summary>
public static class RayExtensions
{
    /// <summary>
    /// Simple ray creation.
    /// </summary>
    /// <param name="ray">The ray.</param>
    /// <param name="device">The device.</param>
    /// <param name="mouseState">State of the mouse.</param>
    /// <param name="view">The view.</param>
    /// <param name="projection">The projection.</param>
    /// <returns>New ray.</returns>
    public static Ray Create(this Ray ray, GraphicsDevice device, MouseState mouseState, Matrix view, Matrix projection)
    {
        Vector3 nearPoint = device.Viewport.Unproject(new Vector3(mouseState.X, mouseState.Y, 0.0f), projection, view, Matrix.Identity);
        Vector3 farPoint = device.Viewport.Unproject(new Vector3(mouseState.X, mouseState.Y, 1.0f), projection, view, Matrix.Identity);

        return new Ray(nearPoint, Vector3.Normalize(farPoint - nearPoint));
    }
}