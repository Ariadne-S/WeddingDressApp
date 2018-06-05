using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Website.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                if (false) //filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    ProcessAjax(filterContext);
                }
                else
                {
                    ProcessNormal(filterContext);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        protected virtual void ProcessNormal(ActionExecutingContext filterContext)
        {
            // Export ModelState to TempData so it's available on next request
            //ExportModelStateToTempData(filterContext);

            // redirect back to GET action
            filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
        }

        protected static void ExportModelStateToTempData(ControllerContext context)
        {
            //context.HttpContext.te[Key] = context.ViewData.ModelState;
        }

        protected virtual void ProcessAjax(ActionExecutingContext filterContext)
        {
            //var errors = filterContext.ModelState.ToSerializableDictionary();
            //var json = new JavaScriptSerializer().Serialize(errors);

            // send 400 status code (Bad Request)
            //filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, json);
        }
    }
}
