using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSPAKE.AGARES.CoreSystem
{
    partial class Agares
    {
        private async Task<UserCredential> NewCredentialAsync(string filename)
        {
            return await OAuthManager.GetCredential(filename).ConfigureAwait(true);
        }
    }
}
