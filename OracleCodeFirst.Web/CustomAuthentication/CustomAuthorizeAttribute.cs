using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OracleCodeFirst.Web.CustomAuthentication
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!string.IsNullOrEmpty(Roles))
            {
                return (CurrentUser != null && CurrentUser.IsInRole(Roles) && CurrentUser.UserId!=null) ? true : false;
            }
            else
            {
                return (CurrentUser.UserId == null && string.IsNullOrEmpty(CurrentUser.FirstName)) ? false : true;
            }           
            

            //bool isTrue = (CurrentUser.UserId == null && !CurrentUser.IsInRole(CurrentUser.Roles.ToString())) ? false : true;
            //return ((CurrentUser != null && !CurrentUser.IsInRole(Roles)) || CurrentUser != null) ? false : true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            RedirectToRouteResult routeData = null;

            if (CurrentUser == null)
            {
                routeData = new RedirectToRouteResult
                    (new System.Web.Routing.RouteValueDictionary
                    (new
                    {
                        controller = "Account",
                        action = "Login",
                    }
                    ));
            }
            else
            {
                routeData = new RedirectToRouteResult
                (new System.Web.Routing.RouteValueDictionary
                 (new
                 {
                     controller = "Error",
                     action = "AccessDenied"
                 }
                 ));
            }

            filterContext.Result = routeData;
        }

    }
}