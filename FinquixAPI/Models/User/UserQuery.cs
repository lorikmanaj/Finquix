using FinquixAPI.Models.AI;

namespace FinquixAPI.Models.User
{
    public class UserQuery
    {
        public int UserId { get; set; }
        public Question Question { get; set; }
    }
}
