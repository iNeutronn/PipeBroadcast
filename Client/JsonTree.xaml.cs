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
            // Implement JSON parsing and tree view loading logic here
            // This could involve using Newtonsoft.Json or your preferred JSON library
            // For simplicity, let's assume you have a JsonTreeViewItem class and LoadTreeView method
            var jsonObject = JObject.Parse(jsonString);

            treeView.Items.Add(Convert(jsonObject));

           // var jsonTreeViewItem = ParseJson(jsonString);
            //LoadTreeView(jsonObject);
        }

        private JsonTreeViewItem ParseJson(string jsonString)
        {
            // Implement your JSON parsing logic here
            // This could involve using Newtonsoft.Json or your preferred JSON library
            // For simplicity, let's assume you have a JsonTreeViewItem class
            // that represents each item in the tree
            // You may want to implement the actual parsing based on your JSON structure
            return new JsonTreeViewItem { Key = "Root", Value = "RootValue" };
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

        //private void LoadTreeView(JToken token)
        //{
        //    if (token.Type == JTokenType.Object)
        //    {
        //        foreach (var child in token.Children<JProperty>())
        //        {
        //            var childItem = ConvertToTreeViewItem(child.Value);
        //            childItem.Key = child.Name;
        //            item.Children.Add(childItem);
        //        }
        //    }
        //    else if (token.Type == JTokenType.Array)
        //    {
        //        int index = 0;
        //        foreach (var child in token.Children())
        //        {
        //            var childItem = ConvertToTreeViewItem(child);
        //            childItem.Key = "[" + index + "]";
        //            item.Children.Add(childItem);
        //            index++;
        //        }
        //    }
        //    else
        //    {
        //        item.Value = token.ToString();
        //    }
        //    //treeView.ItemsSource = new[] { ConvertToTreeViewItem(token) };
        //}

        private JsonTreeViewItem ConvertToTreeViewItem(JsonTreeViewItem item)
        {
            // Implement logic to convert JsonTreeViewItem to TreeViewItem
            // This could involve recursive methods to handle child items
            // For simplicity, let's assume you have a method that converts
            // a JsonTreeViewItem to a TreeViewItem
            return item; // Dummy implementation for simplicity
        }
    }

    public class JsonTreeViewItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public List<JsonTreeViewItem> Children { get; } = new List<JsonTreeViewItem>();
    }
}
