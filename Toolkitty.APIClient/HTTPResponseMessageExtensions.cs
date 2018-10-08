using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace ToolKitty
{
    public static class HTTPResponseMessageExtensions
    {
        public static async Task<TResult> ToObjectAsync<TResult>(this HttpResponseMessage message, MediaTypeFormatterCollection mediaTypeFormatters)
        {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }

            if (mediaTypeFormatters == null) {
                throw new ArgumentNullException(nameof(mediaTypeFormatters));
            }

            var content = message.Content;
            var contentHeaders = content.Headers;

            var reader = mediaTypeFormatters.FindReader(typeof(TResult), contentHeaders.ContentType);

            using (var stream = await content.ReadAsStreamAsync()) {
                var obj = await reader.ReadFromStreamAsync(typeof(TResult), stream, content, null);
                if (obj is TResult objectResult) {
                    return objectResult;
                }

                throw new NotSupportedException($"Got '{obj}' expected '{typeof(TResult)}'");
            }
        }
    }
}
