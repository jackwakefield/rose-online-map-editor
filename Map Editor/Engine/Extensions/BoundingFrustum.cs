using Microsoft.Xna.Framework;

/// <summary>
/// BoundingFrustum Extensions class.
/// </summary>
public static class BoundingFrustumExtensions
{
    /// <summary>
    /// Checks if a bounding frustum is either intersection or containing the specified bounding box.
    /// </summary>
    /// <param name="boundingFrustum">The bounding frustum.</param>
    /// <param name="boundingBox">The bounding box.</param>
    /// <returns>If the bounding box is on the screen.</returns>
    public static bool OnScreen(this BoundingFrustum boundingFrustum, BoundingBox boundingBox)
    {
        return boundingFrustum.Contains(boundingBox) == ContainmentType.Intersects || boundingFrustum.Contains(boundingBox) == ContainmentType.Contains;
    }

    /// <summary>
    /// Checks if a bounding frustum is either intersection or containing the specified bounding sphere.
    /// </summary>
    /// <param name="boundingFrustum">The bounding frustum.</param>
    /// <param name="boundingSphere">The bounding sphere.</param>
    /// <returns>If the bounding box is on the screen.</returns>
    public static bool OnScreen(this BoundingFrustum boundingFrustum, BoundingSphere boundingSphere)
    {
        return boundingFrustum.Contains(boundingSphere) == ContainmentType.Intersects || boundingFrustum.Contains(boundingSphere) == ContainmentType.Contains;
    }
}