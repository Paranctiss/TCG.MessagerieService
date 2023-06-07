using TCG.MessagerieService.Domain;

namespace TCG.MessagerieService.Application.Contracts
{
    public interface IMongoRepositoryConversation
    {
        public Task<conversation> GetAsync(List<int> idsUsers, string idMerchPost);
        public Task<conversation> GetByIdAsync(string id, CancellationToken cancellationToken);
        public Task CreateAsync(conversation conversation);
        public Task UpdateAsync(conversation conversation);
        public Task<List<conversation>> GetAllByUserIdAsync(int idUser, CancellationToken cancellationToken);
    }
}
