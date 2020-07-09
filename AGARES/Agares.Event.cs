using Extension;
using FSPAKE.AGARES.Unit;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using static FSPAKE.AGARES.CoreSystem.UserJudgeSystem;

namespace FSPAKE.AGARES.CoreSystem
{

    public partial class Agares : Form
    {
        private Bael Bael;
        private YouTubeAPIManager YouTubeAPIManager;
        private readonly OAuthManager OAuthManager;
        private UserCredential UserCredential;
        private string LiveChatId;
        private static Dictionary<string, Image> ListenerIconDictionary = new Dictionary<string, Image>();
        private int FlowStopX;
        public static readonly string ApikeyFilename = "APIKeys.txt";
        public static readonly string BaelConfigFilename = "BaelConfig.xml";
        public static readonly string BoomWordFilename = "BoomWordList.xml";
        public static readonly string BlockFilename = "BlockList.xml";
        public static readonly string BanUserFilename = "BanList.xml";
        public static readonly string ClientSecletFilename = "client_secrets.json";
        public static readonly string EmojiFilename = "Emoji.txt";
        public static readonly string IconFoldername = "ListenerIcon";
        public static readonly string GaijiFilename = "OutcodeChars.txt";
        public static readonly string LastLiveIDFilename = "LastLiveID.txt";
        public static readonly string SexualAndViolenceFilename = "SexualAndViolenceWords.xml";
        public static readonly string MembershipFolderName = "MembershipImages";
        public static readonly string YoutubeFolderName = "YouTubeImages";
        private readonly TextfileManager LiveidTextfile;
        private readonly GoogleApikeyManager ApikeyManager;
        private readonly UserBlockJudger UserBlockJudger;
        private UserBanJudger UserBanJudger;
        private bool IsGetChat { get; set; }

        //起動直後にtrueになる
        private bool IsViewSuperchatReviewLabel = false;

        public Agares()
        {
            InitializeComponent();
            ModifyComponent();

            //初回設定
            Agares_SizeChanged(null, null);

            ApikeyManager = new GoogleApikeyManager(ApikeyFilename);
            APIkey.Text = ApikeyManager.GetNextKey();
            OAuthManager = new OAuthManager();
            try { UserBlockJudger = new UserBlockJudger(BlockFilename); }
            catch (FileNotFoundException)
            {
                XMLManager.Create(BlockFilename);
                UserBlockJudger = new UserBlockJudger(BlockFilename);
            }
            try { UserBanJudger = new UserBanJudger(BanUserFilename,BoomWordFilename,EmojiFilename,GaijiFilename,SexualAndViolenceFilename); }
            catch (FileNotFoundException)
            {
                XMLManager.Create(BanUserFilename);
                UserBanJudger = new UserBanJudger(BanUserFilename, BoomWordFilename, EmojiFilename, GaijiFilename, SexualAndViolenceFilename);
            }
            Directory.CreateDirectory(IconFoldername);
            ListenerIconDictionary = GraphicManager.Scale(GraphicManager.Load(IconFoldername, GraphicManager.FileNameType.FileName), Comments.ItemHeight);

            LiveidTextfile= new TextfileManager(LastLiveIDFilename);
            if (LiveidTextfile.IsExist())
            {
                LiveID.Text = LiveidTextfile.ReadNext();
            }
            FlowStopX = SuperChatReviewPanel.Location.X;

            PostCommentBoxPanel.Visible = false;
            MinimumSize = Size;
            this.Controls.Remove(Comments);
            YouTubeAPIManager = new YouTubeAPIManager(APIkey.Text);
            MembershipEmoji.LoadYoutube(YoutubeFolderName);
            superChatのポップ切替現在ToolStripMenuItem_Click(null, null);
            TransformAgares();
        }

