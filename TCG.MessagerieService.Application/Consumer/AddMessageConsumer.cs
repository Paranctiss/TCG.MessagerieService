using MassTransit;
using MongoDB.Bson;
using System.Text.Json;
using TCG.Common.MassTransit.Messages;
using TCG.MessagerieService.Application.Contracts;
using TCG.MessagerieService.Domain;

namespace TCG.MessagerieService.Application.Consumer;

public class AddMessageConsumer : IConsumer<AddMessage>
{
    private readonly IMongoRepositoryConversation _mongoRepositoryConversation;

    public AddMessageConsumer(IMongoRepositoryConversation mongoRepositoryConversation)
    {
        _mongoRepositoryConversation = mongoRepositoryConversation;
    }

    public async Task Consume(ConsumeContext<AddMessage> context)
    {
        var message = context.Message;

        if (message != null)
        {
            var users = JsonSerializer.Deserialize<List<user>>(message.users); 
            var messageConv = JsonSerializer.Deserialize<message>(message.message);
            var salePost = JsonSerializer.Deserialize<salePost>(message.merchPost);

            if (string.IsNullOrEmpty(messageConv.id))
            {
                messageConv._id = ObjectId.GenerateNewId();
                messageConv.id = Guid.NewGuid().ToString();
                if (messageConv.offre != null)
                {
                    messageConv.offre._id = ObjectId.GenerateNewId();
                }
            }

            // On r�cup�re la conversation si elle existe
            var conversation = await _mongoRepositoryConversation.GetAsync(users.Select(u => u.id).ToList(), message.idMerchPost);

            if (conversation != null)
            {
                if (messageConv != null && messageConv.offre != null && conversation.messages != null)
                {
                    foreach (var m in conversation.messages)
                    {
                        // On parcours les offres de tous les messages, si l'offre existe d�j�, on la met � jour.
                        if (m.offre != null && m.offre.etat == 'C')
                        {
                            // Si l'offre est en �tat cr��,
                            // cela signifie que le user change son offre par lui m�me donc il annule son offre pr�c�dente.
                            m.offre.etat = 'A';
                        }
                    }
                }
                conversation.messages.Add(messageConv);
                await _mongoRepositoryConversation.UpdateAsync(conversation);
            }
            else
            {
                conversation = new conversation
                {
                    id = Guid.NewGuid().ToString(),
                    users = users,
                    merchPostId = message.idMerchPost,
                    merchPost = salePost,
                    messages = new List<message> { messageConv }
                };

                await _mongoRepositoryConversation.CreateAsync(conversation);
            }
        }
    }

}