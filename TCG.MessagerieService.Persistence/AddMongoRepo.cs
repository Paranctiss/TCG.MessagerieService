using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using TCG.Common.Settings;
using TCG.MessagerieService.Application.Contracts;
using TCG.Common.Contracts;
using TCG.MessagerieService.Persistence.Repositories;

namespace TCG.MessagerieService.Persistence
{
    public static class AddMongoRepo
    {
        //Register the IMongoDatabse Instance
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            //On enregistre notre bdd avec localhost + port + création bdd avec Catalog en nom
            services.AddSingleton(sericeProvider =>
            {
                var config = sericeProvider.GetService<IConfiguration>();
                //On récupère le nom de la table Catalog

                var serviceSettigns = config.GetSection("ServiceSettings").Get<ServiceSettings>();
                var mongoDbSettings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettigns.ServiceName);
            });
            return services;
        }

        public static IServiceCollection AddMongoRepository(this IServiceCollection services, string collectionName)
        {
            //On vient recurperer le service bdd register juste avant car on veut en plus de lui dire dimplementer IRepo
            //que on lui passe le nom de la colelction (demande en param de L'impelemnt (MongoRepo)
            services.AddSingleton<IMongoRepositoryConversation>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepositoryConversation(database, collectionName);
            });
            return services;
        }
    }
}
