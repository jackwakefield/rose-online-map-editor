using System.Collections.Generic;
using Map_Editor.Engine.Map;
using Map_Editor.Engine.Models;
using Map_Editor.Engine.RenderManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Map_Editor.Misc;

namespace Map_Editor.Engine.Objects
{
    /// <summary>
    /// Object class.
    /// </summary>
    public class Object : DrawableGameComponent
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public enum ObjectType
        {
            /// <summary>
            /// Decoration.
            /// </summary>
            Decoration,

            /// <summary>
            /// Construction.
            /// </summary>
            Construction
        }

        #region Sub Classes

        /// <summary>
        /// WorldObject class.
        /// </summary>
        public class WorldObject
        {
            #region Sub Classes

            /// <summary>
            /// Part class.
            /// </summary>
            public class Part
            {
                #region Member Declarations

                /// <summary>
                /// Gets or sets the model ID.
                /// </summary>
                /// <value>The model ID.</value>
                public int ModelID { get; set; }

                /// <summary>
                /// Gets or sets the texture ID.
                /// </summary>
                /// <value>The texture ID.</value>
                public int TextureID { get; set; }

                /// <summary>
                /// Gets or sets the file ID.
                /// </summary>
                /// <value>The file ID.</value>
                public int FileID { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether [lightmap enabled].
                /// </summary>
                /// <value><c>true</c> if [lightmap enabled]; otherwise, <c>false</c>.</value>
                public bool LightmapEnabled { get; set; }

                /// <summary>
                /// Gets or sets the lightmap manipulation.
                /// </summary>
                /// <value>The lightmap manipulation.</value>
                public Vector4 LightmapManipulation { get; set; }

                /// <summary>
                /// Gets or sets the lightmap texture.
                /// </summary>
                /// <value>The lightmap texture.</value>
                public Texture2D LightmapTexture { get; set; }

                /// <summary>
                /// Gets or sets the bounding box.
                /// </summary>
                /// <value>The bounding box.</value>
                public BoundingBox BoundingBox { get; set; }

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

                /// <summary>
                /// Gets or sets the rotation.
                /// </summary>
                /// <value>The rotation.</value>
                public Quaternion Rotation { get; set; }

                /// <summary>
                /// Gets or sets the batch object.
                /// </summary>
                /// <value>The batch object.</value>
                private ObjectManager.BatchObject batchObject { get; set; }

                #endregion

                /// <summary>
                /// Adds the object to the specified object manager.
                /// </summary>
                /// <param name="objectManager">The object manager.</param>
                /// <param name="world">The world.</param>
                /// <param name="decorationID">The decoration ID.</param>
                public void Add(ObjectManager objectManager, Matrix world, int decorationID)
                {
                    if (LightmapEnabled)
                    {
                        batchObject = objectManager.AddToBatch(ModelID, TextureID, world, Position, Scale, Rotation, BoundingBox, new ObjectManager.EffectParameter[]
                        {
                            new ObjectManager.EffectParameter()
                            {
                                Type = ObjectManager.KeyType.Bool,
                                Key = "LightmapEnabled",
                                Value = true
                            },
                            new ObjectManager.EffectParameter()
                            {
                                Type = ObjectManager.KeyType.Texture,
                                Key = "Lightmap",
                                Value = LightmapTexture
                            },                            
                            new ObjectManager.EffectParameter()
                            {
                                Type = ObjectManager.KeyType.Vector2,
                                Key = "TextureAdd",
                                Value = new Vector2(LightmapManipulation.X, LightmapManipulation.Y)
                            },
                            new ObjectManager.EffectParameter()
                            {
                                Type = ObjectManager.KeyType.Vector2,
                                Key = "TextureMultiply",
                                Value = new Vector2(LightmapManipulation.Z, LightmapManipulation.W)
                            }
                        },
                        decorationID);
                    }
                    else
                    {
                        batchObject = objectManager.AddToBatch(ModelID, TextureID, world, Position, Scale, Rotation, BoundingBox, new ObjectManager.EffectParameter[]
                        {
                            new ObjectManager.EffectParameter()
                            {
                                Type = ObjectManager.KeyType.Bool,
                                Key = "LightmapEnabled",
                                Value = false
                            }
                        },
                        decorationID);
                    }
                }

                /// <summary>
                /// Removes the object from the specified object manager.
                /// </summary>
                /// <param name="objectManager">The object manager.</param>
                public void Remove(ObjectManager objectManager)
                {
                    objectManager.RemoveFromBatch(batchObject);
                }
            }

