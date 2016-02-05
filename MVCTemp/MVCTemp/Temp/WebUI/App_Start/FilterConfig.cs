// ---------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-10-10 19:34:55
// * History: Last Modified By wansan 2015-10-10 19:34:55
// </copyright>
// ---------------------------------------------------------------------
using System.Web;
using System.Web.Mvc;

/// <summary>
/// The RefundChangeManageSystemWeb namespace.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// Class FilterConfig.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}