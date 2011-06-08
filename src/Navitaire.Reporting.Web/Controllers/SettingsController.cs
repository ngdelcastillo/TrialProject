using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Navitaire.Reporting.Web.Models;
using System.Collections.Specialized;
using System.Text;

namespace Navitaire.Reporting.Web.Controllers
{
    public class SettingsController : Controller
    {
        //
        // GET: /Settings/
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            SettingsModel model = new SettingsModel();
            if (login != null)
            {
                Session.Add("Settings", model);
                return View(model);
            }
            else
            {
                LoginModel.ErrorMessage = "Session Expired";
                return RedirectToAction("Login", "Login");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Application()
        {
            SettingsModel.State = "Application";
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            return View(SettingsModel.GetConfigurations(login.AuthenticationUser));
        }

        [Authorize]
        [HttpPost]
        public ActionResult Application(FormCollection collection)
        {
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            Dictionary<string, string> configurations = new Dictionary<string,string>();
            foreach(string key in collection.AllKeys)
            {
                configurations.Add(key, collection[key]);
            }
            return View( SettingsModel.SaveConfigurations(configurations, login.AuthenticationUser));
        }

        [Authorize]
        [HttpGet]
        public ActionResult Roles()
        {
            SettingsModel.State = "Roles";
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            return View(SettingsModel.GetRoles(login.AuthenticationUser));
        }

        [Authorize]
        [HttpPost]
        public ActionResult Roles(FormCollection collection)
        {
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            if (collection["action"] == "Add")
            {
                return View(SettingsModel.SaveConfiguredRoles(collection["role"], "AddRole", login.AuthenticationUser));
            }
            else
            {
                return View(SettingsModel.SaveConfiguredRoles(collection["action"], "DeleteRole", login.AuthenticationUser));
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult ExportMappings()
        {
            SettingsModel.State = "ExportMappings";
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            SettingsModel model = SettingsModel.GetExportMappings(login.AuthenticationUser);
            SettingsModel.ReportPath = filterURLPath(Request.Url.AbsoluteUri);
            model.FillReportStoredProcedures(login.AuthenticationUser, SettingsModel.ReportPath);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ExportMappings(FormCollection collection)
        {
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            if (collection["action"] == "Add")
            {
                return View(SettingsModel.SaveExportMapping(login.AuthenticationUser,
                    collection["reportPath"], collection["storedProc"], collection["displayOption"]));
            }
            else
            {
                return View(SettingsModel.DeleteExportMapping(login.AuthenticationUser, collection["action"]));
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Security()
        {
            SettingsModel.State = "Security";
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            return View(SettingsModel.GetSecurityCatalogItems(login.AuthenticationUser, filterURLPath(Request.Url.AbsoluteUri)));

        }

        [Authorize]
        [HttpPost]
        public ActionResult Security(FormCollection collection)
        {
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            return View();
        }

        
        #region Private Helper Methods

        private string filterURLPath(string path)
        {
            StringBuilder reportPath = new StringBuilder();
            if (Request.Url.AbsoluteUri.Contains('_'))
            {
                reportPath.Append(Request.Url.AbsoluteUri.Substring(Request.Url.AbsoluteUri.IndexOf('_')).Replace("%20", " "));
                reportPath.Replace("_root", "");
            }
            return reportPath.ToString();
        }
        #endregion
    }
}
