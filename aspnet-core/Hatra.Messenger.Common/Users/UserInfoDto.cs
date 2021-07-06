using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatra.Messenger.Common.Users
{
    public class UserInfoDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string AvatarAddress { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
    }
}
