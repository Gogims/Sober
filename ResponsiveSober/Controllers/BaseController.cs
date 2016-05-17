using SoberModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace MvcApplication4.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = filterContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();            
            
            #region throttle
            bool iscrawler = Regex.IsMatch(filterContext.HttpContext.Request.UserAgent, @"bot|crawler|baiduspider|80legs|ia_archiver|voyager|curl|wget|yahoo! slurp|mediapartners-google", RegexOptions.IgnoreCase);
            bool isadmin = filterContext.HttpContext.User.IsInRole("Admin");
            bool isunlimited = filterContext.HttpContext.User.IsInRole("Unlimited Users");

            if (iscrawler == false && isadmin == false && isunlimited == false && controller != "account" && controller != "UserManagement")
            {
                var key = string.Concat(MvcApplication4.Controllers.directoryController.GetIPAddress());
                var accessCookie = HttpContext.Request.Cookies["access"];
                var blockCookie = HttpContext.Request.Cookies["block"];

                if (accessCookie == null)
                {
                    accessCookie = new HttpCookie("access", "1");
                    HttpContext.Response.Cookies.Add(accessCookie);
                }
                else if (blockCookie == null)
                {
                    int val = int.Parse(accessCookie.Value) + 1;
                    accessCookie.Value = val.ToString();

                    if (val > 25)
                    {
                        accessCookie.Value = "0";

                        blockCookie = new HttpCookie("block", DateTime.Now.AddMinutes(1).ToString());
                        blockCookie.Expires = DateTime.Now.AddMinutes(1);
                        HttpContext.Response.Cookies.Add(blockCookie);

                        filterContext.Result = new RedirectResult(Url.Action("ExceedPageVisit", "Throttle"));

                        // see 409 - http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
                        // filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    }

                    HttpContext.Response.SetCookie(accessCookie);
                }
                else
                {
                    filterContext.Result = new RedirectResult(Url.Action("ExceedPageVisit", "Throttle"));
                }
            }
            #endregion

            #region campaign
            string action = filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString();
            string rehab = Request.QueryString["rehab"];
            List<string> phones = new List<string>();

            using (DBManager context = new DBManager())
            {
                short catId = context.ListingCategories.Where(x => x.CategoryName.Equals(rehab)).Select(x => x.CategoryId).FirstOrDefault();

                if(catId == 0)
                    phones = context.Campaigns.Where(x => x.Action.Equals(action) && x.Controller.Equals(controller)).Select(x => x.Phone).ToList();
                else
                    phones = context.Campaigns.Where(x => x.Action.Equals(action) && x.Controller.Equals(controller) && x.CategoryId == catId).Select(x => x.Phone).ToList();
            }

            var numberCookie = new HttpCookie("number");
            System.DateTime expire = System.DateTime.Now;
            numberCookie.Expires = expire.AddDays(2);

            if (phones.Count > 0)
            {
                if (Request.Cookies["number"] == null)
                {
                    numberCookie.Value = "0";
                    ViewBag.Phone = phones[0];
                    HttpContext.Response.Cookies.Add(numberCookie);
                }
                else
                {
                    int current = int.Parse(Request.Cookies["number"].Value);

                    if (++current >= phones.Count)
                        current = 0;

                    ViewBag.Phone = phones[current];
                    numberCookie.Value = current.ToString();
                    HttpContext.Response.SetCookie(numberCookie);
                } 
            }
            #endregion

            #region forcing refresh cache
            //HTTP 1.1
            Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            //HTTP 1.0
            Response.AddHeader("Pragma", "no-cache");
            //Proxies
            Response.AddHeader("Expires", "0");
            #endregion

            #region Saving Error in Viewbag
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            #endregion

            base.OnActionExecuting(filterContext);
        }        
    }
}
