using System;

namespace Hatra.Messenger.Common.DataTransferObjects
{
    public class ChatListItemDto
    {
        public Guid ChatId { get; set; }
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
    }

}
