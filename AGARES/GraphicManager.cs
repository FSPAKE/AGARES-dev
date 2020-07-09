using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Linq;
using System;
using System.Runtime.InteropServices;

namespace Utils
{
    public static class GraphicManager
    {


        public static Image Scale(Image srcImg, int side)
        {
            return Scale(srcImg, side, side);
        }
        public static Image Scale(Image srcImg, int holizontal, int vertical)
        {
            if (srcImg is null) { return null; }
            var destImg = new Bitmap(holizontal, vertical);
            var g = Graphics.FromImage(destImg);

            g.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(srcImg, 0, 0, holizontal, vertical);
            return destImg;
        }
        public static Dictionary<string, Image> Scale(Dictionary<string, Image> srcDictionary, int side)
        {
            if (srcDictionary is null) { return null; }

            var dest = new Dictionary<string, Image>();
            foreach (var strimgpair in srcDictionary)
            {
                dest.Add(strimgpair.Key, Scale(strimgpair.Value, side));
            }
            return dest;
        }
        public static Image Download(string url)
        {
            if (string.IsNullOrEmpty(url)) { return null; }
            using (var webclient = new WebClient())
            {
                var imgbin = webclient.DownloadData(url);
                using (var st = new MemoryStream(imgbin))
                {
                    return (Image)Image.FromStream(st).Clone();
                }
            }
        }
        public static Image Download(string url, string foldername, string filename)
        {
            if (!Directory.Exists(foldername))
            {
                throw new ExternalException($"フォルダ「{foldername}」がありません");
            }
            string pass = $@"{foldername}\{filename}";
            var img = Download(url);
            new Bitmap(img).Save(pass);
            return img;
        }

        public enum FileNameType { FileNameWithoutExtention, FileName };

        public static Dictionary<string, Image> Load(string directoryPass, FileNameType fileNameType)
        {
            var fileNames = Directory.EnumerateFiles(directoryPass, "*", SearchOption.TopDirectoryOnly);
            var dest = new Dictionary<string, Image>();
            foreach (var fileName in fileNames)
            {
                try
                {
                    if (fileNameType == FileNameType.FileNameWithoutExtention)
                    {
                        dest.Add(Path.GetFileNameWithoutExtension(fileName), Image.FromFile(fileName));
                    }
                    if (fileNameType == FileNameType.FileName)
                    {
                        dest.Add(Path.GetFileName(fileName), Image.FromFile(fileName));
                    }
                }
                catch (Exception any) { /*ignored*/ }
            }
            return dest;
        }
    }
}
