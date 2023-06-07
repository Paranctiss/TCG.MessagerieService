namespace TCG.MessagerieService.Application.Conversations.Dto
{
    public class ConversationContexteDtoRequest
    {
        public int IdUser1 { get; set; }
        public int IdUser2 { get; set; }
        public string IdMerchPost { get; set; } = ""; 
    }
}
