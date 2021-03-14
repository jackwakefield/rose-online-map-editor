using System.Collections.Generic;
using Map_Editor.Engine.Shaders;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// ShaderManager class.
/// </summary>
public static class ShaderManager
{
    /// <summary>
    /// Shader type.
    /// </summary>
    public enum ShaderType
    {
        /// <summary>
        /// Height.
        /// </summary>
        Height = 0,

        /// <summary>
        /// Height Editing.
        /// </summary>
        HeightEditing = 1,

        /// <summary>
        /// Simple Texture.
        /// </summary>
        SimpleTexture = 2,

        /// <summary>
        /// Simple Colour.
        /// </summary>
        SimpleColour = 3,

        /// <summary>
        /// Object.
        /// </summary>
        Object = 4,

        /// <summary>
        /// NPC.
        /// </summary>
        NPC = 5,

        /// <summary>
        /// Animation
        /// </summary>
        Animation = 6
    }

    #region List Declarations

    /// <summary>
    /// Gets or sets the shaders.
    /// </summary>
    /// <value>The shaders.</value>
    private static List<Effect> shaders { get; set; }

    #endregion

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="device">The device.</param>
    public static void Initialize(GraphicsDevice device)
    {
        shaders = new List<Effect>(System.Enum.GetNames(typeof(ShaderType)).Length);

        shaders.Add(Height.CreateEffect(device));
        shaders.Add(HeightEditing.CreateEffect(device));
        shaders.Add(SimpleTexture.CreateEffect(device));
        shaders.Add(SimpleColour.CreateEffect(device));
        shaders.Add(Object.CreateEffect(device));
        shaders.Add(NPC.CreateEffect(device));
        shaders.Add(Animation.CreateEffect(device));
    }

    /// <summary>
    /// Gets the shader.
    /// </summary>
    /// <param name="shaderType">Type of the shader.</param>
    /// <returns>The shader.</returns>
    public static Effect GetShader(ShaderType shaderType)
    {
        return shaders[(int)shaderType];
    }
}