using SoberBLL.NA;
using SoberModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using SoberBLL;
using System.Data;

namespace MvcApplication4.Controllers
{
    public partial class meetingsController : BaseController
    {

        [Authorize(Roles = "Admin")]
        public ActionResult na_pending_meeting()
        {
            DBManager context = new DBManager();

            return View(context.MeetingTemps.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditMeeting(int idMeeting)
        {
            DBManager context = new DBManager();

            var temp = context.MeetingTemps.Find(idMeeting);
            NaManager na = new NaManager(temp);
            ViewBag.Address = GetAddress(na.IP); 

            return View(na);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditMeeting(NaManager na, string submit)
        {
            if (submit == "Approve")
            {
                if (ApproveMeeting(na) == false)
                {
                    ViewBag.Error = TempData["Error"].ToString();
                    ViewBag.Address = GetAddress(na.IP);

                    return View(na);
                }
            }
            else if (submit == "Reject")
            {
                try
                {
                    na.RejectMeeting();                    
                }
                catch(Exception ex)
                {
                    ViewBag.Error = ex.Message;
                    ViewBag.Address = GetAddress(na.IP);

                    return View(na);
                }
            }
            else
            {
                try
                {
                    na.EditTempMeeting();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                    ViewBag.Address = GetAddress(na.IP);

                    return View(na);
                }
            }

            return RedirectToAction("na_pending_meeting");
        }

        private bool ApproveMeeting(NaManager temp)
        {
            bool ret = true;

            try
            {
                if (temp.Status == 1)
                {
                    temp.AddMeeting();
                }
                else if (temp.Status == 2)
                {
                    temp.EditMeeting();
                }
                else if (temp.Status == 3)
                {
                    temp.DeleteMeeting();
                }
            }
            catch (Exception ex)
            {
                ret = false;
                TempData["Error"] = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// Parse de IP address
        /// </summary>
        /// <param name="IP">IP of the user</param>
        /// <returns>Address of the user. Empty = IP came null or the IP doesn't exist in the IP DB</returns>
        private string GetAddress(string IP)
        {
            string address = "";

            if (!string.IsNullOrEmpty(IP))
            {
                using (DBManager context = new DBManager())
                {
                    IPLookUp Address = new IPLookUp(IP);

                    var ipRow = (from ip in context.IPs
                                 where ip.IPMin <= Address.IP && ip.IPMax >= Address.IP
                                 select ip).FirstOrDefault();

                    if (!string.IsNullOrEmpty(ipRow.City))
                    {
                        address += ", " + ipRow.City;
                    }

                    if (!string.IsNullOrEmpty(ipRow.State))
                    {
                        address += ", " + ipRow.State;
                    }

                    if (string.IsNullOrEmpty(address))
                    {
                        address += ", " + ipRow.Country + ", " + ipRow.Continent;
                    }
                }
            }

            return address;
        }

    }
}
