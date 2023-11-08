using System.ComponentModel.DataAnnotations.Schema;

namespace LearningMissionLabs.DAL.Models
{
    [Table("ServerOptions")]
    public class ServerOption
    {
        public int Id { get; set; }
        public string RSAKey { get; set; } = null!;
    }
}
