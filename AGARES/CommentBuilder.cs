using System;

namespace FSPAKE.AGARES.Unit
{
    public class CommentBuilder
    {
        private string backColor;
        private string letterColor;
        private string listenerName;
        private string comment;
        private readonly string[] NameCommentDelimiter = { " ： " };
        private readonly string[] Cmddelimiter = { "\0" };
        public CommentBuilder(string text)
        {
            if (text is null) { throw new ArgumentNullException(nameof(text)); }
            if (string.IsNullOrEmpty(text))
            {
                ConcatResult = "";
                backColor = "";
                letterColor = "";
                listenerName = "";
                comment = "";
            }
            else
            {
                ConcatResult = text;
                try
                {
                    var splitWork = ConcatResult.Split(Cmddelimiter, StringSplitOptions.None);
                    DisplayArea = splitWork[splitWork.Length - 1];
                    LetterColor = splitWork[splitWork.Length - 2];
                    BackColor = splitWork[splitWork.Length - 3];
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        public CommentBuilder()
        {
            ConcatResult = "";
        }
        private void UpdateContents()
        {
            ConcatResult = backColor + Cmddelimiter[0] + letterColor + Cmddelimiter[0] + listenerName + NameCommentDelimiter[0] + comment;
        }
        public string DisplayArea
        {
            get
            {
                try { return ConcatResult.Split(Cmddelimiter, StringSplitOptions.None)[2]; }
                catch (Exception splitFailureReason)
                when (splitFailureReason is IndexOutOfRangeException)
                { return ""; }
            }
            set
            {
                if (value is null) { return; }
                try
                {
                    ListenerName = value.Split(NameCommentDelimiter, StringSplitOptions.None)[0];
                    Comment = value.Split(NameCommentDelimiter, StringSplitOptions.None)[1];
                }
                catch (Exception splitFailureReason)
                when (splitFailureReason is IndexOutOfRangeException)
                {/*ignored*/ }
                UpdateContents();
            }
        }
        public string ListenerName
        {
            get
            {
                try { return DisplayArea.Split(NameCommentDelimiter, StringSplitOptions.None)[0]; }
                catch (Exception splitFailureReason)
                when (splitFailureReason is IndexOutOfRangeException)
                { return ""; }

            }
            set
            {
                if (value is null) { return; }
                listenerName = value;
                UpdateContents();
            }
        }
        public string Comment
        {
            get
            {
                try { return DisplayArea.Split(NameCommentDelimiter, StringSplitOptions.None)[1]; }
                catch (Exception splitFailureReason)
                when (splitFailureReason is IndexOutOfRangeException)
                { return ""; }
            }
            set
            {
                if (value is null) { return; }
                comment = value;
                UpdateContents();
            }
        }

        public string BackColor
        {
            get
            {
                try { return ConcatResult.Split(Cmddelimiter, StringSplitOptions.None)[0]; }
                catch (Exception splitFailureReason)
                when (splitFailureReason is IndexOutOfRangeException)
                { return ""; }
            }
            set
            {
                if (value is null) { return; }
                backColor = value;
                UpdateContents();
            }
        }
        public string LetterColor
        {
            get
            {
                try { return ConcatResult.Split(Cmddelimiter, StringSplitOptions.None)[1]; }
                catch (Exception splitFailureReason)
                when (splitFailureReason is IndexOutOfRangeException)
                { return ""; }
            }
            set
            {
                if (value is null) { return; }
                letterColor = value;
                UpdateContents();
            }
        }
        public string ConcatResult { get; private set; }
    }
}
