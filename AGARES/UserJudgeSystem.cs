using FSPAKE.AGARES.Unit;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using Utils;

namespace FSPAKE.AGARES.CoreSystem
{

    public abstract class UserJudgeSystem
    {
        private List<string> NGCompListenerList = new List<string>();
        private List<string> NGPartListenerList = new List<string>();
        private List<string> NGCompWordList = new List<string>();
        private List<string> NGPartWordList = new List<string>();
        private readonly string XMLFilename;
        protected UserJudgeSystem(string xmlFilename)
        {
            if (xmlFilename is null) { throw new ArgumentNullException(nameof(xmlFilename)); }
            if (xmlFilename == "") { throw new ArgumentException(nameof(xmlFilename)); }
            if (new TextfileManager(xmlFilename).IsNotExist()) { throw new FileNotFoundException(xmlFilename, xmlFilename); }
            XMLFilename = xmlFilename;
            LoadNGListener();
            LoadNGWord();
        }
        private enum matchMode
        {
            Comp,
            Part
        }

        private enum targetMode
        {
            Listeners,
            Words
        }
        public class MatchMode
        {
            public MatchMode(string str) { value = str; }
            private readonly string value;
            public static bool operator ==(MatchMode a, MatchMode b)
            {
                return ReferenceEquals(a.ToString(), b.ToString());
            }
            public static bool operator !=(MatchMode a, MatchMode b)
            {
                return !(a == b);
            }
            public override string ToString()
            {
                return value;
            }
        }
        public class TargetMode
        {
            public TargetMode(string str) { value = str; }
            private readonly string value;
            public static bool operator ==(TargetMode a, TargetMode b)
            {
                return ReferenceEquals(a.ToString(), b.ToString());
            }
            public static bool operator !=(TargetMode a, TargetMode b)
            {
                return !(a == b);
            }
            public override string ToString()
            {
                return value;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public static MatchMode Comp => new MatchMode(Enum.GetName(typeof(matchMode), matchMode.Comp));
        public static MatchMode Part => new MatchMode(Enum.GetName(typeof(matchMode), matchMode.Part));
        public static TargetMode Listeners => new TargetMode(Enum.GetName(typeof(targetMode), targetMode.Listeners));
        public static TargetMode Words => new TargetMode(Enum.GetName(typeof(targetMode), targetMode.Words));
        public void Add(MatchMode matchMode, TargetMode targetMode, string value)
        {
            if (targetMode == Listeners)
            {
                AddNGListener(matchMode, value);
            }
            if (targetMode == Words)
            {
                AddNGWord(matchMode, value);
            }
        }
        private void AddNGListener(MatchMode matchMode, string listener)
        {
            if (matchMode is null) { throw new ArgumentNullException(nameof(matchMode)); }
            if (listener is null) { throw new ArgumentNullException(nameof(listener)); }
            if (listener == "") { throw new ArgumentException(nameof(listener)); }
            using (var xmlManager = new XMLManager(XMLFilename))
            {
                xmlManager.Add(XMLManager.NewElement(matchMode + Listeners.ToString(), listener));
                xmlManager.Save();
            }
            LoadNGListener();
        }
        private void AddNGWord(MatchMode matchMode, string word)
        {
            if (matchMode is null) { throw new ArgumentNullException(nameof(matchMode)); }
            if (word is null) { throw new ArgumentNullException(nameof(word)); }
            if (word == "") { throw new ArgumentException(nameof(word)); }
            using (var xmlManager = new XMLManager(XMLFilename))
            {
                xmlManager.Add(XMLManager.NewElement(matchMode + Words.ToString(), word));
                xmlManager.Save();
            }
            LoadNGWord();
        }
        protected List<string> LoadToList(string tag)
        {
            return new XMLManager(XMLFilename).Deserialize((XName)tag);
        }

        public void LoadNGListener()
        {
            var tag = Comp.ToString() + Listeners.ToString();
            NGCompListenerList = LoadToList(tag);
            tag = Part.ToString() + Listeners.ToString();
            NGPartListenerList = LoadToList(tag);
        }

        public void LoadNGWord()
        {
            var tag = Comp.ToString() + Words.ToString();
            NGCompWordList = LoadToList(tag);
            tag = Part.ToString() + Words.ToString();
            NGPartWordList = LoadToList(tag);
        }
        public bool JudgeComp(string text, TargetMode targetMode)
        {
            if (text is null) { throw new ArgumentNullException(nameof(text)); }
            if (text == "") { throw new ArgumentException(nameof(text)); }
            List<string> compList = null;
            if (targetMode == Listeners)
            {
                compList = NGCompListenerList;
            }
            if (targetMode == Words)
            {
                compList = NGCompWordList;
            }
            if (compList.Contains(text))
            {
                return true;
            }
            return false;
        }
        public bool JudgePart(string text, TargetMode targetMode)
        {
            if (text is null) { throw new ArgumentNullException(nameof(text)); }
            if (text == "") { throw new ArgumentException(nameof(text)); }
            List<string> partList = null;
            if (targetMode == Listeners)
            {
                partList = NGPartListenerList;
            }
            if (targetMode == Words)
            {
                partList = NGPartWordList;
            }
            foreach (var part in partList)
            {
                if (text.Contains(part))
                {
                    return true;
                }
            }
            return false;
        }

        protected bool Judge(string text, TargetMode targetMode)
        {
            return JudgeComp(text, targetMode) || JudgePart(text, targetMode);
        }

        protected static bool IsBlockTarget(string text, string search, MatchMode matchMode)
        {
            if (text is null) { throw new ArgumentNullException(nameof(text)); }
            if (search is null) { throw new ArgumentNullException(nameof(search)); }
            if (matchMode is null) { throw new ArgumentNullException(nameof(matchMode)); }
            return (matchMode.ToString() == Comp.ToString() && text == search) ||
                   (matchMode.ToString() == Part.ToString() && text.Contains(search));
        }
        protected static void OverWriteListBox(ListBox updateListBox, int itemIndex, string afterStr)
        {
            var item = updateListBox.Items[itemIndex].ToString();
            var listenerName = new CommentBuilder(item).ListenerName;
            var commentBuilder = new CommentBuilder
            {
                BackColor = new ColorConverter().ConvertToString(Color.Red),
                LetterColor = new ColorConverter().ConvertToString(Color.Black),
                ListenerName = listenerName,
                Comment = afterStr
            };

            updateListBox.Items[itemIndex] = commentBuilder.ConcatResult;
        }
    }
}
