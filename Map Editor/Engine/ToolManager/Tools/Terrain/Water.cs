using System.Collections.Generic;
using System.Windows.Forms;
using Map_Editor.Engine.Map;
using Map_Editor.Engine.Tools.Interfaces;
using Map_Editor.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Map_Editor.Engine.SpriteManager;

namespace Map_Editor.Engine.Tools
{
    /// <summary>
    /// Water class.
    /// </summary>
    public class Water : ITool
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDevice device { get; set; }

        /// <summary>
        /// Gets or sets the basic effect.
        /// </summary>
        /// <value>The basic effect.</value>
        private BasicEffect basicEffect { get; set; }

        /// <summary>
        /// Gets or sets the hovered object.
        /// </summary>
        /// <value>The hovered object.</value>
        public int HoveredObject { get; private set; }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public int SelectedObject { get; private set; }

        #region Adding Related

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Water"/> is adding.
        /// </summary>
        /// <value><c>true</c> if adding; otherwise, <c>false</c>.</value>
        private bool adding { get; set; }

        /// <summary>
        /// Gets or sets the picked position.
        /// </summary>
        /// <value>The picked position.</value>
        private Vector3 pickedPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [mouse down].
        /// </summary>
        /// <value><c>true</c> if [mouse down]; otherwise, <c>false</c>.</value>
        private bool mouseDown { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Water"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public Water(GraphicsDevice device)
        {
            this.device = device;

            basicEffect = new BasicEffect(device, null);

            SelectedObject = -1;
            HoveredObject = -1;
        }

        #region Adding Related

        /// <summary>
        /// Starts the adding process.
        /// </summary>
        public void StartAdding()
        {
            adding = true;

            Cursor.Current = Cursors.Default;

            HoveredObject = -1;
        }

        /// <summary>
        /// Stops the adding process.
        /// </summary>
        public void StopAdding()
        {
            adding = false;
        }

        #endregion

        #region Selecting and Editing Related

        /// <summary>
        /// Selects the specified object.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int Select(int index)
        {
            return ((WaterTool)App.Form.ToolHost.Content).Select(index);
        }

        /// <summary>
        /// Removes the selected object.
        /// </summary>
        /// <param name="undoAction">if set to <c>true</c> [undo action].</param>
        public void Remove(bool undoAction)
        {
            MapManager.Water.RemoveAt(SelectedObject, undoAction);

            ((WaterTool)App.Form.ToolHost.Content).Clean();

            SelectedObject = -1;
        }

        #endregion

        /// <summary>
        /// Updates the tool.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (SelectedObject >= 0)
            {
                if (!((WaterTool)App.Form.ToolHost.Content).Z.Focused && keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Delete))
                    Remove(false);
            }

            MouseState mouseState = Mouse.GetState();

            if (!mouseState.Intersects(device.Viewport))
                return;

