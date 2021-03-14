using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Map_Editor.Engine;
using Map_Editor.Engine.Manipulation;
using Map_Editor.Engine.Map;
using Map_Editor.Forms;
using Map_Editor.Forms.Controls;
using Map_Editor.Forms.Controls.NPC;
using Map_Editor.Forms.Tools;
using Map_Editor.Misc;
using Microsoft.Xna.Framework;
using System.Windows.Controls;

namespace Map_Editor
{
    /// <summary>
    /// Main class.
    /// </summary>
    public partial class Main : Window
    {
        #region Component Accessors

        /// <summary>
        /// Gets the render panel.
        /// </summary>
        /// <value>The render panel.</value>
        public System.Windows.Forms.Panel RenderPanel
        {
            get { return (System.Windows.Forms.Panel)RenderPanelHost.Child; }
        }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the memory timer.
        /// </summary>
        /// <value>The memory timer.</value>
        private System.Windows.Forms.Timer memoryTimer { get; set; }

        /// <summary>
        /// Gets or sets the position timer.
        /// </summary>
        /// <value>The position timer.</value>
        private System.Windows.Forms.Timer positionTimer { get; set; }

        /// <summary>
        /// Gets or sets the preview panel.
        /// </summary>
        /// <value>The preview panel.</value>
        public PreviewPanel PreviewPanel { get; set; }

        /// <summary>
        /// Gets or sets the build version.
        /// </summary>
        /// <value>The build version.</value>
        public static string BuildVersion { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            InitializeComponent();

            BuildVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Output.Initialize(OutputList);

            Manipulation_Click(NoManipulation, null);

            memoryTimer = new System.Windows.Forms.Timer()
            {
                Interval = 5000,
                Enabled = true
            };

            memoryTimer.Tick += new EventHandler(memoryTimer_Tick);

            positionTimer = new System.Windows.Forms.Timer()
            {
                Interval = 50,
                Enabled = true
            };

            positionTimer.Tick += new EventHandler(positionTimer_Tick);
        }

        /// <summary>
        /// Handles the Tick event of the positionTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void positionTimer_Tick(object sender, EventArgs e)
        {
            if (MapManager.Heightmaps == null)
                return;

            Vector3 mousePosition = MapManager.Heightmaps.PickPosition();
            Vector2 blockPosition = MapManager.InsideBlock(mousePosition);

            Position.Text = string.Format("X: {0:0.00}; Y: {1:0.00}; Z: {2:0.00}; [{3:00}, {4:00}]", mousePosition.X, mousePosition.Y, mousePosition.Z, blockPosition.Y, blockPosition.X);
        }

        #region Form Events

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RightColumn.Width = new GridLength(ConfigurationManager.GetValue<int>("UI", "RightSplitterWidth"));
            OutputExpander.IsExpanded = ConfigurationManager.GetValue<bool>("UI", "OutputExpanded");

            App.Engine = new Engine.Main();
            App.Engine.Run();
        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConfigurationManager.SetValue("UI", "RightSplitterWidth", RightColumn.Width);
            ConfigurationManager.SetValue("UI", "OutputExpanded", OutputExpander.IsExpanded);

            ConfigurationManager.SaveConfig();
        }

        /// <summary>
        /// Handles the Closed event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Window_Closed(object sender, EventArgs e)
        {
            App.Engine.Exit();
        }

        #endregion

        #region Timers

        /// <summary>
        /// Handles the Tick event of the memoryTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void memoryTimer_Tick(object sender, EventArgs e)
        {
            GC.Collect();
        }

        #endregion

        #region Menu Events

        #region File Menu

        /// <summary>
        /// Handles the Click event of the New control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void New_Click(object sender, RoutedEventArgs e)
        {
            new New().ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the Open control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            new Open().ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the Save control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            new Thread(new ThreadStart(delegate
            {
                DateTime loadStart = DateTime.Now;

                string mapName = FileManager.STLs["LIST_ZONE_S"].Search(FileManager.STBs["LIST_ZONE"].Cells[MapManager.ID][27]);

                Output.WriteLine(Output.MessageType.Event, string.Format("Saving Map {0}", mapName));

                Output.WriteLine(Output.MessageType.Normal, "- Saving ZON File");

                FileManager.ZON.Save();

                Output.WriteLine(Output.MessageType.Normal, "- Saving IFO Files");

                FileManager.IFOs.ForEach(delegate(IFO file)
                {
                    file.Save();
                });

                Output.WriteLine(Output.MessageType.Normal, "- Saving TIL Files");

                for (int y = 0; y < 100; y++)
                {
                    for (int x = 0; x < 100; x++)
                    {
                        if (MapManager.Heightmaps.Blocks[y, x] == null)
                            continue;

                        MapManager.Heightmaps.Blocks[y, x].TileFile.Save();
                    }
                }

                Output.WriteLine(Output.MessageType.Normal, "- Saving HIM Files");

                for (int y = 0; y < 100; y++)
                {
                    for (int x = 0; x < 100; x++)
                    {
                        if (MapManager.Heightmaps.Blocks[y, x] == null)
                            continue;

                        MapManager.Heightmaps.Blocks[y, x].HeightFile.Save();
                    }
                }

                Output.WriteLine(Output.MessageType.Normal, "- Saving LIT Files");

                FileManager.DecorationLITs.ForEach(delegate(LIT lit)
                {
                    lit.Save();
                });

                FileManager.ConstructionLITs.ForEach(delegate(LIT lit)
                {
                    lit.Save();
                });

                Output.WriteLine(Output.MessageType.Event, string.Format("Saving Completed in {0} Second(s)", (DateTime.Now - loadStart).TotalSeconds));
            })).Start();
        }

