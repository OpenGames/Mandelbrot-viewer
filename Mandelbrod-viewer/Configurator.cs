using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OpenGames.MandelbrodViewer
{
    struct Settings
    {
        public int resolutionWidth;
        public int resolutionHeight;
        public int resolutionMultiplyer;
        public int aspectRatioX;
        public int aspectRatioY;
        public double aspectRatio;
        public double initialX;
        public double initialY;
        public double initialZoom;
        public bool aspectRatioLocked;
        public bool immediateRender;
    }

    class Configurator
    {
        static string configFilePath = "\\resources\\config.cfg";
        static string configFolder = "\\resources";

        public Settings settings = new Settings();
        private FileStream fs;

        public Configurator()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + configFilePath))
            {
                string[] values = File.ReadAllLines(Directory.GetCurrentDirectory() + configFilePath);

                settings.aspectRatioX           = int.Parse(values[0]);
                settings.aspectRatioY           = int.Parse(values[1]);
                settings.aspectRatio            = double.Parse(values[2]);
                settings.resolutionMultiplyer   = int.Parse(values[3]);
                settings.resolutionWidth        = int.Parse(values[4]);
                settings.resolutionHeight       = int.Parse(values[5]);
                settings.initialX               = double.Parse(values[6]);
                settings.initialY               = double.Parse(values[7]);
                settings.initialZoom            = double.Parse(values[8]);
                settings.aspectRatioLocked      = bool.Parse(values[9]);
                settings.immediateRender        = bool.Parse(values[10]);
            }
            else
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + configFolder);
                fs = File.Create(Directory.GetCurrentDirectory() + configFilePath);
                fs.Close();

                //New config file creation (default)
                settings.aspectRatioX = 2;
                settings.aspectRatioY = 1;
                settings.aspectRatio = (double)settings.aspectRatioX / settings.aspectRatioY; // x : y
                settings.resolutionMultiplyer = 800;
                settings.resolutionWidth = settings.aspectRatioX * settings.resolutionMultiplyer;  // is:
                settings.resolutionHeight = settings.aspectRatioY * settings.resolutionMultiplyer; // 1600x800
                settings.initialX = 0;
                settings.initialY = 0;
                settings.initialZoom = 1;

                settings.aspectRatioLocked = true;
                settings.immediateRender = false;
            }
        }

        public void Save()
        {
            File.WriteAllText(Directory.GetCurrentDirectory() + configFilePath, settings.aspectRatioX.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.aspectRatioY.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.aspectRatio.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.resolutionMultiplyer.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.resolutionWidth.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.resolutionHeight.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.initialX.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.initialY.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.initialZoom.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.aspectRatioLocked.ToString() + "\n");
            File.AppendAllText(Directory.GetCurrentDirectory() + configFilePath, settings.immediateRender.ToString() + "\n");
        }

        public static int GCD(int a, int b)
        {
            return b == 0 ? Math.Abs(a) : GCD(b, a % b);
        }
    }
}
