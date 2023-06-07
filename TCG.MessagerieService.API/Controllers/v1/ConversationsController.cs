using MediatR;
using Microsoft.AspNetCore.Mvc;
using TCG.MessagerieService.API.Hub;
using TCG.MessagerieService.Application.Conversations.Dto;
using TCG.MessagerieService.Application.Conversations.Queries;

namespace TCG.MessagerieService.API.Controllers.v1
{
    [ApiController]
    [Route("[controller]")]
    public class ConversationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConversationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("utilisateur/{id}")]
        public async Task<IActionResult> GetAllConversationByUserId(int id, CancellationToken cancellationToken)
        {
            var conversations = await _mediator.Send(new GetAllConversationsByUserIdQuery(id), cancellationToken);

            return Ok(conversations);
        }

        [HttpGet("contexte")] // CancellationToken est en premier parametre car idMerchPost est optionnel -> règle .net, param optionnel = en derniere position 
        public async Task<IActionResult> GetConversationByContext(CancellationToken cancellationToken, int idUser1, int idUser2, string idMerchPost = "")
        {
            var idsUsers = new List<int>
            {
                idUser1,
                idUser2
            };
            
            var conversations = await _mediator.Send(new GetConversationByContextQuery(idsUsers, idMerchPost), cancellationToken);

            return Ok(conversations);
        }
    }
}
