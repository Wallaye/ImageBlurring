using ImageBlurring.Blurs;

namespace ImageBlurring
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            }
        }

        private void btnBlur_Click(object sender, EventArgs e)
        {
            IBlur blur;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    blur = new BoxBlur(3, (Bitmap)pictureBox1.Image);
                    break;
                case 1:
                    blur = new GaussianBlur(3, (Bitmap)pictureBox1.Image);
                    break;
                case 2:
                    blur = new MedianFilter(3, (Bitmap)pictureBox1.Image);
                    break;
                default:
                    blur = new BoxBlur(3, (Bitmap)pictureBox1.Image);
                    break;
            }
            pictureBox2.Image = blur.Blur();
        }
    }
}