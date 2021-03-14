using System.Collections.Generic;
using Map_Editor.Engine.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Map_Editor.Misc;

namespace Map_Editor.Engine.Misc
{
    /// <summary>
    /// Effects class.
    /// </summary>
    public class Effects : DrawableGameComponent
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
            public IFO.Effect Entry { get; set; }

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
        public Tools.Effects Tool
        {
            get { return (Tools.Effects)ToolManager.Tool; }
        }

        /// <summary>
        /// Gets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public Vector3 Colour
        {
            get { return new Vector3(colour, 0.0f, 0.0f); }
        }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the effect model.
        /// </summary>
        /// <value>The effect model.</value>
        public Model EffectModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Effects"/> is loading.
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
        /// Initializes a new instance of the <see cref="Effects"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public Effects(Game game)
            : base(game)
        {
            WorldObjects = new List<WorldObject>();

            EffectModel = Game.Content.Load<Model>("Effect");

            DrawOrder = MapManager.DRAWORDER_EFFECTS;

            colour = 1.0f;
            increasingColour = false;
        }

        /// <summary>
        /// Adds the specified effect.
        /// </summary>
        /// <param name="sound">The effect.</param>
        public void Add(IFO.Effect effect)
        {
            WorldObjects.Add(new WorldObject());

            Add(WorldObjects.Count - 1, effect, false);
        }

        /// <summary>
        /// Adds the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="effect">The effect.</param>
        /// <param name="removePrevious">if set to <c>true</c> [remove previous].</param>
        public void Add(int id, IFO.Effect effect, bool removePrevious)
        {
            WorldObject newObject = new WorldObject()
            {
                Entry = effect,
                BoundingBox = BoundingBox.CreateFromSphere(EffectModel.Meshes[0].BoundingSphere)
            };

            newObject.BoundingBox = new BoundingBox()
            {
                Min = Vector3.Transform(newObject.BoundingBox.Min, Matrix.CreateTranslation(effect.Position)),
                Max = Vector3.Transform(newObject.BoundingBox.Max, Matrix.CreateTranslation(effect.Position))
            };

            WorldObjects[id] = newObject;

            if (removePrevious)
            {
                for (int i = 0; i < UndoManager.Commands.Length; i++)
                {
                    if (UndoManager.Commands[i] == null)
                        continue;

                    if ((UndoManager.Commands[i].GetType() == typeof(Commands.Effects.ValueChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Effects.Positioned) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Effects.Added) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Effects.Removed)) && ((Commands.Effects.Effect)UndoManager.Commands[i]).ObjectID == id)
                        ((Commands.Effects.Effect)UndoManager.Commands[i]).Object = newObject;
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
                UndoManager.AddCommand(new Commands.Effects.Removed()
                {
                    ObjectID = id,
                    Object = WorldObjects[id],
                    Entry = WorldObjects[id].Entry
                });
            }

            WorldObjects[id].Entry.Parent.Effects.Remove(WorldObjects[id].Entry);
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
        /// Draws the effects.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || !ConfigurationManager.GetValue<bool>("Draw", "Effects"))
                return;

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Effects)
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

            for (int i = 0; i < WorldObjects.Count; i++)
            {
                foreach (ModelMesh mesh in EffectModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        if (ToolManager.GetToolMode() == ToolManager.ToolMode.Effects)
                        {
                            if (Tool.SelectedObject == i)
                                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                            else if (Tool.HoveredObject == i)
                                effect.DiffuseColor = new Vector3(colour, 0.0f, colour);
                            else
                                effect.DiffuseColor = new Vector3(colour, 0.0f, 0.0f);
                        }
                        else
                            effect.DiffuseColor = new Vector3(1.0f, 0.0f, 0.0f);

                        Vector3 worldPosition = WorldObjects[i].Entry.Position;

                        effect.World = Matrix.CreateWorld(worldPosition, Vector3.Normalize(worldPosition - CameraManager.Position) * new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        effect.View = CameraManager.View;
                        effect.Projection = CameraManager.Projection;
                    }

                    mesh.Draw();
                }
            }

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Effects)
                Tool.Draw();
        }
    }
}