using System.Threading.Tasks;

namespace rpaapp.Hubs
{
    public interface IHubClient
    {
        Task InformClient(string message);
    }
}