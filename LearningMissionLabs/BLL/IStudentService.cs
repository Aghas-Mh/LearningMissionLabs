using System.Threading.Tasks;

namespace LearningMissionLabs.BLL
{
    public interface IStudentService
    {
        Task<bool> AddStudent(string email, string name);
    }
}