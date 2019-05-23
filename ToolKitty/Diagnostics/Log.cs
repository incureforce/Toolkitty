using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolKitty.Diagnostics
{
    public class Log : Formatters, IFormattable
    {
        public static event EventHandler Creation;

        public static Log Create(LogLevel level, string message, IDictionary<string, object> dictionary)
        {
            if (dictionary == null) {
                throw new ArgumentNullException(nameof(dictionary));
            }

            var self = new Log(level, message, dictionary);

            if (Creation != null) {
                Creation(self, EventArgs.Empty);
            }

            return self;
        }

        public Log() { }

        public Log(LogLevel level, string message)
        {
            var created = DateTime.UtcNow;

            this[nameof(Message)] = message;
            this[nameof(Created)] = created;
            this[nameof(Level)] = level;
        }

        public Log(LogLevel level, string message, IDictionary<string, object> dictionary) : base(dictionary)
        {
            if (dictionary == null) {
                throw new ArgumentNullException(nameof(dictionary));
            }

            var created = DateTime.UtcNow;

            this[nameof(Message)] = message;
            this[nameof(Created)] = created;
            this[nameof(Level)] = level;
        }

        public LogLevel Level {
            get => (LogLevel)this[nameof(Level)];
        }

        public string Message {
            get => (string)this[nameof(Message)];
        }

        public DateTime Created {
            get => (DateTime)this[nameof(Created)];
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) {
                return ToString();
            }

            return format.Format(this, formatProvider);
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
