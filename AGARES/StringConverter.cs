using System;
using System.Collections.Generic;

namespace Utils
{
    public class StringConverter
    {
        /// <summary>
        /// テキストファイルから変換前後の文字列の組を取り込み、
        /// 文字列変換を行うメソッドConvertを提供する
        /// </summary>

        readonly Dictionary<string, string> OldNewPairs = new Dictionary<string, string>();

        /// <summary>
        /// テキストファイルから変換前後の組を取り込む
        /// 変換前文字列はユニークであること
        /// </summary>
        /// <param name="filename">変換前後の組をカンマ区切りで示したテキストファイル</param>
        public StringConverter(string filename)
        {
            var alphabetTextfile = new TextfileManager(filename);
            var pairs = alphabetTextfile.ReadHeadToEnd();
            foreach (var pair in pairs)
            {
                OldNewPairs.Add(
                    pair.Split(new string[] { "," }, StringSplitOptions.None)[0],
                    pair.Split(new string[] { "," }, StringSplitOptions.None)[1]);
            }
        }

        /// <summary>
        /// コンストラクタに指定したファイルに基づいて
        /// 文字列の変換を行う。
        /// 該当がない場合はそのままの文字列を返却する
        /// </summary>
        /// <param name="text">変換対象文字列</param>
        /// <returns>変換後文字列</returns>
        public string Convert(string text)
        {
            if (text is null) { throw new ArgumentNullException(nameof(text)); }
            if (text == "") { throw new ArgumentException(nameof(text)); }

            var sChars = text.ToCharArray();
            foreach (char sChar in sChars)
            {
                try
                {
                    var oldChar = sChar.ToString();
                    var newChar = OldNewPairs[oldChar];
                    text = text.Replace(oldChar, newChar);
                }
                catch (Exception any) { continue; }
            }
            return text;
        }
    }
}
