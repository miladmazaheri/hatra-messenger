using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Hatra.Messenger.Authorization.Users
{
    public class RefreshToken : Entity<long>, IHasCreationTime
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatedByIp { get; set; }
        public string Device { get; set; }
    }
}
