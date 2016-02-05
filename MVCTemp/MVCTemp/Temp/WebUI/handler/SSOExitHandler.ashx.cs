// ---------------------------------------------------------------------
// <copyright file="SSOExitHandler.ashx.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-05-28 10:27:45
// * History: Last Modified By wansan 2015-05-28 10:28:04
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Better517Na.SSO.Client;

/// <summary>
/// The LoginCtrl namespace.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// $codebehindclassname$ 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SSOExitHandler : IHttpHandler
    {
        /// <summary>
        /// 是否独占
        /// </summary>
        /// <value><c>true</c> if this instance is reusable; otherwise, <c>false</c>.</value>
        /// <returns>如果 <see cref="T:System.Web.IHttpHandler" /> 实例可再次使用，则为 true；否则为 false。</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context">当前上下文</param>
        public void ProcessRequest(HttpContext context)
        {
            this.DoWork(context);
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void DoWork(HttpContext context)
        {
            string result = string.Empty;
            string actionType = context.Request.QueryString["type"];

            switch (actionType.ToUpper())
            {
                case "EXIT":
                    // 获取退出站点的Url
                    result = this.GetExitUrl(context);
                    break;
            }

            context.Response.Write(result);
        }

        /// <summary>
        /// 获取SSO 退出URL
        /// </summary>
        /// <param name="context">http请求上下文</param>
        /// <returns>SSO 退出URL</returns>
        private string GetExitUrl(HttpContext context)
        {
            DefaultClient ssoClient = new DefaultClient();

            return ssoClient.GetExitUrl();
        }
    }
}