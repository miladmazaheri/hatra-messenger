using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Hatra.Messenger.Chats.Entities;

namespace Hatra.Messenger.Chats
{
    public class ChatManager : IDomainService
    {
        protected IRepository<Chat, Guid> ChatRepository { get; }

        public ChatManager(IRepository<Chat, Guid> chatRepository)
        {
            ChatRepository = chatRepository;
        }



    }
}