        private Video VideoInfo;
        private string LastLiveId;
        private string LastVideoId;
        private async void GetStart_ClickAsync(object sender, EventArgs e)
        {
            GetStart.Visible = false;

            if (LiveID.Text != LastLiveId)
            {
                VideoInfo = await GetVideinfo();
                LastLiveId = LiveID.Text;
                if (VideoInfo.Id != LastVideoId) { Clear(); }
                LastVideoId = VideoInfo.Id;
            }
            if (VideoInfo is null) { return; }
            if (!LiveStateCheck(VideoInfo)) { return; }

            if (!LiveChatStateCheck(VideoInfo.LiveStreamingDetails.ActiveLiveChatId)) { return; }

            if (LiveChatId != VideoInfo.LiveStreamingDetails.ActiveLiveChatId)
            {
                LiveChatId = VideoInfo.LiveStreamingDetails.ActiveLiveChatId;
                DrawStreamInfo(VideoInfo);
                MembershipEmoji.LoadMembership(MembershipFolderName, VideoInfo.Snippet.ChannelId);
                MembershipEmoji.Scale(Comments.ItemHeight);
            }

            AddNGListenerToolStripMenuItem.Visible = true;
            //PostCommentBoxPanel.Visible = true;//コメント投稿機能は実装を取りやめる

            IsGetChat = true;

            GetStop.Visible = true;
            GetStart.Visible = false; //マルチスレッド実行時、別スレッドでtrueになる場合があるため再度falseを設定する

            while (IsGetChat)
            {
                LiveChatMessageListResponse responce = null;
                try
                {
                    responce = await YouTubeAPIManager.GetLiveChatMessage(LiveChatId).ConfigureAwait(true);
                }
                catch (Exception AgaresFailureReason) when (AgaresFailureReason is GoogleApiException || AgaresFailureReason is AggregateException){}
                try
                {
                    await EnumerateLivechat(responce).ConfigureAwait(true);
                }
                catch (Exception AgaresFailureReason)   when (AgaresFailureReason is GoogleApiException || AgaresFailureReason is AggregateException)
                {
                    if(AgaresFailureReason.Message.Contains("The request was sent too soon after the previous one. "))
                    {
                        await Task.Delay(GetIntervalTime(responce)).ConfigureAwait(true);
                    }
                }
                catch (Exception OtherReason) { /*ignored*/ };
            }
            GetStart.Visible = true;
            GetStop.Visible = false;
            PostCommentBoxPanel.Visible = false;
        }

        private void Clear()
        {
            AlreadyRecievedLiveChatId.Clear();
            AlreadyRecievedLiveChatComment.Clear();
            Suspects.Items.Clear();
            Comments.Items.Clear();
            LiveChatHouse.Clear();
            ColumnsOfImgForLine.Clear();
            ListenerIconDictionary.Clear();
            AnalizedDisplayAreaHouse.Clear();
            ColumnsOfImgForLine.Clear();

            EffectiveChatSeq = 0;
            SuperReviewSeq = 0;
            var list = Controls;
            var i = 0;
            while(i < list.Count)
            {
                var c = list[i];
                if (c.Name.IndexOf("scrl") == 0) 
                {
                    Controls.Remove(c);
                }
                else
                {
                    i++;
                }
            }
        }

