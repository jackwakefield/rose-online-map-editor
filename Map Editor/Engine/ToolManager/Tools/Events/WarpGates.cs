using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Map_Editor.Engine.Map;
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
    /// Warp Gates class.
    /// </summary>
    public class WarpGates : ITool
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
        /// Gets or sets a value indicating whether this <see cref="WarpGates"/> is adding.
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
        /// Initializes a new instance of the <see cref="WarpGates"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public WarpGates(GraphicsDevice device)
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
            ToolManager.Cursor.Position = MapManager.WarpGates.WorldObjects[index].Entry.Position;

            ToolManager.Cursor.PositionChanged += new EventHandler(Cursor_PositionChanged);
            ToolManager.Cursor.Cancelled += new EventHandler(Cursor_Cancelled);
            ToolManager.Cursor.Finished += new EventHandler(Cursor_Finished);

            ToolManager.Position.BoundingBox = MapManager.WarpGates.WorldObjects[index].BoundingBox;
            ToolManager.Position.Position = ToolManager.Position.RealPosition = MapManager.WarpGates.WorldObjects[index].Entry.Position;

            ToolManager.Position.MouseReleased += new System.EventHandler(Translate_MouseReleased);
            ToolManager.Position.PositionChanged += new System.EventHandler(Translate_PositionChanged);

            ToolManager.Position.FirstTime = true;

            ToolManager.Position.Update();

            return ((WarpGateTool)App.Form.ToolHost.Content).Select(index);
        }

        /// <summary>
        /// Removes the selected object.
        /// </summary>
        /// <param name="undoAction">if set to <c>true</c> [undo action].</param>
        public void Remove(bool undoAction)
        {
            MapManager.WarpGates.RemoveAt(SelectedObject, undoAction);

            ((WarpGateTool)App.Form.ToolHost.Content).Clean();

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
                List<Events.WarpGates.WorldObject> worldObjects = MapManager.WarpGates.WorldObjects;

                worldObjects[SelectedObject].Entry.Position = position;
                worldObjects[SelectedObject].Entry.Rotation = rotation;
                worldObjects[SelectedObject].Entry.Scale = scale;

                MapManager.WarpGates.Add(SelectedObject, worldObjects[SelectedObject].Entry, true);

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
            List<Events.WarpGates.WorldObject> worldObjects = MapManager.WarpGates.WorldObjects;

            if (positionCommand == null)
            {
                positionCommand = new Commands.WarpGates.Positioned()
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
            List<Events.WarpGates.WorldObject> worldObjects = MapManager.WarpGates.WorldObjects;

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
            List<Events.WarpGates.WorldObject> worldObjects = MapManager.WarpGates.WorldObjects;

            ChangeWorld(ToolManager.Cursor.Position, worldObjects[SelectedObject].Entry.Rotation, worldObjects[SelectedObject].Entry.Scale);

            ((WarpGateProperty)((WarpGateTool)App.Form.ToolHost.Content).Properties.SelectedObject).Position = MapManager.WarpGates.WorldObjects[SelectedObject].Entry.Position;
            ((WarpGateTool)App.Form.ToolHost.Content).Properties.Refresh();

            if (positionCommand == null)
                return;

            ((Commands.WarpGates.Positioned)positionCommand).NewPosition = ToolManager.Position.Position;
            ((Commands.WarpGates.Positioned)positionCommand).Object = MapManager.WarpGates.WorldObjects[SelectedObject];

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
            List<Events.WarpGates.WorldObject> worldObjects = MapManager.WarpGates.WorldObjects;

            if (positionCommand == null)
            {
                positionCommand = new Commands.WarpGates.Positioned()
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
            ((WarpGateProperty)((WarpGateTool)App.Form.ToolHost.Content).Properties.SelectedObject).Position = MapManager.WarpGates.WorldObjects[SelectedObject].Entry.Position;
            ((WarpGateTool)App.Form.ToolHost.Content).Properties.Refresh();

            if (positionCommand == null)
                return;

            ((Commands.WarpGates.Positioned)positionCommand).NewPosition = ToolManager.Position.Position;
            ((Commands.WarpGates.Positioned)positionCommand).Object = MapManager.WarpGates.WorldObjects[SelectedObject];

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

            if (adding)
            {
                if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && !mouseDown)
                {
                    int fileID = MapManager.InsideFile(pickedPosition);
                    Vector2 fileBlock = MapManager.InsideBlock(pickedPosition);

                    int objectID = FileManager.IFOs[fileID].WarpGates.Count;

                    IFO.BaseIFO newObject = new IFO.BaseIFO()
                    {
                        Description = string.Empty,
                        WarpID = 0,
                        EventID = 0,
                        ObjectID = 10,
                        ObjectType = IFO.ObjectType.User,
                        MapPosition = new Vector2()
                        {
                            X = (int)MapManager.MapPosition(fileBlock, pickedPosition).X,
                            Y = (int)MapManager.MapPosition(fileBlock, pickedPosition).Y
                        },
                        Position = pickedPosition,
                        Rotation = new Quaternion(0, 0, 0, 0),
                        Scale = new Vector3(1.0f, 1.0f, 1.0f),
                        Parent = FileManager.IFOs[fileID]
                    };

                    if (((WarpGateTool)App.Form.ToolHost.Content).Clone.IsChecked == true)
                    {
                        newObject.Description = MapManager.WarpGates.WorldObjects[SelectedObject].Entry.Description;
                        newObject.WarpID = MapManager.WarpGates.WorldObjects[SelectedObject].Entry.WarpID;
                        newObject.EventID = MapManager.WarpGates.WorldObjects[SelectedObject].Entry.EventID;
                        newObject.ObjectType = MapManager.WarpGates.WorldObjects[SelectedObject].Entry.ObjectType;
                        newObject.Description = MapManager.WarpGates.WorldObjects[SelectedObject].Entry.Description;
                        newObject.Rotation = MapManager.WarpGates.WorldObjects[SelectedObject].Entry.Rotation;
                        newObject.Scale = MapManager.WarpGates.WorldObjects[SelectedObject].Entry.Scale;
                    }

                    FileManager.IFOs[fileID].WarpGates.Add(newObject);
                    MapManager.WarpGates.Add(FileManager.IFOs[fileID].WarpGates[objectID]);

                    UndoManager.AddCommand(new Commands.WarpGates.Added()
                    {
                        ObjectID = MapManager.WarpGates.WorldObjects.Count - 1,
                        Object = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.WorldObjects.Count - 1],
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

            List<Events.WarpGates.WorldObject> worldObjects = MapManager.WarpGates.WorldObjects;
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
                ToolTipManager.Set(string.Format("Warp ID: {0}", worldObjects[HoveredObject].Entry.WarpID), worldObjects[HoveredObject].BoundingBox.GetCenter());

                Cursor.Current = Cursors.Hand;
            }
        }

        /// <summary>
        /// Draws the warp areas.
        /// </summary>
        /// <param name="drawWarpAreas">if set to <c>true</c> [draw warp areas].</param>
        public void Draw(bool drawWarpAreas)
        {
            if (adding)
            {
                pickedPosition = MapManager.Heightmaps.PickPosition();

                foreach (ModelMesh mesh in MapManager.WarpGates.WarpGateModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.DiffuseColor = MapManager.WarpGates.Colour;

                        Vector3 worldPosition = pickedPosition;

                        effect.World = Matrix.CreateWorld(worldPosition, Vector3.Normalize(worldPosition - CameraManager.Position) * new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        effect.View = CameraManager.View;
                        effect.Projection = CameraManager.Projection;
                    }

                    mesh.Draw();
                }
            }

            List<Events.WarpGates.WorldObject> worldObjects = MapManager.WarpGates.WorldObjects;

            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleColour);

            shader.Start("SimpleColour");

            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            if (!drawWarpAreas)
            {
                if (SelectedObject >= 0)
                {
                    shader.SetValue("Colour", new Vector4(MapManager.WarpGates.GetColour(SelectedObject), 0.35f));
                    shader.SetValue("WorldViewProjection", MapManager.WarpGates.World * worldObjects[SelectedObject].World * CameraManager.View * CameraManager.Projection);
                    shader.CommitChanges();

                    MapManager.WarpGates.DrawWarpGate();

                    shader.SetValue("Colour", new Vector4(MapManager.WarpGates.GetColour(SelectedObject), 1.00f));
                    shader.CommitChanges();

                    device.RenderState.FillMode = FillMode.WireFrame;

                    MapManager.WarpGates.DrawWarpGate();

                    device.RenderState.FillMode = FillMode.Solid;
                }

                if (HoveredObject != SelectedObject && HoveredObject >= 0)
                {
                    device.RenderState.DepthBufferEnable = false;

                    shader.SetValue("Colour", new Vector4(MapManager.WarpGates.GetColour(HoveredObject), 0.35f));
                    shader.SetValue("WorldViewProjection", MapManager.WarpGates.World * worldObjects[HoveredObject].World * CameraManager.View * CameraManager.Projection);
                    shader.CommitChanges();

                    MapManager.WarpGates.DrawWarpGate();

                    shader.SetValue("Colour", new Vector4(MapManager.WarpGates.GetColour(HoveredObject), 1.00f));
                    shader.CommitChanges();

                    device.RenderState.FillMode = FillMode.WireFrame;

                    MapManager.WarpGates.DrawWarpGate();

                    device.RenderState.FillMode = FillMode.Solid;

                    device.RenderState.DepthBufferEnable = true;
                }

                shader.Finish();

                return;
            }

            for (int i = 0; i < worldObjects.Count; i++)
            {
                if (i == HoveredObject && i != SelectedObject)
                    device.RenderState.DepthBufferEnable = false;

                shader.SetValue("Colour", new Vector4(MapManager.WarpGates.GetColour(i), 0.35f));
                shader.SetValue("WorldViewProjection", MapManager.WarpGates.World * worldObjects[i].World * CameraManager.View * CameraManager.Projection);                
                shader.CommitChanges();

                MapManager.WarpGates.DrawWarpGate();

                shader.SetValue("Colour", new Vector4(MapManager.WarpGates.GetColour(i), 1.00f));
                shader.CommitChanges();

                device.RenderState.FillMode = FillMode.WireFrame;

                MapManager.WarpGates.DrawWarpGate();

                device.RenderState.FillMode = FillMode.Solid;

                if (i == HoveredObject && i != SelectedObject)
                    device.RenderState.DepthBufferEnable = true;
            }

            device.RenderState.AlphaBlendEnable = false;

            shader.Finish();
        }
    }
}