﻿using DatingApp.API.Entities;

namespace DatingApp.API.DTOs.Response
{
    public class Message_DTO
    {
        public int Id { get; set; }
        public int SenderId { get; set; }

        public string SenderUserName { get; set; }

        public string SenderPhotoUrl { get; set; }

        public int RecipientId { get; set; }
        public string RecipientUserName { get; set; }

        public string RecipientPhotoUrl { get; set; }

        public string Content { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime? MessageSent { get; set; } = DateTime.UtcNow;

    }
}
