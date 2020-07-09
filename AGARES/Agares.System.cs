using FSPAKE.AGARES.Unit;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace FSPAKE.AGARES.CoreSystem
{
    partial class Agares
    {
        /// <summary>
        /// Agaresクラスの基幹処理を記述する
        /// </summary>
        /// 


        /// <summary>受信済み判定用ディクショナリ</summary>
        private static readonly List<string> AlreadyRecievedLiveChatId = new List<string>();
        /// <summary>コメント履歴</summary>
        private static readonly List<string> AlreadyRecievedLiveChatComment = new List<string>();

        private bool LiveStateCheck(Video videoInfo)
        {
            if (videoInfo is null)
            {
                UIManager.MessageBoxShow("指定されたライブは存在しないか、既に削除されている可能性があります。",
                    "AGARES", MessageBoxButtons.OK, MessageBoxType.Error);
                return false;
            }
            if (videoInfo.LiveStreamingDetails.ActualStartTime is null)
            {
                UIManager.MessageBoxShow($"このライブはまだ始まっていません。" +
                $"開始予定時刻は{videoInfo.LiveStreamingDetails.ScheduledStartTime}です",
                "AGARES", MessageBoxButtons.OK, MessageBoxType.Information);
            }
            return true;
        }

        private bool LiveChatStateCheck(string liveChatId)
        {

            if (string.IsNullOrEmpty(liveChatId))
            {
                UIManager.MessageBoxShow("ライブチャットが取得できませんでした。\n" +
                    "対象が動画であるか、既にアーカイブ化されている可能性があります。",
                    "AGARES", MessageBoxButtons.OK, MessageBoxType.Error);
                GetStart.Visible = true;
                GetStop.Visible = false;
                AddNGListenerToolStripMenuItem.Visible = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// メイン処理の外郭
        /// </summary>
        /// <param name="liveChatResponse">YouTubeAPIのLiveStreams-Listで取得したコレクション</param>
        /// <returns>Task</returns>
        private async Task EnumerateLivechat(LiveChatMessageListResponse liveChatResponse)
        {
            IntervalTimeTotal = 0;
            foreach (var liveChatMessage in liveChatResponse.Items)
            {
                if (!IsGetChat) { return; }
                //各フィールドにアクセスするGetメソッドを使用できるようにする
                var liveChat = new LiveChatFieldAccesser(liveChatMessage);
                if (!AlreadyRecievedLiveChatId.Contains(liveChat.LiveChatId))
                {
                    //新規受信分だったとき
                    await MainProcessAsync(liveChat).ConfigureAwait(true);
                    AlreadyRecievedLiveChatId.Add(liveChat.LiveChatId);
                    AlreadyRecievedLiveChatComment.Add(liveChat.DisplayMessage);
                    await Task.Delay(GetIntervalTime(liveChatResponse)).ConfigureAwait(true);
                }
            }
            await Task.Delay(GetIntervalTime(liveChatResponse)).ConfigureAwait(true);
        }

        private int IntervalTimeTotal = 0;
        private int GetIntervalTime(LiveChatMessageListResponse responce)
        {
            if (responce == null) { throw new ArgumentNullException(nameof(responce)); }
            if (responce.Items.Count == 0) { return 0; }
            if (responce.PollingIntervalMillis == null) { throw new ArgumentNullException(nameof(responce.PollingIntervalMillis)); }

            var intervalTimeOverall = (int)responce.PollingIntervalMillis;
            if (intervalTimeOverall <= IntervalTimeTotal) { return 0; }

            var intervalTime = intervalTimeOverall / responce.Items.Count;

            if (intervalTimeOverall <= IntervalTimeTotal + intervalTime)
            {
                intervalTime = intervalTimeOverall - IntervalTimeTotal;
            }
            IntervalTimeTotal += intervalTime;
            return intervalTime;
        }

        /// <summary>画面表示対象のチャットの通番</summary>
        private int EffectiveChatSeq = 0;
        private readonly Dictionary<int,string> AnalizedDisplayAreaHouse = new Dictionary<int, string>();
        private readonly Dictionary<int,Dictionary<int,Image>> ColumnsOfImgForLine= new Dictionary<int, Dictionary<int, Image>>();

        /// <summary>チャットの保存領域</summary>
        private readonly Dictionary<int, LiveChatFieldAccesser> LiveChatHouse = new Dictionary<int, LiveChatFieldAccesser>();
        /// <summary>
        /// メイン処理
        /// </summary>
        /// <param name="liveChatMessage">YouTubeAPI Livestreams-Listで取得したコレクションのアイテム</param>
        private async Task MainProcessAsync(LiveChatFieldAccesser liveChat)
        {
            try
            {
                if (!(Bael is null))
                {
                    await BaelJobAsync(liveChat).ConfigureAwait(false);
                }
            }
            catch(Exception e) when(e is Google.GoogleApiException || e is AggregateException)
            {
                //権限のないユーザーがBanを実行したとき、
                //Exceptionが発生し制御が変わってしまうので、
                //それを掬う 
            }
            AgaresJob(liveChat);
        }


        private void AgaresJob(LiveChatFieldAccesser liveChat)
        {
            if (!UserBlockJudger.IsNGWord(liveChat.DisplayMessage) &&
                !UserBlockJudger.IsNGListener(liveChat.DisplayName))
            {
                LiveChatDisplay(liveChat);
            }
        }

        private void LiveChatDisplay(LiveChatFieldAccesser liveChat)
        {
            LiveChatHouse.Add(EffectiveChatSeq, liveChat);
            EffectiveChatSeq++;
            AddAmountByCurrency(liveChat);

            if (IconMode.Checked)
            {
                RegisterListenerIcon(liveChat);
            }

            var commentBuilder = new CommentBuilder
            {
                BackColor = new ColorConverter().ConvertToString(liveChat.BackColor),
                LetterColor = new ColorConverter().ConvertToString(liveChat.LetterColor),
                ListenerName = liveChat.DisplayName,
                Comment = liveChat.DisplayMessage
            };


            var displayArea = commentBuilder.DisplayArea;
            
            MembershipEmoji.Analize(in displayArea, out displayArea, out analizedResult,EmojiFilename);
            ColumnsOfImgForLine.Add(EffectiveChatSeq, analizedResult);
            AnalizedDisplayAreaHouse[EffectiveChatSeq] = displayArea;
            Comments.Items.Add(commentBuilder.ConcatResult);
            if (Comments.TopIndex == Comments.Items.Count - (Comments.Height / Comments.ItemHeight)-1)
            {
                Comments.TopIndex = Comments.Items.Count - (Comments.Height / Comments.ItemHeight);
            }

            PopSuperChatReviewLabel(liveChat);
        }
        /// <summary>
        /// 画面下部にスパチャ金額を加算する
        /// </summary>
        /// <param name="liveChat">チャット</param>
        private void AddAmountByCurrency(LiveChatFieldAccesser liveChat)
        {
            if (liveChat.IsFanFund)
            {
                var amount = (long)liveChat.Amount;
                var currency = liveChat.Currency;
                if (currency == "NTD")
                {
                    currency = "TWD";
                }
                foreach (Control c in Controls)
                {
                    if (c.Name == currency)
                    {
                        c.Text = (Parse.ToInt(c.Text) + amount).ToString();
                    }
                }
            }
        }

        private async Task BaelJobAsync(LiveChatFieldAccesser liveChat)
        {
            if (await Bael.JudgeSuspectAsync(liveChat, AlreadyRecievedLiveChatComment, APIkey.Text).ConfigureAwait(false))
            {
                if (Bael.IsBanning)
                {
                    YouTubeAPIManager.UpdateCredentials(UserCredential);
                    await Bael.LiveChatBanAsync(YouTubeAPIManager,LiveChatId, liveChat).ConfigureAwait(false);
                    YouTubeAPIManager.UpdateCredentials(null);
                }
                else
                {
                    if (Suspects.TopIndex == Suspects.Items.Count - Suspects.Height / Suspects.ItemHeight - 1)
                    {
                        Suspects.TopIndex = Suspects.Items.Count - 1;
                    }
                    Suspects.Items.Add(liveChat.DisplayName + "：" + liveChat.DisplayMessage);
                }
            }
        }
    }
}
