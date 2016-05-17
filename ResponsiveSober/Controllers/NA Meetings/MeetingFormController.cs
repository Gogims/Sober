using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoberBLL.NA;
using SoberModel;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;

namespace MvcApplication4.Controllers
{
    public partial class meetingsController : BaseController
    {
        public ActionResult AddMeeting()
        {
            NaManager na = new NaManager();
            na.IP = directoryController.GetIPAddress();
            
            return View(na);
        }

        [HttpPost]
        public ActionResult AddMeeting(NaManager na)
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
                log.Location = "Add NA Meeting";
                DB.Errors.Add(log);
                DB.SaveChanges();
                #endregion

                ViewBag.Error = message;

                return View(na);
            }

            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

            if (string.IsNullOrEmpty(recaptchaHelper.Response))
            {
                ViewBag.Error = "Captcha answer cannot be empty.";
                return View(na);
            }

            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                ViewBag.Error = "Incorrect captcha answer.";
                return View(na);
            }
            else
            {
                try
                {
                    na.RequestNewMeeting();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;

                    return View(na);
                }
            }            

            return View("ConfirmedNaForm");
        }

        public ActionResult ModifyMeeting(int idMeeting)
        {
            NaManager na = new NaManager(idMeeting);
            na.IP = directoryController.GetIPAddress();

            return View(na);
        }

        [HttpPost]
        public ActionResult ModifyMeeting(NaManager na)
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
                log.Location = "Modify NA Meeting";
                DB.Errors.Add(log);
                DB.SaveChanges();
                #endregion

                ViewBag.Error = message;

                return View(na);
            }

            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

            if (string.IsNullOrEmpty(recaptchaHelper.Response))
            {
                ViewBag.Error = "Captcha answer cannot be empty.";
                return View(na);
            }

            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                ViewBag.Error = "Incorrect captcha answer.";
                return View(na);
            }
            else
            {
                try
                {
                    na.RequestModification();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;

                    return View(na);
                }
            }
            

            return View("ConfirmedNaForm");
        }        

        public ActionResult DeleteMeeting(int idMeeting)
        {
            NaManager na = new NaManager(idMeeting);
            na.IP = directoryController.GetIPAddress();

            return View(na);
        }

        [HttpPost]
        public ActionResult DeleteMeeting(NaManager na)
        {
            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

            if (string.IsNullOrEmpty(recaptchaHelper.Response))
            {
                ViewBag.Error = "Captcha answer cannot be empty.";
                return View(na);
            }

            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                ViewBag.Error = "Incorrect captcha answer.";
                return View(na);
            }
            else
            {
                try
                {
                    na.RequestElimination();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;

                    return View(na);
                }
            }            

            return View("ConfirmedNaForm");
        }
    }
}
