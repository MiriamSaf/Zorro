using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.WebApplication.Chat
{
    //deals with functionality for chat feature 
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        //DI for chat hub
        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //sends a message from a sender to a recipient 
        public async Task SendMessage(string sender, string recipient, string message)
        {
            if (string.IsNullOrWhiteSpace(sender) || string.IsNullOrWhiteSpace(recipient) || string.IsNullOrWhiteSpace(message))
                return;

            // send message to recipient
            await Clients.User(recipient).SendAsync("ReceiveMessage", sender, message);
            // send message back to sender so they see it in their chat window as well
            await Clients.Caller.SendAsync("ReceiveMessage", sender, message);
            //saves the chat message
            await SaveChatMessage(sender, recipient, message, DateTime.Now);
        }

        //saves the chat message with a timestamp
        private async Task SaveChatMessage(string sender, string recipient, string message, DateTime timestamp)
        {
            //check for invalid message and that sender and recipient are both valid 
            if (string.IsNullOrWhiteSpace(message))
                return;
            var senderUser = await _userManager.FindByEmailAsync(sender);
            if (senderUser is null)
                return;
            var recipientuser = await _userManager.FindByEmailAsync(recipient);
            if (recipientuser is null)
                return;
            //save chat details 
            var chatMessage = new ChatMessage()
            {
                Sender = senderUser,
                Recipient = recipientuser,
                Timestamp = timestamp,
                Message = message
            };
            //add the chat message to context and save 
            await _context.ChatMessages.AddAsync(chatMessage);
            await _context.SaveChangesAsync();
        }
    }

    //get the user id of user 
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity?.Name;
        }
    }
}
