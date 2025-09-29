using System;

namespace VTuber.Core.UI
{
    public static class VStringUtils
    {
        public static string GetTime(TimeSpan time)
        {
            var hour = time.Hours > 9 ? time.Hours.ToString() : "0" + time.Hours;
            var minute = time.Minutes > 9 ? time.Minutes.ToString() : "0" + time.Minutes;
            var second = time.Seconds > 9 ? time.Seconds.ToString() : "0" + time.Seconds;
            
            return $"{hour}:{minute}:{second}";
        }
    }
}