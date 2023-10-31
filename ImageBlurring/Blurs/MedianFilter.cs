using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlurring.Blurs
{
    internal class MedianFilter : IBlur
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
        private int _area;
        public Bitmap Source { get; set; }
        public Bitmap BitmapResult { get; set; }
        private int[,] pixelsA { get; set; }
        private int[,] pixelsR { get; set; }
        private int[,] pixelsG { get; set; }
        private int[,] pixelsB { get; set; }

        public MedianFilter(int radius, Bitmap source)
        {
            Source = (Bitmap)source.Clone();
            _radius = radius;
            _area = (2 * _radius + 1) * (2 * _radius + 1);
            pixelsA = new int[Source.Height + 2 * radius, Source.Width + 2 * radius];
            pixelsR = new int[Source.Height + 2 * radius, Source.Width + 2 * radius];
            pixelsG = new int[Source.Height + 2 * radius, Source.Width + 2 * radius];
            pixelsB = new int[Source.Height + 2 * radius, Source.Width + 2 * radius];
            GenerateTables();
        }

        private void GenerateTables()
        {
            for (int i = 0; i < Source.Height + 2 * _radius; i++)
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
            int[] arrayA = new int[_area];
            int[] arrayR = new int[_area];
            int[] arrayG = new int[_area];
            int[] arrayB = new int[_area];
            for (int i = 0; i < Source.Height; i++)
            {
                for (int j = 0; j < Source.Width; j++)
                {
                    int k = 0;
                    for (int x = -_radius; x <= _radius; x++)
                    {
                        for (int y = -_radius; y <= _radius; y++)
                        {
                            arrayA[k] = pixelsA[i + x + _radius, j + y + _radius];
                            arrayR[k] = pixelsR[i + x + _radius, j + y + _radius];
                            arrayG[k] = pixelsG[i + x + _radius, j + y + _radius];
                            arrayB[k] = pixelsB[i + x + _radius, j + y + _radius];
                            k++;
                        }
                    }
                    Array.Sort(arrayA);
                    int A = arrayA[_area / 2];
                    Array.Sort(arrayR);
                    int R = arrayR[_area / 2];
                    Array.Sort(arrayG);
                    int G = arrayG[_area / 2];
                    Array.Sort(arrayB);
                    int B = arrayB[_area / 2];
                    result.SetPixel(j, i, Color.FromArgb(A, R, G, B));
                } 
            }
            return result;
        }
    }
}
