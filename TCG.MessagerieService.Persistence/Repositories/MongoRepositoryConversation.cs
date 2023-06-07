using Microsoft.AspNet.SignalR.Messaging;
using MongoDB.Driver;
using TCG.MessagerieService.Application.Contracts;
using TCG.MessagerieService.Domain;

namespace TCG.MessagerieService.Persistence.Repositories
{
    public class MongoRepositoryConversation : IMongoRepositoryConversation
    {
        private readonly IMongoCollection<conversation> _collection;
        public MongoRepositoryConversation(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<conversation>(collectionName);
        }

        public async Task<conversation> GetAsync(List<int> idsUsers, string idMerchPost)
        {
            var filter = Builders<conversation>.Filter.All(x => x.users.Select(u => u.id), idsUsers)
                         & Builders<conversation>.Filter.Eq(x => x.merchPostId, idMerchPost);

            var conversation = await _collection.Find(filter)
                .FirstOrDefaultAsync();

            if (conversation != null && conversation.messages != null && conversation.messages.Count > 1)
            {
                conversation.messages = conversation.messages.OrderBy(m => m.dateEnvoi).ToList();
            }

            return conversation;
        }

        public async Task CreateAsync(conversation conversation)
        {
            await _collection.InsertOneAsync(conversation);
        }

        public async Task<conversation> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _collection.Find(x => x.id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(conversation conversation)
        {
            var filter = Builders<conversation>.Filter.Eq(x => x.id, conversation.id);
            var update = Builders<conversation>.Update
                .Set(x => x.messages, conversation.messages);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<List<conversation>> GetAllByUserIdAsync(int idUser, CancellationToken cancellationToken)
        {
            var filter = Builders<conversation>.Filter.ElemMatch(x => x.users, u => u.id == idUser);
            var conversations = await _collection.Find(filter).ToListAsync();
            conversations.ForEach(c =>
            {
                if (c.messages != null && c.messages.Count > 1)
                {
                    var mostRecentMessage = c.messages.OrderBy(m => m.dateEnvoi).First();
                    c.messages.Clear();
                    c.messages.Add(mostRecentMessage);
                }
            });

            // Ici on cherche juste à avoir les conversations, pas les messages. 
            // On garde juste le dernier message pour afficher un aperçu de la conversation.

            return conversations;
        }

    }
}
