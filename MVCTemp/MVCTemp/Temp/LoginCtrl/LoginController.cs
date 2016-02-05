// ---------------------------------------------------------------------
// <copyright file="LoginController.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-05-28 10:10:04
// * History: Last Modified By wansan 2015-05-28 11:11:44
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Better.AuthenticateManagement;
using Better.Infrastructures.Log;
using Better517na.Core.AsyncInfrastructure;
using Better517Na.$safeprojectname$.BaseCtrl;
using Better517Na.SSO.Client;
using Better517Na.UserDataManager.Contract;
using Better517Na.UserDataManager.Model;
using Better517Na.UserLogin.Model;
using Better517Na.WCF.IUserLoginService;
using ValidCodeCreate;

/// <summary>
/// The $safeprojectname$ namespace.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// 登陆
    /// </summary>
    public class LoginController : Controller
    {
        /// <summary>
        /// 运行时间变量
        /// </summary>
        private Stopwatch watch = new Stopwatch();

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns>视图</returns>
        public ActionResult Login()
        {
            return this.View();
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns>结果</returns>
        public ActionResult Index()
        {
            ClearCK1Cookie();

            if (SSOClientConfigHelper.SSOType == 0)
            {
                return this.SSOUrl();
            }
            else if (SSOClientConfigHelper.SSOType == 1)
            {
                ////有域名共享cookie方式，主要适用于大平台(自己域名)，同时生成主域名凭证
                return this.SSOCookie();
            }
            else if (SSOClientConfigHelper.SSOType == 2)
            {
                ////兼容方式：先1后0(过渡期间使用)
                if (this.Request.Cookies["Ticket"] != null)
                {
                    // 新单点登陆
                    string uid = string.Empty;
                    try
                    {
                        DefaultClient ssoclient = new DefaultClient();
                        string msg = string.Empty;
                        uid = ssoclient.GetUserTicket();
                        if (!ssoclient.CheckLogin(out msg))
                        {
                            this.TempData["ResMsg"] = "获取账号信息失败";
                            return this.View("ErrorPage");
                        }
                        else
                        {
                            // 权限验证
                            uid = ssoclient.GetUserTicket();

                            TrackIdManager.GetInstance(uid);

                            if (!string.IsNullOrEmpty(uid))
                            {
                                UserLoginServiceHelper.UserLoginServiceHelper userHelper =
                                    new UserLoginServiceHelper.UserLoginServiceHelper();
                                MStaffInfo staffInfo = userHelper.GetStaffInfoModel(uid);
                                ////登录用户不为平台时限制ip
                                int staffType = staffInfo.StaffType;
                                if (staffType != 1)
                                {
                                    if (!userHelper.LimitIpLogin(staffInfo.Department_id, this.GetIpAddr()))
                                    {
                                        return this.Json("当前登录IP不在允许的登录IP范围内！", "text/html", JsonRequestBehavior.AllowGet);
                                    }
                                }

                                if (staffInfo.StaffType != 1)
                                {
                                    this.TempData["ResMsg"] = "当前账号无权限";
                                    return this.View("ErrorPage");
                                }

                                //// 登陆成功
                                FormsAuthentication.SetAuthCookie(staffInfo.Staff_id, false);

                                //// TODO 保存用户部门对象
                                this.Session["$sessionName$_UserInfo"] = staffInfo;

                                // 登录成功，创建本地票据
                                this.SetLocalTicket(staffInfo);

                                //// 页面跳转
                                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["RequestPage"]))
                                {
                                    return this.Redirect("~/" + HttpUtility.UrlDecode(System.Web.HttpContext.Current.Request["RequestPage"]));
                                }
                                else
                                {
                                    return this.Redirect("~/Home/Index");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 单点登录失败吃掉异常
                        AppException appEx = new AppException(string.Empty, ex.Message, ex, null);
                        LogManager.Log.WriteException(appEx);
                    }
                }

                return this.SSOUrl();
            }

            return this.SSOUrl();
        }

        /// <summary>
        /// 登陆按钮
        /// </summary>
        /// <param name="userID">用户名</param>
        /// <param name="pwd">用户密码</param>
        /// <param name="validateCode">验证码</param>
        /// <returns>结果</returns>
        public JsonResult LoginIndex(string userID, string pwd, string validateCode)
        {
            string errMsg = string.Empty;
            string result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(userID))
                {
                    result = "请输入用户名！";
                    return this.Json("请输入用户名！");
                }

                if (string.IsNullOrEmpty(pwd))
                {
                    result = "请输入密码！";
                    return this.Json("请输入密码！");
                }

                TrackIdManager.GetInstance(userID);

                if (string.IsNullOrEmpty(validateCode))
                {
                    ////TODO 提示输入验证码
                    result = "请输入验证码！";
                    return this.Json("请输入验证码！");
                }

                string sessionValidateCode = this.Session["validatecode"] == null ? string.Empty : this.Session["validatecode"].ToString();
                if (validateCode.Trim().ToLower() != sessionValidateCode.ToLower())
                {
                    ////TODO 提示验证码输入错误
                    result = "请输入验证码！";
                    return this.Json("验证码错误！");
                }

                ClearCK1Cookie();

                UserLoginServiceHelper.UserLoginServiceHelper userHelper = new UserLoginServiceHelper.UserLoginServiceHelper();

                string message = string.Empty;
                MLogin login = new MLogin();
                login.AccountId = userID;
                login.HostAddress = Request.UserHostAddress;

                MUserLoginInfo loginInfo = new MUserLoginInfo();
                loginInfo.Staff_Id = userID;
                loginInfo.Password = pwd;
                if (userHelper.CommonLogin(loginInfo, ref message, ref login))
                {
                    MStaffInfo staffInfo = userHelper.GetStaffInfoModel(userID);
                    ////登录用户不为平台时限制ip
                    int staffType = staffInfo.StaffType;
                    if (staffType != 1)
                    {
                       if (!userHelper.LimitIpLogin(staffInfo.Department_id, this.GetIpAddr()))
                       {
                           return this.Json("当前登录IP不在允许的登录IP范围内！", "text/html", JsonRequestBehavior.AllowGet);
                       }
                    }

                    //// 平台登陆 
                    if (staffInfo.StaffType != 1)
                    {
                        result = "当前账号无权限！";
                        return this.Json("当前账号无权限");
                    }

                    //// 登录成功，创建本地票据
                    this.SetLocalTicket(staffInfo);

                    //// TODO 保存用户对象
                    this.Session["$sessionName$_UserInfo"] = staffInfo;

                    FormsAuthentication.SetAuthCookie(userID, false);
                    result = "登陆成功";
                    return this.Json(result);
                }
                else
                {
                    result = "用户名或密码错误！";
                    return this.Json(result);
                }
            }
            catch (AppException app)
            {
                errMsg = app.Message;
                result = errMsg;
            }
            catch (Exception ex)
            {
                AppException app = new AppException(string.Empty, ex.Message, ex, null);
                LogManager.Log.WriteException(app);
                errMsg = app.Message;
                result = errMsg;
            }
            finally
            {
                string addr = string.Empty;
                try
                {
                    addr = IpLocator.GetIpLocation(System.Configuration.ConfigurationManager.AppSettings["IPFile"], this.GetIpAddr()).Country;
                }
                catch
                {
                }

                UiaccParam param = new UiaccParam();
                param.SysId = "您的网站名称";
                param.OperId = "登录";
                param.UiId = "点击登录按钮";
                param.UserIP = this.GetIpAddr();
                param.UserName = userID;
                param.KeyMessage = "您的网站名称用户登录" + "用户ID:" + userID + "登录结果：" + result + "登录域名:" + HttpContext.Request.Url.Authority + " 登录城市：" + addr;
                if (TrackIdManager.CurrentTrackID == null)
                {
                    TrackIdManager.GetInstance(param.UserName);
                }

                this.watch.Stop();
                param.TimeSpan = this.watch.Elapsed;
                Better.Infrastructures.Log.LogManager.Log.WriteUiAcc(param);
            }

            if (!string.IsNullOrEmpty(errMsg))
            {
                return this.Json("用户名或密码错误！");
            }

            return this.Json(string.Empty);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns>结果</returns>
        public ActionResult LoginOut()
        {
            FormsAuthentication.SignOut();

            ////单点登录退出
            try
            {
                if (SSOClientConfigHelper.SSOType != 0)
                {
                    DefaultClient ssoClient = new DefaultClient();
                    ssoClient.Logout();
                }
            }
            finally
            {
                Session.Clear();
                Session.Abandon();
                FormsAuthentication.SignOut();
            }

            ////return this.RedirectToAction("Index"); 
            return this.Redirect("Index?type=exit");
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns>结果</returns>
        public FileResult ValidateCodeImg()
        {
            ValidCodeCreateOp codeCreate = CreateValidType.CreateValidTypeChange();
            codeCreate.Execute();

            string validate = codeCreate.Code;

            this.Session["validatecode"] = validate;

            MemoryStream stream = new MemoryStream();
            codeCreate.CodeMap.Save(stream, ImageFormat.Jpeg);
            codeCreate.CodeMap.Dispose();

            return this.File(stream.ToArray(), "image/jpeg");
        }

        /// <summary>
        /// 清除单点登陆跨域请求标识
        /// </summary>
        private static void ClearCK1Cookie()
        {
            DefaultClient client = new DefaultClient();
            client.ClearCK1Cookie();
        }

        /// <summary>
        /// 跨域跳转日志
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="reqType">token类型</param>
        private static void WriteRedirectLog(string url, string reqType)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("ssotype:", SSOClientConfigHelper.SSOType);
            dic.Add("reqType:", reqType);
            dic.Add("url:", url);

            AccParam acc = new AccParam();
            acc.Direction = AccDirection.Response;
            acc.Method = "单点登录跳转";
            acc.ParasIn = dic;

            acc.SerivicAdress = string.Empty;
            acc.Comment = "单点登录跨域跳转";

            LogManager.Log.WriteServiceAcc(acc);
        }

        /// <summary>
        /// 跨域跳转日志(非设置ssoType类型)
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="reqType">token类型</param>
        private static void WriteRedirectLogDefault(string url, string reqType)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("ssotype:", -1);
            dic.Add("reqType:", reqType);
            dic.Add("url:", url);

            AccParam acc = new AccParam();
            acc.Direction = AccDirection.Response;
            acc.Method = "单点登录跳转";
            acc.ParasIn = dic;

            acc.SerivicAdress = string.Empty;
            acc.Comment = "单点登录跨域跳转";

            LogManager.Log.WriteServiceAcc(acc);
        }

        /// <summary>
        /// 新单点登录方式
        /// </summary>
        /// <returns>执行动作结果</returns>
        private ActionResult SSOCookie()
        {
            if (this.Request.Cookies["Ticket"] != null)
            {
                // 新单点登陆
                string uid = string.Empty;
                try
                {
                    DefaultClient ssoclient = new DefaultClient();
                    string msg = string.Empty;
                    uid = ssoclient.GetUserTicket();
                    if (!ssoclient.CheckLogin(out msg))
                    {
                        this.TempData["ResMsg"] = "获取账号信息失败";
                        return this.View("ErrorPage");
                    }
                    else
                    {
                        // 权限验证
                        uid = ssoclient.GetUserTicket();

                        TrackIdManager.GetInstance(uid);

                        if (!string.IsNullOrEmpty(uid))
                        {
                            UserLoginServiceHelper.UserLoginServiceHelper userHelper =
                                new UserLoginServiceHelper.UserLoginServiceHelper();
                            MStaffInfo staffInfo = userHelper.GetStaffInfoModel(uid);
                            ////登录用户不为平台时限制ip
                            int staffType = staffInfo.StaffType;
                            if (staffType != 1)
                            {
                               if (!userHelper.LimitIpLogin(staffInfo.Department_id, this.GetIpAddr()))
                               {
                                   return this.Json("当前登录IP不在允许的登录IP范围内！", "text/html", JsonRequestBehavior.AllowGet);
                               }
                            }

                            ////  平台才能登陆 
                            if (staffInfo.StaffType != 1)
                            {
                                this.TempData["ResMsg"] = "当前账号无权限";
                                return this.View("ErrorPage");
                            }

                            //// 登陆成功
                            FormsAuthentication.SetAuthCookie(staffInfo.Staff_id, false);

                            //// TODO 保存用户部门对象
                            this.Session["$sessionName$_UserInfo"] = staffInfo;

                            // 登录成功，创建本地票据
                            this.SetLocalTicket(staffInfo);

                            //// 页面跳转
                            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["RequestPage"]))
                            {
                                return this.Redirect("~/" + HttpUtility.UrlDecode(System.Web.HttpContext.Current.Request["RequestPage"]));
                            }
                            else
                            {
                                return this.Redirect("~/Home/Index");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 单点登录失败吃掉异常
                    AppException appEx = new AppException(string.Empty, ex.Message, ex, null);
                    LogManager.Log.WriteException(appEx);
                }
            }
            else
            {
                this.ViewData["result"] = null;
            }

            return this.View();
        }

        /// <summary>
        /// 老单点登录方式
        /// </summary>
        /// <returns>执行结果</returns>
        private ActionResult SSOUrl()
        {
            // 共享登陆
            if (this.Request["PartnerId"] != null)
            {
                try
                {
                    AuthenticateManager manager = new AuthenticateManager(
                        System.Web.HttpContext.Current.Request,
                        ConfigurationManager.AppSettings["SOOUrl"] + "/LoginInfoValidate.aspx");
                    //// 登陆成功
                    if (manager.Validate())
                    {
                        TrackIdManager.GetInstance(manager.AccountId);

                        //// 获取用户对象
                        UserLoginServiceHelper.UserLoginServiceHelper userHelper =
                            new UserLoginServiceHelper.UserLoginServiceHelper();

                        MStaffInfo staff = userHelper.GetStaffInfoModel(manager.AccountId);
                        ////登录用户不为平台时限制ip
                        int staffType = staff.StaffType;
                        if (staffType != 1)
                        {
                           if (!userHelper.LimitIpLogin(staff.Department_id, this.GetIpAddr()))
                           {
                               return this.Json("当前登录IP不在允许的登录IP范围内！", "text/html", JsonRequestBehavior.AllowGet);
                           }
                        }

                        ////  平台才能登陆 
                        if (staff.StaffType != 1)
                        {
                            this.TempData["ResMsg"] = "当前账号无权限";
                            return this.View("ErrorPage");
                        }
                        else
                        {
                            //// 登陆成功
                            FormsAuthentication.SetAuthCookie(staff.Staff_id, false);
                            this.SetLocalTicket(staff);

                            //// 保存会话Token
                            this.Session["Token"] = manager.Token;

                            //// TODO 保存用户对象
                            this.Session["$sessionName$_UserInfo"] = staff;

                            //// 页面跳转
                            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["RequestPage"]))
                            {
                                return this.Redirect("~/" + HttpUtility.UrlDecode(System.Web.HttpContext.Current.Request["RequestPage"]));
                            }
                            else
                            {
                                return this.Redirect("~/Home/Index");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 单点登录失败吃掉异常
                    AppException appEx = new AppException(string.Empty, ex.Message, ex, null);
                    LogManager.Log.WriteException(appEx);
                }
            }
            else
            {
                this.ViewData["result"] = null;
            }

            return this.View();
        }

        /// <summary>
        /// 设置单点登录本地Ticket
        /// </summary>
        /// <param name="staff">用户</param>
        private void SetLocalTicket(MStaffInfo staff)
        {
            try
            {
                if (SSOClientConfigHelper.SSOType == 1 || SSOClientConfigHelper.SSOType == 2)
                {
                    DefaultClient client = new DefaultClient();
                    client.SaveLocalTicket(staff.Staff_id);
                }
            }
            catch (Exception ex)
            {
                AppException appEx = new AppException(string.Empty, "认证中心故障：" + ex.Message, ex, null);
                LogManager.Log.WriteException(appEx);
            }
        }

        /// <summary>
        /// 获得真实IP地址
        /// </summary>
        /// <returns>String.</returns>
        private string GetIpAddr()
        {
            string ip = HttpContext.Request.Headers["x-forwarded-for"];
            if (ip == null || ip.Length == 0 || "unknown".ToUpper() == ip.ToUpper())
            {
                ip = HttpContext.Request.Headers["Proxy-Client-IP"];
            }
            else
            {
                ip = ip.Split(',')[0];
            }

            if (ip == null || ip.Length == 0 || "unknown".ToUpper() == ip.ToUpper())
            {
                ip = HttpContext.Request.Headers["WL-Proxy-Client-IP"];
            }
            else
            {
                ip = ip.Split(',')[0];
            }

            if (ip == null || ip.Length == 0 || "unknown".ToUpper() == ip.ToUpper())
            {
                ip = HttpContext.Request.UserHostAddress.ToString();
            }

            return ip;
        }
    }
}
