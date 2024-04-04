using AutoMapper;
using DatingApp.API.DTOs.Requests;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Extenstions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepo _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public MessageController(IMessageRepo messageRepo, IUserRepository userRepo, IMapper mapper)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }


        [HttpPost]
        public async Task<ActionResult<Message_DTO>> CreateMessage(CreateMessage_DTO dto)
        {
            if (string.IsNullOrEmpty(dto.RecipientUserName))
                return BadRequest("Recipient name cant be blank");


            var username = User.GetUserName();

            if (username.ToLower() == dto.RecipientUserName.ToLower())
                return BadRequest("You cannot message yourself");

            var sender = await _userRepo.GetUserByNameAsync(username);
            var recipient = await _userRepo.GetUserByNameAsync(dto.RecipientUserName);

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = username,
                RecipientUserName = recipient.UserName,
                Content = dto.Content
            };

            _messageRepo.AddMessage(message);

            if (await _messageRepo.SaveAllAsync()) 
                return Ok(_mapper.Map<Message_DTO>(message));

            return BadRequest("Failed to send message");
        }


        [HttpGet]
        public async Task<ActionResult<PagedList<Message_DTO>>> GetMessagesForUsers([FromQuery] MessagesParams messagesParams)
        {
            messagesParams.UserName = User.GetUserName();
            var message = await _messageRepo.GetMessageForUser(messagesParams);

            Response.AddPaginationHeader(new PaginationHeader(message.CurrentPage,message.PageSize,message.TotalCount,message.TotalPages));

            return message;
        }


        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<Message_DTO>>> GetMessageThread(string username)
        {
            string currentUserName = User.GetUserName();
            return Ok(await _messageRepo.GetMessageThread(currentUserName,username));
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();
            var message = await _messageRepo.GetMessageAsync(id);

            if (username != message.SenderUserName && username != message.RecipientUserName)
                return Unauthorized();


            if (message.SenderUserName == username)
                message.SenderDeleted = true;

            if (message.RecipientUserName == username)
                message.RecipientDeleted = true;


            if (message.RecipientDeleted && message.SenderDeleted)
                _messageRepo.DeleteMessage(message);

            if (await _messageRepo.SaveAllAsync())
                return Ok();

            return BadRequest("Problem in deleting the message");

        }

    }
}
