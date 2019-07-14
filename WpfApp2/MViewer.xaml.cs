using System;
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

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MViewer.xaml
    /// </summary>
    public partial class MViewer : Window
    {
        public Bitmap image;
        public double xMin;
        public double xMax;
        public double yMin;
        public double yMax;
        public double xStart;
        public double yStart;
        public double zoom;

        public double resultZoom;
        public double resultX;
        public double resultY;

        private double xEps;
        private double yEps;

        public System.Windows.Point start = new System.Windows.Point();
        public System.Windows.Point end = new System.Windows.Point();

        bool rectDrawing = false;
        bool movingAround = false;

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

                xEps = Math.Abs(xMin - xMax) / image.Width;
                yEps = Math.Abs(yMin - yMax) / image.Height;
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
            if(end.X != start.X)
            {
                resultX = start.X * xEps + xMin;
                resultY = start.Y * yEps + yMin;
                resultZoom = zoom *  image.Width / (2 * Math.Abs(start.X - end.X));
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
                    resultX = start.X * xEps + xMin;
                    resultY = start.Y * yEps + yMin;
                }
                resultZoom = zoom;
            }

            //image.Dispose();
        }

        private void Window1_ContentRendered(object sender, EventArgs e)
        {
        }

        private void Window1_MouseMove(object sender, MouseEventArgs e)
        {
            double x = Mouse.GetPosition(this).X * xEps + xMin;
            double y = Mouse.GetPosition(this).Y * yEps + yMin;

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
                Rectangle1.Margin = new Thickness(start.X - xDiff, start.Y - xDiff/2, Window1.Width - (start.X + xDiff), Window1.Height - (start.Y + xDiff / 2));
            }
            if(movingAround)
            {
                Window1.Left += (Mouse.GetPosition(this).X - start.X);
                Window1.Top += (Mouse.GetPosition(this).Y - start.Y);
            }
        }

        private void Window1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           start = Mouse.GetPosition(this);

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
                Close();
            }
            if(e.Key == Key.LeftShift)
            {
                movingAround = true;
                Cursor = Cursors.Cross;
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
    }
}
