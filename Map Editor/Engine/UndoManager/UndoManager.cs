using Map_Editor.Engine.Commands.Interfaces;

namespace Map_Editor.Engine
{
    /// <summary>
    /// Undo Manager class.
    /// </summary>
    public static class UndoManager
    {
        /// <summary>
        /// Undo limit.
        /// </summary>
        const int UNDO_LIMIT = 100;

        #region Member Declarations

        /// <summary>
        /// Gets or sets the commands.
        /// </summary>
        /// <value>The commands.</value>
        public static ICommand[] Commands { get; set; }

        /// <summary>
        /// Gets or sets the current command.
        /// </summary>
        /// <value>The current command.</value>
        static int currentCommand { get; set; }

        #endregion

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            Commands = new ICommand[UNDO_LIMIT];

            currentCommand = -1;

            Update();
        }

        /// <summary>
        /// Adds a command.
        /// </summary>
        /// <param name="command">The command.</param>
        public static void AddCommand(ICommand command)
        {
            if (currentCommand == -1)
                Commands[currentCommand = 0] = command;
            else if (currentCommand == UNDO_LIMIT - 1)
            {
                for (int i = 1; i < UNDO_LIMIT; i++)
                    Commands[i - 1] = Commands[i];

                Commands[UNDO_LIMIT - 1] = command;
            }
            else
            {
                for (int i = currentCommand + 1; i < UNDO_LIMIT; i++)
                    Commands[i] = null;

                Commands[++currentCommand] = command;
            }

            Update();
        }

        /// <summary>
        /// Undo an action.
        /// </summary>
        public static void Undo()
        {
            if (currentCommand == -1)
                return;

            Commands[currentCommand--].Undo();

            Update();
        }

        /// <summary>
        /// Redo an action.
        /// </summary>
        public static void Redo()
        {
            if (currentCommand == UNDO_LIMIT - 1)
                return;

            Commands[++currentCommand].Redo();

            Update();
        }

        /// <summary>
        /// Updates the form.
        /// </summary>
        public static void Update()
        {
            if (currentCommand == -1)
            {
                App.Form.Undo.Header = "Undo";

                App.Form.Undo.IsEnabled = false;
                App.Form.QuickUndo.IsEnabled = false;
            }
            else
            {
                App.Form.Undo.Header = string.Format("Undo \"{0}\"", Commands[currentCommand].GetName());

                App.Form.Undo.IsEnabled = true;
                App.Form.QuickUndo.IsEnabled = true;
            }

            if (currentCommand == UNDO_LIMIT - 1 || Commands[currentCommand + 1] == null)
            {
                App.Form.Redo.Header = "Redo";

                App.Form.Redo.IsEnabled = false;
                App.Form.QuickRedo.IsEnabled = false;
            }
            else
            {
                App.Form.Redo.Header = string.Format("Redo \"{0}\"", Commands[currentCommand + 1].GetName());

                App.Form.Redo.IsEnabled = true;
                App.Form.QuickRedo.IsEnabled = true;
            }
        }
    }
}