namespace LearningMissionLabsAPI.Requests
{
    public class CreateGroupRequest
    {
        public string groupName { get; set; } = null!;
        public List<string> users { get; set; } = null!;
    }
}
