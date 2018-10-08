using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;

namespace ToolKitty
{
    public class APIEnvironment : Dictionary<string, object>
    {
        static StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

        public APIEnvironment() : base(StringComparer)
        {
        }

        public APIEnvironment(StringComparer stringComparer) : base(stringComparer)
        {

        }

        public IContentNegotiator ContentNegotiator
        {
            get;
            set;
        } = new DefaultContentNegotiator();

        public MediaTypeFormatterCollection MediaTypeFormatters
        {
            get;
            set;
        } = new MediaTypeFormatterCollection();
    }
}
