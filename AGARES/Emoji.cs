using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
namespace Utils
{
    public class Emoji
    {
        private readonly Dictionary<int, HashSet<string>> EmojiShelf = new Dictionary<int, HashSet<string>>();
        public Emoji(string filename)
        {
            using (var sr = new StreamReader(filename, Encoding.Unicode))
            {
                for (int i = 1; i <= 14; i++)
                {
                    EmojiShelf[i] = new HashSet<string>();
                }
                while (!sr.EndOfStream)
                {
                    var s = sr.ReadLine();
                    EmojiShelf[s.Length].Add(s);
                }
            }
        }
        public int Length(string text)
        {
            int cnt = 0;
            while (text.Length > 0)
            {
                var next = this.DeleteTail(text, 1);
                if (text != next)
                {
                    text = next;
                }
                else
                {
                    text = new StringManager(text).RightDelete(1);
                }
                cnt++;
            }
            return cnt;
        }

        public bool Contains(string item)
        {
            if(item is null) { throw new ArgumentNullException(nameof(item)); }
            if (string.IsNullOrEmpty(item)) { return false; }
            return EmojiShelf[item.Length].Contains(item);
        }
        public bool IsTailEmoji(string text)
        {
            if (text is null) { throw new ArgumentNullException(nameof(text)); }
            for (int tailLength = 1; tailLength <= 14; tailLength++)
            {
                if (text.Length < tailLength) { return false; }
                var emoji = new StringManager(text).Right(tailLength);
                if (this.Contains(emoji))
                {
                    return true;
                }
            }
            return false;
        }
        public string DeleteTail(string text,int num)
        {
            if (text is null) { throw new ArgumentNullException(nameof(text)); }
            if (text == "") { throw new ArgumentException(nameof(text)); }
            var dest = text;
            for (var i = 0; i < num; i++)
            {
                var src = new StringManager(dest);
                for (int j = 0; j < text.Length; j++)
                {
                    var c = src.Right(j);
                    if (this.Contains(c))
                    {
                        dest = src.RightDelete(j);
                        break;
                    }
                }
            }
            return dest;
        }
        private readonly Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

        public int GetWide(string s)
        {
            var total = 0;
            var src = new StringManager(s);
            while (src.Length > 0)
            {
                var c = src.Left(1);
                if (this.Contains(c))
                {
                    //１バイト絵文字が見つかったら加算は１
                    total += 1;
                    src.ReplaceFirst(c, "");
                    continue;
                }

                int i;
                for (i = 14; i >= 2; i--)
                {
                    if (i > src.Length)
                    {
                        i = src.Length;
                    }
                    if (EmojiShelf[i].Count == 0)
                    {
                        continue;
                    }
                    try
                    {
                        var ci = src.Left(i);
                        if (this.Contains(ci))
                        {
                            //１４文字まで調べて見つかったら絵文字、加算は２
                            total += 2;
                            src.ReplaceFirst(ci, "");
                            i = 0;//break&find
                        }
                    }
                    catch (ArgumentOutOfRangeException) { }
                }

                if (i != -1)
                {
                    //絵文字以外
                    c = src.Left(1);
                    total += sjisEnc.GetByteCount(c);
                    src = new StringManager(src.Right(src.Length - 1));
                }
            }
            return total;
        }


        /// <summary>
        /// コメント中に含まれる絵文字の数を返却します
        /// </summary>
        /// <param name="text">調査対象の文字列</param>
        /// <param name="tailContinuation">
        /// 末尾の連続する絵文字だけカウントしたい場合：true
        /// コメント中のすべての絵文字をカウントしたい場合：false</param>
        /// <param name="unti">
        /// 絵文字「💩」はカウントしない場合：true
        /// 絵文字「💩」もカウントする場合：false
        /// </param>
        /// <returns>絵文字の個数</returns>
        public int CountTailEmoji(string text, bool tailContinuation, bool unti)
        {
            var cnt = 0;
            var emoji = "";
            var src = new StringManager(text);
            while (!src.IsNullOrEmpty())
            {
                var maxindex = 14 < src.Length ? 14 : src.Length;
                var isEmojiHit = false;
                for (var index = 1; index <= maxindex; index++)
                {
                    emoji = src.Right(index);
                    if (this.Contains(emoji))
                    {
                        isEmojiHit = true;
                        cnt++;
                        src = new StringManager(src.RightDelete(index));
                        break;
                    }
                }
                if (unti && emoji == "💩") { cnt--; }
                if (tailContinuation && !isEmojiHit) { break; }
                if (!isEmojiHit) src = new StringManager(src.RightDelete(1));
            }
            return cnt;
        }
    }
}
