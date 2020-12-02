using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HMSD.EncryptionService.Tests")]
namespace HMSD.EncryptionService.Utils
{

    internal static class KeyUtils
    {
        internal static int GetTimeKey()
        {
            int current_time = DateTimeToTimeKey(DateTime.UtcNow);
            int last_digits = current_time % 100;

            if (IsFibonacci(last_digits))
                return current_time;


            if (last_digits > 89)
                return (current_time - last_digits) + 89;

            var i = last_digits;
            do
            {
                i--;
               
            } while (!IsFibonacci(i));
            
            return (current_time - last_digits) + i;

        }

        internal static bool IsPerfectSquare(int x)
        {
            int s = (int)Math.Sqrt(x);
            return (s * s == x);
        }

        internal static bool IsFibonacci(int n)
        {
            return IsPerfectSquare(5 * n * n + 4) ||
              IsPerfectSquare(5 * n * n - 4);
        }

        internal static DateTime TimeKeyToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMinutes(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        internal static int DateTimeToTimeKey(DateTime datetime)
        {
            return (int)(datetime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
