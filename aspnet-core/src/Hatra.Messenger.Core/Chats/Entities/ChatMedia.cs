using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MimeTypes;
using Hatra.Messenger.Authorization.Users;

namespace Hatra.Messenger.Chats.Entities
{
    public class ChatMedia : Entity<Guid>, ICreationAudited<User>
    {
        public string MimeType { get; set; }
        public int Size { get; set; }
        public string Address { get; set; }

        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
        public User CreatorUser { get; set; }
    }
}
