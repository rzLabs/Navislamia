using System.Threading.Tasks;

namespace Navislamia.Game
{
    public interface IGameModule
    {
        Task Start(string ip, int port, int backlog);
    }
}
