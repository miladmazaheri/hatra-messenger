using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Hatra.Messenger.Authorization.Users;

namespace Hatra.Messenger.Chats.Entities
{
    public class ChatContent:Entity<Guid>,IHasCreationTime
    {
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
        public long? UserId { get; set; }
        public User User { get; set; }
        public string MediaAddress{ get; set; }
        public string Text { get; set; }
        public Guid? ReplyOfId { get; set; }
        public int ViewCount { get; set; }
        public int ReceiveCount { get; set; }
        public string ThumbnailAddress { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
