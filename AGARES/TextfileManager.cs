using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Utils
{
    public class TextfileManager
    {
        private readonly string Filename;
        public readonly int LineCount;
        private int SeekCount = 0;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filename">読み込むテキストファイル</param>
        /// <param name="messageOnFileCreate">対象のファイルが存在しないとき、ファイルを生成し初期値として書き込む文字列</param>
        public TextfileManager(string filename, string messageOnFileCreate)
        {
            this.Filename = filename;
            if (this.IsNotExist())
            {
                using (var sw = new StreamWriter(File.Create(this.Filename)))
                {
                    sw.WriteLine(Microsoft.VisualBasic.Interaction.InputBox(messageOnFileCreate, "", "", -1, -1));
                }
            }
            LineCount = ReadHeadToEnd().Count;
            SeekCount = 0;
        }
        public TextfileManager(string filename)
        {
            if (filename is null) { throw new ArgumentNullException(); }
            if (filename == "") { throw new ArgumentException(); }
            this.Filename = filename;
            if (this.IsExist())
            {
                LineCount = ReadHeadToEnd().Count;
                SeekCount = 0;
            }
        }
        public string ReadNext()
        {
            using (var sr = new StreamReader(Filename))
            {
                Seek(sr);
                if (!sr.EndOfStream)
                {
                    SeekCount++;
                }
                var s = sr.ReadLine();
                if (s is null) { return ""; }
                else { return s; }
            }
        }

        public void WriteLine(string sentence)
        {
            SeekCount = 0;
            using (var sw = new StreamWriter(Filename))
            {
                sw.WriteLine(sentence);
            }
        }
        public void AppendLine(string sentence)
        {
            using (var fs = new FileStream(Filename, FileMode.Append))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(sentence);
            }
        }

        public List<string> ReadHeadToEnd()
        {
            using (var sr = new StreamReader(Filename))
            {
                var list = new List<string>();
                while (!sr.EndOfStream)
                {
                    list.Add(sr.ReadLine());
                    SeekCount++;
                }
                return list;
            }
        }
        public List<string> ReadNextToEnd()
        {
            using (var sr = new StreamReader(Filename))
            {
                Seek(sr);
                var list = new List<string>();
                while (!sr.EndOfStream)
                {
                    list.Add(sr.ReadLine());
                    SeekCount++;
                }
                return list;
            }
        }
        private void Seek(StreamReader sr)
        {
            for (int i = 0; i < SeekCount; i++)
            {
                sr.ReadLine();
            }
        }
        public static void DownloadFile(string filename, string applicationName, string githubRawFileURL, MessageBoxType messageBoxType)
        {
            if (new TextfileManager(filename).IsExist()) { return; }
            int i;
            var dialogResult = UIManager.MessageBoxShow(filename + "が" + AppDomain.CurrentDomain.BaseDirectory + "に見つかりません。\n" +
                "Githubリポジトリからダウンロードします。", applicationName, MessageBoxButtons.YesNoCancel, messageBoxType);
            if (dialogResult == DialogResult.No) { return; }
            var url = githubRawFileURL + filename;
            for (i = 0; i < 10; i++)
            {
                try { new WebClient().DownloadFile(url, filename); }
                catch (WebException) { }
                if (IsExist(filename)) { return; }
            }
            UIManager.MessageBoxShow("ファイルのダウンロードに失敗しました。", applicationName, MessageBoxButtons.OK, MessageBoxType.Error);
            throw new FileNotFoundException("ファイルのダウンロードに失敗しました。", filename);
        }
        public bool IsExist() => IsExist(Filename);
        public bool IsNotExist() => IsNotExist(Filename);
        public static bool IsExist(string filename) => File.Exists(filename);
        public static bool IsNotExist(string filename) => !IsExist(filename);

        public virtual void Create()
        {
            Create(Filename);
        }
        public static void Create(string filename)
        {
            var fs = File.Create(filename);
            fs.Close();
        }
        public virtual void Delete()
        {
            Delete(Filename);
        }
        public static void Delete(string filename)
        {
            File.Delete(filename);
        }
    }
}
