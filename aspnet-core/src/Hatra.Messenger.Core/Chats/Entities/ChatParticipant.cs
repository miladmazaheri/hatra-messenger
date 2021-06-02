using System;
using System.Collections.Generic;
using Abp.Domain.Entities.Auditing;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Chats.Enums;

namespace Hatra.Messenger.Chats.Entities
{
    public class ChatParticipant : IFullAudited<User>
    {
        public virtual long UserId { get; set; }
        public virtual User User { get; set; }

        public virtual Guid ChatId { get; set; }
        public virtual Chat Chat { get; set; }

        public virtual ChatAccessType ChatAccessType { get; set; }
        public virtual string Setting { get; set; }


        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public User CreatorUser { get; set; }
        public User LastModifierUser { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public long? DeleterUserId { get; set; }
        public User DeleterUser { get; set; }
    }
}
