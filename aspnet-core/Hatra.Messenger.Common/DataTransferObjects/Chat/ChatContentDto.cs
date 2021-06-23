using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DNTPersianUtils.Core;

namespace Hatra.Messenger.Common.DataTransferObjects.Chat
{
    public class ReceivedMessageDto
    {
        public Guid Id { get; set; }
        public string MediaAddress { get; set; }
        public string ThumbnailAddress { get; set; }
        public string Text { get; set; }
        public Guid? ReplyOfId { get; set; }

        public bool IsValid() => !string.IsNullOrWhiteSpace(MediaAddress) ||
                               !string.IsNullOrWhiteSpace(Text);

        public void ApplyCorrectYeKe()
        {
            Text?.ApplyCorrectYeKe();
        }
    }
    public class ChatContentDto : ReceivedMessageDto
    {
        public Guid ChatId { get; set; }
        public long UserId { get; set; }
        public DateTime CreationTime { get; set; }
        public ChatContentDto()
        {

        }

        public ChatContentDto(Guid chatId, long userId,DateTime creationTime ,ReceivedMessageDto message)
        {
            ChatId = chatId;
            UserId = userId;
            Id = message.Id;
            CreationTime = creationTime;
            MediaAddress = message.MediaAddress;
            ThumbnailAddress = message.ThumbnailAddress;
            Text = message.Text;
            ReplyOfId = message.ReplyOfId;
        }


    }

    public class ChatContentRequestDto
    {
        [Required]
        public int Count { get; set; } = 10;
        [Required]
        public Guid ChatId { get; set; }
        [Required]
        public DateTime BaseDateTime { get; set; } =DateTime.Now;

    }

    public class DeleteChatContentDto :DeleteChatDto
    {
        [Required]
        public List<Guid> MessageIds { get; set; }
       
    }
    public class DeleteChatDto
    {
        [Required]
        public Guid ChatId { get; set; }
        [Required]
        public long ReceiverId { get; set; }
    }
}
