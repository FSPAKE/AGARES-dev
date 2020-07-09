using Google.Apis.YouTube.v3;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FSPAKE.AGARES.CoreSystem
{
    partial class Agares
    {
        /// <summary>
        /// WindowsFormsデザイナで指定できない、
        /// アプリケーション起動時のコントロール情報更新
        /// </summary>
        private void ModifyComponent()
        {
            GetStop.Visible = false;
            AddNGListenerToolStripMenuItem.Visible = false;
            AddBlockListenerName.Visible = false;
            AddBlockWord.Visible = false;
            AddBanListenerName.Visible = false;
            AddBanWord.Visible = false;
            Comments.Size = ClientSize;
        }



        private void TransformAgares()
        {
            this.Text = "AGARES - AGile Assitant vieweR Exclude Spam users - ";
            CommentsPanel.Left = BaelTools.Left;
            CommentsPanel.Width = this.Width;
            BaelTools.Visible = false;
            AgaresToolStripMenuItem.Visible = false;
            BaelToolStripMenuItem.Visible = true;
            自動BANルール設定ToolStripMenuItem.Visible = false;
        }

        private async Task TransformBaelAsync()
        {
            this.Text = "BAEL - Ban Auto Executioner on Livestream - ";
            CommentsPanel.Left = SuperChatReviewPanel.Left;
            CommentsPanel.Width = SuperChatReviewPanel.Width;
            BaelTools.Visible = true;
            AgaresToolStripMenuItem.Visible = true;
            BaelToolStripMenuItem.Visible = false;
            自動BANルール設定ToolStripMenuItem.Visible = true;
            UserCredential = await NewCredentialAsync(ClientSecletFilename).ConfigureAwait(true);
        }
    }
}
