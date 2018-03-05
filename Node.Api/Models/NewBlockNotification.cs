namespace Node.Api.Models
{
    public class NewBlockNotification
    {
        public int Index { get; set; }

        public int CumulativeDifficulty { get; set; }

        public string PeerUrl { get; set; }
    }
}
