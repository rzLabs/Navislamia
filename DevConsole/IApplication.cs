using System.Threading.Tasks;

namespace DevConsole
{
    public interface IApplication
    {
        public async Task<int> Run() => 0;
    }
}