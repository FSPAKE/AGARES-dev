using FSPAKE.AGARES.Unit;
using System.Windows.Forms;
using Utils;
using static FSPAKE.AGARES.CoreSystem.UserJudgeSystem;

namespace FSPAKE.AGARES.CoreSystem
{
    partial class Agares
    {

        private void BlockBaseCondition(TextBox textBox, TargetMode type)
        {
            if(UIManager.CheckRejectOption(textBox, BlockMatchTypeBox))
            {
                var checkingRadioButton = UIManager.GetCheckingRadioButton(BlockMatchTypeBox);
                var matchMode = new MatchMode(new StringManager(checkingRadioButton.Name).Right(4));
                AddBlockTarget(textBox.Text, matchMode, type);
                textBox.Text = "";
                BlockMatchComp.Checked = false;
                BlockMatchPart.Checked = false;
            }
        }
        private void BlockShootIndividual(string listenerName)
        {
            AddBlockTarget(listenerName, Comp, Listeners);
        }
        private void AddBlockTarget(string target, MatchMode matchMode, TargetMode targetMode)
        {
            UserBlockJudger.Add(matchMode,targetMode,target);
            UserBlockJudger.LoadNGWord();
            UserBlockJudger.LoadNGListener();
            var blockedIndexes = UserBlockJudger.BlockRun(target, Comments, matchMode,targetMode);
            foreach (var blockedIndex in blockedIndexes)
            {
                var commentBuilder = new CommentBuilder(Comments.Items[blockedIndex].ToString());
                var displayArea = commentBuilder.DisplayArea;
                MembershipEmoji.Analize(in displayArea, out displayArea, out analizedResult, EmojiFilename);
                ColumnsOfImgForLine[blockedIndex + 1] = analizedResult;
                AnalizedDisplayAreaHouse[blockedIndex + 1] = displayArea;
                Comments.Items[blockedIndex] = commentBuilder.ConcatResult;
            }
        }
    }
}
