using LearningMissionLabs.BLL.Models;

namespace LearningMissionLabs.BLL.Interfaces
{
    public interface IPostService
    {
        Task<List<PostModel>> GetAllPosts();
        Task<PostModel?> GetPostById(int id);
        Task<bool> AddPost(PostModel post);
    }
}
