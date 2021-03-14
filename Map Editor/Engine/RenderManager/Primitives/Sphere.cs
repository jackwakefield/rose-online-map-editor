using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.RenderManager.Primitives
{
    /// <summary>
    /// Sphere class.
    /// </summary>
    public class Sphere
    {
        /// <summary>
        /// Draws a sphere.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="phi_seg">The phi_seg.</param>
        /// <param name="theta_seg">The theta_seg.</param>
        /// <param name="transform">The transform.</param>
        public static void Draw(GraphicsDevice device, Vector3 center, float radius, int phi_seg, int theta_seg, Matrix transform)
        {
            float delta_phi = MathHelper.Pi / phi_seg;
            float delta_theta = MathHelper.TwoPi / theta_seg;

            int num_vtx = phi_seg * theta_seg * 4;

            Vector3[] vtx = new Vector3[num_vtx];
            int[] ind = new int[(phi_seg * theta_seg) * 6];

            int ivtx = 0;
            int iind = 0;
            float phi = 0;
            float theta = 0;

            for (int pseg = 0; pseg < phi_seg; pseg++, phi += delta_phi)
            {
                for (int tseg = 0; tseg < theta_seg; tseg++, theta += delta_theta)
                {
                    float cphi = (float)Math.Cos(phi);
                    float sphi = (float)Math.Sin(phi);
                    float cphi_d = (float)Math.Cos(phi + delta_phi);
                    float sphi_d = (float)Math.Sin(phi + delta_phi);
                    float ctheta = (float)Math.Cos(theta);
                    float stheta = (float)Math.Sin(theta);
                    float ctheta_d = (float)Math.Cos(theta + delta_theta);
                    float stheta_d = (float)Math.Sin(theta + delta_theta);

                    vtx[ivtx].X = radius * sphi * ctheta;
                    vtx[ivtx].Z = radius * sphi * stheta;
                    vtx[ivtx].Y = radius * cphi;
                    vtx[ivtx] = Vector3.Transform(vtx[ivtx], transform);
                    vtx[ivtx] += center;
                    ivtx++;

                    vtx[ivtx].X = radius * sphi_d * ctheta;
                    vtx[ivtx].Z = radius * sphi_d * stheta;
                    vtx[ivtx].Y = radius * cphi_d;
                    vtx[ivtx] = Vector3.Transform(vtx[ivtx], transform);
                    vtx[ivtx] += center;

                    ivtx++;

                    vtx[ivtx].X = radius * sphi_d * ctheta_d;
                    vtx[ivtx].Z = radius * sphi_d * stheta_d;
                    vtx[ivtx].Y = radius * cphi_d;
                    vtx[ivtx] = Vector3.Transform(vtx[ivtx], transform);
                    vtx[ivtx] += center;

                    ivtx++;

                    vtx[ivtx].X = radius * sphi * ctheta_d;
                    vtx[ivtx].Z = radius * sphi * stheta_d;
                    vtx[ivtx].Y = radius * cphi;
                    vtx[ivtx] = Vector3.Transform(vtx[ivtx], transform);
                    vtx[ivtx] += center;

                    ivtx++;

                    ind[iind++] = ivtx - 4;
                    ind[iind++] = ivtx - 3;
                    ind[iind++] = ivtx - 2;
                    ind[iind++] = ivtx - 2;
                    ind[iind++] = ivtx - 1;
                    ind[iind++] = ivtx - 4;
                }
            }

            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vtx, 0, num_vtx, ind, 0, phi_seg * theta_seg * 2);
        }

        /// <summary>
        /// Gets the bounding box of a sphere.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="phi_seg">The phi_seg.</param>
        /// <param name="theta_seg">The theta_seg.</param>
        /// <param name="transform">The transform.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox BoundingBox(Vector3 center, float radius, int phi_seg, int theta_seg, Matrix transform)
        {
            float delta_phi = MathHelper.Pi / phi_seg;
            float delta_theta = MathHelper.TwoPi / theta_seg;

            int num_vtx = phi_seg * theta_seg * 4;

            Vector3[] vertices = new Vector3[num_vtx];
            int[] indices = new int[(phi_seg * theta_seg) * 6];

            int ivtx = 0;
            int iind = 0;
            float phi = 0;
            float theta = 0;

            for (int pseg = 0; pseg < phi_seg; pseg++, phi += delta_phi)
            {
                for (int tseg = 0; tseg < theta_seg; tseg++, theta += delta_theta)
                {
                    float cphi = (float)Math.Cos(phi);
                    float sphi = (float)Math.Sin(phi);
                    float cphi_d = (float)Math.Cos(phi + delta_phi);
                    float sphi_d = (float)Math.Sin(phi + delta_phi);
                    float ctheta = (float)Math.Cos(theta);
                    float stheta = (float)Math.Sin(theta);
                    float ctheta_d = (float)Math.Cos(theta + delta_theta);
                    float stheta_d = (float)Math.Sin(theta + delta_theta);

                    vertices[ivtx].X = radius * sphi * ctheta;
                    vertices[ivtx].Z = radius * sphi * stheta;
                    vertices[ivtx].Y = radius * cphi;
                    vertices[ivtx] += center;

                    vertices[ivtx] = Vector3.Transform(vertices[ivtx], transform);
                    ivtx++;

                    vertices[ivtx].X = radius * sphi_d * ctheta;
                    vertices[ivtx].Z = radius * sphi_d * stheta;
                    vertices[ivtx].Y = radius * cphi_d;
                    vertices[ivtx] += center;

                    vertices[ivtx] = Vector3.Transform(vertices[ivtx], transform);
                    ivtx++;

                    vertices[ivtx].X = radius * sphi_d * ctheta_d;
                    vertices[ivtx].Z = radius * sphi_d * stheta_d;
                    vertices[ivtx].Y = radius * cphi_d;
                    vertices[ivtx] += center;

                    vertices[ivtx] = Vector3.Transform(vertices[ivtx], transform);
                    ivtx++;

                    vertices[ivtx].X = radius * sphi * ctheta_d;
                    vertices[ivtx].Z = radius * sphi * stheta_d;
                    vertices[ivtx].Y = radius * cphi;
                    vertices[ivtx] += center;

                    vertices[ivtx] = Vector3.Transform(vertices[ivtx], transform);
                    ivtx++;

                    indices[iind++] = ivtx - 4;
                    indices[iind++] = ivtx - 3;
                    indices[iind++] = ivtx - 2;
                    indices[iind++] = ivtx - 2;
                    indices[iind++] = ivtx - 1;
                    indices[iind++] = ivtx - 4;
                }
            }

            return Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(vertices);
        }
    }
}