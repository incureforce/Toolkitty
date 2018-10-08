using System;

namespace System.Text
{
    public class DateTimeFormatter : IFormattable
    {
        public const string Key = "DATETIME";

        public static DateTimeFormatter Default
        {
            get;
        } = new DateTimeFormatter();

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var utc = DateTime.Now;

            return utc.ToString(format, formatProvider);
        }

        public override string ToString()
        {
            return ToString(null, null);
        }
    }
}
