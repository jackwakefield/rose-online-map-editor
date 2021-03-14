using System.Collections.Generic;
using Map_Editor.Engine.Map;
using Map_Editor.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Misc
{
    /// <summary>
    /// Sounds class.
    /// </summary>
    public class Sounds : DrawableGameComponent
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
            public IFO.Sound Entry { get; set; }

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
        public Tools.Sounds Tool
        {
            get { return (Tools.Sounds)ToolManager.Tool; }
        }

        /// <summary>
        /// Gets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public Vector3 Colour
        {
            get { return new Vector3(0.0f, colour, colour); }
        }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the sound model.
        /// </summary>
        /// <value>The sound model.</value>
        public Model SoundModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Sounds"/> is loading.
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
        /// Initializes a new instance of the <see cref="Sounds"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public Sounds(Game game)
            : base(game)
        {
            WorldObjects = new List<WorldObject>();

            SoundModel = Game.Content.Load<Model>("Sound");

            DrawOrder = MapManager.DRAWORDER_SOUNDS;

            colour = 1.0f;
            increasingColour = false;
        }

        /// <summary>
        /// Adds the specified sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        public void Add(IFO.Sound sound)
        {
            WorldObjects.Add(new WorldObject());

            Add(WorldObjects.Count - 1, sound, false);
        }

        /// <summary>
        /// Adds the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sound">The sound.</param>
        /// <param name="removePrevious">if set to <c>true</c> [remove previous].</param>
        public void Add(int id, IFO.Sound sound, bool removePrevious)
        {
            WorldObject newObject = new WorldObject()
            {
                Entry = sound,
                BoundingBox = BoundingBox.CreateFromSphere(SoundModel.Meshes[0].BoundingSphere)
            };

            newObject.BoundingBox = new BoundingBox()
            {
                Min = Vector3.Transform(newObject.BoundingBox.Min, Matrix.CreateTranslation(sound.Position)),
                Max = Vector3.Transform(newObject.BoundingBox.Max, Matrix.CreateTranslation(sound.Position))
            };

            WorldObjects[id] = newObject;

            if (removePrevious)
            {
                for (int i = 0; i < UndoManager.Commands.Length; i++)
                {
                    if (UndoManager.Commands[i] == null)
                        continue;

                    if ((UndoManager.Commands[i].GetType() == typeof(Commands.Sounds.ValueChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Sounds.Positioned) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Sounds.Added) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Sounds.Removed)) && ((Commands.Sounds.Sound)UndoManager.Commands[i]).ObjectID == id)
                        ((Commands.Sounds.Sound)UndoManager.Commands[i]).Object = newObject;
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
                UndoManager.AddCommand(new Commands.Sounds.Removed()
                {
                    ObjectID = id,
                    Object = WorldObjects[id],
                    Entry = WorldObjects[id].Entry
                });
            }

            WorldObjects[id].Entry.Parent.Sounds.Remove(WorldObjects[id].Entry);
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
        /// Draws the sounds.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || !ConfigurationManager.GetValue<bool>("Draw", "Sounds"))
                return;

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Sounds)
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
                foreach (ModelMesh mesh in SoundModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        if (ToolManager.GetToolMode() == ToolManager.ToolMode.Sounds)
                        {
                            if (Tool.SelectedObject == i)
                                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                            else if (Tool.HoveredObject == i)
                                effect.DiffuseColor = new Vector3(colour, 0.0f, 0.0f);
                            else
                                effect.DiffuseColor = new Vector3(0.0f, colour, colour);
                        }
                        else
                            effect.DiffuseColor = new Vector3(0.0f, 1.0f, 1.0f);

                        Vector3 worldPosition = WorldObjects[i].Entry.Position;

                        effect.World = Matrix.CreateWorld(worldPosition, Vector3.Normalize(worldPosition - CameraManager.Position) * new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        effect.View = CameraManager.View;
                        effect.Projection = CameraManager.Projection;
                    }

                    mesh.Draw();
                }
            }

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Sounds)
                Tool.Draw(ConfigurationManager.GetValue<bool>("Sounds", "DrawRadii"));
        }
    }
}