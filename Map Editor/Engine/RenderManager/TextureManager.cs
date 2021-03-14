using System.Collections.Generic;
using Map_Editor.Engine.Models;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.RenderManager
{
    /// <summary>
    /// TextureManager class.
    /// </summary>
    public class TextureManager
    {
        /// <summary>
        /// RenderState structure.
        /// </summary>
        public struct RenderState
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the alpha.
            /// </summary>
            /// <value>The alpha.</value>
            public float Alpha { get; set; }

            /// <summary>
            /// Gets or sets the alpha reference.
            /// </summary>
            /// <value>The alpha reference.</value>
            public int AlphaReference { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [alpha enabled].
            /// </summary>
            /// <value><c>true</c> if [alpha enabled]; otherwise, <c>false</c>.</value>
            public bool AlphaEnabled { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [alpha ref enabled].
            /// </summary>
            /// <value><c>true</c> if [alpha ref enabled]; otherwise, <c>false</c>.</value>
            public bool AlphaRefEnabled { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [alpha test enabled].
            /// </summary>
            /// <value><c>true</c> if [alpha test enabled]; otherwise, <c>false</c>.</value>
            public bool AlphaTestEnabled { get; set; }

            /// <summary>
            /// Gets or sets the source blend.
            /// </summary>
            /// <value>The source blend.</value>
            public Blend SourceBlend { get; set; }

            /// <summary>
            /// Gets or sets the destination blend.
            /// </summary>
            /// <value>The destination blend.</value>
            public Blend DestinationBlend { get; set; }

            /// <summary>
            /// Gets or sets the alpha function.
            /// </summary>
            /// <value>The alpha function.</value>
            public CompareFunction AlphaFunction { get; set; }

            /// <summary>
            /// Gets or sets the blend function.
            /// </summary>
            /// <value>The blend function.</value>
            public BlendFunction BlendFunction { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [Z test enabled].
            /// </summary>
            /// <value><c>true</c> if [Z test enabled]; otherwise, <c>false</c>.</value>
            public bool ZTestEnabled { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [Z write enabled].
            /// </summary>
            /// <value><c>true</c> if [Z write enabled]; otherwise, <c>false</c>.</value>
            public bool ZWriteEnabled { get; set; }

            /// <summary>
            /// Gets or sets the cull.
            /// </summary>
            /// <value>The cull.</value>
            public CullMode Cull { get; set; }

            #endregion
        };

        /// <summary>
        /// Texture structure.
        /// </summary>
        public struct Texture
        {
            #region Member Declarations
            
            /// <summary>
            /// Gets or sets the image.
            /// </summary>
            /// <value>The image.</value>
            public Texture2D Image { get; set; }

            /// <summary>
            /// Gets or sets the state of the render.
            /// </summary>
            /// <value>The state of the render.</value>
            public RenderState RenderState { get; set; }

            /// <summary>
            /// Gets or sets the file path.
            /// </summary>
            /// <value>The file path.</value>
            public string FilePath { get; set; }

            #endregion
        };

        #region Member Declarations

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDevice device { get; set; }

        #endregion

        #region List Declarations

        /// <summary>
        /// Gets or sets the texture list.
        /// </summary>
        /// <value>The texture list.</value>
        private List<Texture> textureList { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureManager"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public TextureManager(GraphicsDevice device)
        {
            this.device = device;

            textureList = new List<Texture>();
        }

        /// <summary>
        /// Gets the <see cref="Map_Editor.Engine.RenderManager.TextureManager.Texture"/> with the specified id.
        /// </summary>
        /// <value></value>
        public Texture this[int id]
        {
            get { return textureList[id]; }
        }

        /// <summary>
        /// Adds the specified texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns></returns>
        public int Add(ZSC.Texture texture)
        {
            return Add(texture.Path, new RenderState()
            {
                Alpha = texture.Alpha,
                AlphaReference = texture.AlphaReference,
                AlphaEnabled = texture.AlphaEnabled,
                AlphaTestEnabled = texture.AlphaTestEnabled,
                AlphaFunction = CompareFunction.GreaterEqual,
                SourceBlend = GetSourceBlend(texture.BlendingMode),
                DestinationBlend = GetDestinationBlend(texture.BlendingMode),
                BlendFunction = GetBlendFunction(texture.BlendingMode),
                ZTestEnabled = texture.ZTestEnabled,
                ZWriteEnabled = texture.ZWriteEnabled,
                Cull = (texture.TwoSided) ? CullMode.None : CullMode.CullClockwiseFace
            });
        }

        /// <summary>
        /// Adds the specified texture path.
        /// </summary>
        /// <param name="texturePath">The texture path.</param>
        /// <param name="renderStates">The render states.</param>
        /// <returns></returns>
        public int Add(string texturePath, RenderState renderStates)
        {
            for (int i = 0; i < textureList.Count; i++)
            {
                if (string.Compare(textureList[i].FilePath, texturePath, true) == 0)
                    return i;
            }

            textureList.Add(new Texture()
            {
                Image = Texture2D.FromFile(device, texturePath),
                FilePath = texturePath,
                RenderState = renderStates,
            });

            return textureList.Count - 1;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="id">The id.</param>
        public void RemoveAt(int id)
        {
            textureList[id].Image.Dispose();

            textureList.RemoveAt(id);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            textureList.ForEach(delegate(Texture texture)
            {
                texture.Image.Dispose();
            });

            textureList.Clear();
        }

        #region Static Functions

        /// <summary>
        /// Gets the source blend.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>The blend mode.</returns>
        public static Blend GetSourceBlend(ZSC.BlendingType mode)
        {
            if (mode == ZSC.BlendingType.Lighten)
                return Blend.One;
            else
                return Blend.SourceAlpha;
        }

        /// <summary>
        /// Gets the destination blend.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>The blend mode.</returns>
        public static Blend GetDestinationBlend(ZSC.BlendingType mode)
        {
            if (mode == ZSC.BlendingType.Lighten)
                return Blend.One;
            else
                return Blend.InverseSourceAlpha;
        }

        /// <summary>
        /// Gets the blend function.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>The blend function.</returns>
        public static BlendFunction GetBlendFunction(ZSC.BlendingType mode)
        {
            return BlendFunction.Add;
        }

        /// <summary>
        /// Gets the blend mode.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The blend mode.</returns>
        public static Blend BlendingMode(int type)
        {
            switch (type)
            {
                case 1:
                    return Blend.Zero;
                case 2:
                    return Blend.One;
                case 3:
                    return Blend.SourceColor;
                case 4:
                    return Blend.InverseSourceColor;
                case 5:
                    return Blend.SourceAlpha;
                case 6:
                    return Blend.InverseSourceAlpha;
                case 7:
                    return Blend.DestinationAlpha;
                case 8:
                    return Blend.InverseDestinationAlpha;
                case 9:
                    return Blend.DestinationColor;
                case 10:
                    return Blend.InverseDestinationColor;
                case 11:
                    return Blend.SourceAlphaSaturation;
                default:
                    return Blend.One;
            }
        }

        #endregion
    }
}