            #endregion

            #region Member Declarations

            /// <summary>
            /// Gets or sets the file ID.
            /// </summary>
            /// <value>The file ID.</value>
            public int FileID { get; set; }

            /// <summary>
            /// Gets or sets the entry ID.
            /// </summary>
            /// <value>The entry ID.</value>
            public int EntryID { get; set; }

            /// <summary>
            /// Gets or sets the entry.
            /// </summary>
            /// <value>The entry.</value>
            public IFO.BaseIFO Entry { get; set; }

            /// <summary>
            /// Gets or sets the world.
            /// </summary>
            /// <value>The world.</value>
            public Matrix World { get; set; }

            /// <summary>
            /// Gets or sets the bounding box.
            /// </summary>
            /// <value>The bounding box.</value>
            public BoundingBox BoundingBox { get; set; }

            #endregion

            #region List Declarations

            /// <summary>
            /// Gets or sets the parts.
            /// </summary>
            /// <value>The parts.</value>
            public List<Part> Parts { get; set; }

            #endregion

            /// <summary>
            /// Adds all objects to the specified object manager.
            /// </summary>
            /// <param name="objectManager">The object manager.</param>
            /// <param name="decorationID">The decoration ID.</param>
            public void Add(ObjectManager objectManager, int decorationID)
            {
                Parts.ForEach(delegate(Part part)
                {
                    part.Add(objectManager, World, decorationID);
                });
            }

            /// <summary>
            /// Removes all objects from the specified object manager.
            /// </summary>
            /// <param name="objectManager">The object manager.</param>
            public void Remove(ObjectManager objectManager)
            {
                Parts.ForEach(delegate(Part part)
                {
                    part.Remove(objectManager);
                });
            }
        }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the object manager.
        /// </summary>
        /// <value>The object manager.</value>
        public ObjectManager ObjectManager { get; set; }

