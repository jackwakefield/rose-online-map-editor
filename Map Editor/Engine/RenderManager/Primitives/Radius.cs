using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.RenderManager.Primitives
{
    /// <summary>
    /// Radius class.
    /// </summary>
    public class Radius
    {
        /// <summary>
        /// Draws a circular radius.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="boundingSphere">The bounding sphere.</param>
        public static void Draw(GraphicsDevice device, BoundingSphere boundingSphere)
        {
            Vector3[] vertices = new Vector3[(120 + 1) * 3];

            int index = 0;

            float step = MathHelper.TwoPi / 120;

            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
                vertices[index++] = new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f);

            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
                vertices[index++] = new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a));

            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
                vertices[index++] = new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a));

            Matrix World = Matrix.CreateScale(boundingSphere.Radius) * Matrix.CreateTranslation(boundingSphere.Center);

            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = Vector3.Transform(vertices[i], World);

            device.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 120);
        }
    }
}