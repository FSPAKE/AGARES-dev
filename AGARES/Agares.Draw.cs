using FSPAKE.AGARES.Unit;
using Google;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Utils;
namespace FSPAKE.AGARES.CoreSystem
{
    /// <summary>
    /// アプリケーションの描画に関する処理を記述する
    /// </summary>
    partial class Agares
    {
        ///<summary>マウス選択しているコメントのリスナー名とコメントの連結</summary>
        string SelectingConcatedLine;
        int SelectingIndex;

        /// <summary>
        /// オーナードロー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentsDrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index != -1)
            {
                if (IconMode.Checked)
                {
                    DrawIcon(e);
                }

                DrawComment(sender, e);
            }
            e.DrawFocusRectangle();
        }

        /// <summary>
        /// リスナーアイコンの描画
        /// </summary>
        /// <param name="e"></param>
        private void DrawIcon(DrawItemEventArgs e)
        {
            try
            {
                var listenerChannelID = LiveChatHouse[e.Index].ChannelId;
                var icon = ListenerIconDictionary[listenerChannelID];
                e.Graphics.DrawImage(icon, e.Bounds.X, e.Bounds.Y);
            }
            catch (Exception ex) when 
                            (ex is KeyNotFoundException ||
                             ex is GoogleApiException ||
                             ex is AggregateException)
            { /*ignored*/ }
        }

        /// <summary>描画中コメント中のメンバーシップ絵文字の描画位置（文字数位置）と対象画像のペア</summary>
        private Dictionary<int, Image> analizedResult = new Dictionary<int, Image>();

        /// <summary>
        /// コメント文章とメンバーシップ絵文字の描画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawComment(object sender, DrawItemEventArgs e)
        {
            var textbounds = e.Bounds;
            var split = new CommentBuilder(((ListBox)sender).Items[e.Index].ToString());

            if (IconMode.Checked)
            {
                textbounds.X += e.Bounds.Height;
            }

            split.DisplayArea = AnalizedDisplayAreaHouse[e.Index + 1];

            Color letterColor;
            Color backColor;
            if (e.State.HasFlag(DrawItemState.Selected))
            {
                //マウスで行を選択している時
                letterColor = Color.White;
                backColor = Color.Transparent;
                SelectingIndex = e.Index;
                SelectingConcatedLine = split.ConcatResult;
            }
            else
            {
                letterColor = (Color)new ColorConverter().ConvertFromString(split.LetterColor);
                backColor = (Color)new ColorConverter().ConvertFromString(split.BackColor);
            }

            TextRenderer.DrawText(e.Graphics, split.DisplayArea, e.Font, textbounds, letterColor, backColor, TextFormatFlags.Default);

            foreach (var v in ColumnsOfImgForLine[e.Index + 1])
            {
                var place = v.Key - 1;
                var img = v.Value;
                if (IconMode.Checked)
                {
                    place += 2;
                }
                const int adjustmentLiteral = 4;
                e.Graphics.DrawImage(img, (e.Bounds.Height / 2) * place + adjustmentLiteral, e.Bounds.Y);
            }
        }
        /// <summary>
        /// 配信動画のサムネイル・タイトル／チャンネル名をセットする
        /// </summary>
        /// <param name="videoInfo"></param>
        private void DrawStreamInfo(Video videoInfo)
        {
            using (var canvas = new Bitmap(Thumb.Width, Thumb.Height))
            using (var g = Graphics.FromImage(canvas))
            {
                var font = new Font("ＭＳ ゴシック", 10);
                g.DrawString("サムネイルを\nロード中....", font, Brushes.Black, 0, 0);
                Thumb.Image = (Image)canvas.Clone();
            }
            //サムネイル
            using (var canvas = new Bitmap(Thumb.Width, Thumb.Height))
            using (var g = Graphics.FromImage(canvas))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                using (var image = DownloadThumbnail(videoInfo))
                {
                    g.DrawImage(image, 0, 0, canvas.Width, canvas.Height);
                    Thumb.Image = (Image)canvas.Clone();
                }
            }
            //チャンネル名／タイトル
            StreamTitle.Text = $"{videoInfo.Snippet.Title}{" Streamed By "}{videoInfo.Snippet.ChannelTitle}";
        }
        public static Image DownloadThumbnail(Video videoID)
        {
            if (videoID is null) { return null;}
            return GraphicManager.Download(videoID.Snippet.Thumbnails.Default__.Url);
        }
    }
}