            if (adding)
            {
                if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && !mouseDown)
                {
                    int fileID = MapManager.InsideFile(pickedPosition);
                    Vector2 fileBlock = MapManager.InsideBlock(pickedPosition);

                    Vector2 blockPosition = new Vector2(fileBlock.Y * 160.0f, 10400.0f - ((fileBlock.X + 1) * 160.0f));

                    int objectID = FileManager.IFOs[fileID].Water.Count;

                    Vector3 minimum = new Vector3()
                    {
                        X = pickedPosition.X - 10.0f,
                        Y = pickedPosition.Y - 10.0f,
                        Z = pickedPosition.Z + 2.0f
                    };

                    Vector3 maximum = new Vector3()
                    {
                        X = pickedPosition.X + 10.0f,
                        Y = pickedPosition.Y + 10.0f,
                        Z = pickedPosition.Z + 2.0f
                    };

                    if (minimum.X < blockPosition.X)
                        minimum.X = blockPosition.X;

                    if (minimum.Y < blockPosition.Y)
                        minimum.Y = blockPosition.Y;

                    if (maximum.X > blockPosition.X + 160.0f)
                        maximum.X = blockPosition.X + 160.0f;

                    if (maximum.Y > blockPosition.Y + 160.0f)
                        maximum.Y = blockPosition.Y + 160.0f;

                    IFO.WaterBlock newObject = new IFO.WaterBlock()
                    {
                        Minimum = minimum,
                        Maximum = maximum,
                        Parent = FileManager.IFOs[fileID]
                    };

                    FileManager.IFOs[fileID].Water.Add(newObject);

                    MapManager.Water.Add(FileManager.IFOs[fileID].Water[objectID]);

                    UndoManager.AddCommand(new Commands.Water.Added()
                    {
                        ObjectID = MapManager.Water.WorldObjects.Count - 1,
                        Object = MapManager.Water.WorldObjects[MapManager.Water.WorldObjects.Count - 1],
                        IFOEntry = newObject
                    });

                    mouseDown = true;
                }
                else if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && mouseDown)
                    mouseDown = false;

                return;
            }

            if (ToolManager.GetManipulationMode() == ToolManager.ManipulationMode.Position)
            {
                if (!ToolManager.Position.Update())
                    return;
            }

            List<Terrain.Water.WorldObject> worldObjects = MapManager.Water.WorldObjects;
            List<int> mouseSelections = new List<int>();

            BoundingFrustum boundingFrustum = new BoundingFrustum(CameraManager.View * CameraManager.Projection);
            Ray pickRay = new Ray().Create(device, mouseState, CameraManager.View, CameraManager.Projection);

            for (int i = 0; i < worldObjects.Count; i++)
            {
                if (boundingFrustum.OnScreen(worldObjects[i].BoundingBox) && pickRay.Intersects(worldObjects[i].BoundingBox).HasValue)
                    mouseSelections.Add(i);
            }

            if (mouseSelections.Count == 0)
            {
                Cursor.Current = Cursors.Default;

                HoveredObject = -1;

                return;
            }

            int closestObject = mouseSelections[0];
            float currentDistance = Vector3.Distance(CameraManager.Position, worldObjects[closestObject].BoundingBox.GetCenter());

            for (int i = 0; i < mouseSelections.Count; i++)
            {
                float distance = Vector3.Distance(CameraManager.Position, worldObjects[i].BoundingBox.GetCenter());

                if (distance > currentDistance)
                    continue;

                closestObject = mouseSelections[i];
                currentDistance = distance;
            }

            if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && closestObject != SelectedObject)
                SelectedObject = Select(closestObject);
            else
                HoveredObject = closestObject;

            if (HoveredObject != SelectedObject)
            {
                ToolTipManager.Set(string.Format("Size: {0} x {1}", worldObjects[HoveredObject].Entry.Maximum.X - worldObjects[HoveredObject].Entry.Minimum.X, -(worldObjects[HoveredObject].Entry.Maximum.Y - worldObjects[HoveredObject].Entry.Minimum.Y)), worldObjects[HoveredObject].BoundingBox.GetCenter());

                Cursor.Current = Cursors.Hand;
            }
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void Draw()
        {
            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleTexture);

            if (adding)
            {
                pickedPosition = MapManager.Heightmaps.PickPosition();
                Vector2 fileBlock = MapManager.InsideBlock(pickedPosition);

                Vector2 blockPosition = new Vector2(fileBlock.Y * 160.0f, 10400.0f - ((fileBlock.X + 1) * 160.0f));

                Vector3 minimum = new Vector3()
                {
                    X = pickedPosition.X - 10.0f,
                    Y = pickedPosition.Y - 10.0f,
                    Z = pickedPosition.Z + 2.0f
                };

                Vector3 maximum = new Vector3()
                {
                    X = pickedPosition.X + 10.0f,
                    Y = pickedPosition.Y + 10.0f,
                    Z = pickedPosition.Z + 2.0f
                };

                if (minimum.X < blockPosition.X)
                    minimum.X = blockPosition.X;

                if (minimum.Y < blockPosition.Y)
                    minimum.Y = blockPosition.Y;

                if (maximum.X > blockPosition.X + 160.0f)
                    maximum.X = blockPosition.X + 160.0f;

                if (maximum.Y > blockPosition.Y + 160.0f)
                    maximum.Y = blockPosition.Y + 160.0f;

                VertexPositionTexture[] veritces = new VertexPositionTexture[]
                {
                    new VertexPositionTexture()
                    {
                        Position = new Vector3(minimum.X, minimum.Y, minimum.Z),
                        TextureCoordinate = new Vector2(minimum.X / 16.0f, minimum.Y / 16.0f)
                    },                
                    new VertexPositionTexture()
                    {
                        Position = new Vector3(maximum.X, minimum.Y, minimum.Z),
                        TextureCoordinate = new Vector2(maximum.X / 16.0f, minimum.Y / 16.0f)
                    },
                    new VertexPositionTexture()
                    {
                        Position = new Vector3(maximum.X, maximum.Y, minimum.Z),
                        TextureCoordinate = new Vector2(maximum.X / 16.0f, maximum.Y / 16.0f)
                    },
                    new VertexPositionTexture()
                    {
                        Position = new Vector3(minimum.X, maximum.Y, minimum.Z),
                        TextureCoordinate = new Vector2(minimum.X / 16.0f, maximum.Y / 16.0f)
                    }
                };

                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, veritces, 0, 4, new short[]
                { 
                    0, 3, 2,
                    2, 1, 0
                }, 0, 2);

                shader.Finish();

                device.RenderState.DepthBufferEnable = false;

                shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleColour);
                shader.SetValue("WorldViewProjection", CameraManager.View * CameraManager.Projection);

                shader.Start("SimpleColour");

                shader.SetValue("Colour", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                shader.CommitChanges();

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(minimum.X, minimum.Y, minimum.Z),
                    new Vector3(maximum.X, minimum.Y, minimum.Z)
                }, 0, 1);

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(maximum.X, minimum.Y, minimum.Z),
                    new Vector3(maximum.X, maximum.Y, minimum.Z)
                }, 0, 1);

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(maximum.X, maximum.Y, minimum.Z),
                    new Vector3(minimum.X, maximum.Y, minimum.Z)
                }, 0, 1);

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(minimum.X, maximum.Y, minimum.Z),
                    new Vector3(minimum.X, minimum.Y, minimum.Z)
                }, 0, 1);

                shader.Finish();

                device.RenderState.DepthBufferEnable = true;
            }
            else
                shader.Finish();

            List<Terrain.Water.WorldObject> worldObjects = MapManager.Water.WorldObjects;

            device.RenderState.DepthBufferEnable = false;

            shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleColour);
            shader.SetValue("WorldViewProjection", CameraManager.View * CameraManager.Projection);

            shader.Start("SimpleColour");

            if (SelectedObject >= 0)
            {
                shader.SetValue("Colour", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                shader.CommitChanges();

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(worldObjects[SelectedObject].Entry.Minimum.X, worldObjects[SelectedObject].Entry.Minimum.Y, worldObjects[SelectedObject].Entry.Minimum.Z),
                    new Vector3(worldObjects[SelectedObject].Entry.Maximum.X, worldObjects[SelectedObject].Entry.Minimum.Y, worldObjects[SelectedObject].Entry.Minimum.Z)
                }, 0, 1);

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(worldObjects[SelectedObject].Entry.Maximum.X, worldObjects[SelectedObject].Entry.Minimum.Y, worldObjects[SelectedObject].Entry.Minimum.Z),
                    new Vector3(worldObjects[SelectedObject].Entry.Maximum.X, worldObjects[SelectedObject].Entry.Maximum.Y, worldObjects[SelectedObject].Entry.Minimum.Z)
                }, 0, 1);

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(worldObjects[SelectedObject].Entry.Maximum.X, worldObjects[SelectedObject].Entry.Maximum.Y, worldObjects[SelectedObject].Entry.Minimum.Z),
                    new Vector3(worldObjects[SelectedObject].Entry.Minimum.X, worldObjects[SelectedObject].Entry.Maximum.Y, worldObjects[SelectedObject].Entry.Minimum.Z)
                }, 0, 1);

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(worldObjects[SelectedObject].Entry.Minimum.X, worldObjects[SelectedObject].Entry.Maximum.Y, worldObjects[SelectedObject].Entry.Minimum.Z),
                    new Vector3(worldObjects[SelectedObject].Entry.Minimum.X, worldObjects[SelectedObject].Entry.Minimum.Y, worldObjects[SelectedObject].Entry.Minimum.Z)
                }, 0, 1);
            }

            if (HoveredObject != SelectedObject && HoveredObject >= 0)
            {
                shader.SetValue("Colour", new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
                shader.CommitChanges();

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(worldObjects[HoveredObject].Entry.Minimum.X, worldObjects[HoveredObject].Entry.Minimum.Y, worldObjects[HoveredObject].Entry.Minimum.Z),
                    new Vector3(worldObjects[HoveredObject].Entry.Maximum.X, worldObjects[HoveredObject].Entry.Minimum.Y, worldObjects[HoveredObject].Entry.Minimum.Z)
                }, 0, 1);

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(worldObjects[HoveredObject].Entry.Maximum.X, worldObjects[HoveredObject].Entry.Minimum.Y, worldObjects[HoveredObject].Entry.Minimum.Z),
                    new Vector3(worldObjects[HoveredObject].Entry.Maximum.X, worldObjects[HoveredObject].Entry.Maximum.Y, worldObjects[HoveredObject].Entry.Minimum.Z)
                }, 0, 1);

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(worldObjects[HoveredObject].Entry.Maximum.X, worldObjects[HoveredObject].Entry.Maximum.Y, worldObjects[HoveredObject].Entry.Minimum.Z),
                    new Vector3(worldObjects[HoveredObject].Entry.Minimum.X, worldObjects[HoveredObject].Entry.Maximum.Y, worldObjects[HoveredObject].Entry.Minimum.Z)
                }, 0, 1);

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    new Vector3(worldObjects[HoveredObject].Entry.Minimum.X, worldObjects[HoveredObject].Entry.Maximum.Y, worldObjects[HoveredObject].Entry.Minimum.Z),
                    new Vector3(worldObjects[HoveredObject].Entry.Minimum.X, worldObjects[HoveredObject].Entry.Minimum.Y, worldObjects[HoveredObject].Entry.Minimum.Z)
                }, 0, 1);
            }

            shader.Finish();

            device.RenderState.DepthBufferEnable = true;
        }
    }
}