using System;

namespace Hatra.Messenger.Common.DataTransferObjects
{
    public class ChatListItemDto
    {
        public Guid ChatId { get; set; }
        public long UserId { get; set; }
        public string Title { get; set; }
        public int UnreadCount { get; set; }
        public string LogoAddress { get; set; }
    }

    public class ChatListItemWithLastContentDto : ChatListItemDto
    {
        public Guid? LastContent_Id { get; set; }
        public long? LastContent_UserId { get; set; }
        public string LastContent_Text { get; set; }
        public DateTime? LastContent_DateTime { get; set; }

        public int LastContent_ViewCount { get; set; }
        public int LastContent_ReceiveCount { get; set; }
    }

    public class GetChatDto
    {
        public Guid? ChatId { get; set; }
    }

}
