using DI.Contracts.Models.Domain;

namespace DI.DAL.Repository.Interface
{
    public interface IUserRepository
    {
        string? CreateUser(User user);
        List<Meeting?> GetMeetings(int userId);
        List<User> GetAllUsers();
    }
}
        