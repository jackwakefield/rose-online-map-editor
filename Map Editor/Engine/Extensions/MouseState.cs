using Map_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// MouseState Extensions class.
/// </summary>
public static class MouseStateExtensions
{
    /// <summary>
    /// Checks if the mouse is inside the specified viewport.
    /// </summary>
    /// <param name="mouseState">The mouse state</param>
    /// <param name="viewport">The viewport.</param>
    /// <returns>
    /// 	<c>true</c> if the mouse is inside the viewport; otherwise, <c>false</c>.
    /// </returns>
    public static bool Intersects(this MouseState mouseState, Viewport viewport)
    {
        return Intersects(mouseState, new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height)) && App.Engine.IsActive && App.Form.RenderPanel.Focused;
    }

    /// <summary>
    /// Checks if the mouse is inside the specified rectangle.
    /// </summary>
    /// <param name="mouseState">The mouse state</param>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>
    /// 	<c>true</c> if the mouse is inside the rectangle; otherwise, <c>false</c>.
    /// </returns>
    public static bool Intersects(this MouseState mouseState, Rectangle rectangle)
    {
        return mouseState.X > rectangle.Left && mouseState.Y > rectangle.Top && mouseState.X < rectangle.Right && mouseState.Y < rectangle.Bottom;
    }
}