using System.Collections.Generic;
using Map_Editor.Engine.Models;
using Map_Editor.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Map_Editor.Engine.RenderManager
{
    /// <summary>
    /// ObjectManager class.
    /// </summary>
    public class ObjectManager
    {
        /// <summary>
        /// Key type.
        /// </summary>
        public enum KeyType
        {
            /// <summary>
            /// Float.
            /// </summary>
            Float,

            /// <summary>
            /// Vector2.
            /// </summary>
            Vector2,

            /// <summary>
            /// Vector3.
            /// </summary>
            Vector3,

            /// <summary>
            /// Vector4.
            /// </summary>
            Vector4,

            /// <summary>
            /// Quaternion.
            /// </summary>
            Quaternion,

            /// <summary>
            /// Int.
            /// </summary>
            Int,

            /// <summary>
            /// Texture.
            /// </summary>
            Texture,

            /// <summary>
            /// Bool.
            /// </summary>
            Bool
        }

        #region Sub Classes

        /// <summary>
        /// Object class.
        /// </summary>
        public class Object
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the vertex buffer.
            /// </summary>
            /// <value>The vertex buffer.</value>
            public VertexBuffer VertexBuffer { get; set; }

            /// <summary>
            /// Gets or sets the index buffer.
            /// </summary>
            /// <value>The index buffer.</value>
            public IndexBuffer IndexBuffer { get; set; }

            /// <summary>
            /// Gets or sets the vertex count.
            /// </summary>
            /// <value>The vertex count.</value>
            public int VertexCount { get; set; }

            /// <summary>
            /// Gets or sets the index count.
            /// </summary>
            /// <value>The index count.</value>
            public int IndexCount { get; set; }

            /// <summary>
            /// Gets or sets the vertices.
            /// </summary>
            /// <value>The vertices.</value>
            public ZMS.Vertex[] Vertices { get; set; }

            /// <summary>
            /// Gets or sets the frame.
            /// </summary>
            /// <value>The frame.</value>
            public int Frame { get; set; }

            /// <summary>
            /// Gets or sets the motion.
            /// </summary>
            /// <value>The motion.</value>
            public ZMO Motion { get; set; }

            /// <summary>
            /// Gets or sets the motion position.
            /// </summary>
            /// <value>The motion position.</value>
            public Vector3? MotionPosition { get; set; }

            /// <summary>
            /// Gets or sets the motion scale.
            /// </summary>
            /// <value>The motion scale.</value>
            public float? MotionScale { get; set; }

            /// <summary>
            /// Gets or sets the motion rotation.
            /// </summary>
            /// <value>The motion rotation.</value>
            public Quaternion? MotionRotation { get; set; }

            /// <summary>
            /// Gets or sets the file path.
            /// </summary>
            /// <value>The file path.</value>
            public string FilePath { get; set; }

            /// <summary>
            /// Gets or sets the last update.
            /// </summary>
            /// <value>The last update.</value>
            public double LastUpdate { get; set; }

            #endregion
        }

        #endregion

        /// <summary>
        /// BatchObject structure.
        /// </summary>
        public struct BatchObject
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the object ID.
            /// </summary>
            /// <value>The object ID.</value>
            public int ObjectID { get; set; }

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
            /// Gets or sets the bounding box.
            /// </summary>
            /// <value>The bounding box.</value>
            public BoundingBox BoundingBox { get; set; }

            /// <summary>
            /// Gets or sets the world.
            /// </summary>
            /// <value>The world.</value>
            public Matrix World { get; set; }

            /// <summary>
            /// Gets or sets the effect parameters.
            /// </summary>
            /// <value>The effect parameters.</value>
            public EffectParameter[] EffectParameters { get; set; }

            #endregion
        };

        /// <summary>
        /// Effect Parameter structure.
        /// </summary>
        public struct EffectParameter
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            /// <value>The key.</value>
            public string Key { get; set; }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>The type.</value>
            public KeyType Type { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public object Value { get; set; }

            #endregion
        };

        #region Member Declarations

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDevice device { get; set; }

        /// <summary>
        /// Gets or sets the vertex declaration.
        /// </summary>
        /// <value>The vertex declaration.</value>
        private VertexDeclaration vertexDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the size in bytes.
        /// </summary>
        /// <value>The size in bytes.</value>
        private int sizeInBytes { get; set; }

        #endregion

        #region List Declarations

        /// <summary>
        /// Gets or sets the object list.
        /// </summary>
        /// <value>The object list.</value>
        private List<Object> objectList { get; set; }

        /// <summary>
        /// Gets or sets the batch objects.
        /// </summary>
        /// <value>The batch objects.</value>
        private List<BatchObject> batchObjects { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectManager"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="vertexElements">The vertex elements.</param>
        /// <param name="sizeInBytes">The size in bytes.</param>
        public ObjectManager(GraphicsDevice device, VertexElement[] vertexElements, int sizeInBytes)
        {
            objectList = new List<Object>();
            batchObjects = new List<BatchObject>();

            this.device = device;

            vertexDeclaration = new VertexDeclaration(device, vertexElements);
            this.sizeInBytes = sizeInBytes;
        }

        /// <summary>
        /// Adds the specified model path.
        /// </summary>
        /// <param name="modelPath">The model path.</param>
        /// <returns></returns>
        public int Add(string modelPath)
        {
            return Add(modelPath, null);
        }

        /// <summary>
        /// Adds the specified model path.
        /// </summary>
        /// <param name="modelPath">The model path.</param>
        /// <param name="animationPath">The animation path.</param>
        /// <returns></returns>
        public int Add(string modelPath, string animationPath)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                if (string.Compare(objectList[i].FilePath, modelPath, true) == 0)
                    return i;
            }

            ZMS newModel = new ZMS(modelPath);

            if (newModel.VertexCount == 0)
                return -1;

            ZMO newAnimation = null;

            if (animationPath != null)
                newAnimation = new ZMO(animationPath, false, true);

            try
            {
                objectList.Add(new Object()
                {
                    FilePath = modelPath,
                    VertexBuffer = newModel.CreateVertexBuffer(device),
                    IndexBuffer = newModel.CreateIndexBuffer(device),
                    VertexCount = newModel.VertexCount,
                    IndexCount = newModel.IndexCount,
                    Vertices = newModel.Vertices,
                    Motion = newAnimation
                });
            }
            catch
            {
                Output.WriteLine(Output.MessageType.Error, string.Format("Unable to Load Model {0}", modelPath));
            }

            return objectList.Count - 1;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="id">The id.</param>
        public void RemoveAt(int id)
        {
            objectList.RemoveAt(id);
        }

        /// <summary>
        /// Removes by the object ID.
        /// </summary>
        /// <param name="id">The id.</param>
        public void RemoveByObjectID(int id)
        {
            batchObjects.ForEach(delegate(BatchObject batchObject)
            {
                if (batchObject.ObjectID == id)
                    batchObjects.Remove(batchObject);
            });
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            objectList.Clear();

            batchObjects.ForEach(delegate(BatchObject batchObject)
            {
                for (int i = 0; i < batchObject.EffectParameters.Length; i++)
                {
                    if (batchObject.EffectParameters[i].Type == KeyType.Texture)
                        ((Texture2D)batchObject.EffectParameters[i].Value).Dispose();
                }
            });

            batchObjects.Clear();
        }

        /// <summary>
        /// Clears the batch.
        /// </summary>
        public void ClearBatch()
        {
            batchObjects.Clear();
        }

        /// <summary>
        /// Creates the bounding box.
        /// </summary>
        /// <param name="modelID">The model ID.</param>
        /// <param name="world">The world.</param>
        /// <returns>The bounding box.</returns>
        public BoundingBox CreateBox(int modelID, Matrix world)
        {
            Vector3[] vertices = new Vector3[objectList[modelID].Vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = Vector3.Transform(objectList[modelID].Vertices[i].Position, world);

            return BoundingBox.CreateFromPoints(vertices);
        }

        /// <summary>
        /// Adds to batch.
        /// </summary>
        /// <param name="modelID">The model ID.</param>
        /// <param name="textureID">The texture ID.</param>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="boundingBox">The bounding box.</param>
        /// <returns>The batch object.</returns>
        public BatchObject AddToBatch(int modelID, int textureID, Matrix world, Vector3 position, Vector3 scale, Quaternion rotation, BoundingBox boundingBox)
        {
            return AddToBatch(modelID, textureID, world, position, scale, rotation, boundingBox, new EffectParameter[0]);
        }

        /// <summary>
        /// Adds to batch.
        /// </summary>
        /// <param name="modelID">The model ID.</param>
        /// <param name="textureID">The texture ID.</param>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="boundingBox">The bounding box.</param>
        /// <param name="effectParamters">The effect paramters.</param>
        /// <returns>The batch object.</returns>
        public BatchObject AddToBatch(int modelID, int textureID, Matrix world, Vector3 position, Vector3 scale, Quaternion rotation, BoundingBox boundingBox, EffectParameter[] effectParamters)
        {
            BatchObject batchObject = new BatchObject()
            {
                ModelID = modelID,
                TextureID = textureID,
                World = world,
                Position = position,
                Scale = scale,
                Rotation = rotation,
                BoundingBox = boundingBox,
                EffectParameters = effectParamters
            };

            batchObjects.Add(batchObject);

            return batchObject;
        }

        /// <summary>
        /// Adds to batch.
        /// </summary>
        /// <param name="modelID">The model ID.</param>
        /// <param name="textureID">The texture ID.</param>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="boundingBox">The bounding box.</param>
        /// <param name="objectID">The object ID.</param>
        /// <returns>The batch object.</returns>
        public BatchObject AddToBatch(int modelID, int textureID, Matrix world, Vector3 position, Vector3 scale, Quaternion rotation, BoundingBox boundingBox, int objectID)
        {
            return AddToBatch(modelID, textureID, world, position, scale, rotation, boundingBox, new EffectParameter[0], objectID);
        }

        /// <summary>
        /// Adds to batch.
        /// </summary>
        /// <param name="modelID">The model ID.</param>
        /// <param name="textureID">The texture ID.</param>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="boundingBox">The bounding box.</param>
        /// <param name="effectParamters">The effect paramters.</param>
        /// <param name="objectID">The object ID.</param>
        /// <returns>The batch object.</returns>
        public BatchObject AddToBatch(int modelID, int textureID, Matrix world, Vector3 position, Vector3 scale, Quaternion rotation, BoundingBox boundingBox, EffectParameter[] effectParamters, int objectID)
        {
            BatchObject batchObject = new BatchObject()
            {
                ModelID = modelID,
                TextureID = textureID,
                World = world,
                Position = position,
                Scale = scale,
                Rotation = rotation,
                BoundingBox = boundingBox,
                EffectParameters = effectParamters,
                ObjectID = objectID
            };

            batchObjects.Add(batchObject);

            return batchObject;
        }

        /// <summary>
        /// Removes from the batch.
        /// </summary>
        /// <param name="batchObject">The batch object.</param>
        public void RemoveFromBatch(BatchObject batchObject)
        {
            batchObjects.Remove(batchObject);
        }

        /// <summary>
        /// Sorts this instance.
        /// </summary>
        public void Sort()
        {
            batchObjects.Sort(SortByModelID);
        }

        /// <summary>
        /// Updates the objects.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                if (objectList[i].Motion == null)
                    continue;

                if (gameTime.TotalGameTime.TotalMilliseconds - objectList[i].LastUpdate <= 1000.0f / objectList[i].Motion.FPS)
                    continue;

                objectList[i].Frame++;
                objectList[i].LastUpdate = gameTime.TotalGameTime.TotalMilliseconds;

                if (objectList[i].Frame >= objectList[i].Motion.Frames.Length)
                    objectList[i].Frame = 0;

                objectList[i].MotionPosition = null;
                objectList[i].MotionScale = null;
                objectList[i].MotionRotation = null;

                for (int j = 0; j < objectList[i].Motion.Frames[objectList[i].Frame].Channels.Length; j++)
                {
                    if (objectList[i].Motion.Channels[j].Type == ZMO.ChannelType.Position)
                        objectList[i].MotionPosition = objectList[i].Motion.Frames[objectList[i].Frame].Channels[j].Position;
                    else if (objectList[i].Motion.Channels[j].Type == ZMO.ChannelType.Rotation)
                        objectList[i].MotionRotation = objectList[i].Motion.Frames[objectList[i].Frame].Channels[j].Rotation;
                    else if (objectList[i].Motion.Channels[j].Type == ZMO.ChannelType.Scale)
                        objectList[i].MotionScale = objectList[i].Motion.Frames[objectList[i].Frame].Channels[j].Scale;
                }
            }
        }

        /// <summary>
        /// Draws the batch list.
        /// </summary>
        /// <param name="textureManager">The texture manager.</param>
        /// <param name="shader">The shader.</param>
        /// <param name="boundingBoxes">if set to <c>true</c> [bounding boxes].</param>
        public void Draw(TextureManager textureManager, Effect shader, bool boundingBoxes)
        {
            Draw(textureManager, shader, boundingBoxes, CameraManager.View, CameraManager.Projection);
        }

        /// <summary>
        /// Draws the batch list.
        /// </summary>
        /// <param name="textureManager">The texture manager.</param>
        /// <param name="shader">The shader.</param>
        /// <param name="boundingBoxes">if set to <c>true</c> [bounding boxes].</param>
        /// <param name="view">The view.</param>
        /// <param name="projection">The projection.</param>
        public void Draw(TextureManager textureManager, Effect shader, bool boundingBoxes, Matrix view, Matrix projection)
        {
            BoundingFrustum boundingFrustum = new BoundingFrustum(view * projection);

            device.VertexDeclaration = vertexDeclaration;

            int currentTexture = -1;
            int currentModel = -1;

            batchObjects.ForEach(delegate(BatchObject batchObject)
            {
                if (objectList[batchObject.ModelID].Motion != null || !boundingBoxes || boundingFrustum.OnScreen(batchObject.BoundingBox))
                {
                    if (currentModel != batchObject.ModelID)
                    {
                        currentModel = batchObject.ModelID;

                        device.Vertices[0].SetSource(objectList[batchObject.ModelID].VertexBuffer, 0, sizeInBytes);
                        device.Indices = objectList[batchObject.ModelID].IndexBuffer;
                    }

                    Matrix world = Matrix.CreateFromQuaternion(objectList[batchObject.ModelID].MotionRotation ?? batchObject.Rotation);

                    if (objectList[batchObject.ModelID].MotionScale.HasValue)
                        world *= Matrix.CreateScale(objectList[batchObject.ModelID].MotionScale.Value);
                    else
                        world *= Matrix.CreateScale(batchObject.Scale);

                    world *= Matrix.CreateTranslation(objectList[batchObject.ModelID].MotionPosition ?? batchObject.Position);

                    shader.SetValue("WorldViewProjection", world * batchObject.World * view * projection);

                    if (currentTexture != batchObject.TextureID)
                    {
                        currentTexture = batchObject.TextureID;
                        shader.SetValue("Texture", textureManager[batchObject.TextureID].Image);

                        if (device.RenderState.AlphaBlendEnable != textureManager[batchObject.TextureID].RenderState.AlphaEnabled)
                            device.RenderState.AlphaBlendEnable = textureManager[batchObject.TextureID].RenderState.AlphaEnabled;

                        if (textureManager[batchObject.TextureID].RenderState.AlphaEnabled)
                        {
                            device.RenderState.AlphaTestEnable = textureManager[batchObject.TextureID].RenderState.AlphaTestEnabled;
                            device.RenderState.SourceBlend = textureManager[batchObject.TextureID].RenderState.SourceBlend;
                            device.RenderState.DestinationBlend = textureManager[batchObject.TextureID].RenderState.DestinationBlend;
                            device.RenderState.ReferenceAlpha = textureManager[batchObject.TextureID].RenderState.AlphaReference;
                            device.RenderState.AlphaFunction = textureManager[batchObject.TextureID].RenderState.AlphaFunction;
                            device.RenderState.BlendFunction = textureManager[batchObject.TextureID].RenderState.BlendFunction;
                        }
                    }

                    for (int j = 0; j < batchObject.EffectParameters.Length; j++)
                    {
                        switch (batchObject.EffectParameters[j].Type)
                        {
                            case KeyType.Texture:
                                shader.SetValue(batchObject.EffectParameters[j].Key, (Texture2D)batchObject.EffectParameters[j].Value);
                                break;
                            case KeyType.Bool:
                                shader.SetValue(batchObject.EffectParameters[j].Key, (bool)batchObject.EffectParameters[j].Value);
                                break;
                            case KeyType.Vector4:
                                shader.SetValue(batchObject.EffectParameters[j].Key, (Vector4)batchObject.EffectParameters[j].Value);
                                break;
                            case KeyType.Float:
                                shader.SetValue(batchObject.EffectParameters[j].Key, (float)batchObject.EffectParameters[j].Value);
                                break;
                            case KeyType.Vector2:
                                shader.SetValue(batchObject.EffectParameters[j].Key, (Vector2)batchObject.EffectParameters[j].Value);
                                break;
                            case KeyType.Vector3:
                                shader.SetValue(batchObject.EffectParameters[j].Key, (Vector3)batchObject.EffectParameters[j].Value);
                                break;
                            case KeyType.Quaternion:
                                shader.SetValue(batchObject.EffectParameters[j].Key, (Quaternion)batchObject.EffectParameters[j].Value);
                                break;
                            case KeyType.Int:
                                shader.SetValue(batchObject.EffectParameters[j].Key, (int)batchObject.EffectParameters[j].Value);
                                break;
                        }
                    }

                    shader.CommitChanges();

                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, objectList[batchObject.ModelID].VertexCount, 0, objectList[batchObject.ModelID].IndexCount);
                }
            });

            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.AlphaTestEnable = false;
            device.RenderState.ReferenceAlpha = 0;
            device.RenderState.AlphaBlendOperation = BlendFunction.Add;
            device.RenderState.AlphaFunction = CompareFunction.Always;
        }

        #region Static Members

        /// <summary>
        /// Sorts by model ID.
        /// </summary>
        /// <param name="batchObjectA">The batch object A.</param>
        /// <param name="batchObjectB">The batch object B.</param>
        /// <returns></returns>
        public static int SortByModelID(BatchObject batchObjectA, BatchObject batchObjectB)
        {
            return batchObjectA.ModelID.CompareTo(batchObjectB.ModelID);
        }

        #endregion
    }
}