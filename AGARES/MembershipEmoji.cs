using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Utils;

namespace FSPAKE.AGARES.Unit
{
    static class MembershipEmoji
    {
        /// <summary>
        /// メンバーシップ絵文字をローカルのディレクトリからロードする
        /// </summary>
        private static Dictionary<string, Image> MembershipEmojiDic = new Dictionary<string, Image>();
        private static Dictionary<string, Image> NameImgPairsY = new Dictionary<string, Image>();
        private static Dictionary<string, Image> NameImgPairsM = new Dictionary<string, Image>();
        public static void LoadMembership(string membershipImageFoldername,string channelId)
        {
            try
            {
                NameImgPairsM = GraphicManager.Load($@"{membershipImageFoldername}\{channelId}", GraphicManager.FileNameType.FileNameWithoutExtention);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(membershipImageFoldername);
            }
            MembershipEmojiDic = MergeDictionary(NameImgPairsY,NameImgPairsM);
        }

        public static void LoadYoutube(string youtubeImageFoldername)
        {
            try
            {
                NameImgPairsY = GraphicManager.Load(youtubeImageFoldername, GraphicManager.FileNameType.FileNameWithoutExtention);
            }
            catch (DirectoryNotFoundException)
            {
            }
            MembershipEmojiDic = MergeDictionary(NameImgPairsY, NameImgPairsM);
        }

        private static Dictionary<string, Image> MergeDictionary(Dictionary<string, Image> @base,Dictionary<string, Image> plus)
        {
            return @base.Concat(
                plus.Where(pair => !@base.ContainsKey(pair.Key)))
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value
                );
        }

        /// <summary>
        /// ":～～:"の形式を含むコメントを表示用に置換して絵文字の表示位置を返却する
        /// </summary>
        /// <param name="srcDisplayArea">":～～:"の形式を含むコメント</param>
        /// <param name="destDisplayArea">":～～:"の形式を含まない表示用のコメント</param>
        /// <param name="result">解析により求められた各絵文字の表示位置</param>
        public static void Analize(in string srcDisplayArea, out string destDisplayArea, out Dictionary<int, Image> result, string emojiFilename)
        {
            var emojiManager = new Emoji(emojiFilename);
            result = new Dictionary<int, Image>();

            var src = new StringManager(srcDisplayArea);

            var splitedItems = src.SplitReversible(":");
            if (splitedItems.Length == 1)
            {
                destDisplayArea = srcDisplayArea;
                return;
            }

            for (int i = 0; i < splitedItems.Length; i++)
            {
                if (string.IsNullOrEmpty(splitedItems[i]))
                {
                    splitedItems[i] = ":";
                }
            }
            for (int i = 0; i < splitedItems.Length; i++)
            {
                if (MembershipEmojiDic.ContainsKey(splitedItems[i]) &&
                    (i > 0 && splitedItems[i - 1] == ":") &&
                    (i < splitedItems.Length - 1 && splitedItems[i + 1] == ":"))
                {
                    Image img = MembershipEmojiDic[splitedItems[i]];
                    splitedItems[i - 1] = splitedItems[i - 1].Replace(":", "");
                    splitedItems[i] = "\0";
                    splitedItems[i + 1] = splitedItems[i + 1].Replace(":", "");

                    int putPlace = 0;
                    string[] partStrs = new string[i + 1];
                    Array.Copy(splitedItems, 0, partStrs, 0, i + 1);
                    foreach (var partStr in partStrs)
                    {
                        putPlace += emojiManager.GetWide(partStr);
                    }
                    try
                    {
                        result.Add(putPlace + result.Count, img);
                    }
                    catch (ArgumentException)
                    {
                        //重複登録はスルー
                    }
                }
            }
            StringBuilder sb = new StringBuilder("");
            foreach (var splitedItem in splitedItems)
            {
                sb.Append(splitedItem);
            }
            sb.Replace("\0", "　");
            destDisplayArea = sb.ToString();
        }

        public static void Scale(int side)
        {
            MembershipEmojiDic = GraphicManager.Scale(MembershipEmojiDic, side);
        }
    }
}
