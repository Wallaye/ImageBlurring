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
            IBlur blur = new BoxBlur(5, (Bitmap)pictureBox1.Image);
            pictureBox2.Image = blur.Blur();
        }
    }
}