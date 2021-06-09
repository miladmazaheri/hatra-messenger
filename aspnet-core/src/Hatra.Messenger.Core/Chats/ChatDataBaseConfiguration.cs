﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Chats.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hatra.Messenger.Chats
{
    public static class ChatDataBaseConfiguration
    {
        public static void ConfigureChat(this ModelBuilder builder)
        {
            builder.Entity<RefreshToken>().Property(x => x.Token).IsRequired();
            builder.Entity<RefreshToken>().Property(x => x.CreatedByIp).HasMaxLength(20).IsRequired();
            builder.Entity<RefreshToken>().Property(x => x.Device).HasMaxLength(50).IsRequired();
            builder.Entity<RefreshToken>().HasOne(x => x.User).WithMany(x=>x.RefreshTokens).HasForeignKey(x => x.UserId).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<RefreshToken>().HasIndex(x => new {x.Token, x.Device, x.Expires}).IsUnique();

            builder.Entity<Chat>().Property(x => x.Title).HasMaxLength(50).IsRequired(false);

            builder.Entity<ChatParticipant>().HasKey(c => new { c.UserId, c.ChatId});
            builder.Entity<ChatParticipant>().HasOne(x => x.Chat).WithMany(x=>x.Participants).HasForeignKey(x => x.ChatId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ChatParticipant>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ChatParticipant>().Property(x => x.Setting).IsRequired(false);

            builder.Entity<ChatContent>().Property(x => x.Text).IsRequired(false);
            builder.Entity<ChatContent>().Property(x => x.ThumbnailAddress).IsRequired(false);
            builder.Entity<ChatContent>().HasOne(x => x.Chat).WithMany().HasForeignKey(x => x.ChatId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ChatContent>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ChatContent>().Property(x => x.ReplyOfId).IsRequired(false);


            builder.Entity<ChatMedia>().Property(x => x.MimeType).HasMaxLength(100).IsRequired(true);
            builder.Entity<ChatMedia>().Property(x => x.Address).IsRequired(true);


        }
    }
}
