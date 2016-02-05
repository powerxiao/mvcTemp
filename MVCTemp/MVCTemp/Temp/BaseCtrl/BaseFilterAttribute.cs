// ---------------------------------------------------------------------
// <copyright file="BaseFilterAttribute.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-10-09 10:31:15
// * History: Last Modified By wansan 2015-10-09 10:31:16
// </copyright>
// ---------------------------------------------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Better.Infrastructures.Log;

/// <summary>
/// The $safeprojectname$.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// 登陆权限控制类
    /// </summary>
    public class BaseFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 运行时间变量
        /// </summary>
        private Stopwatch watch = new Stopwatch();

        /// <summary>
        /// 控制器执行之前
        /// </summary>
        /// <param name="filterContext">控制器执行上下文</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.watch.Reset();
            this.watch.Start();

            //// TODO 这里做权限的管理
            if (HttpContext.Current.Session["$safeprojectname$_UserInfo"] == null || !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("/");
            }
        }

        /// <summary>
        /// 控制器执行之后
        /// </summary>
        /// <param name="filterContext">控制器执行上下文</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                string pnr = string.Empty;
                string orderid = string.Empty;
                Dictionary<string, object> lists = new Dictionary<string, object>();
                try
                {
                    foreach (string item in filterContext.HttpContext.Request.Form.AllKeys)
                    {
                        if (filterContext.HttpContext.Request.Form[item].Trim().Length == 6)
                        {
                            pnr = filterContext.HttpContext.Request.Form[item];
                        }
                        else if (filterContext.HttpContext.Request.Form[item].Trim().Length == 18)
                        {
                            orderid = filterContext.HttpContext.Request.Form[item];
                        }

                        lists.Add(item, filterContext.HttpContext.Request.Form[item]);
                    }
                }
                catch
                {
                }
                finally
                {
                    UiaccParam param = new UiaccParam();
                    param.SysId = "您的网站名称"; // 填写网站名称
                    param.OperId = ((ControllerContext)filterContext).RouteData.Values["controller"].ToString();
                    param.UiId = ((ControllerContext)filterContext).RouteData.Values["action"].ToString();
                    param.UserIP = filterContext.HttpContext.Request.UserHostAddress;
                    param.Pnr = pnr;
                    param.OrderID = orderid;
                    param.UserName = HttpContext.Current.User.Identity.Name;
                    param.DicContext = lists;
                    if (TrackIdManager.CurrentTrackID == null)
                    {
                        TrackIdManager.GetInstance(param.UserName);
                    }

                    this.watch.Stop();
                    param.TimeSpan = this.watch.Elapsed;
                    Better.Infrastructures.Log.LogManager.Log.WriteUiAcc(param);
                }
            }
            catch
            {
            }

            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// 视图显示之前
        /// </summary>
        /// <param name="filterContext">视图执行上下文</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        /// 视图显示之后
        /// </summary>
        /// <param name="filterContext">视图执行上下文</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
    }
}
