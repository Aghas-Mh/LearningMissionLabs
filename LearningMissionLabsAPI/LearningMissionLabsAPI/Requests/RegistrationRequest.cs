namespace LearningMissionLabsAPI.Requests
{
    public class RegistrationRequest
    {
        public string Email { get; set; } = null !;
        public string Name { get; set; } = null !;
        public string Password { get; set; } = null !;
        public string Confirm { get; set; } = null !;
    }
}