using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// BoundingBox Extensions class.
/// </summary>
public static class BoundingBoxExtensions
{
    /// <summary>
    /// Splits the specified bounding box.
    /// </summary>
    /// <param name="boundingBox">The bounding box.</param>
    /// <returns>Split bounding box.</returns>
    public static BoundingBox[] Split(this BoundingBox boundingBox)
    {
        return new BoundingBox[]
        {
            new BoundingBox(new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Min.Z), new Vector3((boundingBox.Min.X + boundingBox.Max.X) / 2.0f, (boundingBox.Min.Y + boundingBox.Max.Y) / 2.0f, boundingBox.Max.Z)),
            new BoundingBox(new Vector3(boundingBox.Min.X, (boundingBox.Min.Y + boundingBox.Max.Y) / 2.0f, boundingBox.Min.Z), new Vector3((boundingBox.Min.X + boundingBox.Max.X) / 2.0f, boundingBox.Max.Y, boundingBox.Max.Z)),
            new BoundingBox(new Vector3((boundingBox.Min.X + boundingBox.Max.X) / 2.0f, boundingBox.Min.Y, boundingBox.Min.Z), new Vector3(boundingBox.Max.X, (boundingBox.Min.Y + boundingBox.Max.Y) / 2.0f, boundingBox.Max.Z)),
            new BoundingBox(new Vector3((boundingBox.Min.X + boundingBox.Max.X) / 2.0f, (boundingBox.Min.Y + boundingBox.Max.Y) / 2.0f, boundingBox.Min.Z), new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Max.Z)),
        };
    }

    /// <summary>
    /// Determines whether the specified bounding box is closer.
    /// </summary>
    /// <param name="boundingBox">The bounding box.</param>
    /// <param name="secondBoundingBox">The second bounding box.</param>
    /// <param name="cameraPosition">The camera position.</param>
    /// <returns>
    /// 	<c>true</c> if the specified bounding box is closer; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsCloser(this BoundingBox boundingBox, BoundingBox secondBoundingBox, Vector3 cameraPosition)
    {
        Vector3[] cornersA = boundingBox.GetCorners();
        Vector3[] cornersB = secondBoundingBox.GetCorners();

        for (int i = 0; i < BoundingBox.CornerCount; i++)
        {
            if (Vector3.Distance(cameraPosition, cornersA[i]) > Vector3.Distance(cameraPosition, cornersB[i]))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the center.
    /// </summary>
    /// <param name="boundingBox">The bounding box.</param>
    /// <returns>Center position.</returns>
    public static Vector3 GetCenter(this BoundingBox boundingBox)
    {
        return (boundingBox.Min + boundingBox.Max) / 2.0f;
    }

    /// <summary>
    /// Draws the specified bounding box.
    /// </summary>
    /// <param name="boundingBox">The bounding box.</param>
    /// <param name="device">The device.</param>
    /// <param name="effect">The effect.</param>
    /// <param name="view">The view.</param>
    /// <param name="projection">The projection.</param>
    /// <param name="colour">The colour.</param>
    public static void Draw(this BoundingBox boundingBox, GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection, Color colour)
    {
        short[] indices = new short[]
        {
            0, 1, 1,
            2, 2, 3,
            3, 0, 0,
            4, 1, 5,
            2, 6, 3,
            7, 4, 5,
            5, 6, 6,
            7, 7, 4,
        };

        effect.VertexColorEnabled = true;
        effect.LightingEnabled = false;

        effect.World = Matrix.Identity;
        effect.View = view;
        effect.Projection = projection;

        Vector3[] corners = boundingBox.GetCorners();
        VertexPositionColor[] vertices = new VertexPositionColor[8];

        for (int i = 0; i < 8; i++)
        {
            vertices[i].Position = corners[i];
            vertices[i].Color = colour;
        }

        device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);

        effect.Begin();

        for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
        {
            effect.CurrentTechnique.Passes[i].Begin();

            device.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, 8, indices, 0, indices.Length / 2);

            effect.CurrentTechnique.Passes[i].End();
        }

        effect.End();
    }

    /// <summary>
    /// Draws the specified bounding box.
    /// </summary>
    /// <param name="boundingBox">The bounding box.</param>
    /// <param name="device">The device.</param>
    public static void Draw(this BoundingBox boundingBox, GraphicsDevice device)
    {
        short[] indices = new short[]
        {
            0, 1, 1,
            2, 2, 3,
            3, 0, 0,
            4, 1, 5,
            2, 6, 3,
            7, 4, 5,
            5, 6, 6,
            7, 7, 4,
        };

        Vector3[] corners = boundingBox.GetCorners();
        VertexPositionColor[] vertices = new VertexPositionColor[8];

        for (int i = 0; i < 8; i++)
        {
            vertices[i].Position = corners[i];
            vertices[i].Color = Color.White;
        }

        device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);

        device.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, 8, indices, 0, indices.Length / 2);
    }
}
