using FSPAKE.AGARES.Unit;
using Utils;

namespace FSPAKE.AGARES.CoreSystem
{
    partial class Agares
    {
        private void RegisterListenerIcon(LiveChatFieldAccesser liveChat)
        {
            string listenerChannelId = liveChat.ChannelId;
            var listenerChannelurl = liveChat.ProfileImageURL;
            if (!ListenerIconDictionary.ContainsKey(listenerChannelId))
            {
                var img = GraphicManager.Download(listenerChannelurl, IconFoldername,listenerChannelId);
                ListenerIconDictionary.Add(listenerChannelId, GraphicManager.Scale(img, Comments.ItemHeight));
            }
        }
    }
}
