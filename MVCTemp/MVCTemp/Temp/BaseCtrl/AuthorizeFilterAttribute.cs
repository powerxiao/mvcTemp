// ---------------------------------------------------------------------
// <copyright file="AuthorizeFilterAttribute.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-10-09 09:53:06
// * History: Last Modified By wansan 2015-10-09 10:40:35
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

/// <summary>
/// The namespaceName namespace.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// Class AuthorizeFilterAttribute.
    /// </summary>
    public class AuthorizeFilterAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 验证失败后执行
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                context.Result = new HttpStatusCodeResult(309);  // 尽量不要与现有的Http状态码冲突
                context.HttpContext.Response.Write("登录已过期，请重新登陆!"); // 可以根据需要返回其他信息
            }
            else
            {
                base.HandleUnauthorizedRequest(context);
            }
        }
    }
}
