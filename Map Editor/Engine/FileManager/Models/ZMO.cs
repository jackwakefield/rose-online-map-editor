using System.IO;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Models
{
    /// <summary>
    /// ZMO class.
    /// </summary>
    public class ZMO
    {
        /// <summary>
        /// Channel type.
        /// </summary>
        public enum ChannelType
        {
            /// <summary>
            /// Position.
            /// </summary>
            Position = 1 << 1,

            /// <summary>
            /// Rotation.
            /// </summary>
            Rotation = 1 << 2,

            /// <summary>
            /// Normal.
            /// </summary>
            Normal = 1 << 3,

            /// <summary>
            /// Alpha.
            /// </summary>
            Alpha = 1 << 4,

            /// <summary>
            /// Texture coordinate.
            /// </summary>
            UV0 = 1 << 5,

            /// <summary>
            /// Texture coordinate.
            /// </summary>
            UV1 = 1 << 6,

            /// <summary>
            /// Texture coordinate.
            /// </summary>
            UV2 = 1 << 7,

            /// <summary>
            /// Texture coordinate.
            /// </summary>
            UV3 = 1 << 8,

            /// <summary>
            /// Texture animation.
            /// </summary>
            TextureAnimation = 1 << 9,

            /// <summary>
            /// Scale.
            /// </summary>
            Scale = 1 << 10
        }

        /// <summary>
        /// Channel structure.
        /// </summary>
        public struct Channel
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>The type.</value>
            public ChannelType Type { get; set; }

            /// <summary>
            /// Gets or sets the ID.
            /// </summary>
            /// <value>The ID.</value>
            public int ID { get; set; }

            #endregion
        };

        /// <summary>
        /// Frame structure.
        /// </summary>
        public struct Frame
        {
            /// <summary>
            /// Channel structure.
            /// </summary>
            public struct Channel
            {
                #region Member Declarations

                /// <summary>
                /// Gets or sets the position.
                /// </summary>
                /// <value>The position.</value>
                public Vector3 Position { get; set; }

                /// <summary>
                /// Gets or sets the rotation.
                /// </summary>
                /// <value>The rotation.</value>
                public Quaternion Rotation { get; set; }

                /// <summary>
                /// Gets or sets the normal.
                /// </summary>
                /// <value>The normal.</value>
                public Vector3 Normal { get; set; }

                /// <summary>
                /// Gets or sets the alpha.
                /// </summary>
                /// <value>The alpha.</value>
                public float Alpha { get; set; }

                /// <summary>
                /// Gets or sets the texture coordinate.
                /// </summary>
                /// <value>The texture coordinate.</value>
                public Vector2 UV0 { get; set; }

                /// <summary>
                /// Gets or sets the texture coordinate.
                /// </summary>
                /// <value>The texture coordinate.</value>
                public Vector2 UV1 { get; set; }

                /// <summary>
                /// Gets or sets the texture coordinate.
                /// </summary>
                /// <value>The texture coordinate.</value>
                public Vector2 UV2 { get; set; }

                /// <summary>
                /// Gets or sets the texture coordinate.
                /// </summary>
                /// <value>The texture coordinate.</value>
                public Vector2 UV3 { get; set; }
                
                /// <summary>
                /// Gets or sets the texture animation.
                /// </summary>
                /// <value>The texture animation.</value>
                public float TextureAnimation { get; set; }

                /// <summary>
                /// Gets or sets the scale.
                /// </summary>
                /// <value>The scale.</value>
                public float Scale { get; set; }

                #endregion
            };

            #region Member Declarations

            /// <summary>
            /// Gets or sets the channels.
            /// </summary>
            /// <value>The channels.</value>
            public Channel[] Channels { get; set; }

            #endregion
        };

        #region Member Declarations

        /// <summary>
        /// Gets or sets the FPS.
        /// </summary>
        /// <value>The FPS.</value>
        public int FPS { get; set; }

        /// <summary>
        /// Gets or sets the channels.
        /// </summary>
        /// <value>The channels.</value>
        public Channel[] Channels { get; set; }

        /// <summary>
        /// Gets or sets the frames.
        /// </summary>
        /// <value>The frames.</value>
        public Frame[] Frames { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ZMO"/> class.
        /// </summary>
        public ZMO()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZMO"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="camera">if set to <c>true</c> [camera].</param>
        /// <param name="divide">if set to <c>true</c> [divide].</param>
        public ZMO(string filePath, bool camera, bool divide)
        {
            Load(filePath, camera, divide);
        }

        /// <summary>
        /// Loads the specified file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="camera">if set to <c>true</c> [camera].</param>
        /// <param name="divide">if set to <c>true</c> [divide].</param>
        public void Load(string filePath, bool camera, bool divide)
        {
            FileHandler fh = new FileHandler(filePath, FileHandler.FileOpenMode.Reading, null);

            fh.Seek(8, SeekOrigin.Begin);

            FPS = fh.Read<int>();
            int frameCount = fh.Read<int>();
            int channelCount = fh.Read<int>();

            Channels = new Channel[channelCount];

            for (int i = 0; i < channelCount; i++)
            {
                Channels[i] = new Channel()
                {
                    Type = (ChannelType)fh.Read<int>(),
                    ID = fh.Read<int>()
                };
            }

            Frames = new Frame[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                Frames[i] = new Frame()
                {
                    Channels = new Frame.Channel[channelCount]
                };

                for (int j = 0; j < channelCount; j++)
                {
                    switch (Channels[j].Type)
                    {
                        case ChannelType.Position:
                            {
                                Frames[i].Channels[j].Position = fh.Read<Vector3>();

                                if (camera && (j == 0 || j == 1))
                                    Frames[i].Channels[j].Position = (fh.Read<Vector3>() + new Vector3(520000.0f, 520000.0f, 0.0f)) / 100.0f;
                                else if (divide)
                                    Frames[i].Channels[j].Position /= 100.0f;
                            }
                            break;
                        case ChannelType.Rotation:
                            {
                                Frames[i].Channels[j].Rotation = new Quaternion()
                                {
                                    W = fh.Read<float>(),
                                    X = fh.Read<float>(),
                                    Y = fh.Read<float>(),
                                    Z = fh.Read<float>()
                                };
                            }
                            break;
                        case ChannelType.Normal:
                            Frames[i].Channels[j].Normal = fh.Read<Vector3>();
                            break;
                        case ChannelType.Alpha:
                            Frames[i].Channels[j].Alpha = fh.Read<float>();
                            break;
                        case ChannelType.UV0:
                            Frames[i].Channels[j].UV0 = fh.Read<Vector2>();
                            break;
                        case ChannelType.UV1:
                            Frames[i].Channels[j].UV1 = fh.Read<Vector2>();
                            break;
                        case ChannelType.UV2:
                            Frames[i].Channels[j].UV2 = fh.Read<Vector2>();
                            break;
                        case ChannelType.UV3:
                            Frames[i].Channels[j].UV3 = fh.Read<Vector2>();
                            break;
                        case ChannelType.TextureAnimation:
                            Frames[i].Channels[j].TextureAnimation = fh.Read<float>();
                            break;
                        case ChannelType.Scale:
                            Frames[i].Channels[j].Scale = fh.Read<float>();
                            break;
                    }
                }
            }

            fh.Close();
        }
    }
}