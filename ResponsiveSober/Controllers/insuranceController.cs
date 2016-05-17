using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using SoberModel;
using AutoMapper;
using System;

namespace MvcApplication4.Controllers
{
    public class insuranceController : BaseController
    {
        public const string PROFILE_INSURANCE_LOCATION = "C:\\inetpub\\SoberSurveyImagesAndVideos\\insurance\\";
        
        public ActionResult Index(string userid)
        {
            TempData["gobacktreatmentcenterid"] = userid;
            TempData.Keep("treatmentcenter");

            List<Insurance> insurances = new List<Insurance>();

            using (DBManager context = new DBManager())
            {

                foreach (var item in context.Insurance_Providers.OrderBy(x => x.Ins_ProviderName))
                {
                    insurances.Add(new Insurance
                    {
                        Ins_AboutUs = item.Ins_AboutUs,
                        Ins_ID = item.Ins_ID,
                        Ins_ProviderLogo = item.Ins_ProviderLogo,
                        Ins_ProviderName = item.Ins_ProviderName
                    });
                }
            }
            
            return View(insurances);
        }
        
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            TempData.Keep("treatmentcenter");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(Insurance insurance, HttpPostedFileBase logo)
        {
            if (!ModelState.IsValid)
            {
                string message = "";
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });

                foreach (var item in errors)
                {
                    message += "Error binding " + item.Key + ": ";

                    foreach (var error in item.Errors)
                    {
                        message += error.ErrorMessage + "\n";
                    }
                }

                #region Saving Error in DB
                DBManager DB = new DBManager();
                Error log = new Error();
                log.Created = DateTime.Now;
                log.ErrorType = 2;
                log.Message = message;
                log.Location = "Create Insurance";
                DB.Errors.Add(log);
                DB.SaveChanges();
                #endregion

                TempData["Error"] = message;

                TempData.Keep("treatmentcenter");
                return RedirectToAction("Index");
            }
            
            Mapper.CreateMap<Insurance, Insurance_Providers>();
            Insurance_Providers insurance_providers = new Insurance_Providers();
            insurance_providers = Mapper.Map<Insurance, Insurance_Providers>(insurance);

            using (DBManager context = new DBManager())
            {
                Dictionary<string, Insurance_Providers> insuranceProviders = context.Insurance_Providers.ToDictionary(x => x.Ins_ProviderName);

                if (!insuranceProviders.ContainsKey(insurance_providers.Ins_ProviderName))
                {
                    if (logo != null && logo.ContentLength > 0)
                    {
                        string newLogoName = insurance_providers.Ins_ProviderName.Replace(' ', '-');
                        if ((logo.ContentType == "image/jpg")
                            || (logo.ContentType == "image/png")
                            || (logo.ContentType == "image/jpeg")
                            || (logo.ContentType == "image/gif")
                            || (logo.ContentType == "image/x-png")
                            || (logo.ContentType == "image/pjpeg"))
                        {

                            string loc = PROFILE_INSURANCE_LOCATION + logo.FileName;
                            if (!System.IO.Directory.Exists(PROFILE_INSURANCE_LOCATION))
                            {
                                System.IO.Directory.CreateDirectory(PROFILE_INSURANCE_LOCATION);
                            }
                            logo.SaveAs(loc);
                            ResizeSettings settings = new ResizeSettings(100, 100, FitMode.Max, null);
                            ImageResizer.ImageBuilder.Current.Build(loc, loc, settings);
                        }
                        insurance_providers.Ins_ProviderLogo = logo.FileName;
                        insurance_providers.Ins_ProviderName = insurance_providers.Ins_ProviderName.ToUpper();
                        context.Insurance_Providers.Add(insurance_providers);

                        try
                        {
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Message;
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "The insurance you were adding already exists.";
                }
            }

            TempData.Keep("treatmentcenter");
            return RedirectToAction("Index");
        }

        //
        // GET: /insurance/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id = 0)
        {
            Insurance insurance_providers = new Insurance(id);

            if (string.IsNullOrEmpty(insurance_providers.Ins_ProviderName))
            {
                return HttpNotFound();
            }

            return View(insurance_providers);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(Insurance insurance_providers, HttpPostedFileBase logo)
        {
            if (ModelState.IsValid)
            {

                if (logo != null && logo.ContentLength > 0)
                {
                    string newLogoName = insurance_providers.Ins_ProviderName.Replace(' ', '-');
                    if ((logo.ContentType == "image/jpg") 
                        || (logo.ContentType == "image/png") 
                        || (logo.ContentType == "image/jpeg") 
                        || (logo.ContentType == "image/gif") 
                        || (logo.ContentType == "image/x-png") 
                        || (logo.ContentType == "image/pjpeg"))
                    {

                        string loc = PROFILE_INSURANCE_LOCATION + logo.FileName;
                        if (!System.IO.Directory.Exists(PROFILE_INSURANCE_LOCATION))
                        {
                            System.IO.Directory.CreateDirectory(PROFILE_INSURANCE_LOCATION);
                        }
                        logo.SaveAs(loc);
                        ResizeSettings settings = new ResizeSettings(100, 100, FitMode.Max, null);
                        ImageResizer.ImageBuilder.Current.Build(loc, loc, settings);
                    }
                    insurance_providers.Ins_ProviderLogo = logo.FileName;
                    
                }

                using (DBManager context = new DBManager())
                {
                    var ins = (from insurance in context.Insurance_Providers
                               where insurance.Ins_ID == insurance_providers.Ins_ID
                               select insurance);

                    if (ins != null && ins.Count() > 0)
                    {
                        var insurance = ins.First();

                        insurance.Ins_ProviderLogo = string.IsNullOrEmpty(insurance_providers.Ins_ProviderLogo) ? insurance.Ins_ProviderLogo : insurance_providers.Ins_ProviderLogo;
                        insurance.Ins_ProviderName = insurance_providers.Ins_ProviderName.ToUpper();
                        insurance.Ins_AboutUs = insurance_providers.Ins_AboutUs;

                        try
                        {
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Message;
                        }
                    }
                }
            }
            else
            {
                string message = "";
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });

                foreach (var item in errors)
                {
                    message += "Error binding " + item.Key + ": ";

                    foreach (var error in item.Errors)
                    {
                        message += error.ErrorMessage + "\n";
                    }
                }

                #region Saving Error in DB
                DBManager DB = new DBManager();
                Error log = new Error();
                log.Created = DateTime.Now;
                log.ErrorType = 2;
                log.Message = message;
                log.Location = "Editing Insurance";
                DB.Errors.Add(log);
                DB.SaveChanges();
                #endregion

                TempData["Error"] = message;
            }

            return RedirectToAction("index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id = 0)
        {
            Insurance insurance_providers = new Insurance(id);
            if (string.IsNullOrEmpty(insurance_providers.Ins_ProviderName))
            {
                return HttpNotFound();
            }
            return View(insurance_providers);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(Insurance insurance)
        {
            using (DBManager context = new DBManager())
            {
                var ins = context.Insurance_Providers.Find(insurance.Ins_ID);

                try
                {
                    context.Insurance_Providers.Remove(ins);
                    context.SaveChanges();
                }
                catch (System.Exception ex)
                {
                    ViewBag.Error = ex.Message;
                    return View(insurance);
                }
            }
            
            return RedirectToAction("Index");
        }
    }
}