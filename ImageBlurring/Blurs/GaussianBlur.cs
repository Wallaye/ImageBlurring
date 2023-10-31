using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlurring.Blurs
{
    internal class GaussianBlur : IBlur
    {
        public int Radius
        {
            get => _radius;
            set
            {
                if (value >= 0 && value <= 50 && value % 2 == 0)
                    _radius = value;
            }
        }
        private int _radius;
        public Bitmap Source { get; set; }
        public Bitmap BitmapResult { get; set; }
        private const double sigma = 1;

        private double[,] _weightMatrix { get; set; }

        public GaussianBlur(int radius, Bitmap source)
        {
            _radius = radius;
            Source = (Bitmap)source.Clone();
            _weightMatrix = new double[(2 * radius + 1), (2 * radius + 1)];
            CalculateWeights();
        }

        private void CalculateWeights()
        {
            for (int i = 0; i < _weightMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < _weightMatrix.GetLength(1); j++)
                {
                    int x = i - _radius;
                    int y = j - _radius;
                    double gauss = Math.Exp(-(x * x + y * y) / (2 * sigma * sigma)) / (2 * Math.PI * sigma * sigma);
                    _weightMatrix[i, j] = gauss;
                }
            }
        }

        public Bitmap Blur()
        {
            Bitmap result = (Bitmap)Source.Clone();
            for (int i = _radius; i < Source.Height - _radius; i++)
            {
                for (int j = _radius; j < Source.Width - _radius; j++)
                {
                    double A = 0.0;
                    double R = 0.0;
                    double G = 0.0;
                    double B = 0.0;
                    for (int x = -_radius; x < _radius; x++)
                    {
                        for (int y = -_radius; y < _radius; y++)
                        {
                            double kernelValue = _weightMatrix[x + _radius, y + _radius];
                            A += kernelValue * Source.GetPixel(j + y, i + x).A; 
                            R += kernelValue * Source.GetPixel(j + y, i + x).R; 
                            G += kernelValue * Source.GetPixel(j + y, i + x).G; 
                            B += kernelValue * Source.GetPixel(j + y, i + x).B; 
                        }
                    }
                    result.SetPixel(j, i, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }

            return result;
        }
    }
}
