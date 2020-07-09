using System.Windows.Forms;
using Utils;

namespace FSPAKE.AGARES.Unit
{
    public class GoogleApikeyManager
    {
        private TextfileManager Apikeyfile;

        public GoogleApikeyManager(string filename)
        {
            Apikeyfile = new TextfileManager(filename, "APIKeyを入力");
        }
        public string GetNextKey()
        {
            var nextApiKey = Apikeyfile.ReadNext();
            if (string.IsNullOrEmpty(nextApiKey))
            {
                MessageBox.Show("準備されている全てのApiKeyが使用できません。");
                return "";
            }
            else
            {
                return nextApiKey;
            }
        }
        public void AppendKey(string apiKey)
        {
            Apikeyfile.AppendLine("\n"+apiKey);
        }
    }
}
