using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for JsonTree.xaml
    /// </summary>
    public partial class JsonTreeViewControl : UserControl
    {
    
        public JsonTreeViewControl()
        {
            InitializeComponent();
            DataContext = this; // Set DataContext to self for data binding
        }

        public string JsonString
        {
            get { return (string)GetValue(JsonStringProperty); }
            set { SetValue(JsonStringProperty, value); }
        }

        public static readonly DependencyProperty JsonStringProperty =
            DependencyProperty.Register("JsonString", typeof(string), typeof(JsonTreeViewControl), new PropertyMetadata(OnJsonStringPropertyChanged));

        private static void OnJsonStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JsonTreeViewControl jsonTreeViewControl)
            {
                jsonTreeViewControl.LoadJson(e.NewValue as string);
            }
        }

        private void LoadJson(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);

            treeView.Items.Add(Convert(jsonObject));
        }

        private TreeViewItem Convert(JToken token)
        {
            TreeViewItem item = new TreeViewItem();

            if (token.Type == JTokenType.Object)
            {
                item.Header = "{} " + item.Header;

                foreach (var child in token.Children<JProperty>())
                {
                    if(child.Value.Type == JTokenType.Object ||
                        child.Value.Type == JTokenType.Array)
                    {
                        TreeViewItem childItem = Convert(child.Value);

                        if (child.Value.Type == JTokenType.Object) childItem.Header = "{} " + $"{child.Name}";
                        else childItem.Header = "[] " + $"{child.Name}";

                        item.Items.Add(childItem);
                    }
                    else
                    {
                        item.Items.Add(Convert(child));
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                item.Header = "[] " + item.Header;

                int index = 0;
                foreach (var child in token.Children())
                {
                    TreeViewItem childItem = Convert(child);

                    childItem.Header = $"{index} : {childItem.Header}";

                    item.Items.Add(childItem);
                    index++;
                }
            }
            else
            {
                item.Header = token.ToString();
            }

            return item;
        }

    }

}
