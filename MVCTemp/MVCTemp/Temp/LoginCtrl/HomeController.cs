// ---------------------------------------------------------------------
// <copyright file="HomeController.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-10-12 09:48:51
// * History: Last Modified By wansan 2015-10-12 09:48:51
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Better517Na.$safeprojectname$.BaseCtrl;

/// <summary>
/// The $safeprojectname$ namespace.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// Class HomeController.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult Index()
        {
            return this.View();
        }
    }
}
