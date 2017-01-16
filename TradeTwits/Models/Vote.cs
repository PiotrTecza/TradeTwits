namespace TradeTwits.Models
{
    public class Vote
    {
        public string TwitId { get; set; }
        public string CommentId { get; set; }
        public bool Up { get; set; }
    }
}