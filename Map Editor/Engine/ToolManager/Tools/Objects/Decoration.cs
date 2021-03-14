using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Tools
{
    /// <summary>
    /// Decoration class.
    /// </summary>
    public class Decoration : Object
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Decoration"/> class.
        /// </summary>
        /// <param name="device">Graphics Device</param>
        public Decoration(GraphicsDevice device)
            : base(device)
        {
            Type = ObjectType.Decoration;
        }
    }
}
