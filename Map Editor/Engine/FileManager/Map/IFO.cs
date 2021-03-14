using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

namespace Map_Editor.Engine.Map
{
    /// <summary>
    /// IFO class.
    /// </summary>
    public class IFO
    {
        /// <summary>
        /// Type of object.
        /// </summary>
        public enum ObjectType
        {
            /// <summary>
            /// Nothing.
            /// </summary>
            Null,

            /// <summary>
            /// Morph object.
            /// </summary>
            Morph,

            /// <summary>
            /// Item object.
            /// </summary>
            Item,

            /// <summary>
            /// Collision object.
            /// </summary>
            Collision,

            /// <summary>
            /// Ground object.
            /// </summary>
            Ground,

            /// <summary>
            /// Construction object.
            /// </summary>
            Construction,

            /// <summary>
            /// NPC object.
            /// </summary>
            NPC,

            /// <summary>
            /// Monster object.
            /// </summary>
            Monster,

            /// <summary>
            /// Avatar object.
            /// </summary>
            Avatar,

            /// <summary>
            /// User object.
            /// </summary>
            User,

            /// <summary>
            /// Cart object.
            /// </summary>
            Cart,

            /// <summary>
            /// Castle Gear object.
            /// </summary>
            CastleGear,

            /// <summary>
            /// Event object.
            /// </summary>
            EventObject
        }

        /// <summary>
        /// Block type.
        /// </summary>
        public enum BlockType
        {
            /// <summary>
            /// Map information.
            /// </summary>
             MapInfo,

             /// <summary>
             /// Decoration objects.
             /// </summary>
             Decoration,

             /// <summary>
             /// NPCs.
             /// </summary>
             NPCs,

             /// <summary>
             /// Construction objects.
             /// </summary>
             Construction,

             /// <summary>
             /// Ambient sounds.
             /// </summary>
             Sounds,

             /// <summary>
             /// Particle and mesh effects.
             /// </summary>
             Effects,

             /// <summary>
             /// Animation based objects.
             /// </summary>
             Animation,

             /// <summary>
             /// Wide water, not used in ROSE.
             /// </summary>
             WideWater,

             /// <summary>
             /// Monster spawns.
             /// </summary>
             Monsters,

             /// <summary>
             /// Water.
             /// </summary>
             Water,

             /// <summary>
             /// Warp gates.
             /// </summary>
             WarpGates,

             /// <summary>
             /// Collision objects
             /// </summary>
             Collision,

             /// <summary>
             /// Event trigger objects.
             /// </summary>
             EventTriggers
        }

        #region Sub Classes

        /// <summary>
        /// IFO Entry class.
        /// </summary>
        public class IFOEntry
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the parent.
            /// </summary>
            /// <value>The parent.</value>
            public IFO Parent { get; set; }

            #endregion
        }

        /// <summary>
        /// Base IFO class.
        /// </summary>
        public class BaseIFO : IFOEntry
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <value>The description.</value>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the warp ID.
            /// </summary>
            /// <value>The warp ID.</value>
            public short WarpID { get; set; }

            /// <summary>
            /// Gets or sets the event ID.
            /// </summary>
            /// <value>The event ID.</value>
            public short EventID { get; set; }

            /// <summary>
            /// Gets or sets the object ID.
            /// </summary>
            /// <value>The object ID.</value>
            public int ObjectID { get; set; }

            /// <summary>
            /// Gets or sets the type of the object.
            /// </summary>
            /// <value>The type of the object.</value>
            public ObjectType ObjectType { get; set; }

            /// <summary>
            /// Gets or sets the map position.
            /// </summary>
            /// <value>The map position.</value>
            public Vector2 MapPosition { get; set; }

            /// <summary>
            /// Gets or sets the rotation.
            /// </summary>
            /// <value>The rotation.</value>
            public Quaternion Rotation { get; set; }

            /// <summary>
            /// Gets or sets the position.
            /// </summary>
            /// <value>The position.</value>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Gets or sets the scale.
            /// </summary>
            /// <value>The scale.</value>
            public Vector3 Scale { get; set; }

