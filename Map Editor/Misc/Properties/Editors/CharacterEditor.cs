using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Map_Editor.Misc.Properties.Editors
{
    /// <summary>
    /// Character Editor class
    /// </summary>
    public class CharacterEditor : UITypeEditor
    {
        #region Member Declarations

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>The service.</value>
        private IWindowsFormsEditorService service { get; set; }

        #endregion

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (service != null)
                {
                    App.Form.PreviewPanel.Reset();

                    App.Form.PreviewPanel.OnSelect += new EventHandler(delegate
                    {
                        App.Form.PreviewPanel.Hide();

                        if (context.Instance.GetType() == typeof(NPCProperty))
                            ((NPCProperty)context.Instance).CharacterID = App.Form.PreviewPanel.NPCList.SelectedIndex + 1;
                        else if (context.Instance.GetType() == typeof(MonsterProperty))
                            ((MonsterProperty)context.Instance).ID = App.Form.PreviewPanel.NPCList.SelectedIndex + 1;
                    });

                    if (context.Instance.GetType() == typeof(NPCProperty))
                        App.Form.PreviewPanel.NPCList.SelectedIndex = ((NPCProperty)context.Instance).CharacterID - 1;
                    else if (context.Instance.GetType() == typeof(MonsterProperty))
                        App.Form.PreviewPanel.NPCList.SelectedIndex = ((MonsterProperty)context.Instance).ID - 1;
                    
                    App.Form.PreviewPanel.NPCList.ScrollIntoView(App.Form.PreviewPanel.NPCList.Items[App.Form.PreviewPanel.NPCList.SelectedIndex]);

                    App.Form.PreviewPanel.Show();
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
                return UITypeEditorEditStyle.Modal;

            return base.GetEditStyle(context);
        }
    }
}