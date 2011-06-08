using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using Navitaire.Reporting.Services.Authentication;

namespace Navitaire.Reporting.Web.Models
{
    public class LoginModel
    {
        public static string ErrorMessage = string.Empty;
        #region Properties
        
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string Domain { get; set; }
        public AuthenticatedUserManager AuthenticationUser 
        { 
            get 
            {
                AuthenticatedUserManager authenticationUser = new AuthenticatedUserManager(Username, Password, Domain);
                return authenticationUser; 
            } 
        }        
        #endregion

        #region Methods
        public static bool Login(string username, string password, string domain)
        {
            try
            {
                if (AuthenticatedUserManager.Authenticate(username, password, domain))
                {
                    FormsAuthentication.SetAuthCookie(username, false);                    
                    return true;
                }
                else
                {
                    return false;
                }
            }

            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        public static void Logout(LoginModel login)
        {
            if (login != null)
            {
                login.AuthenticationUser.Dispose();
            }
            FormsAuthentication.SignOut();
        }


        #endregion

    }

    public class Domain
    {
        public int DomainId { get; set; }
        public string DomainName { get; set; }
    }

    public static class DomainList
    {
        public static IEnumerable<Domain> Domains = new List<Domain>
        {
            new Domain {DomainId = 0, DomainName = "SYS"},
            new Domain {DomainId = 1, DomainName = "DEF"},
            new Domain {DomainId = 2, DomainName = "EXT"}
        };
    }
}