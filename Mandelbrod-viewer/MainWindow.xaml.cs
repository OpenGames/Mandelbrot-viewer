using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace OpenGames.MandelbrodViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string InterpolationCode = @"(color1, color2, fraction) => { 
                return Color.FromArgb(
                (int)((color2.R - color1.R) *  Math.Pow(Math.Sin(Math.PI*fraction), 2)   ) + color1.R,
                (int)((color2.G - color1.G) *  Math.Pow(Math.Sin(Math.PI*fraction), 2)   ) + color1.G,
                (int)((color2.B - color1.B) *  Math.Pow(Math.Sin(Math.PI*fraction), 2)   ) + color1.B
                );
            }";

        // 1/(1+e^(-10(x- 0.5)) =                                   1.0 / (1 + Math.Exp(-10.0*(fraction - 0.5)))                                        -- sigmoid
        // (10 e^(-10 (-0.5 + x)))/(1 + e^(-10 (-0.5 + x)))^2       4*Math.Exp(-10*(fraction - 0.5))/Math.Pow(Math.Exp(-10*(fraction - 0.5)) + 1, 2)    -- sigmoid derrivative from 0 to 1
        // sin^2(pi*x) =                                            Math.Pow(Math.Sin(Math.PI*fraction), 2)                                             -- sin

        Renderer renderer;
        Bitmap image;
        Configurator configurator;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var color in (KnownColor[])Enum.GetValues(typeof(KnownColor)))
            {
                ComboBox2.Items.Add(color);
                ComboBox1.Items.Add(color);
            }

            ComboBox1.SelectedItem = KnownColor.Black;
            ComboBox2.SelectedItem = KnownColor.Blue;

            configurator = new Configurator();

            renderer = new Renderer();
            renderer.onRenderComplete += (i) => {
                image = i;

                Dispatcher.Invoke(() =>
                {
                    MViewer viewer = new MViewer();

                    //viewer.sett

                    viewer.MW = this;
                    viewer.Settings = configurator.settings;
                    viewer.xRadius = renderer.xRadius;
                    viewer.yRadius = renderer.yRadius;
                    viewer.xStart = renderer.xStart;
                    viewer.yStart = renderer.yStart;
                    viewer.zoom = renderer.zoom;
                    viewer.image = image;
                    var r = viewer.ShowDialog();

                    if ((bool)r)
                    {
                        XTextBox.Text = viewer.resultX.ToString();
                        YTextBox.Text = viewer.resultY.ToString();
                        ZoomTextBox.Text = viewer.resultZoom.ToString();
                    }
                });
                GC.Collect();
            };

            renderer.renderProgressChanged += (d) => {
                Dispatcher.Invoke(() => { ProgressBar1.Value = d; });
            };
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            image?.Dispose();

            renderer.maxIterations = Convert.ToInt32(MaxIterationsTextBox.Text);
            renderer.xStart = Convert.ToDouble(XTextBox.Text.Replace(".", ","));
            renderer.yStart = Convert.ToDouble(YTextBox.Text.Replace(".", ","));
            renderer.zoom = Convert.ToDouble(ZoomTextBox.Text.Replace(".", ","));

            renderer.settings = configurator.settings;

            renderer.color1 = System.Drawing.Color.FromKnownColor((KnownColor)ComboBox1.SelectedItem);
            renderer.color2 = System.Drawing.Color.FromKnownColor((KnownColor)ComboBox2.SelectedItem);

            renderer.InnerCode = InterpolationCode;
            renderer.DOMCompile();

            renderer.RenderAsync(); 
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) // Open Color Interpolation Editor
        {
            CIEditor editor = new CIEditor();
            editor.text = InterpolationCode;

            if((bool)editor.ShowDialog())
            {
                InterpolationCode = editor.text;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (image != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = "PNG Image (*.png)|*.png|JPG Image (*.jpg)|*.jpg|BMP Image (*.bmp)|*.bmp";
                sfd.InitialDirectory = Directory.GetCurrentDirectory() + "\\pictures";
                sfd.AddExtension = true;
                var result = (bool)sfd.ShowDialog();

                if(result)
                {
                    if(Regex.Match(sfd.SafeFileName, "\\.jpg$").Success)
                        image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                    if (Regex.Match(sfd.SafeFileName, "\\.png$").Success)
                        image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);

                    if (Regex.Match(sfd.SafeFileName, "\\.bmp$").Success)
                        image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            else
            {
                MessageBox.Show("Render some pictures before saving", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SettingsClicked(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Settings = this.configurator.settings;
            bool r = (bool)settingsWindow.ShowDialog();

            if(r)
            {
                this.configurator.settings = settingsWindow.Settings;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            configurator.Save();
        }
    }
}
