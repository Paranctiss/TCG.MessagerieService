using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TCG.MessagerieService.Domain
{
    public class conversation
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public List<message> messages { get; set; }
        public string merchPostId { get; set; }
        public List<user> users { get; set; }
    }

    public class message
        
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public offre offre { get; set; }
        public int idUserEnvoi { get; set; }
        public DateTime dateEnvoi { get; set; }
        public string texte { get; set; }
    }

    public class offre
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public int id { get; set; }
        public decimal prixPropose { get; set; }
        public char etat { get; set; }
    }

    public class user
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public int id { get; set; }
        public string userName { get; set; }
        public string photoProfil { get; set; }
    }
}