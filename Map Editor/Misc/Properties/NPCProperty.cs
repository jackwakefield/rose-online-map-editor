using System.ComponentModel;
using System.Drawing.Design;
using System.Windows;
using Map_Editor.Engine;
using Map_Editor.Misc.Properties.Editors;
using Microsoft.Xna.Framework;

namespace Map_Editor.Misc.Properties
{
    /// <summary>
    /// NPC Property class.
    /// </summary>
    public class NPCProperty
    {
        #region Delegates

        /// <summary>
        /// ObjectID delegate.
        /// </summary>
        public delegate void ObjectID(int value);

        #endregion

        #region Member Declarations

        /// <summary>
        /// Object ID.
        /// </summary>
        private int objectID;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [object ID changed].
        /// </summary>
        public event ObjectID ObjectIDChanged;

        #endregion

        /// <summary>
        /// Gets or sets the character ID.
        /// </summary>
        /// <value>The character ID.</value>
        [DisplayName("\t\t\tCharacter ID")]
        [Category("\t\t\tObject")]
        [Editor(typeof(CharacterEditor), typeof(UITypeEditor))]
        public int CharacterID
        {
            get { return objectID; }
            set
            {
                if (value >= FileManager.STBs["LIST_NPC"].Cells.Count)
                {
                    MessageBox.Show("Invalid value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                objectID = value;

                if (ObjectIDChanged != null)
                    ObjectIDChanged(objectID);
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DisplayName("\t\tDescription")]
        [Category("\t\t\tObject")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the event ID.
        /// </summary>
        /// <value>The event ID.</value>
        [DisplayName("\tEvent ID")]
        [Category("\t\t\tObject")]
        public short EventID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the AI pattern ID.
        /// </summary>
        /// <value>The AI pattern ID.</value>
        [DisplayName("\t\tAI Pattern ID")]
        [Category("\t\tCharacter")]
        public int AIPatternID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dialog.
        /// </summary>
        /// <value>The dialog.</value>
        [DisplayName("\tDialog")]
        [Category("\t\tCharacter")]
        public string Dialog
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [DisplayName("\t\tPosition")]
        [Category("\tWorld")]
        public Vector3 Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        [DisplayName("\tRotation")]
        [Category("\tWorld")]
        public Quaternion Rotation
        {
            get;
            set;
        }
    }
}