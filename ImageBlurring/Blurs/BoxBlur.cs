﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlurring.Blurs
{
    internal class BoxBlur : IBlur
    {
        public Bitmap Source { get; set; }
        public Bitmap BitmapResult { get; set; }

        public int[,] _summedTableR { get; set; }
        public int[,] _summedTableG { get; set; }
        public int[,] _summedTableB { get; set; }
        public int[,] _summedTableA { get; set; }

        public int Radius { 
            get => _radius;
            set { if (value >= 0 && value <= 50 && value % 2 == 0)
                    _radius = value;
            } }
        private int _radius;

        public BoxBlur(int radius, Bitmap src)
        {
            _radius = radius;
            Source = (Bitmap)src.Clone();
            _summedTableR = new int[Source.Height + 2 * radius + 1, Source.Width + 2 * radius + 1];
            _summedTableA = new int[Source.Height + 2 * radius + 1, Source.Width + 2 * radius + 1];
            _summedTableG = new int[Source.Height + 2 * radius + 1, Source.Width + 2 * radius + 1];
            _summedTableB = new int[Source.Height + 2 * radius + 1, Source.Width + 2 * radius + 1];
            SetSummedTables();
        }

        public Bitmap Blur()
        {
            int area = (int)Math.Pow(2 * _radius + 1, 2);
            Bitmap result = (Bitmap)Source.Clone();
            for (int i = 0; i < Source.Height; i++)
            {   
                for (int j = 0; j < Source.Width; j++)
                {
                    int offset = _radius + 1;
                    int A = _summedTableA[i + _radius + offset, j + _radius + offset]
                        - _summedTableA[i + _radius + offset, j - _radius - 1 + offset]
                        - _summedTableA[i - _radius - 1 + offset, j + _radius + offset]
                        + _summedTableA[i - _radius - 1 + offset, j - _radius - 1 + offset];

                    int R = _summedTableR[i + _radius + offset, j + _radius + offset]
                        - _summedTableR[i + _radius + offset, j - _radius - 1 + offset]
                        - _summedTableR[i - _radius - 1 + offset, j + _radius + offset]
                        + _summedTableR[i - _radius - 1 + offset, j - _radius - 1 + offset];

                    int G = _summedTableG[i + _radius + offset, j + _radius + offset]
                        - _summedTableG[i + _radius + offset, j - _radius - 1 + offset]
                        - _summedTableG[i - _radius - 1 + offset, j + _radius + offset]
                        + _summedTableG[i - _radius - 1 + offset, j - _radius - 1 + offset];

                    int B = _summedTableB[i + _radius + offset, j + _radius + offset]
                        - _summedTableB[i + _radius + offset, j - _radius - 1 + offset]
                        - _summedTableB[i - _radius - 1 + offset, j + _radius + offset]
                        + _summedTableB[i - _radius - 1 + offset, j - _radius - 1 + offset];
                    A /= area;
                    R /= area; G /= area; B /= area;
                    result.SetPixel(j, i, Color.FromArgb(A, R, G, B));
                }
            }
            return result;
        }

        private void SetSummedTables()
        {
            //initializing first elements
            _summedTableA[1, 1] = Source.GetPixel(0, 0).A;
            _summedTableR[1, 1] = Source.GetPixel(0, 0).R;
            _summedTableG[1, 1] = Source.GetPixel(0, 0).G;
            _summedTableB[1, 1] = Source.GetPixel(0, 0).B;

            //initializing 1st row of matrix
            for (int i = 1; i < Source.Width + 2 * _radius + 1; i++)
            {
                int index = i < _radius + 1 ? 0 : (i > Source.Width + _radius - 1 ? Source.Width - 1 : i - _radius);
                _summedTableA[1, i] = _summedTableA[1, i - 1] + Source.GetPixel(index, 0).A;
                _summedTableR[1, i] = _summedTableR[1, i - 1] + Source.GetPixel(index, 0).R;
                _summedTableG[1, i] = _summedTableG[1, i - 1] + Source.GetPixel(index, 0).G;
                _summedTableB[1, i] = _summedTableB[1, i - 1] + Source.GetPixel(index, 0).B;
            }

            //initializing 1st col of matrix
            for (int j = 1; j < Source.Height + 2 * _radius + 1; j++)
            {
                int index = j < _radius + 1 ? 0 : (j > Source.Height + _radius - 1 ? Source.Height - 1 : j - _radius);

                _summedTableA[j, 1] = _summedTableA[j - 1, 1] + Source.GetPixel(0, index).A;
                _summedTableR[j, 1] = _summedTableR[j - 1, 1] + Source.GetPixel(0, index).R;
                _summedTableG[j, 1] = _summedTableG[j - 1, 1] + Source.GetPixel(0, index).G;
                _summedTableB[j, 1] = _summedTableB[j - 1, 1] + Source.GetPixel(0, index).B;
            }

            //setting table;

            for (int i = 2; i < Source.Height + 2 * _radius + 1; i++)
            {
                for (int j = 2; j < Source.Width + 2 * _radius + 1; j++) 
                {
                    int indexI = i < _radius ? 0 : (i > Source.Height + _radius - 1 ? Source.Height - 1 : i - _radius);
                    int indexJ = j < _radius ? 0 : (j > Source.Width + _radius - 1 ? Source.Width - 1 : j - _radius);
                    _summedTableA[i, j] = _summedTableA[i - 1, j] + _summedTableA[i, j - 1] 
                        - _summedTableA[i - 1, j - 1] + Source.GetPixel(indexJ, indexI).A;
                    _summedTableR[i, j] = _summedTableR[i - 1, j] + _summedTableR[i, j - 1] 
                        - _summedTableR[i - 1, j - 1] + Source.GetPixel(indexJ, indexI).R;
                    _summedTableG[i, j] = _summedTableG[i - 1, j] + _summedTableG[i, j - 1] 
                        - _summedTableG[i - 1, j - 1] + Source.GetPixel(indexJ, indexI).G;
                    _summedTableB[i, j] = _summedTableB[i - 1, j] + _summedTableB[i, j - 1] 
                        - _summedTableB[i - 1, j - 1] + Source.GetPixel(indexJ, indexI).B;
                }
            }
        }

        public Bitmap Blur1()
        {
            int size = _radius;
            if (size % 2 == 0) size++;
            Bitmap blurred = new Bitmap((Bitmap)Source.Clone());

            float avg = (float) 1 / size;

            for (int j = 0; j < Source.Height; j++)
            {
                float[] hSum = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
                float[] iAvg = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
                for (int x = 0; x < size; x++)
                {
                    Color tempColor = Source.GetPixel(x, j);
                    hSum[0] += tempColor.A;
                    hSum[1] += tempColor.R;
                    hSum[2] += tempColor.G;
                    hSum[3] += tempColor.B;
                    
                }
                iAvg[0] = hSum[0] * avg;
                iAvg[1] = hSum[1] * avg;
                iAvg[2] = hSum[2] * avg;
                iAvg[3] = hSum[3] * avg;

                for (int i = 0; i < Source.Width; i++)
                {
                    if ((i - size / 2 >= 0) && (i + 1 + size / 2 < Source.Width))
                    {
                        Color tmp_pColor = Source.GetPixel(i - size / 2, j);
                        hSum[0] -= tmp_pColor.A;
                        hSum[1] -= tmp_pColor.R;
                        hSum[2] -= tmp_pColor.G;
                        hSum[3] -= tmp_pColor.B;
                        Color tmp_nColor = Source.GetPixel(i + 1 + size / 2, j);
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
                    Color tempColor = Source.GetPixel(i, y);
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
                        Color tmp_pColor = Source.GetPixel(i, j - size / 2);
                        hSum[0] -= tmp_pColor.A;
                        hSum[1] -= tmp_pColor.R;
                        hSum[2] -= tmp_pColor.G;
                        hSum[3] -= tmp_pColor.B;
                        Color tmp_nColor = Source.GetPixel(i, j + 1 + size / 2); ;
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