        /// <summary>
        /// Handles the Click event of the Exit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Edit Menu

        /// <summary>
        /// Handles the Click event of the Undo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            UndoManager.Undo();
        }

        /// <summary>
        /// Handles the Click event of the Redo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            UndoManager.Redo();
        }

        /// <summary>
        /// Handles the Click event of the Cut control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            Copy_Click(null, null);

            switch (ToolManager.GetToolMode())
            {
                case ToolManager.ToolMode.Animation:
                    MapManager.Animation.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.Collision:
                    MapManager.Collision.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.Construction:
                    MapManager.Construction.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.Decoration:
                    MapManager.Decoration.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.Effects:
                    MapManager.Effects.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.EventTriggers:
                    MapManager.EventTriggers.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.Monsters:
                    MapManager.Monsters.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.NPCs:
                    MapManager.NPCs.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.Sounds:
                    MapManager.Sounds.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.SpawnPoints:
                    MapManager.SpawnPoints.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.WarpGates:
                    MapManager.WarpGates.Tool.Remove(false);
                    break;
                case ToolManager.ToolMode.Water:
                    MapManager.Water.Tool.Remove(false);
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the Copy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            switch (ToolManager.GetToolMode())
            {
                case ToolManager.ToolMode.Animation:
                    {
                        Forms.Controls.AnimationTool.CopiedObject = new IFO.BaseIFO()
                        {
                            Description = MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.Scale
                        };
                    }
                    break;
                case ToolManager.ToolMode.Collision:
                    {
                        Forms.Controls.CollisionTool.CopiedObject = new IFO.BaseIFO()
                        {
                            Description = MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.Scale
                        };
                    }
                    break;
                case ToolManager.ToolMode.Construction:
                    {
                        Forms.Controls.ConstructionTool.CopiedObject = new IFO.BaseIFO()
                        {
                            Description = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Scale
                        };
                    }
                    break;
                case ToolManager.ToolMode.Decoration:
                    {
                        Forms.Controls.DecorationTool.CopiedObject = new IFO.BaseIFO()
                        {
                            Description = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Scale
                        };
                    }
                    break;
                case ToolManager.ToolMode.Effects:
                    {
                        Forms.Controls.EffectTool.CopiedObject = new IFO.Effect()
                        {
                            Description = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.Scale,
                            Path = MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.Path
                        };
                    }
                    break;
                case ToolManager.ToolMode.EventTriggers:
                    {
                        Forms.Controls.EventTriggerTool.CopiedObject = new IFO.EventTrigger()
                        {
                            Description = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.Scale,
                            QSDTrigger = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.QSDTrigger,
                            LUATrigger = MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.LUATrigger
                        };
                    }
                    break;
                case ToolManager.ToolMode.Monsters:
                    {
                        Forms.Controls.MonsterTool.CopiedObject = new IFO.MonsterSpawn()
                        {
                            Description = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Scale,
                            Name = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Name,
                            Interval = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Interval,
                            Limit = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Limit,
                            Range = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Range,
                            TacticPoints = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.TacticPoints,
                            Basic = new List<IFO.MonsterSpawn.Monster>(),
                            Tactic = new List<IFO.MonsterSpawn.Monster>()
                        };

                        for (int i = 0; i < MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Basic.Count; i++)
                        {
                            Forms.Controls.MonsterTool.CopiedObject.Basic.Add(new IFO.MonsterSpawn.Monster()
                            {
                                Description = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Basic[i].Description,
                                ID = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Basic[i].ID,
                                Count = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Basic[i].Count
                            });
                        }

                        for (int i = 0; i < MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Tactic.Count; i++)
                        {
                            Forms.Controls.MonsterTool.CopiedObject.Tactic.Add(new IFO.MonsterSpawn.Monster()
                            {
                                Description = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Tactic[i].Description,
                                ID = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Tactic[i].ID,
                                Count = MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Tactic[i].Count
                            });
                        }
                    }
                    break;
                case ToolManager.ToolMode.NPCs:
                    {
                        Forms.Controls.NPCTool.CopiedObject = new IFO.NPC()
                        {
                            Description = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.Scale,
                            AIPatternIndex = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.AIPatternIndex,
                            Path = MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.Path
                        };
                    }
                    break;
                case ToolManager.ToolMode.Sounds:
                    {
                        Forms.Controls.SoundTool.CopiedObject = new IFO.Sound()
                        {
                            Description = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Scale,
                            Path = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Path,
                            Interval = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Interval,
                            Range = MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Range
                        };
                    }
                    break;
                case ToolManager.ToolMode.SpawnPoints:
                    {
                        Forms.Controls.SpawnPointTool.CopiedObject = new ZON.SpawnPoint()
                        {
                            Name = MapManager.SpawnPoints.WorldObjects[MapManager.SpawnPoints.Tool.SelectedObject].Entry.Name,
                            Position = MapManager.SpawnPoints.WorldObjects[MapManager.SpawnPoints.Tool.SelectedObject].Entry.Position,
                        };
                    }
                    break;
                case ToolManager.ToolMode.WarpGates:
                    {
                        Forms.Controls.WarpGateTool.CopiedObject = new IFO.BaseIFO()
                        {
                            Description = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.Description,
                            EventID = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.EventID,
                            WarpID = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.WarpID,
                            ObjectID = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.ObjectID,
                            ObjectType = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.ObjectType,
                            MapPosition = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.MapPosition,
                            Position = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.Position,
                            Rotation = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.Rotation,
                            Scale = MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.Scale
                        };
                    }
                    break;
            }

            Paste.IsEnabled = true;
            QuickPaste.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Click event of the Paste control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            switch (ToolManager.GetToolMode())
            {
                case ToolManager.ToolMode.Animation:
                    {
                        MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.Description = Forms.Controls.AnimationTool.CopiedObject.Description;
                        MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.EventID = Forms.Controls.AnimationTool.CopiedObject.EventID;
                        MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.WarpID = Forms.Controls.AnimationTool.CopiedObject.WarpID;
                        MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.AnimationTool.CopiedObject.ObjectID;
                        MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.AnimationTool.CopiedObject.ObjectType;
                        MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.Rotation = Forms.Controls.AnimationTool.CopiedObject.Rotation;
                        MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry.Scale = Forms.Controls.AnimationTool.CopiedObject.Scale;

                        MapManager.Animation.Add(MapManager.Animation.Tool.SelectedObject, MapManager.Animation.WorldObjects[MapManager.Animation.Tool.SelectedObject].Entry, true);
                        MapManager.Animation.Tool.Select(MapManager.Animation.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.Collision:
                    {
                        MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.Description = Forms.Controls.CollisionTool.CopiedObject.Description;
                        MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.EventID = Forms.Controls.CollisionTool.CopiedObject.EventID;
                        MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.WarpID = Forms.Controls.CollisionTool.CopiedObject.WarpID;
                        MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.CollisionTool.CopiedObject.ObjectID;
                        MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.CollisionTool.CopiedObject.ObjectType;
                        MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.Rotation = Forms.Controls.CollisionTool.CopiedObject.Rotation;
                        MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry.Scale = Forms.Controls.CollisionTool.CopiedObject.Scale;

                        MapManager.Collision.Add(MapManager.Collision.Tool.SelectedObject, MapManager.Collision.WorldObjects[MapManager.Collision.Tool.SelectedObject].Entry, true);
                        MapManager.Collision.Tool.Select(MapManager.Collision.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.Construction:
                    {
                        if (MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.ObjectID != Forms.Controls.ConstructionTool.CopiedObject.ObjectID)
                        {
                            UndoManager.AddCommand(new Engine.Commands.Construction.ObjectChanged()
                            {
                                ObjectID = MapManager.Construction.Tool.SelectedObject,
                                Object = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject],
                                OldObject = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.ObjectID,
                                NewObject = Forms.Controls.ConstructionTool.CopiedObject.ObjectID
                            });
                        }

                        UndoManager.AddCommand(new Engine.Commands.Construction.ValueChanged()
                        {
                            ObjectID = MapManager.Construction.Tool.SelectedObject,
                            Object = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject],
                            OldValue = new Engine.Commands.Construction.ValueChanged.ObjectValue()
                            {
                                Description = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Description,
                                EventID = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.EventID,
                                Position = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Position,
                                Rotation = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Rotation,
                                Scale = MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Scale
                            },
                            NewValue = new Engine.Commands.Construction.ValueChanged.ObjectValue()
                            {
                                Description = Forms.Controls.ConstructionTool.CopiedObject.Description,
                                EventID = Forms.Controls.ConstructionTool.CopiedObject.EventID,
                                Position = Forms.Controls.ConstructionTool.CopiedObject.Position,
                                Rotation = Forms.Controls.ConstructionTool.CopiedObject.Rotation,
                                Scale = Forms.Controls.ConstructionTool.CopiedObject.Scale
                            }
                        });

                        MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Description = Forms.Controls.ConstructionTool.CopiedObject.Description;
                        MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.EventID = Forms.Controls.ConstructionTool.CopiedObject.EventID;
                        MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.WarpID = Forms.Controls.ConstructionTool.CopiedObject.WarpID;
                        MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.ConstructionTool.CopiedObject.ObjectID;
                        MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.ConstructionTool.CopiedObject.ObjectType;
                        MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Rotation = Forms.Controls.ConstructionTool.CopiedObject.Rotation;
                        MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry.Scale = Forms.Controls.ConstructionTool.CopiedObject.Scale;

                        MapManager.Construction.Add(MapManager.Construction.Tool.SelectedObject, MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].Entry, MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].FileID, MapManager.Construction.WorldObjects[MapManager.Construction.Tool.SelectedObject].EntryID, true);
                        MapManager.Construction.Tool.Select(MapManager.Construction.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.Decoration:
                    {
                        if (MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.ObjectID != Forms.Controls.DecorationTool.CopiedObject.ObjectID)
                        {
                            UndoManager.AddCommand(new Engine.Commands.Decoration.ObjectChanged()
                            {
                                ObjectID = MapManager.Decoration.Tool.SelectedObject,
                                Object = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject],
                                OldObject = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.ObjectID,
                                NewObject = Forms.Controls.DecorationTool.CopiedObject.ObjectID
                            });
                        }

                        UndoManager.AddCommand(new Engine.Commands.Decoration.ValueChanged()
                        {
                            ObjectID = MapManager.Decoration.Tool.SelectedObject,
                            Object = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject],
                            OldValue = new Engine.Commands.Decoration.ValueChanged.ObjectValue()
                            {
                                Description = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Description,
                                EventID = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.EventID,
                                Position = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Position,
                                Rotation = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Rotation,
                                Scale = MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Scale
                            },
                            NewValue = new Engine.Commands.Decoration.ValueChanged.ObjectValue()
                            {
                                Description = Forms.Controls.DecorationTool.CopiedObject.Description,
                                EventID = Forms.Controls.DecorationTool.CopiedObject.EventID,
                                Position = Forms.Controls.DecorationTool.CopiedObject.Position,
                                Rotation = Forms.Controls.DecorationTool.CopiedObject.Rotation,
                                Scale = Forms.Controls.DecorationTool.CopiedObject.Scale
                            }
                        });

                        MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Description = Forms.Controls.DecorationTool.CopiedObject.Description;
                        MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.EventID = Forms.Controls.DecorationTool.CopiedObject.EventID;
                        MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.WarpID = Forms.Controls.DecorationTool.CopiedObject.WarpID;
                        MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.DecorationTool.CopiedObject.ObjectID;
                        MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.DecorationTool.CopiedObject.ObjectType;
                        MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Rotation = Forms.Controls.DecorationTool.CopiedObject.Rotation;
                        MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry.Scale = Forms.Controls.DecorationTool.CopiedObject.Scale;

                        MapManager.Decoration.Add(MapManager.Decoration.Tool.SelectedObject, MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].Entry, MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].FileID, MapManager.Decoration.WorldObjects[MapManager.Decoration.Tool.SelectedObject].EntryID, true);
                        MapManager.Decoration.Tool.Select(MapManager.Decoration.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.Effects:
                    {
                        MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.Description = Forms.Controls.EffectTool.CopiedObject.Description;
                        MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.EventID = Forms.Controls.EffectTool.CopiedObject.EventID;
                        MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.WarpID = Forms.Controls.EffectTool.CopiedObject.WarpID;
                        MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.EffectTool.CopiedObject.ObjectID;
                        MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.EffectTool.CopiedObject.ObjectType;
                        MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.Rotation = Forms.Controls.EffectTool.CopiedObject.Rotation;
                        MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.Scale = Forms.Controls.EffectTool.CopiedObject.Scale;
                        MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry.Path = Forms.Controls.EffectTool.CopiedObject.Path;

                        MapManager.Effects.Add(MapManager.Effects.Tool.SelectedObject, MapManager.Effects.WorldObjects[MapManager.Effects.Tool.SelectedObject].Entry, true);
                        MapManager.Effects.Tool.Select(MapManager.Effects.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.EventTriggers:
                    {
                        MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.Description = Forms.Controls.EventTriggerTool.CopiedObject.Description;
                        MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.EventID = Forms.Controls.EventTriggerTool.CopiedObject.EventID;
                        MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.WarpID = Forms.Controls.EventTriggerTool.CopiedObject.WarpID;
                        MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.EventTriggerTool.CopiedObject.ObjectID;
                        MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.EventTriggerTool.CopiedObject.ObjectType;
                        MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.Rotation = Forms.Controls.EventTriggerTool.CopiedObject.Rotation;
                        MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.Scale = Forms.Controls.EventTriggerTool.CopiedObject.Scale;
                        MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.QSDTrigger = Forms.Controls.EventTriggerTool.CopiedObject.QSDTrigger;
                        MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry.LUATrigger = Forms.Controls.EventTriggerTool.CopiedObject.LUATrigger;

                        MapManager.EventTriggers.Add(MapManager.EventTriggers.Tool.SelectedObject, MapManager.EventTriggers.WorldObjects[MapManager.EventTriggers.Tool.SelectedObject].Entry, true);
                        MapManager.EventTriggers.Tool.Select(MapManager.EventTriggers.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.Monsters:
                    {
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Description = Forms.Controls.MonsterTool.CopiedObject.Description;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.EventID = Forms.Controls.MonsterTool.CopiedObject.EventID;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.WarpID = Forms.Controls.MonsterTool.CopiedObject.WarpID;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.MonsterTool.CopiedObject.ObjectID;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.MonsterTool.CopiedObject.ObjectType;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Rotation = Forms.Controls.MonsterTool.CopiedObject.Rotation;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Scale = Forms.Controls.MonsterTool.CopiedObject.Scale;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Name = Forms.Controls.MonsterTool.CopiedObject.Name;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Interval = Forms.Controls.MonsterTool.CopiedObject.Interval;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Limit = Forms.Controls.MonsterTool.CopiedObject.Limit;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Range = Forms.Controls.MonsterTool.CopiedObject.Range;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.TacticPoints = Forms.Controls.MonsterTool.CopiedObject.TacticPoints;
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Basic.Clear();
                        MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Tactic.Clear();

                        for (int i = 0; i < Forms.Controls.MonsterTool.CopiedObject.Basic.Count; i++)
                        {
                            MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Basic.Add(new IFO.MonsterSpawn.Monster()
                            {
                                Description = Forms.Controls.MonsterTool.CopiedObject.Basic[i].Description,
                                ID = Forms.Controls.MonsterTool.CopiedObject.Basic[i].ID,
                                Count = Forms.Controls.MonsterTool.CopiedObject.Basic[i].Count
                            });
                        }

                        for (int i = 0; i < Forms.Controls.MonsterTool.CopiedObject.Tactic.Count; i++)
                        {
                            MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry.Tactic.Add(new IFO.MonsterSpawn.Monster()
                            {
                                Description = Forms.Controls.MonsterTool.CopiedObject.Tactic[i].Description,
                                ID = Forms.Controls.MonsterTool.CopiedObject.Tactic[i].ID,
                                Count = Forms.Controls.MonsterTool.CopiedObject.Tactic[i].Count
                            });
                        }

                        MapManager.Monsters.Add(MapManager.Monsters.Tool.SelectedObject, MapManager.Monsters.WorldObjects[MapManager.Monsters.Tool.SelectedObject].Entry, true);
                        MapManager.Monsters.Tool.Select(MapManager.Monsters.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.NPCs:
                    {
                        MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.Description = Forms.Controls.NPCTool.CopiedObject.Description;
                        MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.EventID = Forms.Controls.NPCTool.CopiedObject.EventID;
                        MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.WarpID = Forms.Controls.NPCTool.CopiedObject.WarpID;
                        MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.NPCTool.CopiedObject.ObjectID;
                        MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.NPCTool.CopiedObject.ObjectType;
                        MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.Rotation = Forms.Controls.NPCTool.CopiedObject.Rotation;
                        MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.Scale = Forms.Controls.NPCTool.CopiedObject.Scale;
                        MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.Path = Forms.Controls.NPCTool.CopiedObject.Path;
                        MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry.AIPatternIndex = Forms.Controls.NPCTool.CopiedObject.AIPatternIndex;

                        MapManager.NPCs.Add(MapManager.NPCs.Tool.SelectedObject, MapManager.NPCs.WorldObjects[MapManager.NPCs.Tool.SelectedObject].Entry, true);
                        MapManager.NPCs.Tool.Select(MapManager.NPCs.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.Sounds:
                    {
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Description = Forms.Controls.SoundTool.CopiedObject.Description;
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.EventID = Forms.Controls.SoundTool.CopiedObject.EventID;
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.WarpID = Forms.Controls.SoundTool.CopiedObject.WarpID;
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.SoundTool.CopiedObject.ObjectID;
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.SoundTool.CopiedObject.ObjectType;
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Rotation = Forms.Controls.SoundTool.CopiedObject.Rotation;
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Scale = Forms.Controls.SoundTool.CopiedObject.Scale;
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Path = Forms.Controls.SoundTool.CopiedObject.Path;
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Range = Forms.Controls.SoundTool.CopiedObject.Range;
                        MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry.Interval = Forms.Controls.SoundTool.CopiedObject.Interval;

                        MapManager.Sounds.Add(MapManager.Sounds.Tool.SelectedObject, MapManager.Sounds.WorldObjects[MapManager.Sounds.Tool.SelectedObject].Entry, true);
                        MapManager.Sounds.Tool.Select(MapManager.Sounds.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.SpawnPoints:
                    {
                        MapManager.SpawnPoints.WorldObjects[MapManager.SpawnPoints.Tool.SelectedObject].Entry.Name = Forms.Controls.SpawnPointTool.CopiedObject.Name;

                        MapManager.SpawnPoints.Add(MapManager.SpawnPoints.Tool.SelectedObject, MapManager.SpawnPoints.WorldObjects[MapManager.SpawnPoints.Tool.SelectedObject].Entry, true);
                        MapManager.SpawnPoints.Tool.Select(MapManager.SpawnPoints.Tool.SelectedObject);
                    }
                    break;
                case ToolManager.ToolMode.WarpGates:
                    {
                        MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.Description = Forms.Controls.WarpGateTool.CopiedObject.Description;
                        MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.EventID = Forms.Controls.WarpGateTool.CopiedObject.EventID;
                        MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.WarpID = Forms.Controls.WarpGateTool.CopiedObject.WarpID;
                        MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.ObjectID = Forms.Controls.WarpGateTool.CopiedObject.ObjectID;
                        MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.ObjectType = Forms.Controls.WarpGateTool.CopiedObject.ObjectType;
                        MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.Rotation = Forms.Controls.WarpGateTool.CopiedObject.Rotation;
                        MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry.Scale = Forms.Controls.WarpGateTool.CopiedObject.Scale;

                        MapManager.WarpGates.Add(MapManager.WarpGates.Tool.SelectedObject, MapManager.WarpGates.WorldObjects[MapManager.WarpGates.Tool.SelectedObject].Entry, true);
                        MapManager.WarpGates.Tool.Select(MapManager.WarpGates.Tool.SelectedObject);
                    }
                    break;
            }
        }

        #endregion

        #region Help Menu

        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("Map Editor\nBuild: {0}\n\nCopyright © 2009 Jack \"xadet\" W.\n\nThis application may not be sold without permission.\nThis application may not be distributed to any persons\nother than those working on the server\nthis application was purchased by.\n\nUser acknowledges that he or she has\nagreed to these terms by using this application", BuildVersion), "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #endregion

        #region Tool Bar Events

        /// <summary>
        /// Handles the Click event of the Tool control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Tool_Click(object sender, RoutedEventArgs e)
        {
            if (PreviewPanel.IsVisible)
                PreviewPanel.Hide();

            switch (ToolManager.GetToolMode())
            {
                case ToolManager.ToolMode.Height:
                    {
                        for (int y = 0; y < 100; y++)
                        {
                            for (int x = 0; x < 100; x++)
                            {
                                if (MapManager.Heightmaps.Blocks[y, x] == null)
                                    continue;

                                MapManager.Heightmaps.Blocks[y, x].BuildBuffer();
                            }
                        }
                    }
                    break;
                case ToolManager.ToolMode.Tiles:
                    {
                        Forms.Controls.TileTool.ImageThread.Abort();
                        Forms.Controls.TileTool.GeneratingImages = false;

                        for (int y = 0; y < 100; y++)
                        {
                            for (int x = 0; x < 100; x++)
                            {
                                if (MapManager.Heightmaps.Blocks[y, x] == null)
                                    continue;

                                MapManager.Heightmaps.Blocks[y, x].BuildBuffer();
                            }
                        }
                    }
                    break;
                case ToolManager.ToolMode.Brush:
                    {
                        Forms.Controls.BrushTool.ImageThread.Abort();
                        Forms.Controls.BrushTool.GeneratingImages = false;

                        for (int y = 0; y < 100; y++)
                        {
                            for (int x = 0; x < 100; x++)
                            {
                                if (MapManager.Heightmaps.Blocks[y, x] == null)
                                    continue;

                                MapManager.Heightmaps.Blocks[y, x].BuildBuffer();
                            }
                        }
                    }
                    break;
                case ToolManager.ToolMode.Decoration:
                    {
                        Forms.Controls.DecorationTool.ImageThread.Abort();
                        Forms.Controls.DecorationTool.GeneratingImages = false;
                    }
                    break;
                case ToolManager.ToolMode.Construction:
                    {
                        Forms.Controls.ConstructionTool.ImageThread.Abort();
                        Forms.Controls.ConstructionTool.GeneratingImages = false;
                    }
                    break;
                case ToolManager.ToolMode.EventTriggers:
                    {
                        Forms.Controls.EventTriggerTool.ImageThread.Abort();
                        Forms.Controls.EventTriggerTool.GeneratingImages = false;
                    }
                    break;
                case ToolManager.ToolMode.Animation:
                    {
                        Forms.Controls.AnimationTool.ImageThread.Abort();
                        Forms.Controls.AnimationTool.GeneratingImages = false;
                    }
                    break;
                case ToolManager.ToolMode.Sounds:
                    {
                        ((SoundTool)ToolHost.Content).Clean();

                        PlaySound.IsEnabled = false;
                    }
                    break;
            }

            ToolHost.Content = null;
            ToolManager.SetToolMode(ToolManager.ToolMode.None);

            ToggleButton toolButton = (ToggleButton)sender;

            for (int i = 0; i < ToolToolBar.Items.Count; i++)
            {
                if (ToolToolBar.Items[i].GetType() == typeof(ToggleButton))
                    ((ToggleButton)ToolToolBar.Items[i]).IsChecked = false;
            }

            Cut.IsEnabled = false;
            QuickCut.IsEnabled = false;

            Copy.IsEnabled = false;
            QuickCopy.IsEnabled = false;

            Paste.IsEnabled = false;
            QuickPaste.IsEnabled = false;

            toolButton.IsChecked = true;

            ToolManager.Position = new Translate(App.Engine.GraphicsDevice);
            ToolManager.Cursor = new CursorTranslate(App.Engine.GraphicsDevice);

            switch (toolButton.Name)
            {
                case "HeightTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Height);
                        ToolHost.Content = new HeightTool();
                    }
                    break;
                case "TileTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Tiles);
                        ToolHost.Content = new TileTool();
                    }
                    break;
                case "BrushTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Brush);
                        ToolHost.Content = new BrushTool();
                    }
                    break;
                case "DecorationTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Decoration);
                        ToolHost.Content = new DecorationTool();
                    }
                    break;
                case "ConstructionTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Construction);
                        ToolHost.Content = new ConstructionTool();
                    }
                    break;
                case "EventTriggerTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.EventTriggers);
                        ToolHost.Content = new EventTriggerTool();
                    }
                    break;
                case "NPCTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.NPCs);
                        ToolHost.Content = new NPCTool();
                    }
                    break;
                case "MonsterTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Monsters);
                        ToolHost.Content = new MonsterTool();
                    }
                    break;
                case "SpawnPointTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.SpawnPoints);
                        ToolHost.Content = new SpawnPointTool();
                    }
                    break;
                case "WarpGateTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.WarpGates);
                        ToolHost.Content = new WarpGateTool();
                    }
                    break;
                case "SoundTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Sounds);
                        ToolHost.Content = new SoundTool();
                    }
                    break;
                case "EffectTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Effects);
                        ToolHost.Content = new EffectTool();
                    }
                    break;
                case "CollisionTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Collision);
                        ToolHost.Content = new CollisionTool();
                    }
                    break;
                case "WaterTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Water);
                        ToolHost.Content = new WaterTool();
                    }
                    break;
                case "AnimationTool":
                    {
                        ToolManager.SetToolMode(ToolManager.ToolMode.Animation);
                        ToolHost.Content = new AnimationTool();
                    }
                    break;
            }

            if (ToolHost.Content != null)
                ((Control)ToolHost.Content).Width = double.NaN;
        }

        /// <summary>
        /// Handles the Click event of the Manipulation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void Manipulation_Click(object sender, RoutedEventArgs e)
        {
            ToolManager.SetManipulationMode(ToolManager.ManipulationMode.None);

            ToggleButton toolButton = (ToggleButton)sender;

            for (int i = 0; i < ManipulationToolBar.Items.Count; i++)
            {
                if (ManipulationToolBar.Items[i].GetType() != typeof(ToggleButton) || !((ToggleButton)ManipulationToolBar.Items[i]).Name.EndsWith("Manipulation"))
                    continue;

                ((ToggleButton)ManipulationToolBar.Items[i]).IsChecked = false;
            }

            toolButton.IsChecked = true;

            switch (toolButton.Name)
            {
                case "CursorManipulation":
                    ToolManager.SetManipulationMode(ToolManager.ManipulationMode.Cursor);
                    break;
                case "PositionManipulation":
                    ToolManager.SetManipulationMode(ToolManager.ManipulationMode.Position);
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the PlaySound control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PlaySound_Click(object sender, RoutedEventArgs e)
        {
            ((SoundTool)ToolHost.Content).Play();
        }

        /// <summary>
        /// Handles the Click event of the PauseSound control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PauseSound_Click(object sender, RoutedEventArgs e)
        {
            PauseSound.IsChecked = true;

            ((SoundTool)ToolHost.Content).Pause();
        }

        /// <summary>
        /// Handles the Click event of the StopSound control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void StopSound_Click(object sender, RoutedEventArgs e)
        {
            ((SoundTool)ToolHost.Content).Stop();
        }

        #endregion

        /// <summary>
        /// Resets the form.
        /// </summary>
        public void ResetForm()
        {
            switch (ToolManager.GetToolMode())
            {
                case ToolManager.ToolMode.Sounds:
                    {
                        ((SoundTool)ToolHost.Content).Clean();

                        PlaySound.IsEnabled = false;
                    }
                    break;
            }

            ToolHost.Content = null;

            ToolManager.SetToolMode(ToolManager.ToolMode.None);
            ToolManager.SetManipulationMode(ToolManager.ManipulationMode.None);

            ToolManager.Position = new Translate(App.Engine.GraphicsDevice);
            ToolManager.Cursor = new CursorTranslate(App.Engine.GraphicsDevice);
            
            for (int i = 0; i < ToolToolBar.Items.Count; i++)
            {
                if (ToolToolBar.Items[i].GetType() == typeof(ToggleButton))
                    ((ToggleButton)ToolToolBar.Items[i]).IsChecked = false;
            }

            Cut.IsEnabled = false;
            QuickCut.IsEnabled = false;

            Copy.IsEnabled = false;
            QuickCopy.IsEnabled = false;

            Paste.IsEnabled = false;
            QuickPaste.IsEnabled = false;

            UndoManager.Initialize();
        }

        /// <summary>
        /// Freezes the form disabling user interaction.
        /// </summary>
        public void Freeze()
        {
            New.IsEnabled = false;
            Open.IsEnabled = false;
            Save.IsEnabled = false;

            QuickNew.IsEnabled = false;
            QuickOpen.IsEnabled = false;
            QuickSave.IsEnabled = false;

            Cut.IsEnabled = false;
            Copy.IsEnabled = false;
            Paste.IsEnabled = false;

            QuickCut.IsEnabled = false;
            QuickCopy.IsEnabled = false;
            QuickPaste.IsEnabled = false;

            NoManipulation.IsEnabled = false;
            CursorManipulation.IsEnabled = false;
            PositionManipulation.IsEnabled = false;
            FollowTerrain.IsEnabled = false;
            SnapToGrid.IsEnabled = false;

            HeightTool.IsEnabled = false;
            TileTool.IsEnabled = false;
            BrushTool.IsEnabled = false;
            DecorationTool.IsEnabled = false;
            ConstructionTool.IsEnabled = false;
            NPCTool.IsEnabled = false;
            MonsterTool.IsEnabled = false;
            SpawnPointTool.IsEnabled = false;
            WarpGateTool.IsEnabled = false;
            EventTriggerTool.IsEnabled = false;
            SoundTool.IsEnabled = false;
            EffectTool.IsEnabled = false;
            CollisionTool.IsEnabled = false;
            WaterTool.IsEnabled = false;
            AnimationTool.IsEnabled = false;
        }

        /// <summary>
        /// UnFreeze the form enabling user interaction.
        /// </summary>
        public void UnFreeze()
        {
            New.IsEnabled = true;
            Open.IsEnabled = true;
            Save.IsEnabled = true;

            QuickNew.IsEnabled = true;
            QuickOpen.IsEnabled = true;
            QuickSave.IsEnabled = true;

            NoManipulation.IsEnabled = true;
            CursorManipulation.IsEnabled = true;
            PositionManipulation.IsEnabled = true;
            FollowTerrain.IsEnabled = true;
            SnapToGrid.IsEnabled = true;

            HeightTool.IsEnabled = true;
            TileTool.IsEnabled = true;
            BrushTool.IsEnabled = true;
            DecorationTool.IsEnabled = true;
            ConstructionTool.IsEnabled = true;
            NPCTool.IsEnabled = true;
            MonsterTool.IsEnabled = true;
            SpawnPointTool.IsEnabled = true;
            WarpGateTool.IsEnabled = true;
            EventTriggerTool.IsEnabled = true;
            SoundTool.IsEnabled = true;
            EffectTool.IsEnabled = true;
            CollisionTool.IsEnabled = true;
            WaterTool.IsEnabled = true;
            AnimationTool.IsEnabled = true;

            if (SnapToGrid.SelectedIndex == -1)
                SnapToGrid.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Click event of the Panel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Panel_Click(object sender, EventArgs e)
        {
            RenderPanel.Focus();
        }

        #region Key Up Events

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyUp"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O && Open.IsEnabled)
                Open_Click(null, null);
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S && Save.IsEnabled)
                Save_Click(null, null);
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z && Undo.IsEnabled)
                Undo_Click(null, null);
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Y && Redo.IsEnabled)
                Redo_Click(null, null);
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X && Cut.IsEnabled)
                Cut_Click(null, null);
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C && Copy.IsEnabled)
                Copy_Click(null, null);
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V && Paste.IsEnabled)
                Paste_Click(null, null);

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Handles the KeyUp event of the Panel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void Panel_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == System.Windows.Forms.Keys.O && Open.IsEnabled)
                Open_Click(null, null);
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.S && Save.IsEnabled)
                Save_Click(null, null);
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.Z && Undo.IsEnabled)
                Undo_Click(null, null);
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.Y && Redo.IsEnabled)
                Redo_Click(null, null);
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.X && Cut.IsEnabled)
                Cut_Click(null, null);
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.C && Copy.IsEnabled)
                Copy_Click(null, null);
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.V && Paste.IsEnabled)
                Paste_Click(null, null);
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the Options control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Options_Click(object sender, RoutedEventArgs e)
        {
            new Options().ShowDialog();
        }
    }
}