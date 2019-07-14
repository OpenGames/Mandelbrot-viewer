using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OpenGames.MandelbrodViewer
{
    class Renderer
    {

        public int maxIterations = 150;
        public double zoom = 1;
        public double xStart = 0;
        public double yStart = 0;
        public string InnerCode = "";

        public Color color1;
        public Color color2;

        public double done;

        public double xMin;
        public double xMax;
        public double yMin;
        public double yMax;

        public delegate void renderComplete(Bitmap image);
        public delegate void renderState(double p);
        public event renderComplete onRenderComplete;
        public event renderState renderProgressChanged;

        private Func<Color, Color, double, Color> ColorInterpolationFunc;

        int width;
        int height;

        double xEps;
        double yEps;

        public Renderer()
        {
        }

        [STAThread]
        public void RenderAsync()
        {

            Thread thread = new Thread(() => { 
                xMin = -2 / zoom + xStart;
                xMax = 2 / zoom + xStart;
                yMin = -1 / zoom + yStart;
                yMax = 1 / zoom + yStart;

                width = 2 * 800;
                height = 1 * 800;



                xEps = Math.Abs(xMin - xMax) / width;
                yEps = Math.Abs(yMin - yMax) / height;

                Bitmap image = new Bitmap(width, height);

                for (int x = 0; x < width; x++)
                {
                    //Console.Write("\r{0}%", 100 * (x * 1.0f / width));
                    done = 100 * ((x + 1) * 1.0f / width);

                    if (renderProgressChanged != null)
                        renderProgressChanged(done);

                    for (int y = 0; y < height; y++)
                    {
                        double p = x * xEps + xMin;
                        double q = y * yEps + yMin;

                        double xn = 0;
                        double yn = 0;

                        int iterations = 0;
                        while (true)
                        {
                            double xtmp = (xn * xn - yn * yn) + p;
                            yn = (2 * xn * yn) + q;
                            xn = xtmp;

                            if (xn * xn + yn * yn > 4)
                                break;

                            if (iterations > maxIterations)
                                break;

                            iterations++;
                        }

                        if (iterations >= maxIterations)
                        {
                            image.SetPixel(x, y, Color.Black);
                        }
                        else
                        {
                            image.SetPixel(x, y, ColorInterpolationFunc(color1, color2, (float)iterations / maxIterations));
                        }
                    }
                }

                done = 0;
                if(onRenderComplete != null)
                    onRenderComplete(image);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            //System.IO.Directory.CreateDirectory(string.Format("res/x={0} y={1}/", xStart.ToString("0.000"), yStart.ToString("0.000")));
            //image.Save(string.Format("res/x={0} y={1}/i{2}z{3}.jpg", xStart.ToString("0.000"), yStart.ToString("0.000"), maxIterations, zoom));
        }
        
        public void DOMCompile()
        {
            string code = "using System;" +
            "using System.Drawing;" +
            "" +
            "namespace Mandelbrot" +
            "{" +
            "    public class Renderer" +
            "    {" +
            "        public static Func<Color, Color, double, Color> ColorInterpolation()" +
            "        {" +
            "            Func<Color, Color, double, Color> func =" +
            "            " + InnerCode + ";" +
            "" +
            "            return func;" +
            "        }" +
            "    }" +
            "}";
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add(Assembly.GetEntryAssembly().Location);
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);
            if (results.Errors.HasErrors)
            {
                string errors = "";
                foreach (CompilerError error in results.Errors)
                {
                    errors += string.Format("Error #{0}: {1}\n", error.ErrorNumber, error.ErrorText);
                }
                MessageBox.Show(errors);
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;
                Type program = assembly.GetType("Mandelbrot.Renderer");
                MethodInfo main = program.GetMethod("ColorInterpolation");
                ColorInterpolationFunc = (Func<Color, Color, double, Color>)main.Invoke(null, null);
            }
        }

        //public static Color ColorInterpolation(Color color1, Color color2, double fraction)
        //{
        //    return Color.FromArgb(
        //        (int)((color2.R - color1.R) * fraction) + color1.R,
        //        (int)((color2.G - color1.G) * fraction) + color1.G,
        //        (int)((color2.B - color1.B) * fraction) + color1.B
        //        );
        //}

    }
}
