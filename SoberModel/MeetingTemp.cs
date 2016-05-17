//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoberModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class MeetingTemp
    {
        public long Id { get; set; }
        public Nullable<long> IdMeeting { get; set; }
        public string Name { get; set; }
        public Nullable<short> DayId { get; set; }
        public Nullable<System.DateTime> MeetingTime { get; set; }
        public Nullable<bool> ClosedToPublic { get; set; }
        public Nullable<bool> WheelChair { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Nullable<decimal> Zip { get; set; }
        public string Place { get; set; }
        public string MeetingFormat { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public short Status { get; set; }
        public string Reason { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserAssociated { get; set; }
        public string IP { get; set; }
    }
}
