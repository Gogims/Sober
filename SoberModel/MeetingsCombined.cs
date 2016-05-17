using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoberModel
{
    public abstract class MeetingsCombined
    {
        public List<CitiesStates> States { get; set; }
        public List<CityNear> citiesNear { get; set; }
        public List<usp_GetMeetings_Result> MeetingsResultList { get; set; }

        public MeetingsCombined()
        {
            States = new List<CitiesStates>();
            citiesNear = new List<CityNear>();
            MeetingsResultList = new List<usp_GetMeetings_Result>();
        }

        /// <summary>
        /// Gets all the days of the week
        /// </summary>
        /// <returns>A dictionary with all the days and the value it holds in the DB</returns>
        public Dictionary<int, string> GetDays()
        {
            Dictionary<int, string> days = new Dictionary<int, string>();
            days.Add(1, "Monday");
            days.Add(2, "Tuesday");
            days.Add(3, "Wednesday");
            days.Add(4, "Thursday");
            days.Add(5, "Friday");
            days.Add(6, "Saturday");
            days.Add(7, "Sunday");

            return days;
        }

        /// <summary>
        /// Gets all the hours of the day
        /// </summary>
        /// <returns>A dictionary with all the hours of the day in 12 hour format</returns>
        public Dictionary<int, string> GetTimes()
        {
            Dictionary<int, string> timeList = new Dictionary<int, string>();

            timeList.Add(0, "12am");
            for (int i = 1; i < 24; i++)
            {
                if (i > 12)
                {
                    timeList.Add(i, (i - 12).ToString() + "pm");
                }
                else
                {
                    if (i == 12)
                    {
                        timeList.Add(i, i.ToString() + "pm");
                    }
                    else
                    {
                        timeList.Add(i, i.ToString() + "am");
                    }
                }
            }

            return timeList;
        }

        /// <summary>
        /// Get 4 different distances to do the search
        /// </summary>
        /// <returns>A dictionary with 4 different distances (in miles)</returns>
        public Dictionary<int, string> GetDistances()
        {
            Dictionary<int, string> distanceList = new Dictionary<int, string>();
            distanceList.Add(5, "5 Miles");
            distanceList.Add(15, "15 Miles");
            distanceList.Add(25, "25 Miles");
            distanceList.Add(50, "50 Miles");

            return distanceList;
        }

        /// <summary>
        /// Gets all if it's an opened/closed meeting
        /// </summary>
        /// <returns>Dictionary with the value of OpenClosed</returns>
        public Dictionary<bool, string> GetMeetingStyles()
        {
            Dictionary<bool, string> openclosedlist = new Dictionary<bool, string>();
            openclosedlist.Add(false, "Yes, all are welcome");
            openclosedlist.Add(true, "Closed meeting");

            return openclosedlist;
        }

        /// <summary>
        /// Gets all the formats given the type and if it's allowed multiple formats
        /// </summary>
        /// <param name="allowMultiple">true= belongs in the "other criteria" dropdown, false = belongs in the "format" dropdown</param>
        /// <param name="type">1=NA, 2=AA</param>
        /// <returns>Dictionary with the values of the format and it's description</returns>
        
        public Dictionary<short, string> GetFormats(bool allowMultiple, short type)
        {
            var context = new SoberMVCEntities();
            return (from f in context.MeetingFormatTypes
                    where f.AllowMultiple == allowMultiple && f.MeetingType == type
                    select new { f.FormatTypeId, f.FormatTypeFullName }
                    )
                    .OrderBy(x => x.FormatTypeFullName)
                    .ToDictionary(x => x.FormatTypeId, x => x.FormatTypeFullName);
        }
    }
}
