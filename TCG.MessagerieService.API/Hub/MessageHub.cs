using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using TCG.MessagerieService.Application.Contracts;
using TCG.MessagerieService.Domain;

namespace TCG.MessagerieService.API.Hub
{
    public class MessageHub : Hub<IMessageHubClient>
    {
        private readonly IMongoRepositoryConversation _mongoRepositoryConversation;
        
        public MessageHub(IMongoRepositoryConversation mongoRepositoryConversation) 
        {
            _mongoRepositoryConversation = mongoRepositoryConversation;
        }

        public async Task SendToAllUsers(string message)
        {
            await Clients.All.SendToAllUsers(message);
        }

        public async Task CreatePrivateConversation(int idUser1, int idUser2, string idMerchPost)
        {
            string groupName = GetPrivateConversationGroupName(idUser1, idUser2, idMerchPost);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        private string GetPrivateConversationGroupName(int idUser1, int idUser2, string idMerchPost)
        {
            // Tri des ID des utilisateurs pour garantir l'unicité du nom du groupe
            var sortedIds = new List<int> { idUser1, idUser2 };
            sortedIds.Sort();

            // Génération du nom du groupe en combinant les ID des utilisateurs
            string groupName = $"PrivateConversation_{sortedIds[0]}_{sortedIds[1]}_{idMerchPost}";

            return groupName;
        }

        public async Task UpdateMessageInConversation(int idSender, int idReceiver, string idMerchPost, message message)
        {
            string groupName = GetPrivateConversationGroupName(idSender, idReceiver, idMerchPost);
            await Clients.Group(groupName).UpdateMessageInConversation(idSender, idReceiver, idMerchPost,JsonSerializer.Serialize(message));
        }

        public async Task SendMessageInConversation(int idSender, int idReceiver, string idMerchPost, message message)
        {
            string groupName = GetPrivateConversationGroupName(idSender, idReceiver, idMerchPost);
            //if (string.IsNullOrEmpty(groupName))
            //{
            //    await CreatePrivateConversation(idSender, idReceiver, idMerchPost);
            //    groupName = GetPrivateConversationGroupName(idSender, idReceiver, idMerchPost);
            //}
            await Clients.Group(groupName).SendMessageInConversation(idSender, idReceiver, idMerchPost, JsonSerializer.Serialize(message));
            var conversation = await _mongoRepositoryConversation.GetAsync(new List<int> { idSender, idReceiver }, idMerchPost);
            if (conversation == null)
            {
                conversation = new conversation
                {
                    id = Guid.NewGuid().ToString(),
                    users = new List<user> { new user { id = idSender }, new user { id = idReceiver } },
                    merchPostId = idMerchPost,
                    messages = new List<message>()
                };

                await _mongoRepositoryConversation.CreateAsync(conversation);
            }
            conversation.messages.Add(message);
            await _mongoRepositoryConversation.UpdateAsync(conversation);
        }
    }
}
