using MassTransit;
using MongoDB.Bson;
using System.Text.Json;
using TCG.Common.MassTransit.Messages;
using TCG.MessagerieService.Application.Consumer.Dto;
using TCG.MessagerieService.Application.Contracts;
using TCG.MessagerieService.Domain;

namespace TCG.MessagerieService.Application.Consumer;

public class UpdateOfferInMessageConsumer : IConsumer<UpdateOfferInMessage>
{
    private readonly IMongoRepositoryConversation _mongoRepositoryConversation;

    public UpdateOfferInMessageConsumer(IMongoRepositoryConversation mongoRepositoryConversation)
    {
        _mongoRepositoryConversation = mongoRepositoryConversation;
    }

    public async Task Consume(ConsumeContext<UpdateOfferInMessage> context)
    {
        var message = context.Message;

        if (message != null)
        {
            var offer = JsonSerializer.Deserialize<OfferDto>(message.offer);
            var users = new List<user>
            {
                new user{ id = offer.SellerId },
                new user { id = offer.BuyerId}
            };
            
            // On récupère la conversation si elle existe
            var conversation = await _mongoRepositoryConversation.GetAsync(users.Select(u => u.id).ToList(), offer.MerchPostId);

            if (conversation != null)
            {
                foreach (var msg in conversation.messages)
                {
                    if (msg.offre != null && msg.offre.id == offer.Id)
                    {
                        msg.offre.etat = offer.OfferStatePostId;
                    }
                }   
                await _mongoRepositoryConversation.UpdateAsync(conversation);
            }
        }
    }

}