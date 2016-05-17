using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoberModel
{
    public class AAMeeting : AbstractMeeting
    {
        public AAMeeting() 
        {
            _type = GetType("Alcoholic Anonymous");
        }

        public AAMeeting(string _name, string _city, string _state, int? _day, int? _timeStart24Hour, int? _timeEnd24Hour, int? _zipcode, int? _milesWithin, bool? _ClosedToPublic, short[] _idFormat, string _statename)
            :base( _name,  _city,  _state,  _day,  _timeStart24Hour,  _timeEnd24Hour,  _zipcode,  _milesWithin,  _ClosedToPublic,  _idFormat, _statename)
        {
            _type = GetType("Alcoholic Anonymous");
        }
    }

    public class AAMeetingsCombined : MeetingsCombined
    {
        public AAMeeting MyAAMeeting { get; set; }

        public AAMeetingsCombined()
        {
            MyAAMeeting = new AAMeeting();
        }

    }
}
