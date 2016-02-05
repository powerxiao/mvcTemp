// ---------------------------------------------------------------------
// <copyright file="SSOHandler.ashx.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-05-28 10:25:05
// * History: Last Modified By wansan 2015-05-28 10:25:05
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using Better517Na.$safeprojectname$.BaseCtrl;
using Better517Na.SSO.Client;
using Better517Na.UserLogin.Model;

/// <summary>
/// The ashx namespace.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// $codebehindclassname$ 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SSOHandler : IHttpHandler, IRequiresSessionState
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
        /// 通过实现 <see cref="T:System.Web.IHttpHandler" /> 接口的自定义 HttpHandler 启用 HTTP Web 请求的处理。
        /// </summary>
        /// <param name="context"><see cref="T:System.Web.HttpContext" /> 对象，它提供对用于为 HTTP 请求提供服务的内部服务器对象（如 Request、Response、Session 和 Server）的引用。</param>
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
                case "GETURL":
                    // 获取SSO 请求的Url
                    result = this.GetSSOUrl(context);
                    break;
            }

            context.Response.Write(result);
        }

        /// <summary>
        /// 获取SSO URL
        /// </summary>
        /// <param name="context">http请求上下文</param>
        /// <returns>SSO URL</returns>
        public string GetSSOUrl(HttpContext context)
        {
            MStaffInfo staffInfo = HttpContext.Current.Session["$safeprojectname$_UserInfo"] as MStaffInfo;

            string strReturn = "{\"status\":false,\"url\":\"\",\"flg\":false,\"domainUrl\":\"\"}";

            if (staffInfo == null)
            {
                return strReturn;
            }

            if (SSOClientConfigHelper.SSOType == 0)
            {
                return strReturn;
            }

            DefaultClient client = new DefaultClient();
            strReturn = client.GetSSOUrl(staffInfo.Staff_id, staffInfo.Password);

            return strReturn;
        }
    }
}