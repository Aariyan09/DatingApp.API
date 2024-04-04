using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;

namespace DatingApp.API.Interfaces
{
    public interface IMessageRepo
    {
        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message> GetMessageAsync(int id);

        Task<PagedList<Message_DTO>> GetMessageForUser(MessagesParams @params);

        Task<IEnumerable<Message_DTO>> GetMessageThread(string currentUserName,string recipientUserName);


        Task<bool> SaveAllAsync();   


    }
}
