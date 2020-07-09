using FSPAKE.AGARES.CoreSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace FSPAKE.AGARES.CoreSystem
{
    public partial class SuperChatReviewLabel : Label
    {
        public int Id { get; set; }
        private string ViewIconDirectory;
        public Action<int> DeleteEvent;

        public SuperChatReviewLabel()
        {
            InitializeComponent();
        }

        public SuperChatReviewLabel(string viewIconDirectory)
        {
            ViewIconDirectory = viewIconDirectory;
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

        private void SuperChatReviewButton_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                var form1 = new SuperDisplayForm(ViewIconDirectory)
                {
                    WindowName = Text,
                    StartPosition = FormStartPosition.CenterParent,
                    ShowInTaskbar = false,
                    BackColor = this.BackColor,
                    ChannelId = ChannelId,
                    DisplayMessage = displayMessage,
                };
                form1.ShowDialog();
                form1.Dispose();
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

        private void Delete_Click(object sender, EventArgs e)
        {
            var id = Parse.ToInt(new StringManager(this.Name).LeftDelete(4));
            DeleteEvent.Invoke(id);
        }
    }
}
