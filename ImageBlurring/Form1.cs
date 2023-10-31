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
            int radius = (int)numericUpDown1.Value;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    blur = new BoxBlur(radius, (Bitmap)pictureBox1.Image);
                    break;
                case 1:
                    blur = new GaussianBlur(radius, (Bitmap)pictureBox1.Image);
                    break;
                case 2:
                    blur = new MedianFilter(radius, (Bitmap)pictureBox1.Image);
                    break;
                case 3:
                    blur = new SobelOperator((Bitmap)pictureBox1.Image);
                    break;
                default:
                    blur = new BoxBlur(radius, (Bitmap)pictureBox1.Image);
                    break;
            }
            pictureBox2.Image = blur.Blur();
        }
    }
}