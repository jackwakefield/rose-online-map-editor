using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Shaders
{
    /// <summary>
    /// Animation class.
    /// </summary>
    public static class Animation
    {
        /// <summary>
        /// Shader code.
        /// </summary>
        public const string Shader =
      @"float4x4 WorldViewProjection;

        texture Texture;
        sampler T_Sampler = sampler_state
        {
            Texture = <Texture>;
            MinFilter = Linear;
            MagFilter = Linear;
            MipFilter = Linear;
            AddressU = mirror;
            AddressV = mirror;
        };

        struct VertexToPixel
        {
            float4 Position : POSITION0;
            float2 TexCoord : TEXCOORD0;
            float4 Alpha : COLOR0;
        };

        struct VertexToPixelOut
        {
            float4 Position : POSITION0;
            float2 TexCoord : TEXCOORD0;
            float Alpha : COLOR0;
        };

        VertexToPixelOut VertexShaderFunction(VertexToPixel input)
        {
            VertexToPixelOut Output;

            Output.Position = mul(input.Position, WorldViewProjection);
            Output.TexCoord = input.TexCoord;
            Output.Alpha = input.Alpha.a;

            return Output;
        }

        float4 PixelShaderFunction(VertexToPixelOut PSIn) : COLOR0
        {
            return tex2D(T_Sampler, PSIn.TexCoord) * PSIn.Alpha;
        }

        technique Animation
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