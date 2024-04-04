using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.Data;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DatingApp.API.Services
{
    public class MessageRepo : IMessageRepo
    {
        private readonly DataContext _db;
        private readonly IMapper mapper;

        public MessageRepo(DataContext db, IMapper mapper)
        {
            _db = db;
            this.mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _db.Message.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _db.Message.Remove(message);
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await _db.Message.FindAsync(id);
        }

        public async Task<PagedList<Message_DTO>> GetMessageForUser(MessagesParams @params)
        {
            var query = _db.Message.OrderByDescending(x => x.MessageSent).AsQueryable();

            query = @params.Container switch
            {
                "Inbox" => query.Where(x => x.RecipientUserName == @params.UserName && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.SenderUserName == @params.UserName && x.SenderDeleted == false),
                _ => query.Where(x => x.RecipientUserName == @params.UserName && x.DateRead == null && x.RecipientDeleted == false)
            };

            var message = query.ProjectTo<Message_DTO>(mapper.ConfigurationProvider);

            return await PagedList<Message_DTO>.CreateAsync(message, @params.PageNumber, @params.PageSize);
        }

        public async Task<IEnumerable<Message_DTO>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = await _db.Message
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                .Where(m =>
                    (m.RecipientUserName == currentUserName && m.SenderUserName == recipientUserName && m.RecipientDeleted == false) ||
                    (m.RecipientUserName == recipientUserName && m.SenderUserName == currentUserName && m.SenderDeleted == false))
                .OrderBy(m => m.MessageSent).ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUserName == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                unreadMessages.ForEach(item => item.DateRead = DateTime.UtcNow);
                await _db.SaveChangesAsync();
            }

            return mapper.Map<IEnumerable<Message_DTO>>(messages);
        } 

        public async Task<bool> SaveAllAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
