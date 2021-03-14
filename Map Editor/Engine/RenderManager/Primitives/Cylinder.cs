using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.RenderManager.Primitives
{
    /// <summary>
    /// Cylinder class.
    /// </summary>
    public class Cylinder
    {
        /// <summary>
        /// Draws a cylinder.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="center">The center.</param>
        /// <param name="extents">The extents.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="segments">The segments.</param>
        /// <param name="transform">The transform.</param>
        public static void Draw(GraphicsDevice device, Vector3 center, float extents, float radius, int segments, Matrix transform)
        {
            float ival = MathHelper.TwoPi / segments;

            Vector3[] vertices = new Vector3[(segments + 1) << 1];

            for (int i = 0; i < ((segments + 1) << 1); i += 2)
            {
                float rot = i * ival;

                float a = radius * (float)Math.Cos(rot);
                float b = radius * (float)Math.Sin(rot);

                vertices[i + (((i % 2) == 0) ? (0) : (1))] = Vector3.Transform(new Vector3(a, extents, b), transform);
                vertices[i + (((i % 2) == 0) ? (0) : (1))] += center;
                vertices[i + (((i % 2) == 0) ? (1) : (0))] = Vector3.Transform(new Vector3(a, -extents, b), transform);
                vertices[i + (((i % 2) == 0) ? (1) : (0))] += center;
            }

            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, (segments) << 1);
        }

        /// <summary>
        /// Gets the bounding box of a cylinder.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="extents">The extents.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="segments">The segments.</param>
        /// <param name="transform">The transform.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox BoundingBox(Vector3 center, float extents, float radius, int segments, Matrix transform)
        {
            float ival = MathHelper.TwoPi / segments;

            Vector3[] vertices = new Vector3[(segments + 1) << 1];

            for (int i = 0; i < ((segments + 1) << 1); i += 2)
            {
                float rot = i * ival;

                float a = radius * (float)Math.Cos(rot);
                float b = radius * (float)Math.Sin(rot);

                vertices[i + (((i % 2) == 0) ? (0) : (1))] = Vector3.Transform(new Vector3(a, extents, b), transform);
                vertices[i + (((i % 2) == 0) ? (0) : (1))] += center;
                vertices[i + (((i % 2) == 0) ? (1) : (0))] = Vector3.Transform(new Vector3(a, -extents, b), transform);
                vertices[i + (((i % 2) == 0) ? (1) : (0))] += center;
            }

            return Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(vertices);
        }
    }
}