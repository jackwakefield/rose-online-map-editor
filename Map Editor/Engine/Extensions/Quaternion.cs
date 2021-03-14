using System;
using Microsoft.Xna.Framework;

/// <summary>
/// Quaternion Extensions class.
/// </summary>
public static class QuaternionExtensions
{
    /// <summary>
    /// Returns the angle of an arc.
    /// </summary>
    /// <param name="X">X.</param>
    /// <param name="Y">Y.</param>
    /// <returns>Angle.</returns>
    public static float ArcTanAngle(float X, float Y)
    {
        if (X == 0)
        {
            if (Y == 1)
                return (float)MathHelper.PiOver2;

            return (float)-MathHelper.PiOver2;
        }

        if (X > 0)
            return (float)Math.Atan(Y / X);

        if (X < 0)
        {
            if (Y > 0)
                return (float)Math.Atan(Y / X) + MathHelper.Pi;

            return (float)Math.Atan(Y / X) - MathHelper.Pi;
        }

        return 0;
    }

    /// <summary>
    /// Returns the Euler angles that point from one point to another.
    /// </summary>
    /// <param name="from">The position.</param>
    /// <param name="location">The location.</param>
    /// <returns>Eular angles.</returns>
    public static Vector3 AngleTo(Vector3 from, Vector3 location)
    {
        Vector3 vector = Vector3.Normalize(location - from);

        return new Vector3
        {
            X = (float)Math.Asin(vector.Y),
            Y = ArcTanAngle(-vector.Z, -vector.X),
        };
    }

    /// <summary>
    /// Converts a Quaternion to Euler angles.
    /// </summary>
    /// <param name="rotation">The quaternion.</param>
    /// <returns>Eular Angles.</returns>
    public static Vector3 ToEular(this Quaternion rotation)
    {
        Vector3 forwardVector = Vector3.Transform(Vector3.Forward, rotation);
        Vector3 upVector = Vector3.Transform(Vector3.Up, rotation);
        Vector3 rotationAxes = AngleTo(Vector3.Zero, forwardVector);

        if (rotationAxes.X == MathHelper.PiOver2)
        {
            rotationAxes.Y = ArcTanAngle(upVector.Z, upVector.X);
            rotationAxes.Z = 0;
        }
        else if (rotationAxes.X == -MathHelper.PiOver2)
        {
            rotationAxes.Y = ArcTanAngle(-upVector.Z, -upVector.X);
            rotationAxes.Z = 0;
        }
        else
        {
            upVector = Vector3.Transform(upVector, Matrix.CreateRotationY(-rotationAxes.Y));
            upVector = Vector3.Transform(upVector, Matrix.CreateRotationX(-rotationAxes.X));

            rotationAxes.Z = ArcTanAngle(upVector.Y, -upVector.X);
        }

        return rotationAxes;
    }
}