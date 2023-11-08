using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningMissionLabs.DAL.Models
{
    public class Group
    {
        public int groupID { get; set; }
        public string groupName { get; set; } = null!;
        public int creatorID { get; set; }
    }
}
