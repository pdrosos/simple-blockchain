namespace Node.Api.Models
{
    public class NewBlockNotification
    {
        public Block Block { get; set; }
        public Peer Sender { get; set; }
    }
}
