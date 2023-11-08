using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LearningMissionLabs.BLL.Interfaces;
using LearningMissionLabs.BLL.Models;
using LearningMissionLabs.DAL;
using LearningMissionLabs.DAL.Models;

namespace LearningMissionLabs.BLL.Services
{
    public class PostService : IPostService
    {
        private IMapper _mapper;
        private readonly ILearningMissionContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostService(ILearningMissionContext context, IWebHostEnvironment webHostEnvironment)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, PostModel>();
                cfg.CreateMap<PostModel, Post>();
            });
            _mapper = config.CreateMapper();
            _dbContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<PostModel>> GetAllPosts()
        {
            var posts = await _dbContext.Posts.ToListAsync();
            if (posts == null || posts.Count == 0)
            {
                return null;
            }
            List<PostModel> postModels = new List<PostModel>();
            foreach (var post in posts)
            {
                var postModel = _mapper.Map<Post, PostModel>(post);
                if (post.ImagePath != null)
                {
                    byte[] image = await readImage(post.ImagePath);
                    postModel.ImageBytes = image;
                }
                postModels.Add(postModel);
            }
            return postModels;
        }

        public async Task<PostModel?> GetPostById(int id)
        {
            var post = await _dbContext.Posts.Where(post => post.Id == id).FirstOrDefaultAsync();
            if (post == null)
            {
                return null;
            }
            return _mapper.Map<Post, PostModel>(post);
        }

        public async Task<bool> AddPost(PostModel postModel)
        {
            string file_name = postModel.Image.FileName;
            var post = _mapper.Map<PostModel, Post>(postModel);
            post.ImagePath = $"\\Images\\{file_name}";
            await _dbContext.Posts.AddAsync(post);
            int result = await _dbContext.SaveChangesAsync();
            if (result != 1)
            {
                return false;
            }
            return await createImage(postModel.Image);
        }
       
        private async Task<bool> createImage(IFormFile image)
        {
            try
            {
                string imagePath = _webHostEnvironment.WebRootPath + "\\Images\\";
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }
                using (FileStream fileStream = File.Create(imagePath + image.FileName))
                {
                    image.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        private async Task<byte[]?> readImage(string path)
        {
            var splited = path.Split('\\').Where(str => str != "").ToList();
            string imagePath = _webHostEnvironment.WebRootPath + "\\" + splited[0];
            try
            {
                if (!Directory.Exists(imagePath) || !File.Exists(imagePath + "\\" + splited[1]))
                {
                    return null;
                }
                byte[] image = File.ReadAllBytes(imagePath + "\\" + splited[1]);
                return image;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"READ ERROR: {ex.Message}");
                return null;
            }
        }
    }
}
