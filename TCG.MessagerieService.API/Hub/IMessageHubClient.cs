using TCG.MessagerieService.Domain;

namespace TCG.MessagerieService.API.Hub
{
    public interface IMessageHubClient
    {
        Task SendToAllUsers(string message);

        Task CreatePrivateConversation(int idSender, int idReceiver);

        Task UpdateMessageInConversation(int idSender, int idReceiver, string idMerchPost, string message);

        Task SendMessageInConversation(int idSender, int idReceiver, string idMerchPost, string message);
    }
}
