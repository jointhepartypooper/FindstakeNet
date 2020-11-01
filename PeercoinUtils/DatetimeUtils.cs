using System;

namespace PeercoinUtils
{
	public class DatetimeUtils
	{
		public DatetimeUtils()
		{
		}
		
		/// <summary>
		/// Y2K issues at Tuesday, January 19, 2038 4:14:07 AM GMT+01:00
		/// </summary>
		/// <param name="time"></param>
		/// <returns>Unix Timestamp in seconds since epoch</returns>
		public static int ToUnix(DateTime time)
		{			
			return (Int32)(time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}
		
				
		/// <summary>
		/// Y2K issues at Tuesday, January 19, 2038 4:14:07 AM GMT+01:00
		/// </summary>
		/// <param name="seconds">seconds since epoch</param>
		/// <returns></returns>
		public static DateTime FromUnix(int seconds)
		{
		    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
		    return epoch.AddSeconds((double)seconds);
		}
	}
}
