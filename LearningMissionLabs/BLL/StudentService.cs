using LearningMissionLabs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LearningMissionLabs.BLL
{
    public class StudentService : IStudentService
    {
        public async Task<bool> AddStudent(string email, string name)
        {
            try
            {
                using (var contex = new LearningMissionContext())
                {
                   await contex.Database.ExecuteSqlRawAsync("insertStudent @p0, @p1", parameters: new[] { email, name });
                }
            }
            catch(Exception ex)
            {

                return false;
            }
            return true;
        }
    }
}
