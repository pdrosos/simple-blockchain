namespace Node.Api.Models
{
    public class TransactionSubmissionResponse
    {
        public string TransactionHash { get; set; }

        public int? StatusCode { get; set; }

        public string Message { get; set; }
    }
}
