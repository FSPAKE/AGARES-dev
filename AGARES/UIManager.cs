using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utils
{
    public class UIManager
    {
        /// <summary>
        /// Panelの配下にあるラジオボタンのうち選択されているものを取得
        /// </summary>
        /// <param name="radioBox">ラジオボタンが所属するパネル</param>
        /// <returns>選択されているラジオボタンのインスタンス</returns>
        public static RadioButton GetCheckingRadioButton(Panel radioBox)
        {
            if (radioBox is null) { return null; }
            foreach (var control in radioBox.Controls)
            {
                if (!(control is RadioButton)) { continue; }

                var rdo = (RadioButton)control;
                if (rdo.Checked == true)
                {
                    return rdo;
                }
            }
            return null;
        }
        public static bool CheckRejectOption(TextBox textBox, Panel radioBox)
        {
            if (textBox is null) { throw new ArgumentNullException(); }
            if (radioBox is null) { throw new ArgumentNullException(); }
            if (string.IsNullOrEmpty(textBox.Text))
            {
                MessageBoxShow("テキストボックスにブロックするテキストを入力してください",
                    "AGARES", MessageBoxButtons.OK, MessageBoxType.Information);
                textBox.Focus();
                return false;
            }
            return CheckRadioButton(radioBox);
        }
        public static bool CheckRadioButton(Panel radioBox)
        {
            var checkingRadioButton = UIManager.GetCheckingRadioButton(radioBox);
            if (checkingRadioButton is null)
            {
                MessageBoxShow("完全一致か部分一致かをラジオボタンで指定してください",
                    "AGARES", MessageBoxButtons.OK, MessageBoxType.Information);
                return false;
            }
            return true;
        }
        List<Control> ExtensionedControlList = new List<Control>();
        int StackNum = 0;
        public List<Control> ExtensionControlCollection(Control.ControlCollection controlCollection)
        {
            if (controlCollection is null) { return new List<Control>(); }
            if (StackNum == 0) { ExtensionedControlList.Clear(); }
            for (int i = 0; i < controlCollection.Count; i++)
            {
                var c = controlCollection[i];
                if (c is Panel)
                {
                    StackNum++;
                    ExtensionedControlList.AddRange(ExtensionControlCollection(((Panel)c).Controls));
                    StackNum--;
                }
                else
                {
                    ExtensionedControlList.Add(c);
                }
            }
            ExtensionedControlList = ExtensionedControlList.Distinct().ToList();
            return ExtensionedControlList;
        }

        private static readonly string[] AlertString = { "メッセージ", "エラーメッセージ", "メッセージ", "ワーニングメッセージ", "インフォメーション" };
        private static readonly MessageBoxIcon[] AlertOption = { MessageBoxIcon.None, MessageBoxIcon.Error, MessageBoxIcon.Question, MessageBoxIcon.Warning, MessageBoxIcon.Information };
        public static DialogResult MessageBoxShow(string text, string applicationName,
            MessageBoxButtons messageBoxButtons, MessageBoxType alertType)
        {
            if (applicationName != "")
            {
                return MessageBox.Show(text,
                    $"{applicationName}からの{AlertString[(int)alertType]}",
                    messageBoxButtons, AlertOption[(int)alertType]);

            }
            else
            {
                return MessageBox.Show(text, "",
                    messageBoxButtons, AlertOption[(int)alertType]);
            }

        }

        /// <summary>
        /// ラベルコントロールに格納できる文字幅と文字数を返却する
        /// </summary>
        /// <param name="target">対象とするラベル</param>
        /// <param name="approxWidth">求めたい幅よりは大きい大まかな幅</param>
        /// <param name="zenkaku">全角：true　半角：false</param>
        /// <returns>幅と文字数の組</returns>
        public static Tuple<int, int> GetLabelWidthPerLine(Label target ,bool zenkaku)
        {
            var maxWidth = target.Width;
            var textBackup = target.Text;
            target.Text = "";
            var autosizeBackup = target.AutoSize;
            target.AutoSize = true;
            int width = 0;
            int ctr = 0;
            while (target.Width <= maxWidth)
            {
                width = target.Width;
                if (zenkaku)
                {
                    target.Text += "　";
                }
                else
                {
                    target.Text += " ";
                }
                ctr++;
            }
            target.AutoSize = autosizeBackup;
            target.Text = textBackup;

            return new Tuple<int,int>(width,ctr);
        }
    }
}
