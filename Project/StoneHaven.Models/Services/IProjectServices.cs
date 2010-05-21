namespace StoneHaven.Models.Services
{
    public interface IProjectServices
    {
        void SaveProject(Project project);
        Project FindProject(int id);
    }
}