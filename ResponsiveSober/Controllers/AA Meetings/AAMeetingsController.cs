using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoberModel;
using SoberBLL;
using SoberBLL.Directory;
using SoberBLL.Search;
using MvcApplication4.Security;

namespace MvcApplication4.Controllers
{
    public partial class meetingsController : BaseController
    {
        [HttpGet]
        public ActionResult aa(string place, string city, string state, int? day, int? timeStart24Hour, int? timeEnd24Hour, int? zipcode, int? distance, bool? ClosedToPublic, short[] format)
        {
            AAMeetingsCombined aaMeeting = new AAMeetingsCombined();            

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
            else if (!string.IsNullOrEmpty(city) && string.IsNullOrEmpty(state))
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
                        
            aaMeeting.MyAAMeeting = new AAMeeting(place, city, state, day, timeStart24Hour, timeEnd24Hour, zipcode, distance, ClosedToPublic, format, statename);
            string viewName = "";
            
            if (!string.IsNullOrEmpty(aaMeeting.MyAAMeeting.city))
            {
                aaMeeting.citiesNear = naMeetingsBll.GetCitiesNear(city, day, timeStart24Hour, timeEnd24Hour, 20, ClosedToPublic, format, aaMeeting.MyAAMeeting.type);
                decimal? zip = aaMeeting.MeetingsResultList.Where(x => x.Zip != null).Select(x => x.Zip).FirstOrDefault();

                if (!zip.HasValue)
                    zip = 0;

                ViewBag.TotalCity = naMeetingsBll.GetMeetingsByCity(aaMeeting.MyAAMeeting.city, aaMeeting.MyAAMeeting.state, aaMeeting.MyAAMeeting.type);
                ViewBag.TotalByRange = naMeetingsBll.GetMeetingsByRange((int)zip.Value, 25, aaMeeting.MyAAMeeting.type);
                ViewBag.StateName = BLL_Listings.GetStateNameFromAbbrevation(aaMeeting.MyAAMeeting.state);

                using (DBManager context = new DBManager())
                {
                    ViewBag.RehabState = context.tbl_SamhsaListings.Where(x => x.location_state == aaMeeting.MyAAMeeting.state).Count();
                    BLL_Listings bllListing = new SoberBLL.Directory.BLL_Listings();
                    string zip2; string city2; string StateAbbr; string StateAbbreviation; string StateName;
                    string address2 = aaMeeting.MyAAMeeting.city + ", " + aaMeeting.MyAAMeeting.state;

                    SearchHelper searchHelper = new SearchHelper();
                    searchHelper.GetAddress(address2, out  city2, out StateAbbr, out zip2, out StateAbbreviation, out StateName);
                    ViewBag.RehabCity = bllListing.GetFreeListingsCount("drug and alcohol treatment centers", aaMeeting.MyAAMeeting.state, aaMeeting.MyAAMeeting.city, zip2, 25, 0, null, null, "any");
                }

                aaMeeting.MeetingsResultList = naMeetingsBll.GetMeetings(aaMeeting.MyAAMeeting);
                viewName = "aaCity";
                
                return View("aaCity", aaMeeting);
            }
            else if (!string.IsNullOrEmpty(aaMeeting.MyAAMeeting.state))
            {
                aaMeeting.States.Add(naMeetingsBll.GetCitiesStates(state, day, timeStart24Hour, timeEnd24Hour, zipcode, ClosedToPublic, format, AAMeeting.GetType("Alcoholic Anonymous")));
                viewName = "aaState";
            }
            else
            {
                viewName = "aa";
                aaMeeting.States = naMeetingsBll.GetStates();
            }

            return View(viewName, aaMeeting);
        }

