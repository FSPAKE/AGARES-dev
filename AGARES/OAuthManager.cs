using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Utils
{
    public class OAuthManager
    {
        UserCredential UserCredential = null;
        public OAuthManager() { }

        public async Task<UserCredential> GetCredential(string filename)
        {
            var success = false;
            var faildCnt = 0;
            while (!success && faildCnt < 10)
            {
                try
                {
                    var filepass = "../../" + filename;
                    using (var stream = new FileStream(filepass, FileMode.Open, FileAccess.Read))
                    {
                        UserCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            // This OAuth 2.0 access scope allows an application to upload files to the
                            // authenticated user's YouTube channel, but doesn't allow other types of access.
                            new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload, YouTubeService.Scope.YoutubeForceSsl },
                            "user",
                            CancellationToken.None
                        ).ConfigureAwait(true);
                    }
                    success = true;

                }
                catch (TokenResponseException e)
                {
                    Console.WriteLine(e);
                    faildCnt++;
                }
            }
            if (!success)
            {
                UIManager.MessageBoxShow("認証情報の取得に失敗しました", "OAuth", MessageBoxButtons.OK, MessageBoxType.Error);
            }
            return UserCredential;
        }
    }
}
