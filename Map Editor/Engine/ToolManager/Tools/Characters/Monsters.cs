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
    /// Monsters class.
    /// </summary>
    public class Monsters : ITool
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
        /// Gets or sets a value indicating whether this <see cref="Monsters"/> is adding.
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
        /// Initializes a new instance of the <see cref="Monsters"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public Monsters(GraphicsDevice device)
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
            ToolManager.Cursor.Position = MapManager.Monsters.WorldObjects[index].Entry.Position;

            ToolManager.Cursor.PositionChanged += new EventHandler(Cursor_PositionChanged);
            ToolManager.Cursor.Cancelled += new EventHandler(Cursor_Cancelled);
            ToolManager.Cursor.Finished += new EventHandler(Cursor_Finished);

            ToolManager.Position.BoundingBox = MapManager.Monsters.WorldObjects[index].BoundingBox;
            ToolManager.Position.Position = ToolManager.Position.RealPosition = MapManager.Monsters.WorldObjects[index].Entry.Position;

            ToolManager.Position.MouseReleased += new EventHandler(Translate_MouseReleased);
            ToolManager.Position.PositionChanged += new EventHandler(Translate_PositionChanged);

            ToolManager.Position.FirstTime = true;

            ToolManager.Position.Update();

            return ((MonsterTool)App.Form.ToolHost.Content).Select(index);
        }

        /// <summary>
        /// Removes the selected object.
        /// </summary>
        /// <param name="undoAction">if set to <c>true</c> [undo action].</param>
        public void Remove(bool undoAction)
        {
            MapManager.Monsters.RemoveAt(SelectedObject, undoAction);

            ((MonsterTool)App.Form.ToolHost.Content).Clean();

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
                List<Characters.Monsters.WorldObject> worldObjects = MapManager.Monsters.WorldObjects;

                worldObjects[SelectedObject].Entry.Position = position;

                MapManager.Monsters.Add(SelectedObject, worldObjects[SelectedObject].Entry, true);

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
            List<Characters.Monsters.WorldObject> worldObjects = MapManager.Monsters.WorldObjects;

            if (positionCommand == null)
            {
                positionCommand = new Commands.Monsters.Positioned()
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
            List<Characters.Monsters.WorldObject> worldObjects = MapManager.Monsters.WorldObjects;

            ChangeWorld(worldObjects[SelectedObject].Entry.Position);
        }

        /// <summary>
        /// Handles the Finished event of the Cursor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Cursor_Finished(object sender, EventArgs e)
        {
            List<Characters.Monsters.WorldObject> worldObjects = MapManager.Monsters.WorldObjects;

            ChangeWorld(ToolManager.Cursor.Position);

            ((MonsterSpawnProperty)((MonsterTool)App.Form.ToolHost.Content).Properties.SelectedObject).Position = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Position;
            ((MonsterTool)App.Form.ToolHost.Content).Properties.Refresh();

            if (positionCommand == null)
                return;

            ((Commands.Monsters.Positioned)positionCommand).NewPosition = ToolManager.Position.Position;
            ((Commands.Monsters.Positioned)positionCommand).Object = MapManager.Monsters.WorldObjects[SelectedObject];

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
            List<Characters.Monsters.WorldObject> worldObjects = MapManager.Monsters.WorldObjects;

            if (positionCommand == null)
            {
                positionCommand = new Commands.Monsters.Positioned()
                {
                    OldPosition = worldObjects[SelectedObject].Entry.Position,
                    ObjectID = SelectedObject
                };
            }

            ChangeWorld(ToolManager.Position.Position);
        }

        /// <summary>
        /// Handles the MouseReleased event of the Translate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Translate_MouseReleased(object sender, EventArgs e)
        {
            if (((MonsterTool)App.Form.ToolHost.Content).Properties.SelectedObject.GetType() == typeof(MonsterSpawnProperty))
            {
                ((MonsterSpawnProperty)((MonsterTool)App.Form.ToolHost.Content).Properties.SelectedObject).Position = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Position;

                ((MonsterTool)App.Form.ToolHost.Content).Properties.Refresh();
            }

            if (positionCommand == null)
                return;

            ((Commands.Monsters.Positioned)positionCommand).NewPosition = ToolManager.Position.Position;
            ((Commands.Monsters.Positioned)positionCommand).Object = MapManager.Monsters.WorldObjects[SelectedObject];

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

                    int objectID = FileManager.IFOs[fileID].Monsters.Count;

                    IFO.MonsterSpawn newObject = new IFO.MonsterSpawn()
                    {
                        Description = string.Empty,
                        WarpID = 0,
                        EventID = 0,
                        ObjectID = 0,
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
                        Basic = new List<IFO.MonsterSpawn.Monster>(),
                        Tactic = new List<IFO.MonsterSpawn.Monster>(),
                        Interval = 7,
                        Limit = 5,
                        Name = "Untitled",
                        Range = 20,
                        TacticPoints = 100
                    };

                    if (((MonsterTool)App.Form.ToolHost.Content).Clone.IsChecked == true)
                    {
                        newObject.Description = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Description;
                        newObject.WarpID = MapManager.Monsters.WorldObjects[SelectedObject].Entry.WarpID;
                        newObject.EventID = MapManager.Monsters.WorldObjects[SelectedObject].Entry.EventID;
                        newObject.ObjectType = MapManager.Monsters.WorldObjects[SelectedObject].Entry.ObjectType;
                        newObject.Description = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Description;
                        newObject.Interval = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Interval;
                        newObject.Limit = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Limit;
                        newObject.Name = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Name;
                        newObject.Range = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Range;
                        newObject.TacticPoints = MapManager.Monsters.WorldObjects[SelectedObject].Entry.TacticPoints;

                        for (int i = 0; i < MapManager.Monsters.WorldObjects[SelectedObject].Entry.Basic.Count; i++)
                        {
                            newObject.Basic.Add(new IFO.MonsterSpawn.Monster()
                            {
                                Description = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Basic[i].Description,
                                ID = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Basic[i].ID,
                                Count = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Basic[i].Count
                            });
                        }

                        for (int i = 0; i < MapManager.Monsters.WorldObjects[SelectedObject].Entry.Tactic.Count; i++)
                        {
                            newObject.Tactic.Add(new IFO.MonsterSpawn.Monster()
                            {
                                Description = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Tactic[i].Description,
                                ID = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Tactic[i].ID,
                                Count = MapManager.Monsters.WorldObjects[SelectedObject].Entry.Tactic[i].Count
                            });
                        }
                    }

                    FileManager.IFOs[fileID].Monsters.Add(newObject);
                    MapManager.Monsters.Add(FileManager.IFOs[fileID].Monsters[objectID]);

                    UndoManager.AddCommand(new Commands.Monsters.Added()
                    {
                        ObjectID = MapManager.Monsters.WorldObjects.Count - 1,
                        Object = MapManager.Monsters.WorldObjects[MapManager.Monsters.WorldObjects.Count - 1],
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

            List<Characters.Monsters.WorldObject> worldObjects = MapManager.Monsters.WorldObjects;
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
                ToolTipManager.Set(string.Format("Basic Monsters: {0} - Tactical Monsters: {1}", worldObjects[HoveredObject].Entry.Basic.Count, worldObjects[HoveredObject].Entry.Tactic.Count), worldObjects[HoveredObject].BoundingBox.GetCenter());

                Cursor.Current = Cursors.Hand;
            }
        }

        /// <summary>
        /// Draws the radii.
        /// </summary>
        /// <param name="drawRadii">if set to <c>true</c> [draw radii].</param>
        public void Draw(bool drawRadii)
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

                foreach (ModelMesh mesh in MapManager.Monsters.EmptySpawn.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.DiffuseColor = MapManager.Monsters.Colour;

                        Vector3 worldPosition = pickedPosition;

                        effect.World = Matrix.CreateWorld(worldPosition, Vector3.Normalize(worldPosition - CameraManager.Position) * new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        effect.View = CameraManager.View;
                        effect.Projection = CameraManager.Projection;
                    }

                    mesh.Draw();
                }
            }

            List<Characters.Monsters.WorldObject> worldObjects = MapManager.Monsters.WorldObjects;
            
            if (!drawRadii)
            {
                if (SelectedObject >= 0)
                    new BoundingSphere(worldObjects[SelectedObject].Entry.Position + new Vector3(0.0f, 0.0f, 3.0f), worldObjects[SelectedObject].Entry.Range).Draw(device, basicEffect, CameraManager.View, CameraManager.Projection, Color.White);

                device.RenderState.DepthBufferEnable = false;

                if (HoveredObject != SelectedObject && HoveredObject >= 0)
                    new BoundingSphere(worldObjects[HoveredObject].Entry.Position + new Vector3(0.0f, 0.0f, 3.0f), worldObjects[HoveredObject].Entry.Range).Draw(device, basicEffect, CameraManager.View, CameraManager.Projection, Color.Red);

                device.RenderState.DepthBufferEnable = true;

                return;
            }

            for(int i = 0; i < worldObjects.Count; i++)
            {
                if (i == SelectedObject || i == HoveredObject)
                    device.RenderState.DepthBufferEnable = false;

                new BoundingSphere(worldObjects[i].Entry.Position + new Vector3(0.0f, 0.0f, 3.0f), worldObjects[i].Entry.Range).Draw(device, basicEffect, CameraManager.View, CameraManager.Projection, (SelectedObject == i) ? Color.White : (HoveredObject == i) ? Color.Red : Color.Blue);

                if (i == SelectedObject || i == HoveredObject)
                    device.RenderState.DepthBufferEnable = true;
            }
        }
    }
}