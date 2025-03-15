namespace FinquixAPI.Models.AI
{
    public class Question
    {
        public int ID { get; set; }
        public string Category { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
    }
}
