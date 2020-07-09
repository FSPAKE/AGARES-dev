using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Utils;
namespace FSPAKE.AGARES.CoreSystem
{
    public partial class BaelConfigMenu : Form
    {
        private readonly string BoomWordFilename;
        private readonly string ConfigFilename;
        private readonly string SexualAndViolenceFilename;
        private Dictionary<XName, string> LoadedElementsDictionary;
        private readonly UIManager UIManager = new UIManager();

        public BaelConfigMenu(string boomWordFilename,string configFilename,string sexualAndViolenceFilename)
        {
            BoomWordFilename = boomWordFilename;
            ConfigFilename = configFilename;
            SexualAndViolenceFilename = sexualAndViolenceFilename;
            InitializeComponent();
        }

        private void ConfigMenu_Load(object sender, EventArgs e)
        {
            MaximumSize = this.Size;
            MinimumSize = this.Size;
            this.Location = new Point(
                this.Owner.Location.X + (this.Owner.Width - this.Width) / 2,
                this.Owner.Location.Y + (this.Owner.Height - this.Height) / 2);
            ConfigXmlLoad();
            ModifyComponent();
        }

        private void ModifyComponent()
        {
            var items = UIManager.ExtensionControlCollection(this.Controls);
            for (int i = 0; i < items.Count; i++)
            {
                var control = items[i];
                if (!LoadedElementsDictionary.ContainsKey(control.Name))
                {
                    continue;
                }
                if (control is CheckBox)
                {
                    ((CheckBox)control).Checked = Parse.ToBool(LoadedElementsDictionary[control.Name]);
                }
                if (control is TextBox)
                {
                    control.Text = LoadedElementsDictionary[control.Name];
                }
                if (control is RadioButton)
                {
                    ((RadioButton)control).Checked = Parse.ToBool(LoadedElementsDictionary[control.Name]);
                }
            }
        }




        private void ConfigXmlSave()
        {
            using (var xmlManager = new XMLManager(ConfigFilename))
            {
                xmlManager.Clear();

                foreach (Control c in UIManager.ExtensionControlCollection(this.Controls))
                {
                    string tag;
                    string element;
                    if (c is RadioButton)
                    {
                        tag = c.Name;
                        element = ((RadioButton)c).Checked.ToString();
                    }
                    else if (c is CheckBox)
                    {
                        tag = c.Name;
                        element = ((CheckBox)c).Checked.ToString();
                    }
                    else if (c is TextBox)
                    {
                        tag = c.Name;
                        element = c.Text;
                    }
                    else { continue; }
                    xmlManager.Add(XMLManager.NewElement(tag, element));
                }
                xmlManager.Save();
            }
        }

        private void ConfigXmlLoad()
        {
            if (File.Exists(ConfigFilename))
            {
                using (var xmlManager = new XMLManager(ConfigFilename))
                {
                    LoadedElementsDictionary = xmlManager.DeserializeToDictionary();
                }
            }
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            var controls = UIManager.ExtensionControlCollection(this.Controls);
            var controlDictionary = new Dictionary<string, Control>();
            foreach (var control in controls)
            {
                controlDictionary.Add(control.Name, control);
            }

            foreach (var control in controls)
            {
                if (control.Name.Contains("Point") &&
                    (!int.TryParse(control.Text, out int dummy) ||
                     string.IsNullOrEmpty(control.Text)))
                {
                    UIManager.MessageBoxShow("違反点数は数値で入力してください。", "BAEL", MessageBoxButtons.OK, MessageBoxType.Error);
                    return;
                }
                if (control is CheckBox &&
                    control.Name.Contains("Option")&&
                    ((CheckBox)control).Checked)
                {
                    var row = control.Name.Replace("Option", "");
                    var isRadioFind = false;
                    var isCheck = false;
                    for (int i = 1; ; i++)
                    {
                        var name = "DecisionCriteria" + row + i;
                        if (!controlDictionary.ContainsKey(name)) { break; }
                        var decisionCriteria = controlDictionary[name];
                        if (decisionCriteria is TextBox &&
                            string.IsNullOrEmpty(decisionCriteria.Text))
                        {
                            UIManager.MessageBoxShow("条件指定項目の入力に不備があります。", "BAEL", MessageBoxButtons.OK, MessageBoxType.Error);
                            return;
                        }
                        if (decisionCriteria is RadioButton)
                        {
                            isRadioFind = true;
                            isCheck = isCheck || ((RadioButton)decisionCriteria).Checked;
                        }
                    }
                    if (isRadioFind && !isCheck)
                    {
                        UIManager.MessageBoxShow("条件指定項目の入力に不備があります。", "BAEL", MessageBoxButtons.OK, MessageBoxType.Error);
                        return;
                    }
                }
            }
            ConfigXmlSave();
            UIManager.MessageBoxShow("設定内容を保存しました。", "BAEL", MessageBoxButtons.OK, MessageBoxType.Information);
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void QuietOnActivate_Click(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                UIManager.MessageBoxShow("以降、BAELへのモード切替時に設定画面を表示しません。\n" +
                    "設定を戻したい場合は、設定画面を再度開きチェックを外します。\n" +
                    "設定画面はツールバー「設定(S)→自動BANルール設定」からいつでも表示できます。",
                    "BAELからのインフォメーション", MessageBoxButtons.OK, MessageBoxType.Information);
            }
        }

        private void Option5_Click(object sender, EventArgs e)
        {
            if(((CheckBox)sender).Checked)
            {
                var result = UIManager.MessageBoxShow("本オプションをオンにすると、YouTubeからの情報受信を待機するため\n" +
                    "コメントの表示速度が低下します。\n" +
                    "また日単位の実行可能リクエスト量を消費するため、稼働可能時間が短くなります。\n" +
                    "稼働時間の延長には、新しいGoogleAPIキーを取得しAPIkeys.txtに追記して下さい。",
                    "BAELからのワーニング", MessageBoxButtons.OKCancel, MessageBoxType.Warning);
                if (result != DialogResult.OK)
                {
                    ((CheckBox)sender).Checked = false;
                }
            }
        }

        private void DecisionCriteria14_Click(object sender, EventArgs e)
        {
            using (var boomWordsView = new XMLElementBox(BoomWordFilename,"Item"))
            {
                boomWordsView.Owner = this;
                boomWordsView.ShowDialog();
                boomWordsView.BringToFront();
            }
        }

        private void DecisionCriteria51_Click(object sender, EventArgs e)
        {
            using (var sexualAndViolenceView = new XMLElementBox(SexualAndViolenceFilename, "Item"))
            {
                sexualAndViolenceView.Owner = this;
                sexualAndViolenceView.ShowDialog();
                sexualAndViolenceView.BringToFront();
            }
        }
    }
}
