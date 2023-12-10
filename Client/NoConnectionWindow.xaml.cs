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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for NoConnection.xaml
    /// </summary>
    public partial class NoConnectionWindow : Window
    {
        public NoConnectionWindow()
        {
            InitializeComponent();
            Closing += NoConnectionWindow_Closing;
        }

        private void NoConnectionWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Close();
            });          
        }
    }
}
