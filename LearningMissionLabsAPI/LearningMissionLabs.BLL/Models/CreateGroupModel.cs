using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningMissionLabs.BLL.Models
{
    public class CreateGroupModel
    {
        public int creatorID { get; set; }
        public string groupName { get; set; } = null!;
        public List<string> users { get; set; } = null!;
    }
}
