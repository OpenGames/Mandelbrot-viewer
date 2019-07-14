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
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public System.Windows.Point start = new System.Windows.Point();
        public System.Windows.Point end = new System.Windows.Point();

        bool leftMousePressed = false;
        bool movingAround = false;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //Close();
            }
            if(e.Key == Key.Escape)
            {
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(SettingsWindow1), SettingsWindow1);
            }
            if (e.Key == Key.LeftShift)
            {
                movingAround = true;
                Cursor = Cursors.Cross;
            }
        }

        private void SettingsWindow1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                movingAround = false;
                Cursor = Cursors.Arrow;
            }
        }

        private void SettingsWindow1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            start = Mouse.GetPosition(this);
            leftMousePressed = true;
        }

        private void SettingsWindow1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            end = Mouse.GetPosition(this);
            leftMousePressed = false;
        }

        private void SettingsWindow1_MouseMove(object sender, MouseEventArgs e)
        {
            if (movingAround && leftMousePressed)
            {
                SettingsWindow1.Left += (Mouse.GetPosition(this).X - start.X);
                SettingsWindow1.Top += (Mouse.GetPosition(this).Y - start.Y);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
