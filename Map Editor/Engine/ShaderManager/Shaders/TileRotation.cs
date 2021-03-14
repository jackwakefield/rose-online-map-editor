using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Shaders
{
    /// <summary>
    /// TileRotation class.
    /// </summary>
    public static class TileRotation
    {
        /// <summary>
        /// Shader code.
        /// </summary>
        public const string Shader =
       @"sampler2D SpriteTexture : register(s0);

        int RotationValue;

        float4 PPRotate(float2 TexCoord : TEXCOORD0) : COLOR0
        {
            if (RotationValue == 2)
            {
                TexCoord.x = 1.0f - TexCoord.x;
            }
            else if (RotationValue == 3)
            {
                TexCoord.y = 1.0f - TexCoord.y;
            }
            else if (RotationValue == 4)
            {
                TexCoord.x = 1.0f - TexCoord.x;
                TexCoord.y = 1.0f - TexCoord.y;
            }
            else if (RotationValue == 5)
            {
                float TempX = TexCoord.x;
                TexCoord.x = TexCoord.y;
                TexCoord.y = 1.0f - TempX;
            }
            else if (RotationValue == 6)
            {
                float TempX = TexCoord.x;
                TexCoord.x = TexCoord.y;
                TexCoord.y = TempX;
            }	
        	
            return tex2D(SpriteTexture, TexCoord);
        }

        technique TileRotation
        {
            pass Pass1
            {
                PixelShader = compile ps_2_0 PPRotate();
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
