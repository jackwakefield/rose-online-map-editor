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
    /// Spawn Points class.
    /// </summary>
    public class SpawnPoints : ITool
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
        /// Gets or sets a value indicating whether this <see cref="SpawnPoints"/> is adding.
        /// </summary>
        /// <value><c>true</c> if adding; otherwise, <c>false</c>.</value>
        private bool adding { get; set; }

        /// <summary>
        /// Gets or sets the picked position.
        /// </summary>
        /// <value>The picked position.</value>
        private Vector3 pickedPosition;

        /// <summary>
        /// Gets or sets a value indicating whether [mouse down].
        /// </summary>
        /// <value><c>true</c> if [mouse down]; otherwise, <c>false</c>.</value>
        private bool mouseDown { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnPoints"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public SpawnPoints(GraphicsDevice device)
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
            ToolManager.Cursor.Position = MapManager.SpawnPoints.WorldObjects[index].Entry.Position;

            ToolManager.Cursor.PositionChanged += new EventHandler(Cursor_PositionChanged);
            ToolManager.Cursor.Cancelled += new EventHandler(Cursor_Cancelled);
            ToolManager.Cursor.Finished += new EventHandler(Cursor_Finished);

            ToolManager.Position.BoundingBox = MapManager.SpawnPoints.WorldObjects[index].BoundingBox;
            ToolManager.Position.Position = ToolManager.Position.RealPosition = MapManager.SpawnPoints.WorldObjects[index].Entry.Position;

            ToolManager.Position.MouseReleased += new System.EventHandler(Translate_MouseReleased);
            ToolManager.Position.PositionChanged += new System.EventHandler(Translate_PositionChanged);

            ToolManager.Position.FirstTime = true;

            ToolManager.Position.Update();

            return ((SpawnPointTool)App.Form.ToolHost.Content).Select(index);
        }

        /// <summary>
        /// Removes the selected object.
        /// </summary>
        /// <param name="undoAction">if set to <c>true</c> [undo action].</param>
        public void Remove(bool undoAction)
        {
            MapManager.SpawnPoints.RemoveAt(SelectedObject, undoAction);

            ((SpawnPointTool)App.Form.ToolHost.Content).Clean();

            ToolManager.Cursor.Position = Vector3.Zero;

            ToolManager.Position.BoundingBox = new BoundingBox();
            ToolManager.Position.Position = Vector3.Zero;

            SelectedObject = -1;
        }

        /// <summary>
        /// Changes the world.
        /// </summary>
        /// <param name="position">The position.</param>
        public void ChangeWorld(Vector3 position)
        {
            if (SelectedObject >= 0)
            {
                List<Events.SpawnPoints.WorldObject> worldObjects = MapManager.SpawnPoints.WorldObjects;

                worldObjects[SelectedObject].Entry.Position = position;

                MapManager.SpawnPoints.Add(SelectedObject, worldObjects[SelectedObject].Entry, true);

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
            List<Events.SpawnPoints.WorldObject> worldObjects = MapManager.SpawnPoints.WorldObjects;

            if (positionCommand == null)
            {
                positionCommand = new Commands.SpawnPoints.Positioned()
                {
                    OldPosition = worldObjects[SelectedObject].Entry.Position,
                    ObjectID = SelectedObject
                };
            }

            Vector3 realPosition = worldObjects[SelectedObject].Entry.Position;

            ChangeWorld(ToolManager.Cursor.Position);

            worldObjects[SelectedObject].Entry.Position = realPosition;
        }

        /// <summary>
        /// Handles the Cancelled event of the Cursor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Cursor_Cancelled(object sender, EventArgs e)
        {
            List<Events.SpawnPoints.WorldObject> worldObjects = MapManager.SpawnPoints.WorldObjects;

            ChangeWorld(worldObjects[SelectedObject].Entry.Position);

            positionCommand = null;
        }

        /// <summary>
        /// Handles the Finished event of the Cursor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Cursor_Finished(object sender, EventArgs e)
        {
            List<Events.SpawnPoints.WorldObject> worldObjects = MapManager.SpawnPoints.WorldObjects;

            ChangeWorld(ToolManager.Cursor.Position);

            ((SpawnPointProperty)((SpawnPointTool)App.Form.ToolHost.Content).Properties.SelectedObject).Position = MapManager.SpawnPoints.WorldObjects[SelectedObject].Entry.Position;
            ((SpawnPointTool)App.Form.ToolHost.Content).Properties.Refresh();

            if (positionCommand == null)
                return;

            ((Commands.SpawnPoints.Positioned)positionCommand).NewPosition = ToolManager.Position.Position;
            ((Commands.SpawnPoints.Positioned)positionCommand).Object = MapManager.SpawnPoints.WorldObjects[SelectedObject];

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
            List<Events.SpawnPoints.WorldObject> worldObjects = MapManager.SpawnPoints.WorldObjects;

            if (positionCommand == null)
            {
                positionCommand = new Commands.SpawnPoints.Positioned()
                {
                    OldPosition = worldObjects[SelectedObject].Entry.Position,
                    ObjectID = SelectedObject
                };
            }

            worldObjects[SelectedObject].Entry.Position = ToolManager.Position.Position;

            ChangeWorld(worldObjects[SelectedObject].Entry.Position);
        }

        /// <summary>
        /// Handles the MouseReleased event of the Translate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Translate_MouseReleased(object sender, EventArgs e)
        {
            ((SpawnPointProperty)((SpawnPointTool)App.Form.ToolHost.Content).Properties.SelectedObject).Position = MapManager.SpawnPoints.WorldObjects[SelectedObject].Entry.Position;
            ((SpawnPointTool)App.Form.ToolHost.Content).Properties.Refresh();

            if (positionCommand == null)
                return;

            ((Commands.SpawnPoints.Positioned)positionCommand).NewPosition = ToolManager.Position.Position;
            ((Commands.SpawnPoints.Positioned)positionCommand).Object = MapManager.SpawnPoints.WorldObjects[SelectedObject];

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
                    ZON.SpawnPoint newObject = new ZON.SpawnPoint()
                    {
                        Name = "EMPTY",
                        Position = pickedPosition
                    };

                    FileManager.ZON.SpawnPoints.Add(newObject);
                    MapManager.SpawnPoints.Add(newObject);

                    UndoManager.AddCommand(new Commands.SpawnPoints.Added()
                    {
                        ObjectID = MapManager.SpawnPoints.WorldObjects.Count - 1,
                        Object = MapManager.SpawnPoints.WorldObjects[MapManager.SpawnPoints.WorldObjects.Count - 1],
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

            List<Events.SpawnPoints.WorldObject> worldObjects = MapManager.SpawnPoints.WorldObjects;
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
                ToolTipManager.Set(string.Format("Name: {0}", worldObjects[HoveredObject].Entry.Name), worldObjects[HoveredObject].BoundingBox.GetCenter());

                Cursor.Current = Cursors.Hand;
            }
        }

        /// <summary>
        /// Draws the indicators.
        /// </summary>
        /// <param name="drawIndicators">if set to <c>true</c> [draw indicators].</param>
        public void Draw(bool drawIndicators)
        {
            if (adding)
            {
                pickedPosition = MapManager.Heightmaps.PickPosition();

                if (App.Form.SnapToGrid.SelectedIndex > 0)
                {
                    float gridSize = 0.0625f * (float)Math.Pow(2, App.Form.SnapToGrid.SelectedIndex);

                    pickedPosition.X = (float)Math.Round(pickedPosition.X / gridSize) * gridSize;
                    pickedPosition.Y = (float)Math.Round(pickedPosition.Y / gridSize) * gridSize;
                    pickedPosition.Z = Height.GetHeight((int)((pickedPosition.X / 2.5f) + 0.5f), (int)((pickedPosition.Y / 2.5f) + 0.5f));
                }

                foreach (ModelMesh mesh in MapManager.SpawnPoints.SpawnPointModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.DiffuseColor = MapManager.SpawnPoints.Colour;

                        Vector3 worldPosition = pickedPosition;

                        effect.World = Matrix.CreateWorld(worldPosition, Vector3.Normalize(worldPosition - CameraManager.Position) * new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        effect.View = CameraManager.View;
                        effect.Projection = CameraManager.Projection;
                    }

                    mesh.Draw();
                }
            }

            List<Events.SpawnPoints.WorldObject> worldObjects = MapManager.SpawnPoints.WorldObjects;

            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleColour);
            shader.SetValue("WorldViewProjection", CameraManager.View * CameraManager.Projection);

            shader.Start("SimpleColour");

            if (!drawIndicators)
            {
                if (SelectedObject >= 0)
                {
                    shader.SetValue("Colour", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                    shader.CommitChanges();

                    device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                    {
                        worldObjects[SelectedObject].Entry.Position,
                        worldObjects[SelectedObject].Entry.Position + new Vector3(0.0f, 0.0f, 20.0f)
                    }, 0, 1);
                }

                if (HoveredObject != SelectedObject && HoveredObject >= 0)
                {
                    shader.SetValue("Colour", new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
                    shader.CommitChanges();

                    device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                    {
                        worldObjects[HoveredObject].Entry.Position,
                        worldObjects[HoveredObject].Entry.Position + new Vector3(0.0f, 0.0f, 20.0f)
                    }, 0, 1);
                }
                
                shader.Finish();

                return;
            }

            BoundingFrustum boundingFrustum = new BoundingFrustum(CameraManager.View * CameraManager.Projection);

            for (int i = 0; i < worldObjects.Count; i++)
            {
                shader.SetValue("Colour", ((SelectedObject == i) ? Color.White : (HoveredObject == i) ? Color.Red : Color.Green).ToVector4());
                shader.CommitChanges();

                device.DrawUserPrimitives<Vector3>(PrimitiveType.LineList, new Vector3[]
                {
                    worldObjects[i].Entry.Position,
                    worldObjects[i].Entry.Position + new Vector3(0.0f, 0.0f, 20.0f)
                }, 0, 1);
            }
            
            shader.Finish();
        }
    }
}