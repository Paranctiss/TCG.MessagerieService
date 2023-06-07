using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCG.MessagerieService.Application.Contracts;
using TCG.MessagerieService.Domain;

namespace TCG.MessagerieService.Application.Conversations.Queries
{
    public record GetConversationByContextQuery(List<int> idsUsers, string idMerchPost) : IRequest<conversation>;

    public class GetConversationByContextQueryHandler : IRequestHandler<GetConversationByContextQuery, conversation>
    {
        private readonly ILogger<GetConversationByContextQueryHandler> _logger;
        private readonly IMongoRepositoryConversation _mongoRepositoryConversation;

        public GetConversationByContextQueryHandler(
            ILogger<GetConversationByContextQueryHandler> logger,
            IMongoRepositoryConversation mongoRepositoryConversation)
        {
            _logger = logger;
            _mongoRepositoryConversation = mongoRepositoryConversation;
        }

        public async Task<conversation> Handle(GetConversationByContextQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _mongoRepositoryConversation.GetAsync(request.idsUsers, request.idMerchPost);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving search post with public : {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }
}
