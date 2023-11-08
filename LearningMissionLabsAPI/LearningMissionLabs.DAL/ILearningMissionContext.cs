using Microsoft.EntityFrameworkCore;
using LearningMissionLabs.DAL.Models;

namespace LearningMissionLabs.DAL
{
    public interface ILearningMissionContext
    {
        DbSet<Post> Posts { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<Group> Groups { get; set; }
        DbSet<UserInGroup> usersInGroup { get; set; }
        void SaveChanges();
        Task<int> SaveChangesAsync();
        Task<string> GetRSAKey();
        Task<bool>CreateRSA(string privateKey);

    }
}
