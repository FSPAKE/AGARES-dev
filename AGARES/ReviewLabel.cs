using System;
using System.Drawing;
using System.Windows.Forms;
using FSPAKE.AGARES.Unit;

namespace FSPAKE.AGARES.CoreSystem
{
    public partial class ReviewLabel : UserControl
    {
        public int Id { get; set; }
        public Action<int> DeleteEvent;
        public ReviewLabel()
        {
            InitializeComponent();
            this.Click += SuperChatReviewButton_Click;
        }

        long amount;
        public long Amount
        {
            get
            {
                return amount;
            }
            set
            {
                if (amount == 0)
                {
                    amount = value;
                }
            }
        }
        string currency;
        public string Currency
        {
            get
            {
                return currency;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(currency))
                {
                    currency = value;
                }
            }
        }
        string displayMessage;
        public string DisplayMessage
        {
            get
            {
                return displayMessage;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(displayMessage))
                {
                    displayMessage = value;
                }
            }
        }
        string channelId;
        public string ChannelId
        {
            get
            {
                return channelId;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(channelId))
                {
                    channelId = value;
                }
            }
        }

        string userName;
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                if (string.IsNullOrEmpty(userName))
                {
                    userName = value;
                }
            }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                if (value == Color.Transparent)
                {
                    base.BackColor = Color.White;
                }
                else
                {
                    base.BackColor = value;
                }
            }
        }
        public override string Text
        {
            get
            {
                return NameLabel.Text;
            }
            set
            {
                NameLabel.Text = value;
            }
        }
        private void SuperChatReviewButton_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                var superDisplayForm = new CommentDisplayForm(Agares.EmojiFilename,Agares.IconFoldername)
                {
                    ChannelId = ChannelId,
                    WindowName = Text,
                    DisplayMessage = displayMessage,
                    BackColor = this.BackColor,
                    StartPosition = FormStartPosition.CenterParent
                };
                superDisplayForm.ShowDialog();
            }
        }

        public void Entry(int x)
        {
            while (Location.X >= x)
            {
                Left--;
                Refresh();
            }
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteEvent.Invoke(Id);
        }
    }
}