        private async Task<Video> GetVideinfo()
        {
            var canvas = new Bitmap(Thumb.Width, Thumb.Height);
            var g = Graphics.FromImage(canvas);
            var font = new Font("ＭＳ ゴシック", 10);
            g.DrawString("配信情報を\nロード中....",font,Brushes.Black,0,0);
            Thumb.Image = canvas;

            while (true)
            {
                try
                {
                    return await YouTubeAPIManager.GetVideoInfoAsync(LiveID.Text).ConfigureAwait(true);
                }
                catch (GoogleApiException)
                {
                    var key = ApikeyManager.GetNextKey();

                    if (!string.IsNullOrEmpty(key))
                    {
                        APIkey.Text = key;
                        YouTubeAPIManager.UpdateApikey(APIkey.Text);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private void GetStop_Click(object sender, EventArgs e)
        {
            GetStop.Visible = false;
            GetStart.Visible = true;
            IsGetChat = false;
        }

        private void NGListenerName_Leave(object sender, EventArgs e)
        {
            if (ActiveControl != AddBlockListenerName &&
                ActiveControl != BlockMatchComp &&
                ActiveControl != BlockMatchPart)
            {
                AddBlockListenerName.Visible = false;
                BlockMatchComp.Checked = false;
                BlockMatchPart.Checked = false;
            }
        }

        private void NGListenerName_Enter(object sender, EventArgs e)
        {
            AddBlockWord.Visible = false;
            AddBlockListenerName.Visible = true;
        }

        private void NGWord_Leave(object sender, EventArgs e)
        {
            if (ActiveControl != AddBlockWord &&
                ActiveControl != BlockMatchComp &&
                ActiveControl != BlockMatchPart)
            {
                AddBlockWord.Visible = false;
                BlockMatchComp.Checked = false;
                BlockMatchPart.Checked = false;
            }

        }

        private void NGWord_Enter(object sender, EventArgs e)
        {
            AddBlockListenerName.Visible = false;
            AddBlockWord.Visible = true;
        }
        private void BanWord_Leave(object sender, EventArgs e)
        {
            if (ActiveControl != AddBanWord &&
                ActiveControl != BanMatchComp &&
                ActiveControl != BanMatchPart)
            {
                AddBanWord.Visible = false;
                BanMatchComp.Checked = false;
                BanMatchPart.Checked = false;
            }
        }
        private void BanWord_Enter(object sender, EventArgs e)
        {
            AddBanListenerName.Visible = false;
            AddBanWord.Visible = true;
        }
        private void BanListenerName_Leave(object sender, EventArgs e)
        {
            if (ActiveControl != AddBanListenerName &&
                ActiveControl != BanMatchComp &&
                ActiveControl != BanMatchPart)
            {
                AddBanListenerName.Visible = false;
                BanMatchComp.Checked = false;
                BanMatchPart.Checked = false;
            }
        }

        private void BanListenerName_Enter(object sender, EventArgs e)
        {
            AddBanListenerName.Visible = true;
            AddBanWord.Visible = false;
        }
        private void Comments_DrawItem(object sender, DrawItemEventArgs e)
        {
            CommentsDrawItem(sender, e);
        }

        private void SaveLiveID_Click(object sender, EventArgs e)
        {
            LiveidTextfile.WriteLine(LiveID.Text);
        }
        private void SaveApiKey_Click(object sender, EventArgs e)
        {
            ApikeyManager.AppendKey(APIkey.Text);
        }
        private void 終了QToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void アプリケーションについてToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new Signature()
            {
                StartPosition = FormStartPosition.CenterParent,
                ShowInTaskbar = false,
            };
            form.ShowDialog();
            form.Dispose();
        }

        private void Agares_SizeChanged(object sender, EventArgs e)
        {
            Comments.Size = this.ClientSize;

            SuperReviewHolizontalNum = SuperChatReviewPanel.Width / 105;
            AlignmentSuperchatLabel();
        }


        private void AddBlockWord_Click(object sender, EventArgs e)
        {
            BlockBaseCondition(BlockWord, Words);
        }

        private void AddBlockListenerName_Click(object sender, EventArgs e)
        {
            BlockBaseCondition(BlockListenerName, Listeners); 
        }

        private void AddBanWord_Click(object sender, EventArgs e)
        {
            AddBanTarget(BanWord, Words);
        }

        private void AddBanListenerName_Click(object sender, EventArgs e)
        {
            AddBanTarget(BanListenerName, Listeners);
        }

        private async void BaelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await TransformBaelAsync();
            try
            {
                UserBanJudger = new UserBanJudger(BanUserFilename, BoomWordFilename, EmojiFilename, GaijiFilename, SexualAndViolenceFilename);
                using (var xmlManager = new XMLManager(BaelConfigFilename))
                {
                    var configData = xmlManager.DeserializeToDictionary();
                    if (!(configData.ContainsKey("QuietOnActivate") && 
                         Parse.ToBool(configData["QuietOnActivate"])))
                    {
                        詳細設定ToolStripMenuItem_Click(null, null);
                    }
                    Bael = new Bael(UserBanJudger, BaelConfigFilename);
                }
            }
            catch (FileNotFoundException NotFound)
            {
                UIManager.MessageBoxShow(NotFound.FileName + "が見つかりません。BAN自動実行モードを終了します。", "BAEL", MessageBoxButtons.OK, MessageBoxType.Error);
                TransformAgares();
            }
        }


        private void AgarestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransformAgares();
        }


        private void AutoBanSwitch_Click(object sender, EventArgs e)
        {
            if (Bael is null) { return; }
            if (Bael.IsBanning)
            {
                Bael.Stop();
            }
            else
            {
                Bael.Start(BanType.Text, BanTime.Value);
            }
            ((RoundButton)sender).ButtonText = Bael.IsBanning ? "自動BAN終了" : "自動BAN開始";

        }
        private async void PostCommentBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*消費クォータがあまりにも多すぎるため廃止
            var success = false;
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                while (!success)
                {
                    try
                    {
                        await YouTubeAPIManager.InsertCommentAsync(LiveChatId, ((TextBox)sender).Text).ConfigureAwait(true);
                        ((TextBox)sender).Text = "";
                        success = true;
                    }

                    catch (TokenResponseException)
                    {
                        UserCredential = await NewCredentialAsync(ClientSecletFilename).ConfigureAwait(true);
                        YouTubeAPIManager.UpdateCredentials(UserCredential);
                    }
                    catch (GoogleApiException exceptionMessage)
                    {
                        if (exceptionMessage.ToString().Contains("The message could not be created because of insufficient permissions."))
                        {
                            UIManager.MessageBoxShow("コメント投稿権限の不足により投稿に失敗しました。\n" +
                                "YouTubeとのサービス契約が正常に為されていない、\n" +
                                "ストリーマーあるいはモデレーターからタイムアウト・ブロックなどの措置を取られている可能性があります。", 
                                "AGARES", MessageBoxButtons.OK, MessageBoxType.Information);
                            return;
                        }
                        else
                        if (exceptionMessage.ToString().Contains("text is not valid."))
                        {
                            UIManager.MessageBoxShow("コメントテキストが無効です。" +
                                "またこのエラーは、低速モードがオンになっているライブにおいて規定時間内に複数回コメント投稿を行った際にも発生します。", 
                                "AGARES", MessageBoxButtons.OK, MessageBoxType.Information);
                        }
                    }
                }
            }
            */
        }

