namespace Map_Editor.Engine.Commands.Interfaces
{
    /// <summary>
    /// ICommand interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Undo command.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo command.
        /// </summary>
        void Redo();

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>The name.</returns>
        string GetName();
    }
}
