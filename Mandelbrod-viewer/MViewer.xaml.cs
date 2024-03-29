﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenGames.MandelbrodViewer
{
    /// <summary>
    /// Логика взаимодействия для MViewer.xaml
    /// </summary>
    public partial class MViewer : Window
    {
        public Bitmap image;
        public double xRadius;
        public double yRadius;
        public double xStart;
        public double yStart;
        public double zoom;

        public double resultZoom;
        public double resultX;
        public double resultY;

        private Settings settings;
        private MainWindow mW;

        private double xEps;
        private double yEps;

        public System.Windows.Point start = new System.Windows.Point();
        public System.Windows.Point end = new System.Windows.Point();

        bool rectDrawing = false;
        bool leftMousePressed = false;
        bool movingAround = false;

        internal Settings Settings { get => settings; set => settings = value; }
        internal MainWindow MW { get => mW; set => mW = value; }


        private Renderer renderer = new Renderer();

        public MViewer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Window1.Width = image.Width;
                Window1.Height = image.Height;

                Image1.Source = ImageSourceFromBitmap(image);

                xEps = 2 * xRadius / image.Width;
                yEps = 2 * yRadius / image.Height;

                CenterWindowOnScreen();
                Topmost = true;


                renderer.onRenderComplete += (i) => {
<<<<<<< HEAD
=======
                    this.image.Dispose();
>>>>>>> 86612e43dd7c7a3f5993e81787a34f0aaa1c5208
                    this.image = i;

                    //-----
                    xRadius = renderer.xRadius;
                    yRadius = renderer.yRadius;
                    xStart = renderer.xStart;
                    yStart = renderer.yStart;
                    zoom = renderer.zoom;

                    xEps = 2 * xRadius / image.Width;
                    yEps = 2 * yRadius / image.Height;
                    //-----

<<<<<<< HEAD
                    this.mW.image = i;

=======
>>>>>>> 86612e43dd7c7a3f5993e81787a34f0aaa1c5208
                    Dispatcher.Invoke(() => {
                        Image1.Source = ImageSourceFromBitmap(image);
                        ProgressBar1.Visibility = Visibility.Hidden;
                    });
                };

                renderer.renderProgressChanged += (d) => {
                    Dispatcher.Invoke(() => { ProgressBar1.Value = d; });
                };
            }
            catch { }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        private void Window1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
            CalcResults();
        }

        private void Window1_ContentRendered(object sender, EventArgs e)
        {
        }

        private void Window1_MouseMove(object sender, MouseEventArgs e)
        {
            double x = Mouse.GetPosition(this).X * xEps + (xStart - xRadius);
            double y = Mouse.GetPosition(this).Y * yEps + (yStart - yRadius);

            XLabel.Content = string.Format("X: {0}", x);
            YLabel.Content = string.Format("Y: {0}", y);

            Line1.Visibility = Visibility.Visible;
            Line1.X1 = Mouse.GetPosition(this).X;
            Line1.X2 = Mouse.GetPosition(this).X;
            Line1.Y1 = 0;
            Line1.Y2 = Window1.Height;

            Line2.Visibility = Visibility.Visible;
            Line2.X1 = 0;
            Line2.X2 = Window1.Width;
            Line2.Y1 = Mouse.GetPosition(this).Y;
            Line2.Y2 = Mouse.GetPosition(this).Y;

            if (rectDrawing)
            {
                double xDiff = Math.Abs(start.X - Mouse.GetPosition(this).X);
                Rectangle1.Margin = new Thickness(start.X - xDiff, start.Y - xDiff / settings.aspectRatio, Window1.Width - (start.X + xDiff), Window1.Height - (start.Y + xDiff / settings.aspectRatio));
            }
            if(movingAround && leftMousePressed)
            {
                Window1.Left += (Mouse.GetPosition(this).X - start.X);
                Window1.Top += (Mouse.GetPosition(this).Y - start.Y);
            }
        }

        private void Window1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            start = Mouse.GetPosition(this);
            leftMousePressed = true;

            if(!movingAround)
            {
                Rectangle1.Visibility = Visibility.Visible;
                Rectangle1.Margin = new Thickness(Mouse.GetPosition(this).X, Mouse.GetPosition(this).Y, Window1.Width - Mouse.GetPosition(this).X, Window1.Height - Mouse.GetPosition(this).Y);
                rectDrawing = true;
            }
        }

        private void Window1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            end = Mouse.GetPosition(this);
            leftMousePressed = false;

            if (!movingAround)
            {
                rectDrawing = false;
                Rectangle1.Visibility = Visibility.Hidden;
            }
        }

        private void Window1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (settings.immediateRender)
                {
                    CalcResults();

                    renderer.maxIterations = Convert.ToInt32(MW.MaxIterationsTextBox.Text);
                    renderer.xStart = resultX;
                    renderer.yStart = resultY;
                    renderer.zoom = resultZoom;

                    renderer.settings = settings;

                    renderer.color1 = System.Drawing.Color.FromKnownColor((KnownColor)MW.ComboBox1.SelectedItem);
                    renderer.color2 = System.Drawing.Color.FromKnownColor((KnownColor)MW.ComboBox2.SelectedItem);

                    renderer.InnerCode = MW.InterpolationCode;
                    renderer.DOMCompile();

<<<<<<< HEAD
                    image?.Dispose();
=======
>>>>>>> 86612e43dd7c7a3f5993e81787a34f0aaa1c5208
                    renderer.RenderAsync();
                    ProgressBar1.Margin = new Thickness(100, Window1.Height / 2 - 10, 100, 0);
                    ProgressBar1.Visibility = Visibility.Visible;
                }
                else
                {
                    Close();
                }
            }
            if(e.Key == Key.LeftShift)
            {
                movingAround = true;
                Cursor = Cursors.Cross;
            }
            if(e.Key == Key.F11)
            {
                //this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
            if(e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                movingAround = false;
                Cursor = Cursors.Arrow;
            }
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void CalcResults()
        {
            if (end.X != start.X)
            {
                resultX = start.X * xEps + (xStart - xRadius);
                resultY = start.Y * yEps + (yStart - yRadius);
                resultZoom = zoom * image.Width / (2 * Math.Abs(start.X - end.X));
            }
            else
            {
                if (start.X == 0 && start.Y == 0)
                {
                    resultX = xStart;
                    resultY = yStart;
                }
                else
                {
                    resultX = start.X * xEps + (xStart - xRadius);
                    resultY = start.Y * yEps + (yStart - yRadius);
                }
                resultZoom = zoom;
            }
        }
    }
}
