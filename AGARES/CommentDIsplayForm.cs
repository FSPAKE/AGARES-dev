using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Utils;

namespace FSPAKE.AGARES.Unit
{
    public partial class CommentDisplayForm : Form
    {

        private Emoji Emoji;
        private string IconFoldername;
        private string EmojiFilename;
        public CommentDisplayForm(string emojiFilename,string iconFoldername)
        {
            InitializeComponent();
            Emoji = new Emoji(emojiFilename);
            EmojiFilename = emojiFilename;
            IconFoldername = iconFoldername;
            FontSize.SelectedItem = FontSize.Text;
            fontSize_SelectedIndexChanged(FontSize,null);
            this.Front.Size = this.ClientSize;
            Controls.Remove(Front);
            DisplayMessageLabel.Controls.Add(Front);
            Front.BringToFront();
        }

        private Color backColor;
        public override Color BackColor
        {
            get => backColor;
            set
            {
                backColor = value;
                if (backColor == Color.Transparent)
                {
                    backColor = Color.White;
                }
                base.BackColor = backColor;
            }
        }
        string channelId;
        public string ChannelId
        {
            get => channelId;
            set
            {
                channelId = value;
                if (UserIconPicture.Image is null)
                {
                    try
                    {
                        int side = UserIconPicture.Height < UserIconPicture.Width ? UserIconPicture.Height : UserIconPicture.Width;
                        UserIconPicture.Image = GraphicManager.Scale(Image.FromFile($@"{IconFoldername}\{value}"),side);
                    }
                    catch (Exception any) { return; }
                }
            }
        }
        public string WindowName
        {
            get => this.Text;
            set => this.Text = value;
        }
        string OriginMessage = "";
        public string DisplayMessage
        {
            get => DisplayMessageLabel.Text;
            set
            {
                DisplayMessageLabel.Text = value;
                OriginMessage = value;
                int printableBytes = UIManager.GetLabelWidthPerLine(DisplayMessageLabel, false).Item2;
                //折返し
                var width = Emoji.GetWide(value);
                var vertical = width / printableBytes + 1;
                var printableVertical = DisplayMessageLabel.Height/ DisplayMessageLabel.Font.Height;
                if(vertical > printableVertical)
                {
                    var increment = DisplayMessageLabel.Font.Height * (vertical - printableVertical);
                    DisplayMessageLabel.Height += increment;
                    this.Height += increment;
                }
            }
        }
        private void DisplayMessageLabel_Paint(object sender, PaintEventArgs e)
        {
            string displayArea;
            Dictionary<int, Image> columns;
            string text = DisplayMessageLabel.Text;
            MembershipEmoji.Analize(in text , out displayArea, out columns, EmojiFilename);
            foreach (var column in columns)
            {
                var place = column.Key-1;
                var img = GraphicManager.Scale(column.Value, DisplayMessageLabel.Font.Height);

                int printableBytes = DisplayMessageLabel.Width / DisplayMessageLabel.Font.Height*2;
                //折返し
                var vertical = place / printableBytes ;
                var holizontal = place % printableBytes;
                var widthPerByte = DisplayMessageLabel.Width / printableBytes;
                var point = new PointF(holizontal * widthPerByte,
                                    vertical * DisplayMessageLabel.Font.Height);
                Bitmap canvas;
                if (this.Front.Image is null)
                {
                    canvas = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                }
                else
                {
                    canvas = new Bitmap(this.Front.Image);
                }
                var g = Graphics.FromImage(canvas);
                g.DrawImage(img, point);
                this.Front.Image = canvas;
            }
            DisplayMessageLabel.Text = displayArea;
        }

        private void fontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayMessageLabel.Paint -= new PaintEventHandler(this.DisplayMessageLabel_Paint);
            DisplayMessageLabel.Font = new Font("ＭＳ ゴシック", Parse.ToFloat(((ComboBox)sender).SelectedItem.ToString()), FontStyle.Regular, GraphicsUnit.Point, 128);
            DisplayMessageLabel.Paint += new PaintEventHandler(this.DisplayMessageLabel_Paint);
            Front.Image = null;
            DisplayMessage = OriginMessage;
        }
    }
}
