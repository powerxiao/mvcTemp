// ---------------------------------------------------------------------
// <copyright file="BaseController.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-10-09 10:31:15
// * History: Last Modified By wansan 2015-10-09 10:31:16
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Better.Infrastructures.Log;
using Better517Na.UserLogin.Model;

/// <summary>
/// The $safeprojectname$.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// 基类Controller
    /// </summary>
    [AuthorizeFilter]
    [BaseFilter]
    public class BaseController : Controller
    {
        /// <summary>
        /// 运行时间变量
        /// </summary>
        private Stopwatch watch = new Stopwatch();

        /// <summary>
        /// 用户对象
        /// </summary>
        /// <value>The staff.</value>
        public MStaffInfo Staff
        {
            get
            {
                MStaffInfo userModel = new MStaffInfo();
                if (HttpContext.Session["$safeprojectname$_UserInfo"] != null)
                {
                    userModel = HttpContext.Session["$safeprojectname$_UserInfo"] as MStaffInfo;
                }
                else
                {
                    userModel = UserManager.GetUser();
                }

                return userModel;
            }
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="filterContext">异常上下文</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            this.ProcessException(ex);
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="ex">ex</param>
        private void ProcessException(Exception ex)
        {
            string error = ExceptionHandler.ProcessException(ex);
            if (Request.IsAjaxRequest())
            {
                Response.StatusCode = 500;
                Response.Write("{");
                Response.Write(string.Format("\"message\":\"{0}\"", error));
                Response.Write("}");
                Response.End();
            }
            else
            {
                string errorMsg = string.Empty;
                errorMsg = "error=" + Regex.Split(error, "<br />")[0];
                errorMsg += "&exceptionID=" + Regex.Split(error, "<br />")[1];
                errorMsg += "&trackID=" + Regex.Split(error, "<br />")[2];
                errorMsg += "&returnUrl=" + HttpContext.Request.RawUrl;
                Response.Redirect("/Error/Error?" + errorMsg);
            }
        }

        /// <summary>
        /// 错误信息处理
        /// </summary>
        /// <param name="ex">ex</param>
        /// <returns>string</returns>
        private string SetProcessException(Exception ex)
        {
            string msg = string.Empty;
            try
            {
                Type type = ex.GetType();
                if (type.Name == "Exception" ||
                    type.Name == "ApplicationException" ||
                    type.Name == "CommandAnalysisException" ||
                    type.Name == "ComunicateToEMException" ||
                    type.Name == "ObjectInitException")
                {
                    AppException app = new AppException(string.Empty, ex.Message, ex, null);
                    LogManager.Log.WriteException(app);
                    ////msg = ex.Message;
                    msg = string.Format("{0}：{1}<br />{2} <br />{3}", app.Code, app.CustomerMessage, app.ID.ToString(), TrackIdManager.CurrentTrackID.StrTrackID);
                    return msg;
                }

                if (type.Name == "AppException")
                {
                    AppException app = ex as AppException;
                    msg = string.Format("{0}：{1}<br />{2} <br />{3}", app.Code, app.CustomerMessage, app.ID.ToString(), TrackIdManager.CurrentTrackID.StrTrackID);
                    return msg;
                }

                if (type.Name.StartsWith("FaultException"))
                {
                    FaultException<TransferException> fe = ex as FaultException<TransferException>;
                    if (fe != null)
                    {
                        AppException app = new AppException(fe.Detail);
                        msg = string.Format("{0}：{1}<br />{2}<br />{3}", app.Code, app.CustomerMessage, app.ID.ToString(), TrackIdManager.CurrentTrackID.StrTrackID);
                        return msg;
                    }
                }

                AppException appEx = new AppException(string.Empty, ex.Message, ex, null);
                LogManager.Log.WriteException(appEx);
                msg = string.Format("{0}：{1}<br />{2}<br />{3}", appEx.Code, appEx.CustomerMessage, appEx.ID.ToString(), TrackIdManager.CurrentTrackID.StrTrackID);
            }
            catch
            {
                // 不允许吃掉异常
                string str = string.Empty;
            }

            return msg;
        }
    }
}
