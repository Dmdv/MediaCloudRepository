using System;
using System.Globalization;
using DataAccess.Extensions;

namespace DataAccess.Helpers
{
	/// <summary>
	/// Сериализация DateTime.
	/// </summary>
	public abstract class DateTimeSerializer
	{
		public static string Serialize(DateTime dt)
		{
			return dt.ToString(DateFormat);
		}

		public static DateTime Deserialize(string s)
		{
			try
			{
				return DateTime.ParseExact(s, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
			}
			catch (FormatException e)
			{
				throw new FormatException("Cannot deserialize datetime value \"{0}\"".FormatString(s), e);
			}
		}

		private const string DateFormat = "yyyyMMddHHmmss";
	}
}