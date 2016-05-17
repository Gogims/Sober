using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoberModel
{
    public abstract class AbstractMeeting
    {
        public string name { set; get; }
        public string city { set; get; }
        public string state { set; get; }
        public string address { set; get; }
        public int? day { set; get; }
        public int? timeStart24Hour { set; get; }
        public int? timeEnd24Hour { set; get; }
        public int? miles { set; get; }
        /// <summary>
        /// 1=NA, 2=AA
        /// </summary>
        public short type { get { return _type; } }
        public int? zipcode { set; get; }
        public string IP { set; get; }
        public bool? ClosedToPublic { set; get; }
        public short[] FormatMeeting { get; set; }
        public string statename { get; set; }

        protected short _type;

        public AbstractMeeting() { }

        protected AbstractMeeting(string _name, string _city, string _state, int? _day, int? _timeStart24Hour, int? _timeEnd24Hour, int? _zipcode, int? _milesWithin, bool? _ClosedToPublic, short[] _idFormat, string _statename)
        {
            name = name;
            city = _city;
            state = _state;
            day = _day;
            timeStart24Hour = _timeStart24Hour;
            timeEnd24Hour = _timeEnd24Hour;
            zipcode = _zipcode;
            miles = _milesWithin;
            FormatMeeting = _idFormat;
            ClosedToPublic = _ClosedToPublic;
            statename = _statename;
        }

        
        /// <summary>
        /// Gets the id of the meeting type
        /// </summary>
        /// <param name="meetingName">Type of meeting</param>
        /// <returns>1=NA, 2=AA</returns>
        public static short GetType(string meetingName) 
        {
            short? result = 0;

            using (DBManager context = new DBManager())
            {
                result = (from t in context.MeetingTypes
                          where t.TypeName.ToLower() == meetingName.ToLower()
                          select t.TypeId).FirstOrDefault();

                if (result != null)
                {
                    return result.Value;
                }
            }

            return 0;
        }
    }
}
