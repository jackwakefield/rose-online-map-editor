using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Map_Editor.Engine.Map;
using Map_Editor.Engine.Models;
using Map_Editor.Engine.RenderManager;
using Map_Editor.Engine.Tools.Interfaces;
using Map_Editor.Forms.Controls;
using Map_Editor.Misc.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Map_Editor.Engine.SpriteManager;

namespace Map_Editor.Engine.Tools
{
    /// <summary>
    /// EventTriggers class.
    /// </summary>
    public class EventTriggers : ITool
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

        /// <summary>
        /// Gets or sets the position command.
        /// </summary>
        /// <value>The position command.</value>
        private Commands.Interfaces.ICommand positionCommand { get; set; }

        #region Adding Related

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        private int model { get; set; }

        /// <summary>
        /// Gets or sets the world model.
        /// </summary>
        /// <value>The world model.</value>
        private Objects.EventTriggers.WorldObject worldModel { get; set; }

        /// <summary>
        /// Gets or sets the object manager.
        /// </summary>
        /// <value>The object manager.</value>
        private ObjectManager objectManager { get; set; }

        /// <summary>
        /// Gets or sets the texture manager.
        /// </summary>
        /// <value>The texture manager.</value>
        private TextureManager textureManager { get; set; }

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
        /// Initializes a new instance of the <see cref="EventTriggers"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public EventTriggers(GraphicsDevice device)
        {
            this.device = device;
            model = -1;

            basicEffect = new BasicEffect(device, null);

            SelectedObject = -1;
            HoveredObject = -1;
        }

        #region Adding Related

        /// <summary>
        /// Starts the adding process.
        /// </summary>
        /// <param name="model">The model.</param>
        public void StartAdding(int model)
        {
            this.model = model;

            objectManager = new ObjectManager(device, ZMS.Vertex.VertexElements, ZMS.Vertex.SIZE_IN_BYTES);
            textureManager = new TextureManager(device);

            ZSC.Object zscObject = FileManager.ZSCs["EVENT_OBJECT"].Objects[model];

            worldModel = new Objects.EventTriggers.WorldObject()
            {
                Entry = null,
                World = Matrix.Identity,
                Parts = new List<Objects.EventTriggers.WorldObject.Part>(zscObject.Models.Count),
            };

            for (int i = 0; i < zscObject.Models.Count; i++)
            {
                int modelID = -1;

                if (zscObject.Models[i].Motion != null)
                    modelID = objectManager.Add(FileManager.ZSCs["EVENT_OBJECT"].Models[zscObject.Models[i].ModelID], zscObject.Models[i].Motion);
                else
                    modelID = objectManager.Add(FileManager.ZSCs["EVENT_OBJECT"].Models[zscObject.Models[i].ModelID]);

                if (modelID == -1)
                    return;

                Matrix world = Matrix.CreateFromQuaternion(zscObject.Models[i].Rotation) *
                               Matrix.CreateScale(zscObject.Models[i].Scale) *
                               Matrix.CreateTranslation(zscObject.Models[i].Position);

                worldModel.Parts.Add(new Objects.EventTriggers.WorldObject.Part()
                {
                    TextureID = textureManager.Add(FileManager.ZSCs["EVENT_OBJECT"].Textures[zscObject.Models[i].TextureID]),
                    ModelID = modelID,
                    Position = zscObject.Models[i].Position,
                    Scale = zscObject.Models[i].Scale,
                    Rotation = zscObject.Models[i].Rotation,
                    BoundingBox = new BoundingBox()
                });
            }

            worldModel.Add(objectManager, model);

            Cursor.Current = Cursors.Default;

            HoveredObject = -1;
        }

