using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Shaders
{
    /// <summary>
    /// SimpleColour class.
    /// </summary>
    public static class SimpleColour
    {
        /// <summary>
        /// Shader code.
        /// </summary>
        public const string Shader =
      @"float4x4 WorldViewProjection;
        float4 Colour;

        struct VertexToPixel
        {
            float4 Position : POSITION0;
        };

        VertexToPixel VertexShaderFunction(VertexToPixel input)
        {
            VertexToPixel Output;

            Output.Position = mul(input.Position, WorldViewProjection);

            return Output;
        }

        float4 PixelShaderFunction(VertexToPixel PSIn) : COLOR0
        {
            return Colour;
        }

        technique SimpleColour
        {
            pass Pass1
            {
                VertexShader = compile vs_1_1 VertexShaderFunction();
                PixelShader = compile ps_1_1 PixelShaderFunction();
            }
        }";

        /// <summary>
        /// Compiles and returns the shader.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns>The compiled shader.</returns>
        public static Effect CreateEffect(GraphicsDevice device)
        {
            return new Effect(device, Effect.CompileEffectFromSource(Shader, null, null, CompilerOptions.None, TargetPlatform.Windows).GetEffectCode(), CompilerOptions.None, null);
        }
    }
}