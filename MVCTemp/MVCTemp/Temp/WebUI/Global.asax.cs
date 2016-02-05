// ---------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-10-10 19:34:55
// * History: Last Modified By wansan 2015-10-10 19:34:55
// </copyright>
// ---------------------------------------------------------------------
using System.Web.Mvc;
using System.Web.Routing;

/// <summary>
/// The RefundChangeManageSystemWeb namespace.
/// </summary>
namespace $namespaceName$
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    /// <summary>
    /// Class MvcApplication.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application_s the start.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}