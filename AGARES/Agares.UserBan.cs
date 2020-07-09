using System.Windows.Forms;
using Utils;
using static FSPAKE.AGARES.CoreSystem.UserJudgeSystem;

namespace FSPAKE.AGARES.CoreSystem
{
    partial class Agares
    {
        private void AddBanTarget(TextBox textBox, TargetMode type)
        {
            if (UIManager.CheckRejectOption(textBox, BanMatchTypeBox))
            {
                var checkingRadioButton = UIManager.GetCheckingRadioButton(BanMatchTypeBox);
                var matchMode = new MatchMode(new StringManager(checkingRadioButton.Name).Right(4));
                UserBanJudger.Add(matchMode, type, textBox.Text);
                UserBanJudger.LoadNGWord();
                UserBanJudger.LoadNGListener();
                textBox.Text = "";
                BanMatchComp.Checked = false;
                BanMatchPart.Checked = false;
            }
        }
    }
}
