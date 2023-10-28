using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlurring
{
    internal interface IBlur
    {
        public int Radius { get; set; }
        public Bitmap Source { get; set; }
        public Bitmap BitmapResult { get; set; }
        public Bitmap Blur();
    }
}
