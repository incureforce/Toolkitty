using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace ToolKitty
{

    public interface IAPIClientInterceptor
    {
        void Prepare(HttpRequestMessage request, APIEnvironment environment, IParameterInfo body);
    }
}
