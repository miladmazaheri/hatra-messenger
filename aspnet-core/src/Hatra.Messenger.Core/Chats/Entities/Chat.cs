using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Chats.Enums;

namespace Hatra.Messenger.Chats.Entities
{
    public class Chat : Entity<Guid>, IFullAudited<User>
    {
        public Chat()
        {
            Participants = new HashSet<ChatParticipant>();
        }
        public virtual string Title { get; set; }
        public virtual ChatType ChatType { get; set; }
        public virtual Guid? LogoMediaId { get; set; }

        public virtual ICollection<ChatParticipant> Participants{ get; set; }

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
