// ---------------------------------------------------------------------
// <copyright file="SSOURL.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-06-02 10:58:08
// * History: Last Modified By wansan 2015-06-02 15:08:59
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Better.AuthenticateManagement;
using Better517Na.Common.SysParam;
using Better517Na.UserLogin.Model;

/// <summary>
/// The Base namespace.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// Class SSOURL.
    /// </summary>
    public class SSOURL
    {
        /// <summary>
        /// 可以通过单点登录的进行自动登录的网站 跳转 构造跳转字符串
        /// </summary>
        /// <param name="staffModel">登录信息</param>
        /// <param name="siteUrl">访问URL</param>
        /// <param name="autoLoginUrl">登录页面</param>
        /// <param name="pageUrl">The page URL.</param>
        /// <param name="token">token</param>
        /// <returns>返回地址</returns>
        public static string LoginSSOAddress(MStaffInfo staffModel, string siteUrl, string autoLoginUrl, string pageUrl, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                token = GetToken(staffModel);
            }

            return string.Format("{0}{1}?PartnerId={2}&AccountId={3}&DeptId={4}&DeptShortName={5}&ReqType=0&Token={6}&RequestPage={7}", siteUrl, autoLoginUrl, "Change", staffModel.Staff_id, staffModel.Department_id.ToString(), HttpUtility.UrlEncode(staffModel.Department, Encoding.GetEncoding("gb2312")), token, System.Web.HttpUtility.UrlEncode(pageUrl));
        }

        /// <summary>
        /// 获取单点登录url
        /// </summary>
        /// <param name="type">网站简称</param>
        /// <returns>System.String.</returns>
        public static string GetGJUrl(string type)
        {
            MStaffInfo staff = UserManager.GetUser();
            string taken = SSOURL.GetToken(staff);
            if (type == "GJ")
            {
                // 国际票
                string url = string.IsNullOrEmpty(SSOURL.GetSystemParamValue("GNJP_TP_InterPTURL")) ? "http://gj.517na.com" : SSOURL.GetSystemParamValue("GNJP_TP_InterPTURL");
                return LoginSSOAddress(staff, url, "/Login/Index", string.Empty, taken);
            }
            else if (type == "JD")
            {
                // 酒店
                string url = string.IsNullOrEmpty(SSOURL.GetSystemParamValue("GNJP_TP_JDPTURL")) ? "http://jd.517na.com" : SSOURL.GetSystemParamValue("GNJP_TP_JDPTURL");
                return LoginSSOAddress(staff, url, "/Login/Index", string.Empty, taken);
            }
            else if (type == "BX")
            {
                // 保险
                string url = string.IsNullOrEmpty(SSOURL.GetSystemParamValue("GNJP_TP_BXPTURL")) ? "http://bx.517na.com" : SSOURL.GetSystemParamValue("GNJP_TP_BXPTURL");
                return LoginSSOAddress(staff, url, "/Login.aspx", string.Empty, taken);
            }
            else if (type == "SJ")
            {
                // 手机
                string url = "http://app.517na.com";
                return LoginSSOAddress(staff, url, string.Empty, string.Empty, taken);
            }

            return "#";
        }

        /// <summary>
        /// 获取系统参数
        /// </summary>
        /// <param name="sysParamId">参数名</param>
        /// <returns>参数值</returns>
        public static string GetSystemParamValue(string sysParamId)
        {
            SysParamHelper.CacheInit("GNJP_TP_", 120);
            string sysParamValue = SysParamHelper.GetParamById<string>(sysParamId, string.Empty);
            return sysParamValue;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="staff">The staff.</param>
        /// <returns>System.String.</returns>
        private static string GetToken(MStaffInfo staff)
        {
            if (staff != null)
            {
                string crossOOSerivceUrl = ConfigurationManager.AppSettings["SOOUrl"] + "/LoginInfoValidate.aspx";
                AuthenticateManager authManager = new AuthenticateManager(crossOOSerivceUrl, "Change", staff.Staff_id, staff.Department_id.ToString(), staff.Department, string.Empty);
                if (authManager.Login())
                {
                    return authManager.Token;
                }
            }

            return string.Empty;
        }
    }
}
