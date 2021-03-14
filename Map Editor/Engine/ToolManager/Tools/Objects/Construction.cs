using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Tools
{
    /// <summary>
    /// Construction class.
    /// </summary>
    public class Construction : Object
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Construction"/> class.
        /// </summary>
        /// <param name="device">Graphics Device</param>
        public Construction(GraphicsDevice device)
            : base(device)
        {
            Type = ObjectType.Construction;
        }
    }
}