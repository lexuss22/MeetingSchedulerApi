using DI.Contracts.Models.Domain;
using System.Reflection.Metadata;

namespace DI.DAL.Context
{
    public class AppData
    {
        public List<User> Users { get; set; } = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "Alice"
            },
            new User
            {
                Id = 2,
                Name = "Bob"
            }
        };
        public List<Meeting> Meetings { get; set; } = new List<Meeting>();

    }
}
