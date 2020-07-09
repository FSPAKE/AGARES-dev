using Google.Apis.YouTube.v3.Data;
using System;
using System.Drawing;
using System.IO;
using Utils;

namespace FSPAKE.AGARES.Unit
{
    public class LiveChatFieldAccesser
    {
        public LiveChatMessage Contents { get; private set; }
        public LiveChatFieldAccesser(LiveChatMessage liveChatMessage)
        {
            if (liveChatMessage is null) { throw new ArgumentNullException(nameof(liveChatMessage)); }
            Contents = liveChatMessage;
        }
        public string DisplayName => Contents.AuthorDetails.DisplayName.Replace("&", "&&").Replace("\\", "\\\\");
        public string DisplayMessage => Contents.Snippet.DisplayMessage.Replace("&", "&&").Replace("\\", "\\\\");
        public string ChannelId => Contents.AuthorDetails.ChannelId;
        public string LiveChatId => Contents.Id;
        public string ProfileImageURL => Contents.AuthorDetails.ProfileImageUrl;
        public ulong? Amount
        {
            get
            {
                if (!(Contents.Snippet.SuperChatDetails is null))
                {
                    return Contents.Snippet.SuperChatDetails.AmountMicros / 1000000;
                }
                if (!(Contents.Snippet.SuperStickerDetails is null))
                {
                    return Contents.Snippet.SuperStickerDetails.AmountMicros / 1000000;
                }
                return 0;
            }
        }

        public string Currency
        {
            get
            {
                if (!(Contents.Snippet.SuperChatDetails is null))
                {
                    return Contents.Snippet.SuperChatDetails.Currency;
                }
                if (!(Contents.Snippet.SuperStickerDetails is null))
                {
                    return Contents.Snippet.SuperStickerDetails.Currency;
                }
                return "";
            }
        }

        public bool IsFanFund => Contents.Snippet.SuperChatDetails != null ||
                    Contents.Snippet.SuperStickerDetails != null;

        public long? Tier
        {
            get
            {
                if (!(Contents.Snippet.SuperChatDetails is null))
                {
                    return Contents.Snippet.SuperChatDetails.Tier;
                }
                if (!(Contents.Snippet.SuperStickerDetails is null))
                {
                    return Contents.Snippet.SuperStickerDetails.Tier;
                }
                return 0;
            }
        }

        public bool IsSuperChat => Contents.Snippet.SuperChatDetails != null;
        public bool IsSuperSticker => Contents.Snippet.SuperStickerDetails != null;
        public bool IsByChatOwner => (bool)Contents.AuthorDetails.IsChatOwner;
        public bool IsByChatModerator => (bool)Contents.AuthorDetails.IsChatModerator;
        public bool IsByChatSponsor => (bool)Contents.AuthorDetails.IsChatSponsor;

        public Color BackColor
        {
            get
            {
                if (!this.IsFanFund)
                {
                    if ((bool)this.IsByChatOwner)
                    {
                        return Color.Yellow;
                    }
                    else if (this.Contents.Snippet.Type == "newSponsorEvent")
                    {
                        return Color.FromArgb(57, 118, 43);
                    }
                    else
                    {
                        return Color.Transparent;
                    }
                }
                else
                {
                    long? tier = this.Tier;
                    var superchatLogfile = new TextfileManager("superChatlog.txt");
                    if (this.IsSuperChat)
                    {
                        superchatLogfile.AppendLine("SuperChat," + "Tier:" + Tier + "," + Amount + Currency);

                        if (this.Currency == "YEN")
                        {
                            if (this.Amount < 200) { tier = 1; }
                            else if (this.Amount < 500) { tier = 2; }
                            else if (this.Amount < 1000) { tier = 3; }
                            else if (this.Amount < 2000) { tier = 4; }
                            else if (this.Amount < 5000) { tier = 5; }
                            else if (this.Amount < 10000) { tier = 6; }
                            else if (this.Amount >= 10000) { tier = 7; }
                        }
                        switch (tier)
                        {
                            case 1:
                                return Color.FromArgb(48, 99, 186);
                            case 2:
                            case 12://稀に現れる詳細未確認のTier
                            case 13://稀に現れる詳細未確認のTier
                                return Color.FromArgb(104, 226, 251);
                            case 3:
                                return Color.FromArgb(110, 230, 185);
                            case 4:
                                return Color.FromArgb(247, 204, 79);
                            case 5:
                                return Color.FromArgb(229, 131, 49);
                            case 6:
                                return Color.FromArgb(214, 56, 100);
                            case 7:
                                return Color.FromArgb(211, 57, 41);
                        }
                    }
                    if (this.IsSuperSticker)
                    {
                        superchatLogfile.AppendLine("SuperSticker," + "Tier:" + Tier + "," + Amount + Currency);
                        if (this.Currency == "YEN")
                        {
                            if (this.Amount < 200) { tier = 7; }
                            else if (this.Amount < 500) { tier = 1; }
                            else if (this.Amount < 1000) { tier = 2; }
                            else if (this.Amount < 2000) { tier = 3; }
                            else if (this.Amount < 5000) { tier = 4; }
                            else if (this.Amount < 10000) { tier = 5; }
                        }
                        switch (tier)
                        {
                            case 7:
                                return Color.FromArgb(48, 99, 186);
                            case 1:
                                return Color.FromArgb(104, 226, 251);
                            case 2:
                                return Color.FromArgb(110, 230, 185);
                            case 3:
                                return Color.FromArgb(247, 204, 79);
                            case 4:
                                return Color.FromArgb(229, 131, 49);
                            case 5:
                                return Color.FromArgb(214, 56, 100);
                        }
                    }
                    return Color.White;
                }
            }
        }

        public Color LetterColor
        {
            get
            {
                if ((bool)this.IsByChatOwner)
                {
                    return Color.Black;
                }
                else if ((bool)this.IsByChatModerator)
                {
                    return Color.Blue;
                }
                else if ((bool)this.IsByChatSponsor)
                {
                    if (this.Contents.Snippet.Type == "newSponsorEvent")
                    {
                        return Color.Black;
                    }
                    else
                    {
                        return Color.Green;
                    }
                }
                else
                {
                    return Color.Black;
                }
            }
        }
    }
}
