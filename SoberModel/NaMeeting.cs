using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace SoberModel
{
    public class NaMeeting: AbstractMeeting
    {
        public NaMeeting() 
        {
            _type = GetType("Narcotics Anonymous");
        }

        public NaMeeting(string _name, string _city, string _state, int? _day, int? _timeStart24Hour, int? _timeEnd24Hour, int? _zipcode, int? _milesWithin, bool? _ClosedToPublic, short[] _idFormat, string statename)
            :base( _name,  _city,  _state,  _day,  _timeStart24Hour,  _timeEnd24Hour,  _zipcode,  _milesWithin,  _ClosedToPublic,  _idFormat, statename)
        {
            _type = GetType("Narcotics Anonymous");
        }
        
        public List<FreeListing> GetNearbyFreeListings()
        {
            List<FreeListing> freeListingsList = new List<FreeListing>();
            int distance = 200;
            int nofListingsNeeded = 10;
            SoberMVCEntities context = new SoberModel.SoberMVCEntities();
            Nullable<double> z = (from zip in context.zipcodes
                                  where zip.City.ToLower().Trim() == city.ToLower().Trim()
                                  select zip.ZipCode1).FirstOrDefault();

            if(z.HasValue)
            {
                int zipc = (int)z;
                var resultsList = context.usp_GetSortedSamhsaListings(state.Trim(), zipc, distance);
                IEnumerable<usp_GetSortedSamhsaListings_Result> sortedResults = (IEnumerable<usp_GetSortedSamhsaListings_Result>)resultsList;

                List<usp_GetSortedSamhsaListings_Result> freeListings = null;
                freeListings = sortedResults.Take(nofListingsNeeded).ToList();


                foreach (var item in freeListings)
                {

                    FreeListing freeListing = new FreeListing();
                    freeListing.PageName = item.PageName == null ? string.Empty : item.PageName;
                    freeListing.name1 = item.name1 == null ? string.Empty : item.name1;
                    freeListing.name2 = item.name2 == null ? string.Empty : item.name2;
                    freeListing.location_street1 = item.location_street1 == null ? string.Empty : item.location_street1;
                    freeListing.location_street2 = item.location_street2 == null ? string.Empty : item.location_street2;
                    freeListing.location_city = item.location_city == null ? string.Empty : item.location_city;
                    freeListing.location_state = item.location_state == null ? string.Empty : item.location_state;
                    freeListing.location_zip = item.location_zip == null ? "00000" : item.location_zip;
                    freeListing.location_zip4 = item.location_zip4 == null ? string.Empty : item.location_zip4;
                    freeListing.phone = item.phone == null ? string.Empty : item.phone;
                    freeListing.Primary_Focus = item.Primary_Focus == null ? string.Empty : item.Primary_Focus;
                    freeListing.Services_Provided = item.Services_Provided == null ? string.Empty : item.Services_Provided;
                    freeListing.Type_of_Care = item.Type_of_Care == null ? string.Empty : item.Type_of_Care;
                    freeListing.Forms_of_Payment_Accepted = item.Forms_of_Payment_Accepted == null ? string.Empty : item.Forms_of_Payment_Accepted;
                    freeListing.isEnhanced = item.EnhancedProfile.ToLower().Equals("y") ? true : false;
                    freeListing.surveyType = item.Survey;
                    freeListing.distance = item.distance;
                    freeListing.isCertifiedPartner = false;
                    freeListing.IsProfileInactive = item.InActive == 1 ? true : false;
                    if (item.CertifiedPartner != null && item.CertifiedPartner == true)
                    {
                        freeListing.isCertifiedPartner = true;
                    }
                    freeListingsList.Add(freeListing);
                }
            }

            return freeListingsList;
        }

        public bool VerifyInput(ref string message)
        {
            bool returnValue = true;
            if (name != null)
                name = name.Trim();
            if (city != null)
                city = city.Trim();
            if (state != null)
                state = state.Trim();

            if (!String.IsNullOrEmpty(name))
            {
                name = name.Trim();
                if (!Regex.Match(name, @"^[a-zA-Z0-9_,. ]*$").Success)
                {
                    message += Environment.NewLine + "Invalid name provided. Cannot contain non alphanumeric characters other than underscore space and comma";
                    returnValue = false;
                }
            }


            if (!String.IsNullOrEmpty(city))
            {
                city = city.Trim();
                if (!Regex.Match(city, @"^([a-zA-Z]+|[a-zA-Z]+\s[a-zA-Z]+)$").Success)
                {
                    message += Environment.NewLine + "Invalid City Provided";
                    returnValue = false;
                }
            }
            //Make sure states are within USA
            if (!String.IsNullOrEmpty(state))
            {
                state = state.Trim();
                state = state.ToUpper();
                if (!Regex.Match(state, @"^(?-i:A[LKSZRAEP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADLN]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD]|T[NX]|UT|V[AIT]|W[AIVY])$").Success)
                {
                    message += Environment.NewLine + "Invalid State Provided";
                    returnValue = false;
                }
            }


            if (timeStart24Hour < -1 || timeStart24Hour > 23)
            {
                timeStart24Hour = -1;
            }

            if (timeEnd24Hour < -1 || timeEnd24Hour > 23)
            {
                timeEnd24Hour = -1;
            }

            return returnValue;
        }
    }

    public class NaMeetingsCombined : MeetingsCombined
    {
        public NaMeeting MyNaMeeting { get; set; }
        

        public NaMeetingsCombined()
            : base()
        {
            MyNaMeeting = new NaMeeting();            
        }

        
    }
}