        [HttpPost]
        public ActionResult aa(AAMeetingsCombined aaMeetingCombined)
        {
            MeetingsBll aaMeetingsBll = new MeetingsBll();
            var context = new SoberMVCEntities();

            aaMeetingCombined.MyAAMeeting.FormatMeeting = aaMeetingCombined.MyAAMeeting.FormatMeeting.Where(x => x != 0).ToArray();

            if (!string.IsNullOrEmpty(aaMeetingCombined.MyAAMeeting.name) && string.IsNullOrEmpty(aaMeetingCombined.MyAAMeeting.address))
            {
                var address = (from m in context.Meetings
                               where m.Name.Contains(aaMeetingCombined.MyAAMeeting.name)
                               select new { m.City, m.State }).FirstOrDefault();

                if (address != null)
                {
                    aaMeetingCombined.MyAAMeeting.address = address.City + ", " + address.State;
                }
            }
            
            SearchHelper searchHelper = new SearchHelper();
            string city, StateAbbr, zip, StateAbbreviation, StateName;
            searchHelper.GetAddress(aaMeetingCombined.MyAAMeeting.address, out  city, out StateAbbr, out zip, out StateAbbreviation, out StateName);

            aaMeetingCombined.MyAAMeeting.city = city;
            aaMeetingCombined.MyAAMeeting.state = StateAbbr;
            aaMeetingCombined.MyAAMeeting.statename = StateName;
            
            if (!string.IsNullOrWhiteSpace(zip))
            {
                int ZipInt = 0;
                bool success = int.TryParse(zip, out ZipInt);
                if (success)
                {
                    aaMeetingCombined.MyAAMeeting.zipcode = ZipInt;
                }
            }

            string viewname = "";

            if (!string.IsNullOrEmpty(aaMeetingCombined.MyAAMeeting.city))
            {
                aaMeetingCombined.citiesNear = aaMeetingsBll.GetCitiesNear(city, aaMeetingCombined.MyAAMeeting.day, aaMeetingCombined.MyAAMeeting.timeStart24Hour, aaMeetingCombined.MyAAMeeting.timeEnd24Hour,
                                                                            20, aaMeetingCombined.MyAAMeeting.ClosedToPublic, aaMeetingCombined.MyAAMeeting.FormatMeeting, aaMeetingCombined.MyAAMeeting.type);

                aaMeetingCombined.MeetingsResultList = aaMeetingsBll.GetMeetings(aaMeetingCombined.MyAAMeeting);

                decimal? zipcode = aaMeetingCombined.MeetingsResultList.Where(x => x.Zip != null).Select(x => x.Zip).FirstOrDefault();

                if (!zipcode.HasValue)
                    zipcode = 0;

                ViewBag.TotalCity = aaMeetingsBll.GetMeetingsByCity(aaMeetingCombined.MyAAMeeting.city, aaMeetingCombined.MyAAMeeting.state, aaMeetingCombined.MyAAMeeting.type);
                ViewBag.TotalByRange = aaMeetingsBll.GetMeetingsByRange((int)zipcode.Value, 25, aaMeetingCombined.MyAAMeeting.type);
                ViewBag.StateName = aaMeetingCombined.MyAAMeeting.statename;

                BLL_Listings bllListing = new SoberBLL.Directory.BLL_Listings();

                ViewBag.RehabState = context.tbl_SamhsaListings.Where(x => x.location_state == aaMeetingCombined.MyAAMeeting.state).Count();
                ViewBag.RehabCity = bllListing.GetFreeListingsCount("drug and alcohol treatment centers", aaMeetingCombined.MyAAMeeting.state, aaMeetingCombined.MyAAMeeting.city, zip, 25, 0, null, null, "any");
                viewname = "aaCity";
            }
            else if (!string.IsNullOrEmpty(aaMeetingCombined.MyAAMeeting.state))
            {
                aaMeetingCombined.States.Add(aaMeetingsBll.GetCitiesStates(StateAbbr, aaMeetingCombined.MyAAMeeting.day, aaMeetingCombined.MyAAMeeting.timeStart24Hour,
                                                                        aaMeetingCombined.MyAAMeeting.timeEnd24Hour, aaMeetingCombined.MyAAMeeting.zipcode,
                                                                        aaMeetingCombined.MyAAMeeting.ClosedToPublic, aaMeetingCombined.MyAAMeeting.FormatMeeting, aaMeetingCombined.MyAAMeeting.type));

                viewname = "aaState";
            }
            else
            {
                viewname = "aa";
            }

            return View(viewname, aaMeetingCombined);
        }

        public ActionResult aa_meetings(string place, string day, decimal? zipcode, long meetingid, string location)
        {
            MeetingsBll naMeetingsBll = new MeetingsBll();
            Meeting meeting = naMeetingsBll.GetMeetingByID(meetingid);

            if (string.IsNullOrEmpty(meeting.State))
            {
                TempData["Error"] = "Can't access a meeting without a state. If you think this is an error, contact us!";
                return RedirectToAction("aa");
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

        public ActionResult article(string type, int id)
        {
            ViewBag.Article = "";

            if (type == "aa")
            {
                HtmlContent c = new HtmlContent(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), id);
                ViewBag.Article = c.GetHTML();
            }

            return View();
        }

        [ActionName("aa-meetings")]
        public ActionResult aa2()
        {
            HtmlContent c = new HtmlContent(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), null);
            ViewBag.FAQ = c.GetHTML();
            
            return View("aa2");
        }

        [ActionName("aa-meeetings")]
        public ActionResult aa3()
        {
            return RedirectToAction("aa-meetings");
        }

    }
}
