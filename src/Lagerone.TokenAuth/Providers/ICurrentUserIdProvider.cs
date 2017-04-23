using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Lagerone.TokenAuth.Providers
{
    public interface ICurrentUserIdProvider
    {
        Task<string> Get(HttpRequestHeaders httpRequestHeaders);
    }
}