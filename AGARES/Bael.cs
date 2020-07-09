using FSPAKE.AGARES.Unit;
using Google.Apis.Auth.OAuth2.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Utils;
using static FSPAKE.AGARES.CoreSystem.UserJudgeSystem;

namespace FSPAKE.AGARES.CoreSystem
{
    public class Bael
    {
        private readonly string ConfigFilename;
        private readonly UserBanJudger UserBanJudger;
        public ulong TimeoutTime;
        public string TimeoutType;
        Dictionary<XName, string> ConfigData;

        public bool IsBanning { get; set; } = false;

        public Bael(UserBanJudger userBanJudger, string configFilename)
        {
            ConfigFilename = configFilename;
            ConfigLoad();
            UserBanJudger = userBanJudger;
        }
        private void ConfigLoad()
        {
            using (var xmlManager = new XMLManager(ConfigFilename))
            {
                ConfigData = xmlManager.DeserializeToDictionary();
            }
        }

        public async Task<bool> JudgeSuspectAsync(LiveChatFieldAccesser liveChat, List<string> pastCommentList, string apikey)
        {
            if (liveChat is null) { return default; }
            UserBanJudger.Reset();
            if (Parse.ToBool(ConfigData["Option1"]))
            {
                var point = Parse.ToInt(ConfigData["Point1"]);
                var text = liveChat?.DisplayMessage;
                var stringBorderLength = Parse.ToInt(ConfigData["DecisionCriteria11"]);
                var emojiBorderLength = Parse.ToInt(ConfigData["DecisionCriteria12"]);
                var unti = Parse.ToBool(ConfigData["DecisionCriteria15"]);
                UserBanJudger.JudgeCounterfeit(point, text, pastCommentList, emojiBorderLength, stringBorderLength, unti);
            }

            if (Parse.ToBool(ConfigData["Option2"]))
            {
                var point = Parse.ToInt(ConfigData["Point2"]);
                var continous = Parse.ToBool(ConfigData["DecisionCriteria23"]);
                bool unti = Parse.ToBool(ConfigData["DecisionCriteria24"]);
                var emojiBorderLength = Parse.ToInt(ConfigData["DecisionCriteria21"]);
                UserBanJudger.JudgeEmojiCount(point, liveChat.DisplayMessage, emojiBorderLength, continous, unti);
            }

            if (Parse.ToBool(ConfigData["Option3"]))
            {
                var point = Parse.ToInt(ConfigData["Point3"]);
                UserBanJudger.IsJudgedNGListener(point, liveChat.DisplayName);
            }
            if (Parse.ToBool(ConfigData["Option4"]))
            {
                var point = Parse.ToInt(ConfigData["Point4"]);
                UserBanJudger.IsJudgedNGWord(point, liveChat.DisplayMessage);
            }


            if (Parse.ToBool(ConfigData["Option5"]))
            {
                var point = Parse.ToInt(ConfigData["Point5"]);
                UserBanJudger.IsSexualAndViolenceWord(point, liveChat.DisplayName);
            }


            if (Parse.ToBool(ConfigData["Option6"]))
            {
                var point = Parse.ToInt(ConfigData["Point6"]);
                var days = Parse.ToInt(ConfigData["DecisionCriteria61"]);
                await UserBanJudger.JudgeFreshAsync(point, days, liveChat.ChannelId, apikey).ConfigureAwait(true);
            }

            return UserBanJudger.IsGuilty();
        }

        public async Task LiveChatBanAsync(YouTubeAPIManager youTubeAPIManager, string liveChatId, LiveChatFieldAccesser liveChat)
        {
            if (youTubeAPIManager is null ||
                liveChatId is null ||
                liveChat is null){ return; }

            var channelId = liveChat.ChannelId;
            var error = false;
            while (!error)
            {
                try
                {
                    await youTubeAPIManager.UserBANAsync(liveChatId, TimeoutType, TimeoutTime, channelId);
                    return;
                }
                catch (TokenResponseException e)
                {
                    if (e.Message.Contains("invalid_grant"))
                    {
                        /// invalid_grantはリトライする
                    }
                    else
                    {
                        error = true;
                    }
                }
                catch { error = true; }
            }
            UIManager.MessageBoxShow("ユーザーBANにおいて予期せぬエラーが発生しました。\n" +
                "ユーザーBANをキャンセルします。", "BAEL", MessageBoxButtons.OK, MessageBoxType.Information);
        }

        public void Start(string BanType, decimal BanTime)
        {
            switch (BanType)
            {
                case "　一時的":
                    if (BanTime <= 0)
                    {
                        UIManager.MessageBoxShow("タイムアウト時間に1以上の値を指定してください。",
                            "AGARES", MessageBoxButtons.OK, MessageBoxType.Information);
                        return;
                    }
                    TimeoutType = YouTubeAPIManager.BanType.temporary;
                    TimeoutTime = (ulong)BanTime;
                    break;

                case "　永久":
                    if (UIManager.MessageBoxShow("タイムアウトの種別が「永久」になっています。お間違えないですか？",
                            "AGARES", MessageBoxButtons.YesNo, MessageBoxType.Warning) == DialogResult.No)
                    {
                        UIManager.MessageBoxShow("キャンセルしました", "AGARES", MessageBoxButtons.OK, MessageBoxType.None);
                        return;
                    }
                    TimeoutType = YouTubeAPIManager.BanType.permanent;
                    break;
                default:
                    UIManager.MessageBoxShow("BANタイプ（永久／一時的）を指定してください。",
                        "AGARES", MessageBoxButtons.OK, MessageBoxType.Information);
                    return;
            }
            var dialogResult = UIManager.MessageBoxShow(
                                "自動ユーザーBANはあなたの指定した条件に基づいて対象ユーザーを決定\n" +
                                "しますが、あなたの意図しないユーザーを対象に取るかもしれません。\n" +
                                "これについて、当アプリケーション及びその開発者は一切の責任を負わない\n" +
                                "ことをご了承ください。\n\n" +
                                "起動してよろしいですか？", "BAEL", MessageBoxButtons.YesNo, MessageBoxType.Warning);
            if (dialogResult == DialogResult.No)
            {
                UIManager.MessageBoxShow("自動ユーザーBANの起動をキャンセルしました。", "BAEL", MessageBoxButtons.OK, MessageBoxType.Information);
                return;
            }
            IsBanning = true;
        }
        public void Stop()
        {
            IsBanning = false;
        }
        public void AddBan(Panel BanMatchTypeBox, TargetMode targetMode, TextBox textBox)
        {
            if (UIManager.CheckRejectOption(textBox, BanMatchTypeBox))
            {
                using (var checkingRadioButton = UIManager.GetCheckingRadioButton(BanMatchTypeBox))
                {
                    var matchMode = new MatchMode(new StringManager(checkingRadioButton.Name).Right(4));
                    UserBanJudger.Add(matchMode, targetMode, textBox.Text);

                }
            }
        }
    }
}
