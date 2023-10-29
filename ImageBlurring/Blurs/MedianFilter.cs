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

        public MedianFilter(int radius, Bitmap source)
        {
            Source = (Bitmap)source.Clone();
            _radius = radius;
            _area = (2 * _radius + 1) * (2 * _radius + 1);
        }

        public Bitmap Blur()
        {
            Bitmap result = (Bitmap)Source.Clone();
            int[] arrayA = new int[_area];
            int[] arrayR = new int[_area];
            int[] arrayG = new int[_area];
            int[] arrayB = new int[_area];
            for (int i = _radius; i < Source.Height - _radius; i++)
            {
                for (int j = _radius; j < Source.Width - _radius; j++)
                {
                    int k = 0;
                    for (int x = -_radius; x <= _radius; x++)
                    {
                        for (int y = -_radius; y <= _radius; y++)
                        {
                            arrayA[k] = Source.GetPixel(j + y, i + x).A;
                            arrayR[k] = Source.GetPixel(j + y, i + x).R;
                            arrayG[k] = Source.GetPixel(j + y, i + x).G;
                            arrayB[k] = Source.GetPixel(j + y, i + x).B;
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
