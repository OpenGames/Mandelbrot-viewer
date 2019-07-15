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
        private System.Windows.Point start = new System.Windows.Point();
        private System.Windows.Point end = new System.Windows.Point();

        private Settings settings;

        bool leftMousePressed = false;
        bool movingAround = false;

        internal Settings Settings { get => settings; set => settings = value; }

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
            this.DialogResult = true;
            Close();
        }

        private void SettingsWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            WidthTextbox.Text = settings.resolutionWidth.ToString();
            HeightTextbox.Text = settings.resolutionHeight.ToString();
            ARXTextbox.Text = settings.aspectRatioX.ToString();
            ARYTextbox.Text = settings.aspectRatioY.ToString();
            ARLockedCheckBox.IsChecked = settings.aspectRatioLocked;

            ARXTextbox.IsEnabled = false;
            ARYTextbox.IsEnabled = false;

            if (settings.aspectRatioLocked)
            {
                WidthTextbox.IsEnabled = true;
                HeightTextbox.IsEnabled = false;
            }
            else
            {
                WidthTextbox.IsEnabled = true;
                HeightTextbox.IsEnabled = true;
            }
        }

        private void WidthTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (settings.aspectRatioLocked)
                {         
                    HeightTextbox.Text = Convert.ToInt32(Convert.ToInt32(WidthTextbox.Text) / settings.aspectRatio).ToString(); 
                }
                else
                {
                    var r = Configurator.GCD(Convert.ToInt32(WidthTextbox.Text), Convert.ToInt32(HeightTextbox.Text));
                    ARXTextbox.Text = (Convert.ToInt32(WidthTextbox.Text) / r).ToString();
                    ARYTextbox.Text = (Convert.ToInt32(HeightTextbox.Text) / r).ToString();

                    settings.aspectRatioX = Convert.ToInt32(ARXTextbox.Text);
                    settings.aspectRatioY = Convert.ToInt32(ARYTextbox.Text);
                    settings.aspectRatio = (double)settings.aspectRatioX / settings.aspectRatioY;
                }

                settings.resolutionHeight = Convert.ToInt32(HeightTextbox.Text);
                settings.resolutionWidth = Convert.ToInt32(WidthTextbox.Text);
            }
            catch
            {
            }
        }
        private void HeightTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (settings.aspectRatioLocked)
                {
                    //WidthTextbox.Text = (Convert.ToInt32(HeightTextbox.Text) * settings.aspectRatio).ToString();
                }
                else
                {
                    var r = Configurator.GCD(Convert.ToInt32(WidthTextbox.Text), Convert.ToInt32(HeightTextbox.Text));
                    ARXTextbox.Text = (Convert.ToInt32(WidthTextbox.Text) / r).ToString();
                    ARYTextbox.Text = (Convert.ToInt32(HeightTextbox.Text) / r).ToString();

                    settings.aspectRatioX = Convert.ToInt32(ARXTextbox.Text);
                    settings.aspectRatioY = Convert.ToInt32(ARYTextbox.Text);
                    settings.aspectRatio = (double)settings.aspectRatioX / settings.aspectRatioY;
                }

                settings.resolutionHeight = Convert.ToInt32(HeightTextbox.Text);
                settings.resolutionWidth = Convert.ToInt32(WidthTextbox.Text);
            }
            catch
            {
            }
        }

        private void ARLockedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            settings.aspectRatioLocked = (bool)ARLockedCheckBox.IsChecked;
            if (settings.aspectRatioLocked)
            {
                WidthTextbox.IsEnabled = true;
                HeightTextbox.IsEnabled = false;
            }
            else
            {
                WidthTextbox.IsEnabled = true;
                HeightTextbox.IsEnabled = true;
            }
        }

    }
}
