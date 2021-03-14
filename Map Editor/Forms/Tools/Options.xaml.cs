using System.Windows;
using System.Windows.Controls;
using System;

namespace Map_Editor.Forms.Tools
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {        
        struct OptionChild
        {
            public string Name;
            public Type Control;
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        public Options()
        {
            InitializeComponent();

            AddCategory("Render Options", new OptionChild()
            {
                Name = "Objects/Features",
                Control = typeof(Controls.Options.ObjectsFeatures)
            });

            ((TreeViewItem)((TreeViewItem)CategoryTree.Items[0]).Items[0]).IsSelected = true;
        }

        private void AddCategory(string name, params OptionChild[] children)
        {
            TreeViewItem categoryItem = new TreeViewItem()
            {
                Header = name,
                IsExpanded = true
            };

            for (int i = 0; i < children.Length; i++)
            {
                TreeViewItem optionItem = new TreeViewItem()
                {
                    Header = children[i].Name
                };

                Type controlType = children[i].Control;

                optionItem.Selected += new RoutedEventHandler(delegate
                {
                    SettingsExpander.Content = Activator.CreateInstance(controlType);
                });

                categoryItem.Items.Add(optionItem);
            }

            CategoryTree.Items.Add(categoryItem);
        }
    }
}