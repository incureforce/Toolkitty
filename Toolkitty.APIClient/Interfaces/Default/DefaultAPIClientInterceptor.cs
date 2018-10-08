using System.IO;
using System.Net.Http;
using System.Reflection;

namespace ToolKitty
{
    public class DefaultAPIClientInterceptor : IAPIClientInterceptor
    {
        public void Prepare(HttpRequestMessage request, APIEnvironment environment, IParameterInfo body)
        {
            if (body == null) {
                return;
            }

            if (body.Data is Stream stream) {
                request.Content = new StreamContent(stream);
            }
            else {
                var result = environment.ContentNegotiator
                    .Negotiate(body.Type, request, environment.MediaTypeFormatters);

                var writer = result.Formatter;

                request.Content = new ObjectContent(body.Type, body.Data, writer);
            }
        }
    }
}
