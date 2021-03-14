using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Shaders
{
    /// <summary>
    /// Object class.
    /// </summary>
    public static class Object
    {
        /// <summary>
        /// Shader code.
        /// </summary>
        public const string Shader =
       @"float4x4 WorldViewProjection;
        float2 TextureAdd;
        float2 TextureMultiply;

        bool MasterLightmapEnabled = true;
        bool LightmapEnabled;

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

        texture Lightmap;
        sampler L_Sampler = sampler_state
        {
            Texture = <Lightmap>;
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
            float2 Lightmap : TEXCOORD1;
        };

        VertexToPixel VertexShaderFunction(VertexToPixel input)
        {
            VertexToPixel Output;

            Output.Position = mul(input.Position, WorldViewProjection);
            Output.TexCoord = input.TexCoord;	
            Output.Lightmap = (input.Lightmap + TextureAdd) * TextureMultiply;

            return Output;
        }

        float4 PixelShaderFunction(VertexToPixel PSIn) : COLOR0
        {
            float4 finalColour = tex2D(T_Sampler, PSIn.TexCoord);

            if (MasterLightmapEnabled && LightmapEnabled)
            {
                finalColour *= 2;
                finalColour.rgb *= tex2D(L_Sampler, PSIn.Lightmap).rgb;
            }

            return finalColour;
        }

        technique Object
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
