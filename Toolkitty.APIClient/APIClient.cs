using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToolKitty
{
    public class APIClient<T> : HttpClient
    {
        static readonly Regex
            ParameterRegex = new Regex(@"/:([A-Z0-9_]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static IAPIClientInterceptor DefaultInterceptor
        {
            get;
            set;
        } = new DefaultAPIClientInterceptor();

        /// <summary>Initializes a new instance of the <see cref="APIClientAttribute" /> class.</summary>
        public APIClient(APIEnvironment environment)
        {
            if (environment == null) {
                throw new ArgumentNullException(nameof(environment));
            }

            Environment = environment;
        }

        /// <summary>Initializes a new instance of the <see cref="APIClientAttribute" /> class with a specific handler.</summary>
        /// <param name="handler">The HTTP handler stack to use for sending requests. </param>
        public APIClient(APIEnvironment environment, HttpMessageHandler handler) : base(handler)
        {
            if (environment == null) {
                throw new ArgumentNullException(nameof(environment));
            }

            Environment = environment;
        }

        /// <summary>Initializes a new instance of the <see cref="APIClientAttribute" /> class with a specific handler.</summary>
        /// <param name="handler">The <see cref="System.Net.Http.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
        /// <param name="disposeHandler">
        /// <see langword="true" /> if the inner handler should be disposed of by Dispose(), <see langword="false" /> if you intend to reuse the inner handler.</param>
        public APIClient(APIEnvironment environment, HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
            if (environment == null) {
                throw new ArgumentNullException(nameof(environment));
            }

            Environment = environment;
        }

        public APIEnvironment Environment { get; }

        public IAPIClientInterceptor Interceptor
        {
            get;
            set;
        } = DefaultInterceptor;

        public async Task<TResult> SendAsync<TResult>(Expression<Func<T, TResult>> func, IAPIClientInterceptor interceptor = null)
        {
            if (func == null) {
                throw new ArgumentNullException(nameof(func));
            }

            if (interceptor == null) {
                interceptor = Interceptor;
            }

            var lambda = Lambda.Compose(func);

            var msg = await SendLambdaAsync(lambda, interceptor);

            msg.EnsureSuccessStatusCode();

            if (msg as object is TResult result) {
                return result;
            }

            return await msg.ToObjectAsync<TResult>(Environment.MediaTypeFormatters);
        }

        private Task<HttpResponseMessage> SendLambdaAsync(Lambda lambda, IAPIClientInterceptor interceptor)
        {
            if (lambda == null) {
                throw new ArgumentNullException(nameof(lambda));
            }

            if (interceptor == null) {
                throw new ArgumentNullException(nameof(interceptor));
            }

            var methodInfo = lambda.MethodInfo;
            var parameters = lambda.CreateParameters(APIClientParameterInfo.Make);

            if (!(Attribute.GetCustomAttribute(methodInfo, typeof(APIClientAttribute)) is APIClientAttribute methodAttribute)) {
                throw new NotSupportedException($"APICall attribute is missing from Method signature");
            }

            var url = methodAttribute.URL;
            var method = new HttpMethod(methodAttribute.Method);
            URLBuilder urlBuilder = new URLBuilder();

            var ea = new URLBuilder(url);
            if (string.IsNullOrEmpty(ea.Host) && string.IsNullOrEmpty(ea.Scheme)) {
                if (BaseAddress != null && !string.IsNullOrEmpty(BaseAddress.ToString())) { 
                    urlBuilder.Parse(BaseAddress.ToString());
                    urlBuilder.AddPath(url);
                }
            }

            if(string.IsNullOrEmpty(urlBuilder.Host) && string.IsNullOrEmpty(urlBuilder.Scheme)) {
                throw new UriFormatException("No Host or Scheme given in method Url or in BaseAddress of HttpClient");
            }

            var builder = new StringBuilder(urlBuilder.ToString());

            BindParameters(parameters, builder);
            BindQuery(parameters, builder);

            var request = new HttpRequestMessage() {
                RequestUri = new Uri(builder.ToString(), default(UriKind)),
                Method = method,
            };

            var body = parameters.SingleOrDefault(x => x.IsHandled);

            interceptor.Prepare(request, Environment, body);

            return SendAsync(request);
        }

        private static bool BindQuery(APIClientParameterInfo[] parameters, StringBuilder builder)
        {
            var url = builder.ToString();
            var query = url.IndexOf('?') < 0;

            foreach (var argument in parameters) {
                if (argument.IsOptional) {
                    argument.IsHandled = false;

                    if (query) {
                        query = false;

                        builder.Append('?');
                    }
                    else {
                        builder.Append('&');
                    }

                    builder.Append(argument.Name);

                    if (argument.Data == null) {
                        continue;
                    }

                    var text = Uri.EscapeDataString(ObjectFunctions.ToString(argument.Data));

                    builder.Append("=");
                    builder.Append(text);
                }
            }

            return query;
        }

        private void BindParameters(APIClientParameterInfo[] parameters, StringBuilder builder)
        {
            var url = builder.ToString();
            var offset = 0;
            var matches = ParameterRegex.Matches(url);
            var comparer = Environment.Comparer;

            foreach (Match match in matches) {
                var done = false;
                var group = match.Groups[1];

                foreach (var argument in parameters) {
                    if (comparer.Equals(group.ToString(), argument.Name)) {
                        done = true;

                        offset = BindParameter(argument, builder, match, offset);

                        break;
                    }

                    if (Environment.TryGetValue(group.ToString(), out var value)) {
                        done = true;

                        offset = BindParameter(argument, builder, match, offset);

                        break;
                    }
                }

                if (done == false) {
                    throw new NotSupportedException($"No replacement for key '{group}' found");
                }
            }
        }

        private static int BindParameter(APIClientParameterInfo argument, StringBuilder builder, Match match, int offset)
        {
            argument.IsHandled = false;

            var text = argument.Data != null
                ? Uri.EscapeUriString(ObjectFunctions.ToString(argument.Data))
                : string.Empty;

            builder.Remove(offset + match.Index + 1, match.Length - 1);
            builder.Insert(offset + match.Index + 1, text);

            return offset += (text.Length - match.Length);
        }
    }
}