        /// <summary>
        /// Stops the adding process.
        /// </summary>
        public void StopAdding()
        {
            model = -1;
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
            ToolManager.Cursor.Position = MapManager.EventTriggers.WorldObjects[index].Entry.Position;

            ToolManager.Cursor.PositionChanged += new EventHandler(Cursor_PositionChanged);
            ToolManager.Cursor.Cancelled += new EventHandler(Cursor_Cancelled);
            ToolManager.Cursor.Finished += new EventHandler(Cursor_Finished);

            ToolManager.Position.BoundingBox = MapManager.EventTriggers.WorldObjects[index].BoundingBox;
            ToolManager.Position.Position = ToolManager.Position.RealPosition = MapManager.EventTriggers.WorldObjects[index].Entry.Position;

            ToolManager.Position.MouseReleased += new System.EventHandler(Translate_MouseReleased);
            ToolManager.Position.PositionChanged += new EventHandler(Translate_PositionChanged);

            ToolManager.Position.FirstTime = true;

            ToolManager.Position.Update();

            return ((EventTriggerTool)App.Form.ToolHost.Content).Select(index);
        }

        /// <summary>
        /// Changes the model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void ChangeModel(int model)
        {
            if (this.model >= 0)
                StartAdding(model);
            else if (SelectedObject >= 0)
            {
                List<Objects.EventTriggers.WorldObject> worldObjects = MapManager.EventTriggers.WorldObjects;

                if (worldObjects[SelectedObject].Entry.ObjectID != model)
                {
                    UndoManager.AddCommand(new Commands.EventTriggers.ObjectChanged()
                    {
                        Object = worldObjects[SelectedObject],
                        ObjectID = SelectedObject,
                        OldObject = worldObjects[SelectedObject].Entry.ObjectID,
                        NewObject = model
                    });

                    worldObjects[SelectedObject].Entry.ObjectID = model;

                    MapManager.EventTriggers.Add(SelectedObject, worldObjects[SelectedObject].Entry, true);
                }
            }
        }

        /// <summary>
        /// Removes the selected object.
        /// </summary>
        /// <param name="undoAction">if set to <c>true</c> [undo action].</param>
        public void Remove(bool undoAction)
        {
            MapManager.EventTriggers.RemoveAt(SelectedObject, undoAction);

            ((EventTriggerTool)App.Form.ToolHost.Content).Clean();

            ToolManager.Cursor.Position = Vector3.Zero;

            ToolManager.Position.BoundingBox = new BoundingBox();
            ToolManager.Position.Position = Vector3.Zero;

            SelectedObject = -1;
        }

