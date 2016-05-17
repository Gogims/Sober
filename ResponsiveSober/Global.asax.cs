using MvcApplication4.App_Start;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcApplication4
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            if (!Request.Url.Host.StartsWith("www") && !Request.Url.IsLoopback)
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Host = "www." + Request.Url.Host;
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }

            else if (Request.Url.AbsolutePath.ToLower().Contains(("directory/listings").ToLower())
                && Request.Url.Query.ToLower().Contains(("facility_type").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
               // builder.Host = "www.sober.com";
                Response.StatusCode = 404;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }

            //DirectoryRedirects();
            //else
            //{
            //    string lowercaseURL = Request.Url.ToString();
            //    if (Regex.IsMatch(lowercaseURL, @"[A-Z]"))
            //    {
            //        //You don't want to change casing on query strings
            //        lowercaseURL = lowercaseURL.ToLower();
            //        Response.Clear();
            //        Response.StatusCode = 301;
            //        Response.AddHeader("Location", lowercaseURL);
            //        Response.End();
            //    }

            //}
        }
        //protected void Application_EndRequest()
        //{
        //    if (Context.Response.StatusCode == 404)
        //    {
        //        Response.Clear();

        //        var rd = new RouteData();
        //        rd.DataTokens["area"] = "AreaName"; // In case controller is in another area
        //        rd.Values["controller"] = "Errors";
        //        rd.Values["action"] = "NotFound";

        //     //   IController c = new ErrorsController();
        //     //   c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
        //    }
        //}
        //void Application_Error(object sender, EventArgs e)
        //{
        //    // Code that runs when an unhandled error occurs

        //    // Get the exception object.
        //    Exception exc = Server.GetLastError();

        //    // Handle HTTP errors
        //    if (exc.GetType() == typeof(HttpException))
        //    {
        //        // The Complete Error Handling Example generates
        //        // some errors using URLs with "NoCatch" in them;
        //        // ignore these here to simulate what would happen
        //        // if a global.asax handler were not implemented.
        //        if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
        //            return;

        //        //Redirect HTTP errors to HttpError page
        //        Server.Transfer("HttpErrorPage.aspx");
        //    }

        //    // For other kinds of errors give the user some information
        //    // but stay on the default page
        //    Response.Write("<h2>Global Page Error</h2>\n");
        //    Response.Write(
        //        "<p>" + exc.Message + "</p>\n");
        //    Response.Write("Return to the <a href='Default.aspx'>" +
        //        "Default Page</a>\n");

        //    // Log the exception and notify system operators
        //    //    ExceptionUtility.LogException(exc, "DefaultPage");
        //    //    ExceptionUtility.NotifySystemOps(exc);

        //    // Clear the error from the server
        //    Server.ClearError();
        //}
        protected void DirectoryRedirects()
        {
            if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/default.html").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/directory";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/alternative+drug+rehab+programs").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/adolescent_treatment";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/adolescent+services").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/adolescent_treatment";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/drug+detox+centers").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/detox_centers";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/affordable+treatment").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/drug_alcohol";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.Host.ToLower().Contains(("/directory/chronic+pain+treatments").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/drug_alcohol";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/drug+addiction+counselors").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/drug_alcohol";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/drug+and+alcohol+treatment+centers").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/drug_alcohol";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/dual+diagnosis+centers").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/drug_alcoho";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/faith+based+drug+rehabs").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/drug_alcohol";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/gambling+addiction+treatment").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/drug_alcohol";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/outpatient+drug+rehabs").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/drug_alcohol";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/recovery+coaches").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/drug_alcohol";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/eating+disorder+treatment+centers").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/eating_disorders";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/exclusive+drug+rehabs").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/exclusive_rehabs";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/extended+care+facilities").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/extended_care";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/halfway+houses/").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/halfway_houses";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/mens+treatment").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/mens_treatment";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/sober+houses").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/sober_houses";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains(("/directory/womens+treatment").ToLower()))
            {
                UriBuilder builder = new UriBuilder(Request.Url);
                builder.Path = "/facilities/womens_treatment";
                Response.StatusCode = 301;
                Response.AddHeader("Location", builder.ToString());
                Response.End();
            }
            
        }
    }
}