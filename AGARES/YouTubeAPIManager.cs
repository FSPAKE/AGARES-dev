using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace FSPAKE.AGARES.Unit
{
    public class YouTubeAPIManager
    {
        private YouTubeService YouTubeService;
        private string Apikey;
        private string ApplicationName;
        private UserCredential UserCredential;
        private string PageToken = null;

        /// <summary>
        /// インスタンス内部のYouTubeServiceインスタンス生成しないコンストラクタ
        /// </summary>
        public YouTubeAPIManager()
        {
            InitializeDirectory();
        }
        /// <summary>
        /// インスタンス内部のYouTubeServiceインスタンスを認証子なしで生成するコンストラクタ
        /// </summary>
        /// <param name="apikey">APIキー</param>
        public YouTubeAPIManager(string apikey)
        {
            UpdateApikey(apikey);
            InitializeDirectory();
        }



        /// <summary>
        /// インスタンス内部のYouTubeServiceインスタンスを認証子ありで生成するコンストラクタ
        /// </summary>
        /// <param name="apikey">APIキー</param>
        /// <param name="applicationName">アプリケーション名</param>
        /// <param name="userCredential">認証子</param>
        public YouTubeAPIManager(string apikey, string applicationName, UserCredential userCredential)
        {
            UpdateApikey(apikey, applicationName, userCredential);
            InitializeDirectory();
        }


        private static void InitializeDirectory()
        {
            Directory.CreateDirectory(@"MemberShipImages");
        }


        /// <summary>
        /// インスタンス内部のYouTubeServiceインスタンスを認証子なしとして再生成する
        /// </summary>
        /// <param name="newApikey">新しいAPIキー</param>
        public void UpdateApikey(string newApikey)
        {
            Apikey = newApikey;
            YouTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = newApikey
            });
        }
        /// <summary>
        /// インスタンス内部のYouTubeServiceインスタンスを認証子ありとして再生成する
        /// </summary>
        /// <param name="newApikey">新しいAPIキー</param>
        /// <param name="newApplicationName">新しいアプリケーション名</param>
        public void UpdateApikey(string newApikey, string newApplicationName)
        {
            Apikey = newApikey;
            ApplicationName = newApplicationName;
            YouTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = newApikey,
                ApplicationName = newApplicationName
            });
        }
        /// <summary>
        /// インスタンス内部のYouTubeServiceインスタンスを認証子ありとして再生成する
        /// </summary>
        /// <param name="newApikey">新しいAPIキー</param>
        /// <param name="newApplicationName">新しいアプリケーション名</param>
        /// <param name="newUserCredential">新しい認証子</param>
        public void UpdateApikey(string newApikey, string newApplicationName, UserCredential newUserCredential)
        {
            Apikey = newApikey;
            ApplicationName = newApplicationName;
            UserCredential = newUserCredential;
            YouTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = newApikey,
                ApplicationName = newApplicationName,
                HttpClientInitializer = newUserCredential
            });
        }
        /// <summary>
        /// インスタンス内部のYouTubeServiceインスタンスの認証子を更新する
        /// </summary>
        /// <param name="newUserCredential"></param>
        public void UpdateCredentials(UserCredential newUserCredential)
        {
            UserCredential = newUserCredential;
            YouTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Apikey,
                ApplicationName = ApplicationName,
                HttpClientInitializer = newUserCredential
            });
        }
        /// <summary>
        /// YouTubeServiceインスタンスにAPIキーを追加する
        /// </summary>
        private void PlugApikey()
        {
            YouTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Apikey,
                ApplicationName = ApplicationName,
                HttpClientInitializer = UserCredential
            });
        }
        /// <summary>
        /// YouTubeServiceインスタンスからAPIキーを抜く
        /// </summary>
        private void UnplugApikey()
        {
            YouTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = ApplicationName,
                HttpClientInitializer = UserCredential
            });
        }



        /// <summary>
        /// 動画／配信情報を取得する
        /// </summary>
        /// <param name="VideoId">取得対象の動画／配信ID</param>
        /// <returns></returns>
        public async Task<Video> GetVideoInfoAsync(string VideoId)
        {
            YouTubeService temp = YouTubeService;
            UpdateApikey(YouTubeService.ApiKey);
            var videosList = YouTubeService.Videos.List("LiveStreamingDetails,Snippet");
            videosList.Id = VideoId;
            var videoListResponse = await videosList.ExecuteAsync().ConfigureAwait(true);
            foreach (var videoResponce in videoListResponse.Items)
            {
                return videoResponce;
            }
            YouTubeService = temp;
            return null;
        }




        /// <summary>
        /// YouTubeLiveのコメントを取得する
        /// </summary>
        /// <param name="LiveChatId">コメント欄のID</param>
        /// <returns></returns>
        public async Task<LiveChatMessageListResponse> GetLiveChatMessage(string LiveChatId)
        {
            var liveChatRequest = YouTubeService.LiveChatMessages.List(LiveChatId, "snippet,authorDetails");
            liveChatRequest.PageToken = PageToken;
            var liveChatResponse = await liveChatRequest.ExecuteAsync().ConfigureAwait(true);
            PageToken = liveChatResponse.NextPageToken;
            return liveChatResponse;
        }



        public static class BanType
        {
            public const string temporary = "temporary";
            public const string permanent = "permanent";
        }
        public async Task UserBANAsync(string liveId, string type, ulong timeoutTime, string banUserChannelId)
        {
            UpdateApikey(null, Assembly.GetExecutingAssembly().GetName().Name, UserCredential);
            if (UserCredential is null)
            {
                return ;
            }
            var liveChatBan = new LiveChatBan
            {
                Snippet = new LiveChatBanSnippet
                {
                    LiveChatId = liveId,

                    Type = type,

                    BannedUserDetails = new ChannelProfileDetails
                    {
                        ChannelId = banUserChannelId
                    }
                }
            };
            if (liveChatBan.Snippet.Type == BanType.temporary)
            {
                liveChatBan.Snippet.BanDurationSeconds = timeoutTime;
            }
            try
            {
                var responce = YouTubeService.LiveChatBans.Insert(liveChatBan, "snippet");
                await responce.ExecuteAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            UpdateApikey(Apikey);
        }
        public async Task<ChannelListResponse> ChannelListAsync(string channelId, List<string> partList)
        {
            var s = "";
            foreach (var p in partList)
            {
                s += p + ",";
            }
            var part = new StringManager(s).RightDelete(1);
            return await ChannelList(channelId, part).ConfigureAwait(true);
        }


        public Task<ChannelListResponse> ChannelList(string channelId, string part)
        {
            var request = YouTubeService.Channels.List(part);
            request.Id = channelId;
            return request.ExecuteAsync();
        }


        /*実装なし
        /// <summary>
        /// 動画投稿サンプル
        /// </summary>
        /// <param name="youTubeService"></param>
        /// <returns></returns>
        public async Task VideoUploadAsync()
        {
            UnplugApikey();
            var video = new Video
            {
                Snippet = new VideoSnippet()
            };
            video.Snippet.Title = "Default Video Title";
            video.Snippet.Description = "Default Video Description";
            video.Snippet.Tags = new string[] { "tag1", "tag2" };
            video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus
            {
                PrivacyStatus = "unlisted" // or "private" or "public"
            };
            var filePath = @"REPLACE_ME.mp4"; // Replace with path to actual movie file.

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = YouTubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                await videosInsertRequest.UploadAsync().ConfigureAwait(false);
            }
            PlugApikey();
        }
        */
        /*実装なし
        public async Task InsertCommentAsync(string liveChatId, string message)
        {
            UnplugApikey();
            if (liveChatId is null) { return; }
            Console.WriteLine(UserCredential.UserId);
            var liveChatMessage = new LiveChatMessage
            {
                Snippet = new LiveChatMessageSnippet()
                {
                    LiveChatId = liveChatId,
                    Type = "textMessageEvent",

                    TextMessageDetails = new LiveChatTextMessageDetails()
                    {
                        MessageText = message
                    }
                }
            };
            var responce = YouTubeService.LiveChatMessages.Insert(liveChatMessage, "snippet");
            await responce.ExecuteAsync().ConfigureAwait(false);
            PlugApikey();
        }
        */
    }
}
