using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Shaders
{
    /// <summary>
    /// SimpleTexture class.
    /// </summary>
    public static class SimpleTexture
    {
        /// <summary>
        /// Shader code.
        /// </summary>
        public const string Shader =
      @"float4x4 WorldViewProjection;
        float Alpha = 1.0f;
        float Multiply = 1.0f;

        float4 Addition = float4(0.0f, 0.0f, 0.0f, 0.0f);

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
        };

        VertexToPixel VertexShaderFunction(VertexToPixel input)
        {
            VertexToPixel Output;

            Output.Position = mul(input.Position, WorldViewProjection);
            Output.TexCoord = input.TexCoord;

            return Output;
        }

        float4 PixelShaderFunction(VertexToPixel PSIn) : COLOR0
        {
            float4 shaderTexture = tex2D(T_Sampler, PSIn.TexCoord) * Multiply;
            shaderTexture.a = Alpha;

            return tex2D(T_Sampler, PSIn.TexCoord) + Addition;
        }

        technique SimpleTexture
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