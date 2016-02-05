// ---------------------------------------------------------------------
// <copyright file="ErrorController.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-10-09 10:31:15
// * History: Last Modified By wansan 2015-10-09 10:31:16
// </copyright>
// ---------------------------------------------------------------------
using System.Web.Mvc;

/// <summary>
/// The $safeprojectname$.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// 错误信息
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// 错误页面
        /// </summary>
        /// <returns>页面</returns>
        public ActionResult Error()
        {
            string error = string.Empty;
            string exceptionID = string.Empty;
            string trackID = string.Empty;
            string returnUrl = string.Empty;
            error = Request.QueryString["error"];
            exceptionID = Request.QueryString["exceptionID"];
            trackID = Request.QueryString["trackID"];
            returnUrl = Request.QueryString["returnUrl"];
            this.ViewData["Error"] = error;
            this.ViewData["ExceptionID"] = exceptionID;
            this.ViewData["TrackID"] = trackID;
            this.ViewData["returnUrl"] = returnUrl;
            return this.View("Error");
        }
    }
}
