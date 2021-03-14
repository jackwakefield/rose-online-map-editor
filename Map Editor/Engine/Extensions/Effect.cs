using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Effect Extensions class.
/// </summary>
public static class EffectExtensions
{
    /// <summary>
    /// Starts the specified effect.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="technique">The technique.</param>
    public static void Start(this Effect effect, string technique)
    {
        effect.CurrentTechnique = effect.Techniques[technique];

        effect.Begin();
        effect.CurrentTechnique.Passes[0].Begin();
    }

    /// <summary>
    /// Finishes the specified effect.
    /// </summary>
    /// <param name="effect">The effect.</param>
    public static void Finish(this Effect effect)
    {
        effect.CurrentTechnique.Passes[0].End();
        effect.End();
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Quaternion[] value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Matrix value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Vector2[] value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Quaternion value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Vector3[] value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Vector4[] value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, bool value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, int[] value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, float[] value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Texture value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Vector2 value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Vector4 value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Matrix[] value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, Vector3 value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, int value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, float value)
    {
        effect.Parameters[key].SetValue(value);
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetValue(this Effect effect, string key, string value)
    {
        effect.Parameters[key].SetValue(value);
    }
}
