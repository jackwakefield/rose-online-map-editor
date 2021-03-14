using System.Collections.Generic;
using Map_Editor.Engine.Map;
using Map_Editor.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Events
{
    /// <summary>
    /// SpawnPoints class.
    /// </summary>
    public class SpawnPoints : DrawableGameComponent
    {
        #region Sub Classes

        /// <summary>
        /// WorldObject class.
        /// </summary>
        public class WorldObject
        {
            /// <summary>
            /// Gets or sets the entry.
            /// </summary>
            /// <value>The entry.</value>
            public ZON.SpawnPoint Entry { get; set; }

            /// <summary>
            /// Gets or sets the bounding box.
            /// </summary>
            /// <value>The bounding box.</value>
            public BoundingBox BoundingBox { get; set; }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>The tool.</value>
        public Tools.SpawnPoints Tool
        {
            get { return (Tools.SpawnPoints)ToolManager.Tool; }
        }

        /// <summary>
        /// Gets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public Vector3 Colour
        {
            get { return new Vector3(0.0f, colour, 0.0f); }
        }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the spawn point model.
        /// </summary>
        /// <value>The spawn point model.</value>
        public Model SpawnPointModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SpawnPoints"/> is loading.
        /// </summary>
        /// <value><c>true</c> if loading; otherwise, <c>false</c>.</value>
        public bool Loading { get; set; }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        private float colour { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [increasing colour].
        /// </summary>
        /// <value><c>true</c> if [increasing colour]; otherwise, <c>false</c>.</value>
        private bool increasingColour { get; set; }

        #endregion

        #region List Declarations

        /// <summary>
        /// Gets or sets the world objects.
        /// </summary>
        /// <value>The world objects.</value>
        public List<WorldObject> WorldObjects { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnPoints"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public SpawnPoints(Game game)
            : base(game)
        {
            WorldObjects = new List<WorldObject>();

            SpawnPointModel = Game.Content.Load<Model>("SpawnPoint");

            DrawOrder = MapManager.DRAWORDER_SPAWNPOINTS;

            colour = 1.0f;
            increasingColour = false;
        }

        /// <summary>
        /// Adds the specified spawn point.
        /// </summary>
        /// <param name="spawnPoint">The spawn point.</param>
        public void Add(ZON.SpawnPoint spawnPoint)
        {
            WorldObjects.Add(new WorldObject());

            Add(WorldObjects.Count - 1, spawnPoint, false);
        }

        /// <summary>
        /// Adds the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="spawnPoint">The spawn point.</param>
        /// <param name="removePrevious">if set to <c>true</c> [remove previous].</param>
        public void Add(int id, ZON.SpawnPoint spawnPoint, bool removePrevious)
        {
            WorldObject newSpawnPoint = new WorldObject()
            {
                Entry = spawnPoint,
                BoundingBox = BoundingBox.CreateFromSphere(SpawnPointModel.Meshes[0].BoundingSphere)
            };

            newSpawnPoint.BoundingBox = new BoundingBox()
            {
                Min = Vector3.Transform(newSpawnPoint.BoundingBox.Min, Matrix.CreateTranslation(spawnPoint.Position)),
                Max = Vector3.Transform(newSpawnPoint.BoundingBox.Max, Matrix.CreateTranslation(spawnPoint.Position))
            };

            WorldObjects[id] = newSpawnPoint;

            if (removePrevious)
            {
                for (int i = 0; i < UndoManager.Commands.Length; i++)
                {
                    if (UndoManager.Commands[i] == null)
                        continue;

                    if ((UndoManager.Commands[i].GetType() == typeof(Commands.SpawnPoints.ValueChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.SpawnPoints.Positioned) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.SpawnPoints.Added) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.SpawnPoints.Removed)) && ((Commands.SpawnPoints.SpawnPoint)UndoManager.Commands[i]).ObjectID == id)
                        ((Commands.SpawnPoints.SpawnPoint)UndoManager.Commands[i]).Object = WorldObjects[id];
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
            if (!undoAction)
            {
                UndoManager.AddCommand(new Commands.SpawnPoints.Removed()
                {
                    ObjectID = id,
                    Object = WorldObjects[id],
                    Entry = WorldObjects[id].Entry
                });
            }

            FileManager.ZON.SpawnPoints.Remove(WorldObjects[id].Entry);
            WorldObjects.RemoveAt(id);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            WorldObjects.Clear();
        }

        /// <summary>
        /// Draws the spawn points.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || !ConfigurationManager.GetValue<bool>("Draw", "SpawnPoints"))
                return;

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.SpawnPoints)
            {
                if (increasingColour)
                {
                    colour += 0.01f;

                    if (colour >= 0.8f)
                        increasingColour = false;
                }
                else
                {
                    colour -= 0.01f;

                    if (colour <= 0.3f)
                        increasingColour = true;
                }
            }

            for(int i = 0; i < WorldObjects.Count; i++)
            {
                foreach (ModelMesh mesh in SpawnPointModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        if (ToolManager.GetToolMode() == ToolManager.ToolMode.SpawnPoints)
                        {
                            if (Tool.SelectedObject == i)
                                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                            else if (Tool.HoveredObject == i)
                                effect.DiffuseColor = new Vector3(colour, 0.0f, 0.0f);
                            else
                                effect.DiffuseColor = new Vector3(0.0f, colour, 0.0f);
                        }
                        else
                            effect.DiffuseColor = new Vector3(0.0f, 1.0f, 0.0f);                        

                        Vector3 worldPosition = WorldObjects[i].Entry.Position;

                        effect.World = Matrix.CreateWorld(worldPosition, Vector3.Normalize(worldPosition - CameraManager.Position) * new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        effect.View = CameraManager.View;
                        effect.Projection = CameraManager.Projection;
                    }

                    mesh.Draw();
                }
            }

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.SpawnPoints)
                Tool.Draw(ConfigurationManager.GetValue<bool>("SpawnPoints", "DrawIndicators"));
        }
    }
}