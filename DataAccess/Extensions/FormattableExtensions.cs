using System;
using System.Globalization;

namespace DataAccess.Extensions
{
	public static class FormattableExtensions
	{
		public static string ToStringWithInvariantCulture(this IFormattable value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		public static string ToStringWithInvariantCulture(this IFormattable value, string format)
		{
			return value.ToString(format, CultureInfo.InvariantCulture);
		}
	}
}