        /// <summary>
        /// Gets or sets the texture manager.
        /// </summary>
        /// <value>The texture manager.</value>
        public TextureManager TextureManager { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public ObjectType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Object"/> is loading.
        /// </summary>
        /// <value><c>true</c> if loading; otherwise, <c>false</c>.</value>
        public bool Loading { get; set; }

        #endregion

        #region List Declarations

        /// <summary>
        /// Gets or sets the world objects.
        /// </summary>
        /// <value>The world objects.</value>
        public List<WorldObject> WorldObjects { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Object"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="drawOrder">The draw order.</param>
        public Object(Game game, int drawOrder)
            : base(game)
        {
            WorldObjects = new List<WorldObject>();

            ObjectManager = new ObjectManager(game.GraphicsDevice, ZMS.Vertex.VertexElements, ZMS.Vertex.SIZE_IN_BYTES);
            TextureManager = new TextureManager(game.GraphicsDevice);

            DrawOrder = drawOrder;
        }

        /// <summary>
        /// Adds the specified ifo object.
        /// </summary>
        /// <param name="ifoObject">The ifo object.</param>
        /// <param name="fileID">The file ID.</param>
        /// <param name="entryID">The entry ID.</param>
        public void Add(IFO.BaseIFO ifoObject, int fileID, int entryID)
        {
            WorldObjects.Add(new WorldObject());

            Add(WorldObjects.Count - 1, ifoObject, fileID, entryID, false);
        }

        /// <summary>
        /// Adds the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="ifoObject">The ifo object.</param>
        /// <param name="fileID">The file ID.</param>
        /// <param name="entryID">The entry ID.</param>
        /// <param name="removePrevious">if set to <c>true</c> [remove previous].</param>
        public void Add(int id, IFO.BaseIFO ifoObject, int fileID, int entryID, bool removePrevious)
        {
            ZSC.Object zscObject = FileManager.ZSCs[Type.ToString()].Objects[ifoObject.ObjectID];

            Matrix objectWorld = Matrix.CreateFromQuaternion(ifoObject.Rotation) *
                                 Matrix.CreateScale(ifoObject.Scale) *
                                 Matrix.CreateTranslation(ifoObject.Position);

            WorldObject newObject = new WorldObject()
            {
                Entry = ifoObject,
                FileID = fileID,
                EntryID = entryID,
                World = objectWorld,
                Parts = new List<WorldObject.Part>(zscObject.Models.Count),
            };

            LIT blockLIT = (Type == ObjectType.Decoration) ? FileManager.DecorationLITs[fileID] : FileManager.ConstructionLITs[fileID];
            int objectID = blockLIT.SearchObject(entryID + 1);

            for (int i = 0; i < zscObject.Models.Count; i++)
            {
                int modelID = -1;

                if (zscObject.Models[i].Motion != null)
                    modelID = ObjectManager.Add(FileManager.ZSCs[Type.ToString()].Models[zscObject.Models[i].ModelID], zscObject.Models[i].Motion);
                else
                    modelID = ObjectManager.Add(FileManager.ZSCs[Type.ToString()].Models[zscObject.Models[i].ModelID]);

                if (modelID == -1)
                    return;

                Vector2 textureAdd = Vector2.Zero;
                Vector2 textureMultiply = Vector2.One;

                bool lightmapEnabled = false;

                int ddsFileID = -1;

                string lightmapFilePath = string.Empty;

                if (objectID > -1)
                {
                    try
                    {
                        int litObjectID = blockLIT.SearchObject(entryID + 1);

                        LIT.Object lightmapObject = blockLIT.Objects[litObjectID];

                        int litPartID = blockLIT.SearchPart(litObjectID, i);

                        int objectPerWidth = lightmapObject.Parts[litPartID].ObjectsPerWidth;
                        int mapPosition = lightmapObject.Parts[litPartID].MapPosition;
                        ddsFileID = lightmapObject.Parts[litPartID].LightmapID;

                        textureAdd = new Vector2(mapPosition % objectPerWidth, mapPosition / objectPerWidth);
                        textureMultiply = new Vector2(1.0f / objectPerWidth, 1.0f / objectPerWidth);

                        lightmapFilePath = string.Format(@"{0}\{1}", blockLIT.Folder, lightmapObject.Parts[litPartID].DDSName);

                        lightmapEnabled = true;
                    }
                    catch
                    {
                        textureMultiply = Vector2.One;
                        textureAdd = Vector2.Zero;
                    }
                }

                Matrix world = Matrix.CreateFromQuaternion(zscObject.Models[i].Rotation) *
                               Matrix.CreateScale(zscObject.Models[i].Scale) *
                               Matrix.CreateTranslation(zscObject.Models[i].Position);

                newObject.Parts.Add(new WorldObject.Part()
                {
                    TextureID = TextureManager.Add(FileManager.ZSCs[Type.ToString()].Textures[zscObject.Models[i].TextureID]),
                    ModelID = modelID,
                    LightmapManipulation = new Vector4(textureAdd.X, textureAdd.Y, textureMultiply.X, textureMultiply.Y),
                    LightmapTexture = (lightmapFilePath == string.Empty) ? null : Texture2D.FromFile(Game.GraphicsDevice, lightmapFilePath),
                    Position = zscObject.Models[i].Position,
                    Scale = zscObject.Models[i].Scale,
                    Rotation = zscObject.Models[i].Rotation,
                    LightmapEnabled = lightmapEnabled,
                    BoundingBox = ObjectManager.CreateBox(modelID, world * objectWorld)
                });

                if (i == 0)
                    newObject.BoundingBox = newObject.Parts[i].BoundingBox;
                else
                    newObject.BoundingBox = BoundingBox.CreateMerged(newObject.BoundingBox, newObject.Parts[i].BoundingBox);
            }

            if (removePrevious)
                WorldObjects[id].Remove(ObjectManager);

            WorldObjects[id] = newObject;
            WorldObjects[id].Add(ObjectManager, id);

            if (removePrevious)
            {
                for (int i = 0; i < UndoManager.Commands.Length; i++)
                {
                    if (UndoManager.Commands[i] == null)
                        continue;

                    switch (Type)
                    {
                        case ObjectType.Decoration:
                            {
                                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.ValueChanged) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.Positioned) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.Added) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.Removed) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.ObjectChanged) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Decoration.LightmapRemoved)) && ((Commands.Decoration.Decoration)UndoManager.Commands[i]).ObjectID == id)
                                    ((Commands.Decoration.Decoration)UndoManager.Commands[i]).Object = newObject;
                            }
                            break;
                        case ObjectType.Construction:
                            {
                                if ((UndoManager.Commands[i].GetType() == typeof(Commands.Construction.ValueChanged) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Construction.Positioned) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Construction.Added) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Construction.Removed) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Construction.Removed) ||
                                     UndoManager.Commands[i].GetType() == typeof(Commands.Construction.LightmapRemoved)) && ((Commands.Construction.Construction)UndoManager.Commands[i]).ObjectID == id)
                                    ((Commands.Construction.Construction)UndoManager.Commands[i]).Object = newObject;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="undoAction">if set to <c>true</c> [undo action].</param>
        public void RemoveAt(int id, bool undoAction)
        {
            switch (Type)
            {
                case ObjectType.Decoration:
                    {
                        if (!undoAction)
                        {
                            UndoManager.AddCommand(new Commands.Decoration.Removed()
                            {
                                ObjectID = id,
                                Object = WorldObjects[id],
                                File = WorldObjects[id].FileID,
                                Entry = WorldObjects[id].Entry
                            });
                        }

                        WorldObjects[id].Entry.Parent.Decoration.Remove(WorldObjects[id].Entry);
                    }
                    break;
                case ObjectType.Construction:
                    {
                        if (!undoAction)
                        {
                            UndoManager.AddCommand(new Commands.Construction.Removed()
                            {
                                ObjectID = id,
                                Object = WorldObjects[id],
                                File = WorldObjects[id].FileID,
                                Entry = WorldObjects[id].Entry
                            });
                        }

                        WorldObjects[id].Entry.Parent.Construction.Remove(WorldObjects[id].Entry);
                    }
                    break;
            }

            WorldObjects[id].Remove(ObjectManager);
            WorldObjects.RemoveAt(id);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            WorldObjects.Clear();
            ObjectManager.Clear();
            TextureManager.Clear();
        }

        /// <summary>
        /// Updates the objects.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(GameTime gameTime)
        {
            if (Loading)
                return;

            ObjectManager.Update(gameTime);
        }

        /// <summary>
        /// Draws the objects.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || (Type == ObjectType.Decoration && !ConfigurationManager.GetValue<bool>("Draw", "Decoration")) || (Type == ObjectType.Construction && !ConfigurationManager.GetValue<bool>("Draw", "Construction")))
                return;

            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.Object);

            shader.SetValue("MasterLightmapEnabled", true);

            shader.Start("Object");

            ObjectManager.Draw(TextureManager, shader, true);

            shader.Finish();
        }
    }
}