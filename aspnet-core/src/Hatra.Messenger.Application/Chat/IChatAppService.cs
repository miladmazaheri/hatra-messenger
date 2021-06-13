﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Common.DataTransferObjects;

namespace Hatra.Messenger.Chat
{
    public interface IChatAppService :IApplicationService
    {
        Task<List<ChatListItemWithLastContentDto>> GetChatHistoryAsync(long userId);
        Task<ChatListItemDto> StartPrivateChatAsync(long userId, long userReceiverId);
    }
}
