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

namespace OpenGames.MandelbrodViewer
{
    /// <summary>
    /// Логика взаимодействия для CIEditor.xaml
    /// </summary>
    public partial class CIEditor : Window
    {
        public string text;

        public CIEditor()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox1.Text = text;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            text = TextBox1.Text;
            DialogResult = true;
        }
    }
}
