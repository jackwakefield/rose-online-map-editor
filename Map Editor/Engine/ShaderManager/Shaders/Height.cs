using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Shaders
{
    /// <summary>
    /// Height class.
    /// </summary>
    public static class Height
    {
        /// <summary>
        /// Shader code.
        /// </summary>
        public const string Shader =
      @"float4x4 WorldViewProjection;

        bool ModifyingTile;
        int Rotation;

        bool Highlight;
        bool GridOutlineEnabled;

        texture BottomTexture;
        sampler B_Sampler = sampler_state
        {
            Texture = <BottomTexture>;
            MinFilter = Linear;
            MagFilter = Linear;
            MipFilter = Linear;
            AddressU = mirror;
            AddressV = mirror;
        };

        texture TopTexture;
        sampler T_Sampler = sampler_state
        {
            Texture = <TopTexture>;
            MinFilter = Linear;
            MagFilter = Linear;
            MipFilter = Linear;
            AddressU = mirror;
            AddressV = mirror;
        };

        texture ShadowTexture;
        sampler S_Sampler = sampler_state
        {
            Texture = <ShadowTexture>;
            MinFilter = Linear;
            MagFilter = Linear;
            MipFilter = Linear;
            AddressU = mirror;
            AddressV = mirror;
        };

        struct Vertex
        {
            float4 Position : POSITION0;
            float2 TexCoordBottom : TEXCOORD0;
            float2 TexCoordTop : TEXCOORD1;
            float2 TexCoordShadow : TEXCOORD2;
        };

        struct VertexToPixel
        {
            float4 Position : POSITION0;
            float2 TexCoordBottom : TEXCOORD0;
            float2 TexCoordTop : TEXCOORD1;
            float2 TexCoordShadow : TEXCOORD2;
            float4 WorldPosition : TEXCOORD3;
        };

        VertexToPixel VertexShaderFunction(Vertex input)
        {
            VertexToPixel output;

            output.Position = mul(input.Position, WorldViewProjection);
            output.TexCoordBottom = input.TexCoordBottom;
            output.TexCoordTop = input.TexCoordTop;
            output.TexCoordShadow = input.TexCoordShadow;
            output.WorldPosition = input.Position;

            if (ModifyingTile)
            {
                output.TexCoordTop = input.TexCoordBottom;

                if (Rotation == 2)
	                output.TexCoordTop.x = 1.0f - output.TexCoordTop.x;
                else if (Rotation == 3)
	                output.TexCoordTop.y = 1.0f - output.TexCoordTop.y;
                else if (Rotation == 4)
                {
	                output.TexCoordTop.x = 1.0f - output.TexCoordTop.x;
	                output.TexCoordTop.y = 1.0f - output.TexCoordTop.y;
                }
                else if (Rotation == 5)
                {
	                float tempX = output.TexCoordTop.x;
	                output.TexCoordTop.x = output.TexCoordTop.y;
	                output.TexCoordTop.y = 1.0f - tempX;
                }
                else if (Rotation == 6)
                {
	                float tempX = output.TexCoordTop.x;
	                output.TexCoordTop.x = output.TexCoordTop.y;
	                output.TexCoordTop.y = tempX;
                }
            }

            return output;
        }

        float4 PixelShaderFunction(VertexToPixel input) : COLOR0
        {
            float4 bottomTexture = tex2D(B_Sampler, input.TexCoordBottom);
            bottomTexture.a = 1.0f;
           
            float4 topTexture = tex2D(T_Sampler, input.TexCoordTop);
            float alphaAmount = topTexture.a;
            topTexture.a = 1.0f;
        	
            float4 finalColour = lerp(bottomTexture, topTexture, alphaAmount);

            if (Highlight &&
               (input.TexCoordBottom.x < 0.02f ||
                input.TexCoordBottom.y < 0.02f ||
                input.TexCoordBottom.x > 0.98f ||
                input.TexCoordBottom.y > 0.98f))
                finalColour.rg *= 1.5f;

            if (GridOutlineEnabled && 
                (input.TexCoordShadow.x < 0.001f ||
                input.TexCoordShadow.y < 0.001f ||
                input.TexCoordShadow.x > 0.999f ||
                input.TexCoordShadow.y > 0.999f))
                finalColour.rg *= 1.5f;

            finalColour *= tex2D(S_Sampler, input.TexCoordShadow);
            finalColour *= 2;

            return finalColour;   
        }

        technique Ground
        {
            pass Pass1
            {
                VertexShader = compile vs_1_1 VertexShaderFunction();
                PixelShader = compile ps_2_0 PixelShaderFunction();
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
