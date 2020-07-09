using FSPAKE.AGARES.Unit;
using System;
using System.Drawing;
using System.Windows.Forms;
using Utils;

namespace FSPAKE.AGARES.CoreSystem
{
    /// <summary>
    /// スパチャの内容を表示するモーダルウィンドウに関する処理を記述する
    /// </summary>
    partial class Agares
    {


        ///*---------------------------------------------------------------------------------------

        ///<summary>スパチャ通番</summary>
        private static int SuperReviewSeq;
        ///<summary>１行に表示されるラベルの数</summary>
        private int SuperReviewHolizontalNum;

        /// <summary>
        /// 画面右からスパチャの内容を開くためのボタンが現れる
        /// </summary>
        /// <param name="liveChat">チャット情報</param>
        public void PopSuperChatReviewLabel(LiveChatFieldAccesser liveChat)
        {
            if(liveChat is null) { return; }
            if (liveChat.IsFanFund)
            {
                var displayArea = new StringManager(liveChat.DisplayMessage.Split(new string[] { "from " }, StringSplitOptions.None)[1]).SplitOnFirst(":");
                var name = displayArea[0];
                string message;
                if (displayArea.Length >= 2)
                {
                    message = displayArea[1];
                }
                else
                {//無言スパチャの場合
                    message = "";
                }
                var _ReviewLabel = new ReviewLabel()
                {
                    Location = new Point(ClientRectangle.Width + 100, SuperChatReviewPanel.Location.Y + 35 * (SuperReviewSeq / SuperReviewHolizontalNum)),
                    BackColor = liveChat.BackColor,
                    Name = $"scrl{SuperReviewSeq}",
                    Id = SuperReviewSeq,
                    Text = name,
                    DisplayMessage = message,
                    ChannelId = liveChat.ChannelId,
                    Amount = (long)liveChat.Amount,
                    Currency = liveChat.Currency,
                    Visible = IsViewSuperchatReviewLabel
                };
                _ReviewLabel.DeleteEvent += new Action<int>(DeleteSuperchatLabel);    
                Controls.Add(_ReviewLabel);
                _ReviewLabel.BringToFront();
                _ReviewLabel.Entry(FlowStopX);
                SuperReviewSeq++;
                AlignmentSuperchatLabel();
            }
        }
        /// <summary>
        /// スパチャの内容を開くためのボタンの位置を逐次調整する
        /// </summary>
        private void AlignmentSuperchatLabel()
        {
            if (SuperReviewHolizontalNum != 0)
            {
                FlowStopX = SuperChatReviewPanel.Location.X
                    + (105 * (SuperReviewSeq % SuperReviewHolizontalNum));
                foreach (Control c in Controls)
                {
                    if (c is ReviewLabel c2)
                    {
                        var holizon = c2.Id;
                        c.Left = (holizon % SuperReviewHolizontalNum * 105) + SuperChatReviewPanel.Left;
                        c.Top = (holizon / SuperReviewHolizontalNum * 35) + SuperChatReviewPanel.Top;
                    }
                }
            }
        }
        /// <summary>
        /// スパチャの内容を開くためのボタンを削除する
        /// </summary>
        private void DeleteSuperchatLabel(int id)
        {
            foreach(Control c in Controls)
            {
                if (c is ReviewLabel c2)
                {
                    if (c2.Id == id) {
                        Controls.Remove(c);
                        c.Dispose();
                    }
                    if (c2.Id > id)
                    {
                        c2.Id = c2.Id - 1;
                        c2.Name = "scrl" + (c2.Id - 1).ToString();
                    }
                }
            }
            SuperReviewSeq--;
            AlignmentSuperchatLabel();
        }
    }
}
