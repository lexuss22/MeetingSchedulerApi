using DI.BLL.Services.Interface;
using DI.Contracts.Models.Domain;
using DI.Contracts.Models.DTO;
using DI.DAL.Repository.Interface;

namespace DI.BLL.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;

        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public string CreateUser(UserDTO user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                return "Name empty or null";
            }

            var userDomain = Mapping(user);

            var result = _userRepository.CreateUser(userDomain);

            return result;

        }

        public List<Meeting?> GetUserMeetings(int userId)
        {
            if (userId < 0)
            {
                return null; 
            }
            var meetings = _userRepository.GetMeetings(userId);
            return meetings;
        }

        private User Mapping(UserDTO user)
        {
            return new User
            {
                Name = user.Name,
            };
        }
    }
}
