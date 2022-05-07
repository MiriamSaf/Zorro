using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.WebApplication.Chat
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SendMessage(string sender, string recipient, string message)
        {
            bool check = String.IsNullOrWhiteSpace(message);
            if (check)
            {
                
                return ;
            }

            if(sender == null)
            {
                return;
            }

            bool recipReal = String.IsNullOrWhiteSpace(recipient);
            if (recipReal)
            {
                return;
            }

            await Clients.User(recipient).SendAsync("ReceiveMessage", sender, message);
            // Not Sure if this is required? It was causing double messages to be sent after
            // putting in checking if another user exists?
            //await Clients.Caller.SendAsync("ReceiveMessage", sender, message);

            bool messagevalid = false;
            foreach (var recipients in _context.Users)
            {
                if (recipients.Email.Equals(recipient))
                {
                    messagevalid = true;
                }
            }

            if (messagevalid == true)
            {
                await SaveChatMessage(sender, recipient, message, DateTime.Now);
            }
        }

        private async Task SaveChatMessage(string sender, string recipient, string message, DateTime timestamp)
        {
            if(message == null)
            {
                return;
            }

            bool check = String.IsNullOrWhiteSpace(message);
            if (check)
            {
                return;
            }

            bool recipReal = String.IsNullOrWhiteSpace(recipient);
            if (recipReal)
            {
                return;
            }

            bool messagevalid = false;

            foreach (var recipients in _context.Users)
            {
                if (recipients.Email.Equals(recipient))
                {
                    messagevalid = true;
                }
            }

            if (messagevalid == true)
            {
                var chatMessage = new ChatMessage()
                {
                    Sender = await _userManager.FindByEmailAsync(sender),
                    Recipient = await _userManager.FindByEmailAsync(recipient),
                    Timestamp = timestamp,
                    Message = message
                };
                await _context.ChatMessages.AddAsync(chatMessage);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity?.Name;
        }
    }
}
