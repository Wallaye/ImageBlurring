using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public int[,] pixelsA { get; set; }
        public int[,] pixelsR { get; set; }
        public int[,] pixelsG { get; set; }
        public int[,] pixelsB { get; set; }
        private const double sigma = 1;

        private double[,] _weightMatrix { get; set; }

        public GaussianBlur(int radius, Bitmap source)
        {
            _radius = radius;
            Source = (Bitmap)source.Clone();
            _weightMatrix = new double[(2 * radius + 1), (2 * radius + 1)];
            pixelsA = new int[Source.Height + 2 * radius, Source.Width + 2 * radius];
            pixelsR = new int[Source.Height + 2 * radius, Source.Width + 2 * radius];
            pixelsG = new int[Source.Height + 2 * radius, Source.Width + 2 * radius];
            pixelsB = new int[Source.Height + 2 * radius, Source.Width + 2 * radius];
            CalculateWeights();
            GenerateTables();
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

        private void GenerateTables()
        {
            for (int i = 0; i < Source.Height + 2 *_radius; i++)
            {
                for (int j = 0; j < Source.Width + 2 * _radius; j++)
                {
                    int indexI = i < _radius ? 0 : (i > Source.Width + _radius - 1 ? Source.Width - 1 : i - _radius);
                    int indexJ = j < _radius ? 0 : (j > Source.Width + _radius - 1 ? Source.Width - 1 : j - _radius);
                    pixelsA[i, j] = Source.GetPixel(indexJ, indexI).A;
                    pixelsR[i, j] = Source.GetPixel(indexJ, indexI).R;
                    pixelsG[i, j] = Source.GetPixel(indexJ, indexI).G;
                    pixelsB[i, j] = Source.GetPixel(indexJ, indexI).B;
                }
            }
        }

        public Bitmap Blur()
        {
            Bitmap result = (Bitmap)Source.Clone();
            for (int i = 0; i < Source.Height; i++)
            {
                for (int j = 0; j < Source.Width; j++)
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

                            A += kernelValue * pixelsA[i + x + _radius, j + y + _radius];
                            R += kernelValue * pixelsR[i + x + _radius, j + y + _radius];
                            G += kernelValue * pixelsG[i + x + _radius, j + y + _radius];
                            B += kernelValue * pixelsB[i + x + _radius, j + y + _radius];
                        }
                    }
                    result.SetPixel(j, i, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }

            return result;
        }
    }
}
