using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlurring.Blurs
{
    internal class BoxBlur : IBlur
    {
        public Bitmap BitmapSource { get; set; }
        public Bitmap BitmapResult { get; set; }
        public int Radius { 
            get => _radius;
            set { if (value >= 0 && value <= 50)
                    _radius = value;
            } }
        private int _radius;

        public BoxBlur(int radius, Bitmap src)
        {
            _radius = radius;
            BitmapSource = (Bitmap)src.Clone();
        }

        public Bitmap Blur()
        {
            int size = _radius;
            if (size % 2 == 0) size++;
            Bitmap blurred = new Bitmap((Bitmap)BitmapSource.Clone());

            float avg = (float) 1 / size;

            for (int j = 0; j < BitmapSource.Height; j++)
            {
                float[] hSum = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
                float[] iAvg = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
                for (int x = 0; x < size; x++)
                {
                    Color tempColor = BitmapSource.GetPixel(x, j);
                    hSum[0] += tempColor.A;
                    hSum[1] += tempColor.R;
                    hSum[2] += tempColor.G;
                    hSum[3] += tempColor.B;
                }
                iAvg[0] = hSum[0] * avg;
                iAvg[1] = hSum[1] * avg;
                iAvg[2] = hSum[2] * avg;
                iAvg[3] = hSum[3] * avg;

                for (int i = 0; i < BitmapSource.Width; i++)
                {
                    if ((i - size / 2 >= 0) && (i + 1 + size / 2 < BitmapSource.Width))
                    {
                        Color tmp_pColor = BitmapSource.GetPixel(i - size / 2, j);
                        hSum[0] -= tmp_pColor.A;
                        hSum[1] -= tmp_pColor.R;
                        hSum[2] -= tmp_pColor.G;
                        hSum[3] -= tmp_pColor.B;
                        Color tmp_nColor = BitmapSource.GetPixel(i + 1 + size / 2, j);
                        hSum[0] += tmp_nColor.A;
                        hSum[1] += tmp_nColor.R;
                        hSum[2] += tmp_nColor.G;
                        hSum[3] += tmp_nColor.B;
                        //
                        iAvg[0] = hSum[0] * avg;
                        iAvg[1] = hSum[1] * avg;
                        iAvg[2] = hSum[2] * avg;
                        iAvg[3] = hSum[3] * avg;
                    }
                    blurred.SetPixel(i, j, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));
                }
            }
            Bitmap result = (Bitmap)blurred.Clone();
            for (int i = 0; i < blurred.Width; i++)
            {
                float[] hSum = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
                float[] iAvg = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
                for (int y = 0; y < size; y++)
                {
                    Color tempColor = BitmapSource.GetPixel(i, y);
                    hSum[0] += tempColor.A;
                    hSum[1] += tempColor.R;
                    hSum[2] += tempColor.G;
                    hSum[3] += tempColor.B;
                }
                iAvg[0] = hSum[0] * avg;
                iAvg[1] = hSum[1] * avg;
                iAvg[2] = hSum[2] * avg;
                iAvg[3] = hSum[3] * avg;

                for (int j = 0; j < blurred.Height; j++)
                {
                    if ((j - size / 2 >= 0) && (j + 1 + size / 2 < blurred.Height))
                    {
                        Color tmp_pColor = BitmapSource.GetPixel(i, j - size / 2);
                        hSum[0] -= tmp_pColor.A;
                        hSum[1] -= tmp_pColor.R;
                        hSum[2] -= tmp_pColor.G;
                        hSum[3] -= tmp_pColor.B;
                        Color tmp_nColor = BitmapSource.GetPixel(i, j + 1 + size / 2); ;
                        hSum[0] += tmp_nColor.A;
                        hSum[1] += tmp_nColor.R;
                        hSum[2] += tmp_nColor.G;
                        hSum[3] += tmp_nColor.B;
                        //
                        iAvg[0] = hSum[0] * avg;
                        iAvg[1] = hSum[1] * avg;
                        iAvg[2] = hSum[2] * avg;
                        iAvg[3] = hSum[3] * avg;
                    }
                    result.SetPixel(i, j, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));
                }
            }
            BitmapResult = (Bitmap)result.Clone();
            return result;
        }
    }
}
