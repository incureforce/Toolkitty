using System.Reflection;
using System.Threading.Tasks;

namespace System.Reflection.Emit
{
    public interface IProxyInterceptor
    {
        Task InvokeAsync(MethodInfo methodInfo, object[] methodArguments);
    }
}
