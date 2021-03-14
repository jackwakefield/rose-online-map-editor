using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// BoundingSphere Extensions class.
/// </summary>
public static class BoundingSphereExtensions
{
    /// <summary>
    /// Draws the specified bounding sphere.
    /// </summary>
    /// <param name="boundingSphere">The bounding sphere.</param>
    /// <param name="device">The device.</param>
    /// <param name="effect">The effect.</param>
    /// <param name="view">The view.</param>
    /// <param name="projection">The projection.</param>
    /// <param name="colour">The colour.</param>
    public static void Draw(this BoundingSphere boundingSphere, GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection, Color colour)
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

        VertexPositionColor[] vertices = new VertexPositionColor[(120 + 1) * 3];

        int index = 0;

        float step = MathHelper.TwoPi / 120;

        for (float a = 0f; a <= MathHelper.TwoPi; a += step)
        {
            int i = index++;

            vertices[i].Color = colour;
            vertices[i].Position = new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f);
        }

        for (float a = 0f; a <= MathHelper.TwoPi; a += step)
        {
            int i = index++;

            vertices[i].Color = colour;
            vertices[i].Position = new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a));
        }

        for (float a = 0f; a <= MathHelper.TwoPi; a += step)
        {
            int i = index++;

            vertices[i].Color = colour;
            vertices[i].Position = new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a));
        }

        Matrix World = Matrix.CreateScale(boundingSphere.Radius) * Matrix.CreateTranslation(boundingSphere.Center);

        for (int i = 0; i < vertices.Length; i++)
            vertices[i].Position = Vector3.Transform(vertices[i].Position, World);

        device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);

        effect.Begin();

        for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
        {
            effect.CurrentTechnique.Passes[i].Begin();

            device.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 120);

            effect.CurrentTechnique.Passes[i].End();
        }

        effect.End();
    }
}
