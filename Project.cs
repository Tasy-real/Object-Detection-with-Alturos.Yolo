using Alturos.Yolo;
using Alturos.Yolo.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Object_detection
{
    public partial class Form1 : Form
    {
        //Soundplayer
        SoundPlayer sound_player = new SoundPlayer(@"Sounds\Sound1.wav");

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            //Delets last Pic
            picBox_1.Image = null;

            // declear OpenFileDialog
            var ofd = new OpenFileDialog();

            //Filters by Images and .jpg/.png Files
            ofd.Filter = "Image Files|*.jpg;*.png";

            // If OK -> Image gets displayed on PicBox
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                picBox_1.Image = Image.FromFile(ofd.FileName);
            }
        }

        private void button_detect_Click(object sender, EventArgs e)
        {
            //Plays sound, good for checking if the programm has no error , but does not find a thing in the pic
            sound_player.Play();

            // Configuration Detector
            var configDetector = new ConfigurationDetector();
            var config = configDetector.Detect();

            // Add config to a YoloWrapper
            var yolo = new YoloWrapper(config);

            //MemoryStream , picBox Image gets saved to it, so yolo can detect
            var memoryStream = new MemoryStream();
            picBox_1.Image.Save(memoryStream, ImageFormat.Png);
            var _items = yolo.Detect(memoryStream.ToArray()).ToList();

            //call Details on screen
            addInfoToPicBox(picBox_1, _items);
        }

        void addInfoToPicBox(PictureBox picBoxToRender, List<YoloItem> items)
        {
            // adds the pic to Graphics
            var img = picBoxToRender.Image;
            var graph = Graphics.FromImage(img);

            var font = new Font("Open Sans", 18);
            var brush = new SolidBrush(Color.FromArgb(140, 0, 0));

            foreach (var item in items)
            {
                //cordinates
                var x = item.X;
                var y = item.Y;

                //height/width
                var width = item.Width;
                var height = item.Height;

                //pen color/ rect pos etc.
                var rect = new Rectangle(x, y, width, height);
                var pen = new Pen(Color.FromArgb(140, 0, 0), 2);

                var point = new Point(x, y);

                if(width < 200  || height < 200)
                {
                    font = new Font("Open Sans", 13);
                }
               
                //Draws it
                graph.DrawRectangle(pen, rect);

                //short/ round it at the 3. place
                string display_confi = Math.Round(item.Confidence * 100, 2).ToString() + "%";

                //Shows all
                graph.DrawString(item.Type + "\n" + display_confi , font, brush, point);
               }
            // Refreshes it
            picBoxToRender.Image = img;
        }
    }
}
