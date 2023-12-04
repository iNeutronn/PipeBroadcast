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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            qwerty.JsonString = "{\r\n  \"boolean_key\": \"--- true\\n\",\r\n  \"empty_string_translation\": \"\",\r\n  \"key_with_description\": \"Check it out! This key has a description! (At least in some formats)\",\r\n  \"key_with_line-break\": \"This translations contains\\na line-break.\",\r\n  \"nested.deeply.key\": \"Wow, this key is nested even deeper.\",\r\n  \"nested.key\": \"This key is nested inside a namespace.\",\r\n  \"null_translation\": null,\r\n  \"pluralized_key\": {\r\n    \"one\": \"Only one pluralization found.\",\r\n    \"other\": \"Wow, you have %s pluralizations!\",\r\n    \"zero\": \"You have no pluralization.\"\r\n  },\r\n  \"sample_collection\": [\r\n    \"first item\",\r\n    \"second item\",\r\n    \"third item\"\r\n  ],\r\n  \"simple_key\": \"Just a simple key with a simple message.\",\r\n  \"unverified_key\": \"This translation is not yet verified and waits for it. (In some formats we also export this status)\"\r\n}";
        }
         
    }
}
