using SoberModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication4.Controllers
{
    public partial class directoryController : BaseController
    {
        [ActionName("insurance-companies-covering-rehab")]
        public ActionResult PrivateInsurances(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Insurance i = new Insurance();
                ViewBag.Insurances = Insurance.GetInsurances();
                HtmlContent c = new HtmlContent(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), null);
                ViewBag.FAQ = c.GetHTML();
                return View("PrivateInsurances", i); 
            }
            else
            {
                Insurance i = new Insurance(name.ToUpper());
                return View("PrivateInsuranceDetails", i);
            }
        }

        [HttpPost]
        public ActionResult SetLongitudeLatitude(string pagename, decimal longitude, decimal latitude)
        {
            DBManager context = new DBManager();
            var samhsa = (from sam in context.tbl_SamhsaListings
                              where sam.PageName.ToLower().Trim() == pagename.ToLower().Trim()
                              select sam).FirstOrDefault();

            if (samhsa != null)
            {
                samhsa.longitude = longitude;
                samhsa.latitude = latitude;
            }

            context.SaveChanges();
            
            return null;
        }
    }    
}