        /// <summary>
        /// Changes the world.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        public void ChangeWorld(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (SelectedObject >= 0)
            {
                List<Objects.EventTriggers.WorldObject> worldObjects = MapManager.EventTriggers.WorldObjects;

                worldObjects[SelectedObject].Entry.Position = position;
                worldObjects[SelectedObject].Entry.Rotation = rotation;
                worldObjects[SelectedObject].Entry.Scale = scale;

                MapManager.EventTriggers.Add(SelectedObject, worldObjects[SelectedObject].Entry, true);

                ToolManager.Cursor.Position = worldObjects[SelectedObject].Entry.Position;

                ToolManager.Position.BoundingBox = worldObjects[SelectedObject].BoundingBox;
                ToolManager.Position.Position = worldObjects[SelectedObject].Entry.Position;
            }
        }

        #endregion

        #region Manipulation Events


        /// <summary>
        /// Handles the PositionChanged event of the Cursor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Cursor_PositionChanged(object sender, EventArgs e)
        {
            List<Objects.EventTriggers.WorldObject> worldObjects = MapManager.EventTriggers.WorldObjects;

            if (positionCommand == null)
            {
                positionCommand = new Commands.EventTriggers.Positioned()
                {
                    OldPosition = worldObjects[SelectedObject].Entry.Position,
                    ObjectID = SelectedObject
                };
            }

            Vector3 realPosition = worldObjects[SelectedObject].Entry.Position;

            ChangeWorld(ToolManager.Cursor.Position, worldObjects[SelectedObject].Entry.Rotation, worldObjects[SelectedObject].Entry.Scale);

            worldObjects[SelectedObject].Entry.Position = realPosition;
        }

        /// <summary>
        /// Handles the Cancelled event of the Cursor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Cursor_Cancelled(object sender, EventArgs e)
        {
            List<Objects.EventTriggers.WorldObject> worldObjects = MapManager.EventTriggers.WorldObjects;

            ChangeWorld(worldObjects[SelectedObject].Entry.Position, worldObjects[SelectedObject].Entry.Rotation, worldObjects[SelectedObject].Entry.Scale);

            positionCommand = null;
        }

        /// <summary>
        /// Handles the Finished event of the Cursor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Cursor_Finished(object sender, EventArgs e)
        {
            List<Objects.EventTriggers.WorldObject> worldObjects = MapManager.EventTriggers.WorldObjects;

            ChangeWorld(ToolManager.Cursor.Position, worldObjects[SelectedObject].Entry.Rotation, worldObjects[SelectedObject].Entry.Scale);

            ((EventTriggerProperty)((EventTriggerTool)App.Form.ToolHost.Content).Properties.SelectedObject).Position = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.Position;
            ((EventTriggerTool)App.Form.ToolHost.Content).Properties.Refresh();

            if (positionCommand == null)
                return;

            ((Commands.EventTriggers.Positioned)positionCommand).NewPosition = ToolManager.Position.Position;
            ((Commands.EventTriggers.Positioned)positionCommand).Object = MapManager.EventTriggers.WorldObjects[SelectedObject];

            UndoManager.AddCommand(positionCommand);

            positionCommand = null;
        }

        /// <summary>
        /// Handles the PositionChanged event of the Translate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Translate_PositionChanged(object sender, EventArgs e)
        {
            List<Objects.EventTriggers.WorldObject> worldObjects = MapManager.EventTriggers.WorldObjects;

            if (positionCommand == null)
            {
                positionCommand = new Commands.EventTriggers.Positioned()
                {
                    OldPosition = worldObjects[SelectedObject].Entry.Position,
                    ObjectID = SelectedObject
                };
            }

            worldObjects[SelectedObject].Entry.Position = ToolManager.Position.Position;

            ChangeWorld(worldObjects[SelectedObject].Entry.Position, worldObjects[SelectedObject].Entry.Rotation, worldObjects[SelectedObject].Entry.Scale);
        }

        /// <summary>
        /// Handles the MouseReleased event of the Translate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Translate_MouseReleased(object sender, EventArgs e)
        {
            ((EventTriggerProperty)((EventTriggerTool)App.Form.ToolHost.Content).Properties.SelectedObject).Position = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.Position;
            ((EventTriggerTool)App.Form.ToolHost.Content).Properties.Refresh();

            if (positionCommand == null)
                return;

            ((Commands.EventTriggers.Positioned)positionCommand).NewPosition = ToolManager.Position.Position;
            ((Commands.EventTriggers.Positioned)positionCommand).Object = MapManager.EventTriggers.WorldObjects[SelectedObject];

            UndoManager.AddCommand(positionCommand);

            positionCommand = null;
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
                if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Delete))
                    Remove(false);
            }

            MouseState mouseState = Mouse.GetState();

            if (!mouseState.Intersects(device.Viewport))
                return;

            if (model >= 0)
            {
                objectManager.Update(gameTime);

                if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && !mouseDown)
                {
                    int fileID = MapManager.InsideFile(pickedPosition);
                    Vector2 fileBlock = MapManager.InsideBlock(pickedPosition);

                    int objectID = FileManager.IFOs[fileID].EventTriggers.Count;

                    IFO.EventTrigger newObject = new IFO.EventTrigger()
                    {
                        Description = string.Empty,
                        WarpID = 0,
                        EventID = 0,
                        ObjectID = model,
                        ObjectType = IFO.ObjectType.User,
                        MapPosition = new Vector2()
                        {
                            X = (int)MapManager.MapPosition(fileBlock, pickedPosition).X,
                            Y = (int)MapManager.MapPosition(fileBlock, pickedPosition).Y
                        },
                        Position = pickedPosition,
                        Rotation = new Quaternion(0, 0, 0, 0),
                        Scale = new Vector3(1.0f, 1.0f, 1.0f),
                        Parent = FileManager.IFOs[fileID],
                        QSDTrigger = "EMPTY",
                        LUATrigger = "EMPTY"
                    };

                    if (((EventTriggerTool)App.Form.ToolHost.Content).Clone.IsChecked == true)
                    {
                        newObject.Description = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.Description;
                        newObject.WarpID = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.WarpID;
                        newObject.EventID = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.EventID;
                        newObject.ObjectType = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.ObjectType;
                        newObject.Description = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.Description;
                        newObject.Rotation = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.Rotation;
                        newObject.Scale = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.Scale;
                        newObject.QSDTrigger = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.QSDTrigger;
                        newObject.LUATrigger = MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.LUATrigger;
                    }

                    FileManager.IFOs[fileID].EventTriggers.Add(newObject);
                    MapManager.EventTriggers.Add(FileManager.IFOs[fileID].EventTriggers[objectID]);

                    UndoManager.AddCommand(new Commands.EventTriggers.Added()
                    {
                        ObjectID = MapManager.EventTriggers.WorldObjects.Count - 1,
                        Object = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.WorldObjects.Count - 1],
                        Entry = newObject
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
            else if (ToolManager.GetManipulationMode() == ToolManager.ManipulationMode.Cursor)
            {
                ToolManager.Cursor.Update();

                if (ToolManager.Cursor.Position != Vector3.Zero)
                    return;
            }

            List<Objects.EventTriggers.WorldObject> worldObjects = MapManager.EventTriggers.WorldObjects;
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
                ToolTipManager.Set(string.Format("Object ID: {0} - Lua Trigger: {1} - QSD Trigger: {2}", worldObjects[HoveredObject].Entry.ObjectID, worldObjects[HoveredObject].Entry.LUATrigger, worldObjects[HoveredObject].Entry.QSDTrigger), worldObjects[HoveredObject].BoundingBox.GetCenter());

                Cursor.Current = Cursors.Hand;
            }
        }

        /// <summary>
        /// Draws the bounding boxes.
        /// </summary>
        /// <param name="drawBoundingBoxes">if set to <c>true</c> [draw bounding boxes].</param>
        public void Draw(bool drawBoundingBoxes)
        {
            if (model >= 0)
            {
                pickedPosition = MapManager.Heightmaps.PickPosition();

                objectManager.ClearBatch();

                worldModel.World = Matrix.Identity;

                if (((EventTriggerTool)App.Form.ToolHost.Content).Clone.IsChecked == true)
                {
                    worldModel.World *= Matrix.CreateFromQuaternion(MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.Rotation);
                    worldModel.World *= Matrix.CreateScale(MapManager.EventTriggers.WorldObjects[SelectedObject].Entry.Scale);
                }

                worldModel.World *= Matrix.CreateTranslation(pickedPosition);

                worldModel.Add(objectManager, model);

                Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleTexture);
                shader.SetValue("Addition", Vector4.Zero);

                shader.Start("SimpleTexture");

                objectManager.Draw(textureManager, shader, false);

                shader.Finish();
            }

            List<Objects.EventTriggers.WorldObject> worldObjects = MapManager.EventTriggers.WorldObjects;

            if (!drawBoundingBoxes)
            {
                if (SelectedObject >= 0)
                    worldObjects[SelectedObject].BoundingBox.Draw(device, basicEffect, CameraManager.View, CameraManager.Projection, Color.White);

                if (HoveredObject != SelectedObject && HoveredObject >= 0)
                    worldObjects[HoveredObject].BoundingBox.Draw(device, basicEffect, CameraManager.View, CameraManager.Projection, Color.Red);

                return;
            }

            BoundingFrustum boundingFrustum = new BoundingFrustum(CameraManager.View * CameraManager.Projection);

            for (int i = 0; i < worldObjects.Count; i++)
            {
                if (!boundingFrustum.OnScreen(worldObjects[i].BoundingBox))
                    continue;

                worldObjects[i].BoundingBox.Draw(device, basicEffect, CameraManager.View, CameraManager.Projection, (SelectedObject == i) ? Color.White : (HoveredObject == i) ? Color.Red : Color.Blue);
            }
        }
    }
}