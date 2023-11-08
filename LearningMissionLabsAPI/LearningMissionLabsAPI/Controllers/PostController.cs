using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using LearningMissionLabs.BLL.Interfaces;
using LearningMissionLabs.BLL.Models;
using LearningMissionLabsAPI.Requests;
using LearningMissionLabsAPI.Responses;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearningMissionLabsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private IMapper _mapper;
        private readonly ILogger<PostController> _logger;
        private readonly IPostService _service;


        public PostController(ILogger<PostController> logger, IPostService service)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PostModel, PostResponse>();
                cfg.CreateMap<PostRequest, PostModel>();
            });
            _mapper = config.CreateMapper();
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostResponse>>> Get()
        {
            var postModels = await _service.GetAllPosts();
            if (postModels == null)
            {
                return BadRequest("No data");
            }

            List<PostResponse> postResponses = new List<PostResponse>();
            foreach (var post in postModels)
            {
                var postResponse = _mapper.Map<PostModel, PostResponse>(post);
                postResponse.ImageContent = File(post.ImageBytes, "image/png");
                postResponses.Add(postResponse);
            }
            return postResponses;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostResponse?>> Get(int id)
        {
            var postModel = await _service.GetPostById(id);
            if (postModel == null)
            {
                return BadRequest($"User with {id} Id not found!");
            }
            return _mapper.Map<PostModel, PostResponse>(postModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm] PostRequest postRequest)
        {
            var postModel = _mapper.Map<PostRequest, PostModel>(postRequest);
            await _service.AddPost(postModel);
            return Ok(postModel);
        }

        // PUT api/<PostController>/5
        [HttpPut("{id}")]
        private void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        private void Delete(int id)
        {
        }
    }
}
