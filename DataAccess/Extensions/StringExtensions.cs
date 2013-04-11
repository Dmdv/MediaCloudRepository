using System;
using System.Globalization;

namespace DataAccess.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Case insensitive.
		/// </summary>
		/// <param name = "left"></param>
		/// <param name = "right"></param>
		/// <returns></returns>
		public static bool OrdinalEqualsCi(this string left, string right)
		{
			return left.OrdinalEquals(right, true);
		}

		/// <summary>
		/// 	case sensitive
		/// </summary>
		/// <param name = "left"></param>
		/// <param name = "right"></param>
		/// <returns></returns>
		public static bool OrdinalEqualsCs(this string left, string right)
		{
			return left.OrdinalEquals(right, false);
		}

		public static bool OrdinalEquals(this string left, string right, bool ignoreCase)
		{
			if (ReferenceEquals(left, right))
			{
				return true;
			}

			if ((left == null) || (right == null))
			{
				return false;
			}

			var comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			return left.Equals(right, comparisonType);
		}

		public static string FormatString(this string format, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, format, values);
		}
	}
}