using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatra.Messenger.Models.Users
{
    public class GetContactsModel
    {
        public List<string> Phones { get; set; }
        public List<string> Usernames { get; set; }
    }
}
