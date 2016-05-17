using System.Web.Mvc;
using SoberBLL;
using SoberBLL.Search;
using SoberModel;
using System.Collections.Generic;
using SoberBLL.Directory;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MvcApplication4.Controllers
{
    public partial class meetingsController : BaseController
    {
        [HttpGet]
        public ActionResult na(string place, string city, string state, int? day, int? timeStart24Hour, int? timeEnd24Hour, int? zipcode, int? distance, bool? ClosedToPublic, short[] format)
        {            
            NaMeetingsCombined naMeetingsCombined = new NaMeetingsCombined();

            if (format != null)
            {
                format = format.Where(x => x != 0).ToArray();
            }

            MeetingsBll naMeetingsBll = new MeetingsBll();
            string statename = "";

            if (zipcode.HasValue)
            {
                CitiesStates c = CitiesStates.GetCityState(zipcode.Value, 1);

                if (c != null)
                {
                    city = c.Cities[0].CityName.ToLower();
                    state = c.State.ToLower();
                    statename = c.StateFullName;
                }
            }            
            else if(!string.IsNullOrEmpty(city) && string.IsNullOrEmpty(state))
            {
                CitiesStates c = CitiesStates.GetState(city);

                if (c != null)
                {
                    state = c.State;
                    statename = c.StateFullName;
                }   
            }

            if (!string.IsNullOrWhiteSpace(state) && state.Trim().Length > 2)
            {
                statename = state;
                state = BLL_Listings.GetStateAbbrevationFromStateName(state);
            }                    

            naMeetingsCombined.MyNaMeeting = new NaMeeting(place, city, state, day, timeStart24Hour, timeEnd24Hour, zipcode, distance, ClosedToPublic, format, statename);
            string viewname = "";
            
            if (!string.IsNullOrEmpty(naMeetingsCombined.MyNaMeeting.city))
            {
                naMeetingsCombined.citiesNear = naMeetingsBll.GetCitiesNear(city, day, timeStart24Hour, timeEnd24Hour, 20, ClosedToPublic, format, naMeetingsCombined.MyNaMeeting.type);
                naMeetingsCombined.MeetingsResultList = naMeetingsBll.GetMeetings(naMeetingsCombined.MyNaMeeting);
                
                decimal? zip = naMeetingsCombined.MeetingsResultList.Where(x => x.Zip != null).Select(x => x.Zip).FirstOrDefault();
                viewname = "naCity";

                if (!zip.HasValue)
                    zip = 0;
                                
                ViewBag.TotalCity = naMeetingsBll.GetMeetingsByCity(naMeetingsCombined.MyNaMeeting.city, naMeetingsCombined.MyNaMeeting.state, naMeetingsCombined.MyNaMeeting.type);
                ViewBag.TotalByRange = naMeetingsBll.GetMeetingsByRange((int)zip.Value, 25, naMeetingsCombined.MyNaMeeting.type);
                ViewBag.StateName = BLL_Listings.GetStateNameFromAbbrevation(naMeetingsCombined.MyNaMeeting.state);

                using (DBManager context = new DBManager())
                {
                    ViewBag.RehabState = context.tbl_SamhsaListings.Where(x => x.location_state == naMeetingsCombined.MyNaMeeting.state).Count();
                    BLL_Listings bllListing = new SoberBLL.Directory.BLL_Listings();
                    string zip2; string city2; string StateAbbr; string StateAbbreviation; string StateName;
                    string address2 = naMeetingsCombined.MyNaMeeting.city + ", " + naMeetingsCombined.MyNaMeeting.state;

                    SearchHelper searchHelper = new SearchHelper();
                    searchHelper.GetAddress(address2, out  city2, out StateAbbr, out zip2, out StateAbbreviation, out StateName);
                    ViewBag.RehabCity = bllListing.GetFreeListingsCount("drug and alcohol treatment centers", naMeetingsCombined.MyNaMeeting.state, naMeetingsCombined.MyNaMeeting.city, zip2, 25, 0, null, null, "any");
                }
            }
            else if (!string.IsNullOrEmpty(state))
            {
                naMeetingsCombined.States.Add(naMeetingsBll.GetCitiesStates(state, day, timeStart24Hour, timeEnd24Hour, zipcode, ClosedToPublic, format, NaMeeting.GetType("Narcotics Anonymous")));
                viewname = "naState";
            }
            else
            {
                naMeetingsCombined.States = naMeetingsBll.GetStates();
                viewname = "na";
            }

            return View(viewname, naMeetingsCombined);
        }

        [HttpPost]
        public ActionResult na(NaMeetingsCombined naMeetingCombined)
        {
            MeetingsBll naMeetingsBll = new MeetingsBll();
            var context = new SoberMVCEntities();

            naMeetingCombined.MyNaMeeting.FormatMeeting = naMeetingCombined.MyNaMeeting.FormatMeeting.Where(x => x != 0).ToArray();

            if(!string.IsNullOrEmpty(naMeetingCombined.MyNaMeeting.name) && string.IsNullOrEmpty(naMeetingCombined.MyNaMeeting.address))
            {
                var address = (from m in context.Meetings
                               where m.Name.Contains(naMeetingCombined.MyNaMeeting.name)
                               select new { m.City, m.State }).FirstOrDefault();

                if(address != null)
                {
                    naMeetingCombined.MyNaMeeting.address = address.City + ", " + address.State;
                }
            }

            SearchHelper searchHelper = new SearchHelper();
            string city, StateAbbr, zip, StateAbbreviation, StateName;
            searchHelper.GetAddress(naMeetingCombined.MyNaMeeting.address, out  city, out StateAbbr, out zip, out StateAbbreviation, out StateName);
                        
            naMeetingCombined.MyNaMeeting.city = city;
            naMeetingCombined.MyNaMeeting.state = StateAbbr;
            naMeetingCombined.MyNaMeeting.statename = StateName;

            if (!string.IsNullOrWhiteSpace(zip))
            {
                int ZipInt = 0;
                bool success = int.TryParse(zip, out ZipInt);
                if (success)
                {
                    naMeetingCombined.MyNaMeeting.zipcode = ZipInt;
                }
            }

            string viewname = "";
            
            if (!string.IsNullOrEmpty(naMeetingCombined.MyNaMeeting.city))
            {
                naMeetingCombined.citiesNear = naMeetingsBll.GetCitiesNear(city, naMeetingCombined.MyNaMeeting.day, naMeetingCombined.MyNaMeeting.timeStart24Hour, naMeetingCombined.MyNaMeeting.timeEnd24Hour,
                                                                            20, naMeetingCombined.MyNaMeeting.ClosedToPublic, naMeetingCombined.MyNaMeeting.FormatMeeting, naMeetingCombined.MyNaMeeting.type);
                
                naMeetingCombined.MeetingsResultList = naMeetingsBll.GetMeetings(naMeetingCombined.MyNaMeeting);

                decimal? zipcode = naMeetingCombined.MeetingsResultList.Where(x => x.Zip != null).Select(x => x.Zip).FirstOrDefault();

                if (!zipcode.HasValue)
                    zipcode = 0;

                ViewBag.TotalCity = naMeetingsBll.GetMeetingsByCity(naMeetingCombined.MyNaMeeting.city, naMeetingCombined.MyNaMeeting.state, naMeetingCombined.MyNaMeeting.type);
                ViewBag.TotalByRange = naMeetingsBll.GetMeetingsByRange((int)zipcode.Value, 25, naMeetingCombined.MyNaMeeting.type);
                ViewBag.StateName = naMeetingCombined.MyNaMeeting.statename;

                BLL_Listings bllListing = new SoberBLL.Directory.BLL_Listings();

                ViewBag.RehabState = context.tbl_SamhsaListings.Where(x => x.location_state == naMeetingCombined.MyNaMeeting.state).Count();
                ViewBag.RehabCity = bllListing.GetFreeListingsCount("drug and alcohol treatment centers", naMeetingCombined.MyNaMeeting.state, naMeetingCombined.MyNaMeeting.city, zip, 25, 0, null, null, "any");
                viewname = "naCity";
            }
            else if (!string.IsNullOrEmpty(naMeetingCombined.MyNaMeeting.state))
            {
                naMeetingCombined.States.Add(naMeetingsBll.GetCitiesStates(StateAbbr, naMeetingCombined.MyNaMeeting.day, naMeetingCombined.MyNaMeeting.timeStart24Hour,
                                                                        naMeetingCombined.MyNaMeeting.timeEnd24Hour, naMeetingCombined.MyNaMeeting.zipcode,
                                                                        naMeetingCombined.MyNaMeeting.ClosedToPublic, naMeetingCombined.MyNaMeeting.FormatMeeting, naMeetingCombined.MyNaMeeting.type));

                viewname = "naState";

            }
            else

            {
                naMeetingCombined.States = naMeetingsBll.GetStates();
                viewname = "na";
            }

            
            return View(viewname, naMeetingCombined);
        }

        public ActionResult na_meetings(string place, string day, long meetingid, string location)
        {
            MeetingsBll naMeetingsBll = new MeetingsBll();
            Meeting meeting = naMeetingsBll.GetMeetingByID(meetingid);

            if (string.IsNullOrEmpty(meeting.State))
            {
                TempData["Error"] = "Can't access a meeting without a state. If you think this is an error, contact us!";
                
                return RedirectToAction("na");
            }
                

            ViewBag.TotalMeetings = naMeetingsBll.GetTotalMeetings(meeting.Id);
            ViewBag.TotalCity = naMeetingsBll.GetMeetingsByCity(meeting.City, meeting.State, meeting.Type);
            ViewBag.TotalByRange = naMeetingsBll.GetMeetingsByRange(meeting.Zip, 25, meeting.Type);
            ViewBag.StateName = BLL_Listings.GetStateNameFromAbbrevation(meeting.State);            

            using (DBManager context = new DBManager())
            {
                ViewBag.RehabState = context.tbl_SamhsaListings.Where(x => x.location_state == meeting.State).Count();

                BLL_Listings bllListing = new SoberBLL.Directory.BLL_Listings();
                string zip; string city2; string StateAbbr; string StateAbbreviation; string StateName;
                string address2 = meeting.City + ", " + meeting.State + ", United States";

                SearchHelper searchHelper = new SearchHelper();
                searchHelper.GetAddress(address2, out  city2, out StateAbbr, out zip, out StateAbbreviation, out StateName);
                ViewBag.RehabCity = bllListing.GetFreeListingsCount("drug and alcohol treatment centers", meeting.State, meeting.City, zip, 25, 0, null, null, "any");
            }

            return View(meeting);
        }

        public ActionResult naControl()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MeetingsMap(string meeting, int type)
        {
            List<usp_GetMeetings_Result> results = new List<usp_GetMeetings_Result>();

            if (type == 1)
            {
                NaMeetingsCombined meetingCombined = Newtonsoft.Json.JsonConvert.DeserializeObject<NaMeetingsCombined>(meeting);
                results = meetingCombined.MeetingsResultList;
            }
            else if (type == 2)
            {
                AAMeetingsCombined meetingCombined = Newtonsoft.Json.JsonConvert.DeserializeObject<AAMeetingsCombined>(meeting);
                results = meetingCombined.MeetingsResultList;
            }

            return PartialView("_mapsListingsMeeting", results);
        }

        [HttpPost]
        public ActionResult SetLongitudeLatitude(long idMeeting, decimal longitude, decimal latitude)
        {
            using (DBManager context = new DBManager())
            {
                var meeting = context.Meetings.Find(idMeeting);

                if (meeting != null)
                {
                    meeting.Longitude = longitude;
                    meeting.Latitude = latitude;
                }

                context.SaveChanges(); 
            }

            return null;
        }

        //public ActionResult na2(NaMeetingsCombined MC, string city, string state)
        //{
        //    string statename = "";
        //    string zip = null;
        //    string name = null;
        //    NaMeetingsCombined naMeetingsCombined = new NaMeetingsCombined();
        //    NaMeetingsBll naMeetingsBll = new NaMeetingsBll();
        //    DBManager context = new DBManager();

        //    if(MC != null && MC.MyNaMeeting != null && !string.IsNullOrEmpty(MC.MyNaMeeting.address))
        //    {
        //        string stat = "";
        //        SearchHelper searchHelper = new SearchHelper();
        //        searchHelper.GetAddress(MC.MyNaMeeting.address, out  city, out state, out zip, out stat, out statename);

        //        if(!string.IsNullOrEmpty(MC.MyNaMeeting.name))
        //            name = MC.MyNaMeeting.name;
        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(state))
        //        {
        //            IPLookUp Address = new IPLookUp(directoryController.GetIPAddress());
        //            SoberBLL.Directory.BLL_Listings bllListings = new SoberBLL.Directory.BLL_Listings();
        //            Region listing = new Region();                    

        //            var ipRow = (from ip in context.IPs
        //                         where ip.IPMin <= Address.IP && ip.IPMax >= Address.IP
        //                         select ip).FirstOrDefault();

        //            if (ipRow == null && !string.IsNullOrEmpty(ipRow.Country) && !string.IsNullOrEmpty(ipRow.City))
        //            {
        //                string info = directoryController.GetInfo(Address.IPAddress);
        //                Regex rgx = new Regex(@"lat.:null");                        

        //                if (!rgx.IsMatch(info))
        //                {
        //                    listing = JsonConvert.DeserializeObject<Region>(info);
        //                    listing.GetState();
        //                }
        //                else
        //                {
        //                    listing = Region.Default();
        //                }
        //            }
        //            else
        //            {
        //                listing.city = ipRow.City;
        //                listing.country_code = ipRow.CountryCode;
        //                listing.country_name = ipRow.Country;
        //                listing.lat = ipRow.Latitude.HasValue ? (float)ipRow.Latitude : 0;
        //                listing.lng = ipRow.Longitude.HasValue ? (float)ipRow.Longitude : 0;
        //            }

        //            state = listing.state;

        //            if (!string.IsNullOrWhiteSpace(listing.state) && listing.state.Trim().Length <= 2)
        //            {
        //                statename = BLL_Listings.GetStateNameFromAbbrevation(listing.state);
        //            }

        //            city = listing.city.ToLower(); 
        //        }

        //        if (!string.IsNullOrWhiteSpace(state) && state.Trim().Length > 2)
        //        {
        //            statename = state;
        //            state = BLL_Listings.GetStateAbbrevationFromStateName(state);
        //        }
        //    }

        //    ViewBag.State = statename;

        //    //Changing the title
        //    ViewBag.Title = "NA Meetings in USA";
        //    if (!string.IsNullOrEmpty(city))
        //    {
        //        ViewBag.Title = "NA Meetings in " + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(city);

        //        if (string.IsNullOrEmpty(state))
        //        {
        //            CitiesStates c = CitiesStates.GetState(city);

        //            if (c == null)
        //                return View(naMeetingsCombined);

        //            state = c.State;
        //            statename = c.StateFullName;
        //        }

        //        ViewBag.Title += " " + state + " - NA Meetings near me in " + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(statename);
        //    }
        //    else if (!string.IsNullOrEmpty(state))
        //    {
        //        string stateName = ViewBag.State;
        //        ViewBag.Title = "NA Meetings in " + state + " - " + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(stateName);

        //        CitiesStates citiesStatesList = naMeetingsBll.GetCitiesStates(state, null, null, null, null, null, null);
        //        naMeetingsCombined.States.Add(citiesStatesList);
        //    }

        //    int zipcode = 0;

        //    if (!int.TryParse(zip, out zipcode))
        //    {
        //        zipcode = 0;
        //    }

        //    NaMeeting meetingNAA = naMeetingsBll.CreateMeeting(name, city, state, null, null, null, zipcode, null, null, null);
        //    meetingNAA.address = city + ", " + state;

        //    naMeetingsCombined.MeetingsResultList = new List<usp_GetMeetings_Result>();
        //    naMeetingsCombined.MeetingsResultList = naMeetingsBll.GetMeetings(meetingNAA);
        //    naMeetingsCombined.MyNaMeeting = meetingNAA;

        //    if (!string.IsNullOrEmpty(city))
        //    {
        //        naMeetingsCombined.citiesNear = naMeetingsBll.GetCitiesNear(city, null, null, null, 20, null, null);
        //    }      
            
        //    return View(naMeetingsCombined);
        //}

    }
}
