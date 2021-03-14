using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.RenderManager.Primitives
{
    /// <summary>
    /// Cone class.
    /// </summary>
    public class Cone
    {
        /// <summary>
        /// Draws a cone.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="segments">The segments.</param>
        /// <param name="transform">The transform.</param>
        public static void Draw(GraphicsDevice device, Vector3 center, float radius, float height, int segments, Matrix transform)
        {
            float ival = MathHelper.TwoPi / segments;

            Vector3[] vertices = new Vector3[segments + 2];
            vertices[0] = Vector3.Transform(Vector3.UnitY * height, transform);
            vertices[0] += center;

            for (int i = 0; i < segments; ++i)
            {
                float rot = i * ival;

                float a = radius * (float)Math.Cos(rot);
                float b = radius * (float)Math.Sin(rot);

                vertices[i + 1] = Vector3.Transform(new Vector3(a, 0, b), transform);
                vertices[i + 1] += center;
            }

            vertices[segments + 1] = vertices[1];

            device.DrawUserPrimitives(PrimitiveType.TriangleFan, vertices, 0, segments);
        }


        /// <summary>
        /// Gets the bounding box of a cone.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="segments">The segments.</param>
        /// <param name="transform">The transform.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox BoundingBox(Vector3 center, float radius, float height, int segments, Matrix transform)
        {
            float ival = MathHelper.TwoPi / segments;

            Vector3[] vertices = new Vector3[segments + 2];
            vertices[0] = Vector3.Transform(Vector3.UnitY * height, transform);
            vertices[0] += center;

            for (int i = 0; i < segments; ++i)
            {
                float rot = i * ival;

                float a = radius * (float)Math.Cos(rot);
                float b = radius * (float)Math.Sin(rot);

                vertices[i + 1] = Vector3.Transform(new Vector3(a, 0, b), transform);
                vertices[i + 1] += center;
            }

            vertices[segments + 1] = vertices[1];

            return Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(vertices);
        }
    }
}