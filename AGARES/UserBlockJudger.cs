using FSPAKE.AGARES.Unit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FSPAKE.AGARES.CoreSystem
{
    public class UserBlockJudger: UserJudgeSystem
    {

        public UserBlockJudger(string xmlFilename):base(xmlFilename)
        {
        }


        public bool IsNGWord(string wordText)
        {
            var mode = Words;
            return Judge(wordText,mode);
        }
        public bool IsNGListener(string listenerName)
        {
            var mode = Listeners;
            return Judge(listenerName, mode);
        }


        public List<int> BlockRun(string blockStr, ListBox updateListBox, MatchMode matchMode, TargetMode targetMode)
        {
            var processedIndex = new List<int>();
            if (blockStr is null) { throw new ArgumentNullException(nameof(blockStr)); }
            if (updateListBox is null) { throw new ArgumentNullException(nameof(updateListBox)); }
            for (int i = 0; i < updateListBox.Items.Count; i++)
            {
                var split = new CommentBuilder(updateListBox.Items[i].ToString());
                var name = split.ListenerName;
                var processTarget = "";
                var msg = "";
                if (targetMode.ToString() == Listeners.ToString())
                {
                    msg = "<--このユーザーはブロックされました-->";
                    processTarget = name;
                }
                else if (targetMode.ToString() == Words.ToString())
                {
                    msg = "<--このコメントはブロックされました-->";
                    processTarget = split.Comment;
                }

                if (IsBlockTarget(processTarget, blockStr, matchMode))
                {
                    OverWriteListBox(updateListBox, i, msg);
                    processedIndex.Add(i);
                }
            }
            return processedIndex;
        }
    }
}