        private void 詳細設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var baelConfigMenu = new BaelConfigMenu(BoomWordFilename,BaelConfigFilename, SexualAndViolenceFilename))
            {
                baelConfigMenu.Owner = this;
                baelConfigMenu.ShowDialog();
                baelConfigMenu.BringToFront();
            }
        }

        private void AddNGListenerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Comments.SelectedIndex == -1) { return; }

            BlockShootIndividual(new CommentBuilder(SelectingConcatedLine).ListenerName);
        }

        private void BanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string item;
            try { item = ((ComboBox)sender).SelectedItem.ToString(); }
            catch (NullReferenceException) { return; }
            if (item == "　一時的")
            {
                タイムアウト.Visible = true;
                BanTime.Visible = true;
                秒.Visible = true;
            }
            else if (item == "　永久")
            {
                タイムアウト.Visible = false;
                BanTime.Visible = false;
                秒.Visible = false;
            }
        }



        private void superChatのポップ切替現在ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsViewSuperchatReviewLabel = !IsViewSuperchatReviewLabel;
            superChatのポップ切替現在ToolStripMenuItem.Text = IsViewSuperchatReviewLabel
                ? superChatのポップ切替現在ToolStripMenuItem.Text.Split(new string[] { "：" }, StringSplitOptions.None)[0] + "：ＯＮ"
                : superChatのポップ切替現在ToolStripMenuItem.Text.Split(new string[] { "：" }, StringSplitOptions.None)[0] + "：ＯＦＦ";
            foreach(Control c in Controls)
            {
                var name = new StringManager(c.Name);
                if (name.Length >= 4 && name.Left(4) == "scrl") 
                {
                    c.Visible = IsViewSuperchatReviewLabel;
                }
            }
        }

        private void BlockWordListLabel_Click(object sender, EventArgs e)
        {
            BlockBanListLabelClick((Label)sender, BlockMatchTypeBox, BlockFilename);
        }

        private void BlockListenerListLabel_Click(object sender, EventArgs e)
        {
            BlockBanListLabelClick((Label)sender, BlockMatchTypeBox, BlockFilename);
        }

        private void BanWordListLabel_Click(object sender, EventArgs e)
        {
            BlockBanListLabelClick((Label)sender, BanMatchTypeBox, BanUserFilename);
        }

        private void BanListenerListLabel_Click(object sender, EventArgs e)
        {
            BlockBanListLabelClick((Label)sender, BanMatchTypeBox, BanUserFilename);
        }

        private void BlockBanListLabelClick(Label sender,Panel radioBox,string filename)
        {
            if (!UIManager.CheckRadioButton(radioBox)) { return; }

            var checkingButtonName = UIManager.GetCheckingRadioButton(radioBox).Name;
            var matchType = new StringManager(checkingButtonName).Right(4);

            var selectItem = new List<string>();
            var targetType="";
            if (sender.Name.Contains(Words.ToString()))
            {
                targetType = Words.ToString();
            }
            if (sender.Name.Contains(Listeners.ToString()))
            {
                targetType = Listeners.ToString();
            }
            selectItem.Add(matchType + targetType);
            using (var view = new XMLElementBox(filename, matchType + targetType, selectItem))
            {
                view.Owner = this;
                view.ShowDialog();
                view.BringToFront();
            }
            UserBlockJudger.LoadNGWord();
            UserBlockJudger.LoadNGListener();
            UserBanJudger.LoadNGWord();
            UserBanJudger.LoadNGListener();
        }

        private void Comments_DoubleClick(object sender, EventArgs e)
        {
            var livechat = new CommentBuilder(SelectingConcatedLine);
            var commentDisplayForm = new CommentDisplayForm(EmojiFilename,IconFoldername)
            {
                ChannelId = LiveChatHouse[SelectingIndex].ChannelId,
                WindowName = livechat.ListenerName,
                DisplayMessage = livechat.Comment,
                BackColor = LiveChatHouse[SelectingIndex].BackColor,
                StartPosition = FormStartPosition.CenterParent,
            };
            commentDisplayForm.ShowDialog();
        }

        private void IconMode_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                foreach(var livechat in LiveChatHouse)
                {
                    if (!ListenerIconDictionary.ContainsKey(livechat.Value.ChannelId))
                    {
                        ListenerIconDictionary.Add(livechat.Value.ChannelId,GraphicManager.Scale(GraphicManager.Download(livechat.Value.ProfileImageURL,IconFoldername,livechat.Value.ChannelId),Comments.ItemHeight));
                    }
                }
            }
            Comments.Refresh();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var liveChatMessage = new LiveChatMessage
            {
                AuthorDetails = new LiveChatMessageAuthorDetails
                {
                    ChannelId = "aa",
                    DisplayName = "FSP AKE",
                    IsChatModerator = false,
                    IsChatOwner = false,
                    IsChatSponsor = false,
                    ProfileImageUrl = "https://www.youtube.com/watch?v=wXnrsTGbVmA"
                },
                Snippet = new LiveChatMessageSnippet
                {
                    AuthorChannelId = "xxx",
                    DisplayMessage = ":えもじにじれじ:",
                    LiveChatId = "fsjkl"
                }
            };

            var liveChatFieldAccesser = new LiveChatFieldAccesser(liveChatMessage);
            await MainProcessAsync(liveChatFieldAccesser);
        }
    }
}
