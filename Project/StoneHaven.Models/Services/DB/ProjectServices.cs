using Rhino.Commons;

namespace StoneHaven.Models.Services.DB
{
    public class ProjectServices : IProjectServices
    {
        public void SaveProject(Project project)
        {
            Repository<Project>.Save(project);
        }

        public Project FindProject(int id)
        {
            return Repository<Project>.Get(id);
        }
    }
}