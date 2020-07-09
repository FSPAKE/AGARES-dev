using System;
using System.Collections.Generic;

namespace Utils
{
    public class StringManager
    {
        private string Contents;
        public new string ToString() => Contents;

        public StringManager(string s)
        {
            if (s is null) throw new ArgumentNullException(s);
            Contents = s;
        }
        public string[] SplitReversible(string delimiter)
        {
            if (string.IsNullOrEmpty(delimiter)) { return new string[1] { Contents }; }
            var destList = new List<string>();
            var find = Contents.IndexOf(delimiter);
            while (find != -1 && Contents != delimiter)
            {
                var item = Contents.Substring(0, find);
                if (!string.IsNullOrEmpty(item))
                {
                    destList.Add(item);
                }
                Contents = Contents.Substring(find + 1);
                if (!string.IsNullOrEmpty(Contents)) { destList.Add(""); }
                find = Contents.IndexOf(delimiter);
            } (new string[1])[0] = delimiter;
            destList.Add(Contents.Split(new string[1], StringSplitOptions.None)[0]);
            return destList.ToArray();
        }
        public string[] SplitOnFirst(string delimiter)
        {
            var border = Contents.IndexOf(delimiter);
            return new string[] { Left(border), LeftDelete(border + delimiter.Length) };
        }
        public string ReplaceFirst(string oldString, string newString)
        {
            if (oldString is null) { return oldString; }
            var findIndex = Contents.IndexOf(oldString);
            if (findIndex != -1)
            {
                Contents = $"{Contents.Substring(0, findIndex)}{newString}{Contents.Substring(findIndex + oldString.Length, Contents.Length - (findIndex + oldString.Length))}";
            }
            return Contents;
        }


        public int Length => Contents.Length;
        public bool IsNullOrEmpty()
        {
            return string.IsNullOrEmpty(Contents);
        }
        public bool IsNullOrWhiteSpace()
        {
            return string.IsNullOrWhiteSpace(Contents);
        }
        public string Left(int length)
        {
            try { return Contents.Substring(0, length); }
            catch (ArgumentOutOfRangeException e) { throw new IndexOutOfRangeException("", e); }
        }
        public string Right(int length)
        {
            try { return Contents.Substring(Contents.Length - length, length); }
            catch (ArgumentOutOfRangeException e) { throw new IndexOutOfRangeException("", e); }
        }
        public string LeftDelete(int deleteLength)
        {
            try { return Right(Contents.Length - deleteLength); }
            catch (ArgumentOutOfRangeException e) { throw new IndexOutOfRangeException("", e); }
        }
        public string RightDelete(int deleteLength)
        {
            try { return Left(Contents.Length - deleteLength); }
            catch (ArgumentOutOfRangeException e) { throw new IndexOutOfRangeException("", e); }
        }
        public string Middle(int startIndex, int endIndex)
        {
            if (startIndex > endIndex) { throw new ArgumentException($"{nameof(startIndex)}は{nameof(endIndex)}より小さくなければなりません"); }
            if (startIndex == 0) { throw new IndexOutOfRangeException("指定範囲は１～文字列長まででなければなりません。"); }
            try { return Contents.Substring(startIndex - 1, endIndex - startIndex + 1); }
            catch (ArgumentOutOfRangeException e) { throw new IndexOutOfRangeException("", e); }
        }
    }
}
