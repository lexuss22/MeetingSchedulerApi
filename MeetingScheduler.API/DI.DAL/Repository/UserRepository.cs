using DI.Contracts.Models.Domain;
using DI.DAL.Context;
using DI.DAL.Repository.Interface;

namespace DI.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppData _appData;

        public UserRepository(AppData appData)
        {
            _appData = appData;
        }
        public string? CreateUser(User user)
        {
            AddUser(user);
            return "Saved successfully!";
        }

        public List<User> GetAllUsers()
        {
            var user = _appData.Users;
            if (user.Count == 0)
            {
                return null;
            }
            return user;
        }

        public List<Meeting?> GetMeetings(int userId)
        {
            var meetings = _appData.Meetings
                .Where(m => m.Participants.Any(p => p == userId))
                .ToList();
            if (meetings.Count == 0)
            {
                return null;
            }
                return meetings;
        }

        private void AddUser(User user)
        {
            var newUser = new User
            {
                Id = _appData.Users.Count +1,
                Name = user.Name
            };
            _appData.Users.Add(newUser);

        }
    }
}
