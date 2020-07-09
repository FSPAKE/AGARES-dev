using Google;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using FSPAKE.AGARES.Unit;
namespace FSPAKE.AGARES.CoreSystem
{
    public class UserBanJudger : UserJudgeSystem, IDisposable
    {
        private readonly List<string> BoomWordList;
        private readonly StringConverter AlphabetConverter;
        private Emoji EmojiManager;
        private readonly List<string> Master;
        private int ViolationPoint;


        public UserBanJudger(
            string banUserFilename,
            string boomWordFilename,
            string emojiFilename,
            string gaijiFilename,
            string sexualAndViolenceFilename) : base(banUserFilename)
        {
            var githubURL = "https://raw.githubusercontent.com/FSPAKE/AGARES/master/AGARES/";
            TextfileManager.DownloadFile(boomWordFilename, "BAEL", githubURL, MessageBoxType.Information);
            BoomWordList = new XMLManager(boomWordFilename).DeserializeValue();

            TextfileManager.DownloadFile(gaijiFilename, "BAEL", githubURL, MessageBoxType.Information);
            AlphabetConverter = new StringConverter(gaijiFilename);

            TextfileManager.DownloadFile(emojiFilename, "BAEL", githubURL, MessageBoxType.Information);
            EmojiManager = new Emoji(emojiFilename);

            TextfileManager.DownloadFile(sexualAndViolenceFilename, "BAEL", githubURL, MessageBoxType.Information);
            Master = new XMLManager(sexualAndViolenceFilename).DeserializeValue();
        }



        public void IsJudgedNGListener(int point, string Listener)
        {
            TargetMode mode = Listeners;
            if (Judge(Listener, mode))
            {
                ViolationPoint += point;
            }
        }
        public void IsJudgedNGWord(int point, string Listener)
        {
            TargetMode mode = Words;
            if (Judge(Listener, mode))
            {
                ViolationPoint += point;
            }
        }
        public void IsSexualAndViolenceWord(int point, string text)
        {
            if (text is null) { return; }

            var transaction = AlphabetConverter.Convert(text).ToUpper();

            var master = Master.Select(mas => mas.ToUpper()).ToList();
            if (master.Contains(transaction))
            {
                ViolationPoint += point;
            }
        }
        public void Reset()
        {
            ViolationPoint = 0;
        }
        public bool IsGuilty()
        {
            return ViolationPoint >= 100;
        }
        private static class ChannelsObject
        {
            //ChannelsList's Part
            public const string Id = "id";
            public const string Snippet = "snippet";
            public const string BrandingSettings = "brandingSettings";
            public const string ContentDetails = "contentDetails";
            public const string InvideoPromotion = "invideoPromotion";
            public const string Statistics = "statistics";
            public const string TopicDetails = "topicDetails ";

            //Other
            public const string AuditDetails = "AuditDetails";
            public const string ContentOwnerDetails = "ContentOwnerDetails";
            public const string ConversionPings = "ConversionPings";
            public const string ETag = "ETag";
            public const string Kind = "Kind";
            public const string ChannelLocalization = "ChannelLocalization";
            public const string Status = "Status";
        }
        public void JudgeEmojiCount(int violation, string text, int emojiBorderLength, bool tailContinuation, bool unti)
        {

            if (EmojiManager.CountTailEmoji(text, tailContinuation, unti) >= emojiBorderLength)
            {
                ViolationPoint += violation;
            }
        }
        public void JudgeCounterfeit(int violation, string text, List<string> pastCommentList, int emojiBorderLength, int stringBorderLength,bool unti)
        {
            if (text is null) { return; }
            if (pastCommentList is null) { return; }
            if (pastCommentList.Count == 0) { return; }
            if (!EmojiManager.IsTailEmoji(text)) { return; }
            if (EmojiManager.Length(text) >= stringBorderLength &&
                EmojiManager.CountTailEmoji(text, false, unti) >= emojiBorderLength)
            {
                while (true)
                {
                    if (pastCommentList.Contains(text) &&
                        !BoomWordList.Contains(text))
                    {
                        ViolationPoint += violation;
                        break;
                    }
                    if (!EmojiManager.IsTailEmoji(text)) { break; }
                    text = EmojiManager.DeleteTail(text, 1);
                }
            }
        }

        public async Task JudgeFreshAsync(int point, int days, string channelId, string apikey)
        {
            if (point == 0) { return; }
            if (channelId is null) { throw new ArgumentNullException(nameof(channelId)); }
            if (channelId == "") { throw new ArgumentException(nameof(channelId)); }
            if (apikey is null) { throw new ArgumentNullException(nameof(apikey)); }
            if (apikey == "") { throw new ArgumentException(nameof(apikey)); }
            var part = new string[] { ChannelsObject.Snippet }.ToList();
            var YouTubeAPIManager = new YouTubeAPIManager();
            YouTubeAPIManager.UpdateApikey(apikey);
            ChannelListResponse response;
            try
            {
                response = await YouTubeAPIManager.ChannelListAsync(channelId, part).ConfigureAwait(true);
            }
            catch (Exception channelListFailure) when (channelListFailure is GoogleApiException || channelListFailure is AggregateException)
            {
                return;
            }
            if (response.Items.Count == 0) { return; }

            var span = DateTime.Now - response.Items[0].Snippet.PublishedAt;
            if (span.Value.TotalDays <= days)
            {
                ViolationPoint += point;
            }
        }

        /*AuditDetailsは廃止
        public async Task JudgeGuideLineViolationAsync(string channelId, string apikey)
        {
            var LackItem = new List<string>();

            if (AlreadyGetChannelObject.AuditDetails == null)
            {
                LackItem.Add(ChannelsObject.Id);
                LackItem.Add(ChannelsObject.BrandingSettings);
                LackItem.Add(ChannelsObject.ContentDetails);
                LackItem.Add(ChannelsObject.InvideoPromotion);
                LackItem.Add(ChannelsObject.Statistics);
                LackItem.Add(ChannelsObject.TopicDetails);
            }


            if (LackItem.Count != 0)
            {
                YouTubeAPIManager.ApikeyUpdate(apikey);

                var responce = await YouTubeAPIManager.ChannelListAsync(channelId, LackItem).ConfigureAwait(true);
                if (LackItem.Contains(ChannelsObject.AuditDetails))
                {
                    AlreadyGetChannelObject.AuditDetails = responce.Items[0].AuditDetails;
                }
            }
            bool? guidelineViolation;
            if ((bool)AlreadyGetChannelObject.AuditDetails.CommunityGuidelinesGoodStanding)
            {
                guidelineViolation = true;
            }
            else
            {
                guidelineViolation = false;
            }
            if ((bool)guidelineViolation)
            {
                ViolationPoint += 1;
            }
        }
        */

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EmojiManager = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
