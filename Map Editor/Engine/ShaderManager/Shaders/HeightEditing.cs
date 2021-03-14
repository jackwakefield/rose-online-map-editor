using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Shaders
{
    /// <summary>
    /// HeightEditing class.
    /// </summary>
    public static class HeightEditing
    {
        /// <summary>
        /// Shader code.
        /// </summary>
        public const string Shader =
      @"float4x4 WorldViewProjection;

        float3 SelectionPosition;
        float InnerRadius;
        float OutterRadius;

        int HeightMode;

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

            finalColour *= tex2D(S_Sampler, input.TexCoordShadow);
            finalColour *= 2;

            if (HeightMode == 0)
            {
                float2 positionDistance = float2(abs(input.WorldPosition.x - SelectionPosition.x), abs(input.WorldPosition.y - SelectionPosition.y));
                
                if ((positionDistance.x < InnerRadius && positionDistance.x > InnerRadius - 0.5f && positionDistance.y < InnerRadius) ||
                    (positionDistance.y < InnerRadius && positionDistance.y > InnerRadius - 0.5f && positionDistance.x < InnerRadius))
                    finalColour.b = 1.0f;
                else if ((positionDistance.x < OutterRadius && positionDistance.x > OutterRadius - 0.5f && positionDistance.y < OutterRadius) ||
                         (positionDistance.y < OutterRadius && positionDistance.y > OutterRadius - 0.5f && positionDistance.x < OutterRadius))
                    finalColour.r = 1.0f;
            }
            else if (HeightMode == 1)
            {
                float positionDistance = abs(distance(SelectionPosition, input.WorldPosition));

                if (positionDistance < InnerRadius && positionDistance > InnerRadius - 0.5f)
                    finalColour.b = 1.0f;
                else if (positionDistance < OutterRadius && positionDistance > OutterRadius - 0.5f)
                    finalColour.r = 1.0f;
            }

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
