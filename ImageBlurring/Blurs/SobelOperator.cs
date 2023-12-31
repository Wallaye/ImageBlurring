﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlurring.Blurs
{
    internal class SobelOperator : IBlur
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
        private int _radius = 0;
        public Bitmap Source { get; set; }
        public Bitmap BitmapResult { get; set; }
        private int[,] pixelsA { get; set; }
        private int[,] pixelsR { get; set; }
        private int[,] pixelsG { get; set; }
        private int[,] pixelsB { get; set; }

        private int[,] GX = new int[3, 3]
        {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };
        private int[,] GY = new int[3, 3]
        {
            { -1, -2, -1 },
            { 0, 0, 0 },
            { 1, 2, 1 }
        };

        private const double _rWeight = 0.2126;
        private const double _gWeight = 0.7152;
        private const double _bWeight = 0.0722;


        public SobelOperator(Bitmap source)
        {
            Source = (Bitmap)source.Clone();
            pixelsA = new int[Source.Height + 2, Source.Width + 2];
            pixelsR = new int[Source.Height + 2, Source.Width + 2];
            pixelsG = new int[Source.Height + 2, Source.Width + 2];
            pixelsB = new int[Source.Height + 2, Source.Width + 2];
            GenerateTables();
        }

        private void GenerateTables()
        {
            for (int i = 0; i < Source.Height + 2 * _radius; i++)
            {
                for (int j = 0; j < Source.Width + 2 * _radius; j++)
                {   
                    int indexI = i < _radius ? 0 : (i > Source.Height + _radius - 1 ? Source.Height - 1 : i - _radius);
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
                    //Color c = Source.GetPixel(j, i);
                    //Color color = GetGrayColor(c.R, c.G, c.B);
                    int sumRX = 0;
                    int sumRY = 0;
                    int sumGX = 0;
                    int sumGY= 0;
                    int sumBX = 0;
                    int sumBY = 0;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            //Color temp = Source.GetPixel(j + y, i + x);
                            sumRX += pixelsR[i + x + 1, j + y + 1] * GX[x + 1, y + 1];
                            sumRY += pixelsR[i + x + 1, j + y + 1] * GY[x + 1, y + 1];
                            sumGX += pixelsG[i + x + 1, j + y + 1] * GX[x + 1, y + 1];
                            sumGY += pixelsG[i + x + 1, j + y + 1] * GY[x + 1, y + 1];
                            sumBX += pixelsB[i + x + 1, j + y + 1] * GX[x + 1, y + 1];
                            sumBY += pixelsB[i + x + 1, j + y + 1] * GY[x + 1, y + 1];
                        }
                    }
                    int sumR = (int)Math.Sqrt(sumRX * sumRX + sumRY * sumRY);
                    int sumG = (int)Math.Sqrt(sumGX * sumGX + sumGY * sumGY);
                    int sumB = (int)Math.Sqrt(sumBX * sumBX + sumBY * sumBY);
                    //Color pixel = GetGrayColor(sumR, sumG, sumB);
                    int lowest = Math.Min(Math.Min(Math.Min(sumR, sumG), sumB), 255);
                    
                    result.SetPixel(j, i, Color.FromArgb(lowest, lowest, lowest));
                }
            }
            return result;
        }

        private Color GetGrayColor(int R, int G, int B)
        {
            return Color.FromArgb(
                Math.Min((int)(R * _rWeight), 255), 
                Math.Min((int)(G * _gWeight), 255), 
                Math.Min((int)(B * _bWeight), 255));
        }
    }
}
