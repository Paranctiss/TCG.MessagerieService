using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TCG.MessagerieService.Application.Contracts;
using TCG.MessagerieService.Domain;

namespace TCG.MessagerieService.Application.Conversations.Queries
{
    public record GetAllConversationsByUserIdQuery(int idUser) : IRequest<List<conversation>>;

    public class GetAllConversationsByUserIdValidator : AbstractValidator<GetAllConversationsByUserIdQuery>
    {
        public GetAllConversationsByUserIdValidator()
        {
            RuleFor(sp => sp.idUser).NotEmpty();
        }
    }

    public class GetAllConversationsByUserIdQueryHandler : IRequestHandler<GetAllConversationsByUserIdQuery, List<conversation>>
    {
        private readonly ILogger<GetAllConversationsByUserIdQueryHandler> _logger;
        private readonly IMongoRepositoryConversation _mongoRepositoryConversation;

        public GetAllConversationsByUserIdQueryHandler(
            ILogger<GetAllConversationsByUserIdQueryHandler> logger,
            IMongoRepositoryConversation mongoRepositoryConversation)
        {
            _logger = logger;
            _mongoRepositoryConversation = mongoRepositoryConversation;
        }
        public async Task<List<conversation>> Handle(GetAllConversationsByUserIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _mongoRepositoryConversation.GetAllByUserIdAsync(request.idUser, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving search post with public : {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }
}
