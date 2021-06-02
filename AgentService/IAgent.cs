using System.Net.Http;
using System.Threading.Tasks;

namespace AgentService
{
    public interface IAgent
    {
        Task StartAsync();
        Task StopAsync();
    }
}