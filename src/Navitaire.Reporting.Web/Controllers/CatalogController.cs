using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Navitaire.Reporting.Web.Models;

namespace Navitaire.Reporting.Web.Controllers
{
    public class CatalogController : Controller
    {
        //
        // GET: /Catalog/
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            //Gets authentication
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            CatalogModel catalog = (CatalogModel)Session.Contents["Catalog"];
            CatalogModel model = new CatalogModel();
            if (catalog == null)
            {
                if (login != null)
                {
                    model.SRSProxy = CatalogModel.SRSAuthenticate(login.AuthenticationUser);
                    model = CatalogModel.GetCatalogItems(login.AuthenticationUser, model.SRSProxy);
                    RememberMe(model);
                    return View(model);
                }
                else
                {
                    LoginModel.ErrorMessage = "Session Expired";
                    return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                return View(catalog);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Content()
        {
            try
            {
                CatalogModel catalog = SeletedItem();
                return View(catalog);
            }
            catch (Exception ex)
            {
                CatalogModel catalog = (CatalogModel)Session.Contents["Catalog"];
                if (catalog.Error == null)
                {
                    catalog.Error = new Exception(ex.Message, ex.InnerException);                    
                }
                else
                {
                    catalog.Error = ex;                    
                }
                return View(catalog);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Content(FormCollection collection)
        {            
            return PostMethod(collection);
        }

        #region Private Methods
        private CatalogModel SeletedItem()
        {
            string url = Request.Url.AbsoluteUri;
            string key = url.Substring(url.IndexOf('_'));

            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            CatalogModel catalog = (CatalogModel)Session.Contents["Catalog"];
            CatalogModel content = new CatalogModel();

            if (key.Equals("_"))
            {
                //Select Home
                content.Name = "Home";
                content.Path = "/";
                content.ItemType = "Folder";
            }
            else
            {
                //Select other item
                content = CatalogModel.GetItem(catalog.AllItems, key);
                if (content != null)
                {
                    switch (content.ItemType)
                    {
                        case "Report":
                            content.GetReportParameters(login.AuthenticationUser, catalog.SRSProxy);
                            content.GetItemProperty(login.AuthenticationUser);
                            break;

                        case "Folder":
                            content.GetItemProperty(login.AuthenticationUser);                            
                            break;

                        case "LinkedReport":
                            content.GetReportParameters(login.AuthenticationUser, catalog.SRSProxy);
                            content.GetItemProperty(login.AuthenticationUser);
                            break;

                        case "DataSource":
                            content.GetDataSourceItem(login.AuthenticationUser, catalog.SRSProxy);
                            content.GetItemProperty(login.AuthenticationUser);
                            break;
                    }                  
                }
                else
                {
                    //Item not found
                }
            }
            
            catalog.Name = content.Name;
            catalog.ItemType = content.ItemType;
            catalog.Parent = content.Parent;
            catalog.Path = content.Path;
            catalog.ReportParameters = content.ReportParameters;
            catalog.ItemProperties = content.ItemProperties;
            catalog.DataSourceItem = content.DataSourceItem;
            catalog.Result = string.Empty;
            catalog.Error = null;
            return catalog;
        }

        private void RememberMe(CatalogModel model)
        {
            //Add to cookie
            HttpCookie webcookie = new HttpCookie(model.SRSProxy.AuthCookie.Name, model.SRSProxy.AuthCookie.Value);
            webcookie.Expires = DateTime.MaxValue;
            Response.AppendCookie(webcookie);

            //Add to session
            Session.Add("Catalog", model);
        }

        private ActionResult PostMethod(FormCollection collection)
        {
            LoginModel login = (LoginModel)Session.Contents["AuthenticatedUser"];
            CatalogModel content = SeletedItem();
            bool redirect = false;
            string method = collection["commit"];
            switch (method)
            {
                case "Run":
                    content.PostMethod = "Run";
                    content = Run(collection, content);                    
                    break;
                case "Export":
                    break;

                case "Info":
                    content.PostMethod = "Info";                                                            
                    break;

                case "Delete":
                    content.PostMethod = "Delete";
                    Delete(login, content);
                    redirect = true;                    
                    break;

                case "Edit":
                    content.PostMethod = "Edit";
                    break;

                case "Save Data Source":
                    content.PostMethod = "SaveDataSource";
                    redirect = SaveDatasource(collection, content, login);
                    break;

                case "Add Items":
                    content.PostMethod = "AddItems";
                    break;

                case "Add Report":
                    content.PostMethod = "AddReport";
                    AddReport(collection, content, login);
                    break;

                default:
                    break;
            }

            if (redirect)
            {
                return RedirectToAction("Index", "Catalog");
            }
            else
            {
                return View(content);
            }
        }

        private CatalogModel Run(FormCollection collection, CatalogModel content)
        {            
            foreach (var item in content.ReportParameters)
            {
                item.SelectedValue = collection[item.Name];
                if (string.IsNullOrEmpty(item.SelectedValue))
                {
                    if (item.ParameterType.ToString().Equals("Float", StringComparison.OrdinalIgnoreCase) || item.ParameterType.ToString().Equals("Integer", StringComparison.OrdinalIgnoreCase))
                    {
                        item.SelectedValue = "0";
                    }
                }
            }
            content.RunReport(Request.Url.Host);
            return content;
        }
        private void Delete(LoginModel login, CatalogModel content)
        {
            try
            {
                //Delete item in SRS
                content.DeleteItem(login.AuthenticationUser);
                RefreshCatalog(login, content);
            }
            catch (Exception e)
            {
                throw e;
            }            
        }

        private void RefreshCatalog(LoginModel login, CatalogModel content)
        {
            //Refresh the catalog model
            content = CatalogModel.GetCatalogItems(login.AuthenticationUser, content.SRSProxy);

            //Replace the catalog session with the updated one
            Session.Remove("Catalog");
            Session.Add("Catalog", content);
        }
        private bool SaveDatasource(FormCollection collection, CatalogModel content, LoginModel login)
        {
            try
            {
                string address = string.Empty;
                foreach (var item in content.DataSourceItem.itemProperties)
                {
                    item.Value = collection[item.PropertyName];
                }

                if (content.ItemType.Equals("Folder"))
                {
                    address = content.Path.Substring(content.Path.IndexOf('/'));
                }
                else
                {
                    address = content.Parent.Substring(content.Parent.IndexOf('/'));
                }

                content.SaveDataSource(login.AuthenticationUser, address);
                RefreshCatalog(login, content);
                return true;
            }
            catch (Exception e)
            {
                content.Error = e;
                return false;
            }
        }

        private void AddReport(FormCollection collection, CatalogModel content, LoginModel login)
        {
            try
            {
                content.ReportItem.FilePath = collection["ReportFilePath"];
                content.ReportItem.Name = collection["ReportName"];
                content.AddReport();
            }
            catch (Exception e)
            {
                content.Error = e;                
            }
        }
        #endregion

    }
}