            #endregion
        }

        /// <summary>
        /// Map Information class.
        /// </summary>
        public class MapInformation : IFOEntry
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the width.
            /// </summary>
            /// <value>The width.</value>
            public int Width { get; set; }

            /// <summary>
            /// Gets or sets the height.
            /// </summary>
            /// <value>The height.</value>
            public int Height { get; set; }

            /// <summary>
            /// Gets or sets the map cell X.
            /// </summary>
            /// <value>The map cell X.</value>
            public int MapCellX { get; set; }

            /// <summary>
            /// Gets or sets the map cell Y.
            /// </summary>
            /// <value>The map cell Y.</value>
            public int MapCellY { get; set; }

            /// <summary>
            /// Gets or sets the world.
            /// </summary>
            /// <value>The world.</value>
            public Matrix World { get; set; }

            /// <summary>
            /// Gets or sets the name of the map.
            /// </summary>
            /// <value>The name of the map.</value>
            public string MapName { get; set; }

            #endregion
        }

        /// <summary>
        /// NPC class.
        /// </summary>
        public class NPC : BaseIFO
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            public string Path { get; set; }

            /// <summary>
            /// Gets or sets the index of the AI pattern.
            /// </summary>
            /// <value>The index of the AI pattern.</value>
            public int AIPatternIndex { get; set; }

            #endregion
        }

        /// <summary>
        /// Sound class.
        /// </summary>
        public class Sound : BaseIFO
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            public string Path { get; set; }

            /// <summary>
            /// Gets or sets the range.
            /// </summary>
            /// <value>The range.</value>
            public int Range { get; set; }

            /// <summary>
            /// Gets or sets the interval.
            /// </summary>
            /// <value>The interval.</value>
            public int Interval { get; set; }

            #endregion
        }

        /// <summary>
        /// Effect class.
        /// </summary>
        public class Effect : BaseIFO
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            public string Path { get; set; }

            #endregion
        }

        /// <summary>
        /// Unused Water class.
        /// </summary>
        public class UnusedWater : BaseIFO
        {
            /// <summary>
            /// Water Block structure.
            /// </summary>
            public struct WaterBlock
            {
                #region Member Declarations

                /// <summary>
                /// Gets or sets the use.
                /// </summary>
                /// <value>The use.</value>
                public byte Use { get; set; }

                /// <summary>
                /// Gets or sets the height.
                /// </summary>
                /// <value>The height.</value>
                public float Height { get; set; }

                /// <summary>
                /// Gets or sets the type of the water.
                /// </summary>
                /// <value>The type of the water.</value>
                public int WaterType { get; set; }

                /// <summary>
                /// Gets or sets the index of the water.
                /// </summary>
                /// <value>The index of the water.</value>
                public int WaterIndex { get; set; }

                /// <summary>
                /// Gets or sets the reserved.
                /// </summary>
                /// <value>The reserved.</value>
                public int Reserved { get; set; }

                #endregion
            };

            #region Member Declarations

            /// <summary>
            /// Gets or sets the X.
            /// </summary>
            /// <value>The X.</value>
            public int X { get; set; }

            /// <summary>
            /// Gets or sets the Y.
            /// </summary>
            /// <value>The Y.</value>
            public int Y { get; set; }

            /// <summary>
            /// Gets or sets the water blocks.
            /// </summary>
            /// <value>The water blocks.</value>
            public WaterBlock[,] WaterBlocks { get; set; }

            #endregion
        }

        /// <summary>
        /// Monster Spawn class.
        /// </summary>
        public class MonsterSpawn : BaseIFO
        {
            #region Sub Classes

            /// <summary>
            /// Monster class.
            /// </summary>
            public class Monster
            {
                #region Member Declarations

                /// <summary>
                /// Gets or sets the description.
                /// </summary>
                /// <value>The description.</value>
                public string Description { get; set; }

                /// <summary>
                /// Gets or sets the ID.
                /// </summary>
                /// <value>The ID.</value>
                public int ID { get; set; }

                /// <summary>
                /// Gets or sets the count.
                /// </summary>
                /// <value>The count.</value>
                public int Count { get; set; }

                #endregion
            }

            #endregion

            #region Member Declarations

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the interval.
            /// </summary>
            /// <value>The interval.</value>
            public int Interval { get; set; }

            /// <summary>
            /// Gets or sets the limit.
            /// </summary>
            /// <value>The limit.</value>
            public int Limit { get; set; }

            /// <summary>
            /// Gets or sets the range.
            /// </summary>
            /// <value>The range.</value>
            public int Range { get; set; }
            
            /// <summary>
            /// Gets or sets the tactic points.
            /// </summary>
            /// <value>The tactic points.</value>
            public int TacticPoints { get; set; }

            #endregion

            #region List Declarations

            /// <summary>
            /// Gets or sets the basic.
            /// </summary>
            /// <value>The basic.</value>
            public List<Monster> Basic { get; set; }

            /// <summary>
            /// Gets or sets the tactic.
            /// </summary>
            /// <value>The tactic.</value>
            public List<Monster> Tactic { get; set; }

            #endregion
        }

        /// <summary>
        /// Water Block class.
        /// </summary>
        public class WaterBlock : IFOEntry
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the minimum.
            /// </summary>
            /// <value>The minimum.</value>
            public Vector3 Minimum { get; set; }

            /// <summary>
            /// Gets or sets the maximum.
            /// </summary>
            /// <value>The maximum.</value>
            public Vector3 Maximum { get; set; }

            #endregion
        }

        /// <summary>
        /// Event Trigger class.
        /// </summary>
        public class EventTrigger : BaseIFO
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the QSD trigger.
            /// </summary>
            /// <value>The QSD trigger.</value>
            public string QSDTrigger { get; set; }

            /// <summary>
            /// Gets or sets the LUA trigger.
            /// </summary>
            /// <value>The LUA trigger.</value>
            public string LUATrigger { get; set; }

            #endregion
        }

        #endregion

        /// <summary>
        /// Default Amount of Blocks
        /// </summary>
        public const int BLOCK_COUNT = 13;

        #region Member Declarations

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        #endregion

        #region Data Declarations

        /// <summary>
        /// Gets or sets the map info.
        /// </summary>
        /// <value>The map info.</value>
        public MapInformation MapInfo { get; set; }

        /// <summary>
        /// Gets or sets the decoration.
        /// </summary>
        /// <value>The decoration.</value>
        public List<BaseIFO> Decoration { get; set; }

        /// <summary>
        /// Gets or sets the NPCs.
        /// </summary>
        /// <value>The NPCs.</value>
        public List<NPC> NPCs { get; set; }

        /// <summary>
        /// Gets or sets the construction.
        /// </summary>
        /// <value>The construction.</value>
        public List<BaseIFO> Construction { get; set; }
        
        /// <summary>
        /// Gets or sets the sounds.
        /// </summary>
        /// <value>The sounds.</value>
        public List<Sound> Sounds { get; set; }

        /// <summary>
        /// Gets or sets the effects.
        /// </summary>
        /// <value>The effects.</value>
        public List<Effect> Effects { get; set; }

        /// <summary>
        /// Gets or sets the animation.
        /// </summary>
        /// <value>The animation.</value>
        public List<BaseIFO> Animation { get; set; }

        /// <summary>
        /// Gets or sets the wide water.
        /// </summary>
        /// <value>The wide water.</value>
        public UnusedWater WideWater { get; set; }

        /// <summary>
        /// Gets or sets the monsters.
        /// </summary>
        /// <value>The monsters.</value>
        public List<MonsterSpawn> Monsters { get; set; }

        /// <summary>
        /// Gets or sets the water.
        /// </summary>
        /// <value>The water.</value>
        public List<WaterBlock> Water { get; set; }

        /// <summary>
        /// Gets or sets the warp gates.
        /// </summary>
        /// <value>The warp gates.</value>
        public List<BaseIFO> WarpGates { get; set; }

        /// <summary>
        /// Gets or sets the collision.
        /// </summary>
        /// <value>The collision.</value>
        public List<BaseIFO> Collision { get; set; }

        /// <summary>
        /// Gets or sets the event triggers.
        /// </summary>
        /// <value>The event triggers.</value>
        public List<EventTrigger> EventTriggers { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="IFO"/> class.
        /// </summary>
        public IFO()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IFO"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public IFO(string filePath)
        {
            Load(filePath);
        }

        /// <summary>
        /// Loads the specified file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Load(string filePath)
        {
            FileName = Path.GetFileName(filePath);

            FileHandler fh = new FileHandler(FilePath = filePath, FileHandler.FileOpenMode.Reading, Encoding.GetEncoding("EUC-KR"));

            int blockCount = fh.Read<int>();

            MapInfo = new MapInformation();
            Decoration = new List<BaseIFO>();
            NPCs = new List<NPC>();
            Construction = new List<BaseIFO>();
            Sounds = new List<Sound>();
            Effects = new List<Effect>();
            Animation = new List<BaseIFO>();
            WideWater = new UnusedWater();
            Monsters = new List<MonsterSpawn>();
            Water = new List<WaterBlock>();
            WarpGates = new List<BaseIFO>();
            Collision = new List<BaseIFO>();
            EventTriggers = new List<EventTrigger>();

            for (int i = 0; i < blockCount; i++)
            {
                BlockType blockType = (BlockType)fh.Read<int>();
                int offset = fh.Read<int>();
                int position = fh.Tell();

                fh.Seek(offset, SeekOrigin.Begin);

                switch(blockType)
                {
                    case BlockType.MapInfo:
                        {
                            MapInfo.Width = fh.Read<int>();
                            MapInfo.Height = fh.Read<int>();

                            MapInfo.MapCellX = fh.Read<int>();
                            MapInfo.MapCellY = fh.Read<int>();

                            MapInfo.World = new Matrix()
                            {
                                M11 = fh.Read<float>(),
                                M12 = fh.Read<float>(),
                                M13 = fh.Read<float>(),
                                M14 = fh.Read<float>(),
                                M21 = fh.Read<float>(),
                                M22 = fh.Read<float>(),
                                M23 = fh.Read<float>(),
                                M24 = fh.Read<float>(),
                                M31 = fh.Read<float>(),
                                M32 = fh.Read<float>(),
                                M33 = fh.Read<float>(),
                                M34 = fh.Read<float>(),
                                M41 = fh.Read<float>(),
                                M42 = fh.Read<float>(),
                                M43 = fh.Read<float>(),
                                M44 = fh.Read<float>()
                            };

                            MapInfo.MapName = fh.Read<BString>();

                            MapInfo.Parent = this;

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.Decoration:
                        {
                            int entryCount = fh.Read<int>();
                            Decoration = new List<BaseIFO>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                Decoration.Add(new BaseIFO()
                                {
                                    Description = fh.Read<BString>(),

                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.NPCs:
                        {
                            int entryCount = fh.Read<int>();
                            NPCs = new List<NPC>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                NPCs.Add(new NPC()
                                {
                                    Description = fh.Read<BString>(),
                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    AIPatternIndex = fh.Read<int>(),
                                    Path = fh.Read<BString>(),
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.Construction:
                        {
                            int entryCount = fh.Read<int>();
                            Construction = new List<BaseIFO>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                Construction.Add(new BaseIFO()
                                {
                                    Description = fh.Read<BString>(),
                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.Sounds:
                        {
                            int entryCount = fh.Read<int>();
                            Sounds = new List<Sound>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                Sounds.Add(new Sound()
                                {
                                    Description = fh.Read<BString>(),
                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    Path = fh.Read<BString>(),
                                    Range = fh.Read<int>(),
                                    Interval = fh.Read<int>(),
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.Effects:
                        {
                            int entryCount = fh.Read<int>();
                            Effects = new List<Effect>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                Effects.Add(new Effect()
                                {
                                    Description = fh.Read<BString>(),
                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    Path = fh.Read<BString>(),
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.Animation:
                        {
                            int entryCount = fh.Read<int>();
                            Animation = new List<BaseIFO>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                Animation.Add(new BaseIFO()
                                {
                                    Description = fh.Read<BString>(),
                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.WideWater:
                        {
                            WideWater.X = fh.Read<int>();
                            WideWater.Y = fh.Read<int>();

                            WideWater.WaterBlocks = new UnusedWater.WaterBlock[WideWater.X, WideWater.X];

                            for (int j = 0; j < WideWater.X; j++)
                            {
                                for (int k = 0; k < WideWater.Y; k++)
                                {
                                    WideWater.WaterBlocks[j, k] = new UnusedWater.WaterBlock()
                                    {
                                        Use = fh.Read<byte>(),
                                        Height = fh.Read<float>(),
                                        WaterType = fh.Read<int>(),
                                        WaterIndex = fh.Read<int>(),
                                        Reserved = fh.Read<int>()
                                    };
                                }
                            }

                            WideWater.Parent = this;

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.Monsters:
                        {
                            int entryCount = fh.Read<int>();
                            Monsters = new List<MonsterSpawn>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                Monsters.Add(new MonsterSpawn()
                                {
                                    Description = fh.Read<BString>(),
                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    Name = fh.Read<BString>(),
                                    Parent = this
                                });

                                int basicCount = fh.Read<int>();

                                Monsters[j].Basic = new List<MonsterSpawn.Monster>(basicCount);

                                for (int k = 0; k < basicCount; k++)
                                {
                                    Monsters[j].Basic.Add(new MonsterSpawn.Monster()
                                    {
                                        Description = fh.Read<BString>(),
                                        ID = fh.Read<int>(),
                                        Count = fh.Read<int>(),
                                    });
                                }

                                int tacticCount = fh.Read<int>();

                                Monsters[j].Tactic = new List<MonsterSpawn.Monster>(tacticCount);

                                for (int k = 0; k < tacticCount; k++)
                                {
                                    Monsters[j].Tactic.Add(new MonsterSpawn.Monster()
                                    {
                                        Description = fh.Read<BString>(),
                                        ID = fh.Read<int>(),
                                        Count = fh.Read<int>(),
                                    });
                                }

                                Monsters[j].Interval = fh.Read<int>();
                                Monsters[j].Limit = fh.Read<int>();
                                Monsters[j].Range = fh.Read<int>();
                                Monsters[j].TacticPoints = fh.Read<int>();

                                Monsters[j].Parent = this;
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.Water:
                        {
                            fh.Read<float>();

                            int entryCount = fh.Read<int>();
                            Water = new List<WaterBlock>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                Water.Add(new WaterBlock()
                                {
                                    Minimum = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.00f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f,
                                        Y = (fh.Read<float>() + 520000.00f) / 100.0f
                                    },
                                    Maximum = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.00f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f,
                                        Y = (fh.Read<float>() + 520000.00f) / 100.0f
                                    },
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.WarpGates:
                        {
                            int entryCount = fh.Read<int>();
                            WarpGates = new List<BaseIFO>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                WarpGates.Add(new BaseIFO()
                                {
                                    Description = fh.Read<BString>(),
                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                        break;
                    case BlockType.Collision:
                        {
                            int entryCount = fh.Read<int>();
                            Collision = new List<BaseIFO>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                Collision.Add(new BaseIFO()
                                {
                                    Description = fh.Read<BString>(),
                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                        }
                     break;
                    case BlockType.EventTriggers:
                        {
                            int entryCount = fh.Read<int>();
                            EventTriggers = new List<EventTrigger>(entryCount);

                            for (int j = 0; j < entryCount; j++)
                            {
                                EventTriggers.Add(new EventTrigger()
                                {
                                    Description = fh.Read<BString>(),
                                    WarpID = fh.Read<short>(),
                                    EventID = fh.Read<short>(),
                                    ObjectType = (ObjectType)fh.Read<int>(),
                                    ObjectID = fh.Read<int>(),
                                    MapPosition = new Vector2()
                                    {
                                        X = fh.Read<int>(),
                                        Y = fh.Read<int>()
                                    },
                                    Rotation = fh.Read<Quaternion>(),
                                    Position = new Vector3()
                                    {
                                        X = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Y = (fh.Read<float>() + 520000.0f) / 100.0f,
                                        Z = fh.Read<float>() / 100.0f
                                    },
                                    Scale = fh.Read<Vector3>(),
                                    QSDTrigger = fh.Read<BString>(),
                                    LUATrigger = fh.Read<BString>(),
                                    Parent = this
                                });
                            }

                            fh.Seek(position, SeekOrigin.Begin);
                     }
                     break;
                }
            }

            fh.Close();
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        public void Save()
        {
            Save(FilePath);
        }

        /// <summary>
        /// Saves the specified file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Save(string filePath)
        {
            FileName = Path.GetFileName(filePath);

            FileHandler fh = new FileHandler(FilePath = filePath, FileHandler.FileOpenMode.Writing, Encoding.GetEncoding("EUC-KR"));

            fh.Write<int>(BLOCK_COUNT);
            fh.Write<byte[]>(new byte[BLOCK_COUNT * (sizeof(int) * 2)]);

            int[] offsets = new int[BLOCK_COUNT];

            offsets[0] = fh.Tell();

            fh.Write<int>(MapInfo.Width);
            fh.Write<int>(MapInfo.Height);

            fh.Write<int>(MapInfo.MapCellX);
            fh.Write<int>(MapInfo.MapCellY);

            fh.Write<float>(MapInfo.World.M11);
            fh.Write<float>(MapInfo.World.M12);
            fh.Write<float>(MapInfo.World.M13);
            fh.Write<float>(MapInfo.World.M14);
            fh.Write<float>(MapInfo.World.M21);
            fh.Write<float>(MapInfo.World.M22);
            fh.Write<float>(MapInfo.World.M23);
            fh.Write<float>(MapInfo.World.M24);
            fh.Write<float>(MapInfo.World.M31);
            fh.Write<float>(MapInfo.World.M32);
            fh.Write<float>(MapInfo.World.M33);
            fh.Write<float>(MapInfo.World.M34);
            fh.Write<float>(MapInfo.World.M41);
            fh.Write<float>(MapInfo.World.M42);
            fh.Write<float>(MapInfo.World.M43);
            fh.Write<float>(MapInfo.World.M44);

            fh.Write<BString>(MapInfo.MapName);

            offsets[1] = fh.Tell();

            fh.Write<int>(Decoration.Count);

            Decoration.ForEach(delegate(BaseIFO ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
            });

            offsets[2] = fh.Tell();

            fh.Write<int>(NPCs.Count);

            NPCs.ForEach(delegate(NPC ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
                fh.Write<int>(ifoObject.AIPatternIndex);
                fh.Write<BString>(ifoObject.Path);
            });

            offsets[3] = fh.Tell();

            fh.Write<int>(Construction.Count);

            Construction.ForEach(delegate(BaseIFO ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
            });

            offsets[4] = fh.Tell();

            fh.Write<int>(Sounds.Count);

            Sounds.ForEach(delegate(Sound ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
                fh.Write<BString>(ifoObject.Path);
                fh.Write<int>(ifoObject.Range);
                fh.Write<int>(ifoObject.Interval);
            });

            offsets[5] = fh.Tell();

            fh.Write<int>(Effects.Count);

            Effects.ForEach(delegate(Effect ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
                fh.Write<BString>(ifoObject.Path);
            });

            offsets[6] = fh.Tell();

            fh.Write<int>(Animation.Count);

            Animation.ForEach(delegate(BaseIFO ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
            });

            offsets[7] = fh.Tell();

            fh.Write<int>(WideWater.X);
            fh.Write<int>(WideWater.Y);

            for (int x = 0; x < WideWater.X; x++)
            {
                for (int y = 0; y < WideWater.Y; y++)
                {
                    fh.Write<byte>(WideWater.WaterBlocks[x, y].Use);
                    fh.Write<float>(WideWater.WaterBlocks[x, y].Height);
                    fh.Write<int>(WideWater.WaterBlocks[x, y].WaterType);
                    fh.Write<int>(WideWater.WaterBlocks[x, y].WaterIndex);
                    fh.Write<int>(WideWater.WaterBlocks[x, y].Reserved);
                }
            }

            offsets[8] = fh.Tell();

            fh.Write<int>(Monsters.Count);

            Monsters.ForEach(delegate(MonsterSpawn ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
                fh.Write<BString>(ifoObject.Name);

                fh.Write<int>(ifoObject.Basic.Count);

                ifoObject.Basic.ForEach(delegate(MonsterSpawn.Monster monster)
                {
                    fh.Write<BString>(monster.Description);
                    fh.Write<int>(monster.ID);
                    fh.Write<int>(monster.Count);
                });

                fh.Write<int>(ifoObject.Tactic.Count);

                ifoObject.Tactic.ForEach(delegate(MonsterSpawn.Monster monster)
                {
                    fh.Write<BString>(monster.Description);
                    fh.Write<int>(monster.ID);
                    fh.Write<int>(monster.Count);
                });

                fh.Write<int>(ifoObject.Interval);
                fh.Write<int>(ifoObject.Limit);
                fh.Write<int>(ifoObject.Range);
                fh.Write<int>(ifoObject.TacticPoints);
            });

            offsets[9] = fh.Tell();

            fh.Write<float>(2000.0f);
            fh.Write<int>(Water.Count);

            Water.ForEach(delegate(WaterBlock ifoObject)
            {
                fh.Write<float>(-520000.0f + (ifoObject.Minimum.X * 100.0f));
                fh.Write<float>(ifoObject.Minimum.Z * 100.0f);
                fh.Write<float>(-520000.0f + (ifoObject.Minimum.Y * 100.0f));

                fh.Write<float>(-520000.0f + (ifoObject.Maximum.X * 100.0f));
                fh.Write<float>(ifoObject.Maximum.Z * 100.0f);
                fh.Write<float>(-520000.0f + (ifoObject.Maximum.Y * 100.0f));
            });

            offsets[10] = fh.Tell();

            fh.Write<int>(WarpGates.Count);

            WarpGates.ForEach(delegate(BaseIFO ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
            });

            offsets[11] = fh.Tell();

            fh.Write<int>(Collision.Count);

            Collision.ForEach(delegate(BaseIFO ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
            });

            offsets[12] = fh.Tell();

            fh.Write<int>(EventTriggers.Count);

            EventTriggers.ForEach(delegate(EventTrigger ifoObject)
            {
                fh.Write<BString>(ifoObject.Description);
                fh.Write<short>(ifoObject.WarpID);
                fh.Write<short>(ifoObject.EventID);
                fh.Write<int>((int)ifoObject.ObjectType);
                fh.Write<int>(ifoObject.ObjectID);
                fh.Write<int>((int)ifoObject.MapPosition.X);
                fh.Write<int>((int)ifoObject.MapPosition.Y);
                fh.Write<Quaternion>(ifoObject.Rotation);
                fh.Write<float>(-520000.0f + (ifoObject.Position.X * 100.0f));
                fh.Write<float>(-520000.0f + (ifoObject.Position.Y * 100.0f));
                fh.Write<float>(ifoObject.Position.Z * 100.0f);
                fh.Write<Vector3>(ifoObject.Scale);
                fh.Write<BString>(ifoObject.QSDTrigger);
                fh.Write<BString>(ifoObject.LUATrigger);
            });

            fh.Seek(4, SeekOrigin.Begin);

            for (int i = 0; i < BLOCK_COUNT; i++)
            {
                fh.Write<int>(i);
                fh.Write<int>(offsets[i]);
            }

            fh.Close();
        }
    }
}