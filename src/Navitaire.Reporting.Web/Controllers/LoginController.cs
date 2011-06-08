using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Navitaire.Reporting.Web.Models;


namespace Navitaire.Reporting.Web.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Login()
        {
            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            try
            {
                LoginModel.ErrorMessage = string.Empty;
                model.Password.Contains('/'); //crapy validation
            
                if (model.Username.Contains('\\'))
                {
                    string[] userValue = model.Username.Split('\\');
                    model.Domain = userValue[0];
                    model.Username = userValue[1];                                       
                }
                else
                {
                    model.Domain = "DEF";
                }                

                if (LoginModel.Login(model.Username, model.Password, model.Domain))
                {
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        Session.Timeout = 30; //sets the session timout in minutes
                        Session.Add("AuthenticatedUser", model);
                        return RedirectToAction("Index", "Catalog");
                    }
                }
                return View(model);
            }
            catch
            {                
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            LoginModel.Logout(login);
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }

    }
}
