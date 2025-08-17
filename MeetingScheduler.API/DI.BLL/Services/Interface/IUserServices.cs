using DI.Contracts.Models.Domain;
using DI.Contracts.Models.DTO;

namespace DI.BLL.Services.Interface
{
    public interface IUserServices
    {
        string CreateUser(UserDTO user);
        List<Meeting?> GetUserMeetings(int userId);
    }
}
