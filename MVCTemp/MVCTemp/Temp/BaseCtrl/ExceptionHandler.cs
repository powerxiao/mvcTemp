// ---------------------------------------------------------------------
// <copyright file="ExceptionHandler.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-10-09 10:31:15
// * History: Last Modified By wansan 2015-10-09 10:31:16
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Better.Infrastructures.Log;

/// <summary>
/// The $safeprojectname$.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// Class ExceptionHandler.
    /// </summary>
    public class ExceptionHandler
    {
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>结果</returns>
        public static string ProcessException(Exception ex)
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
                if (TrackIdManager.CurrentTrackID == null)
                {
                    TrackIdManager.GetInstance("pt");
                }

                msg = string.Format("{0}：{1}<br />{2}<br />{3}", appEx.Code, appEx.CustomerMessage, appEx.ID.ToString(), TrackIdManager.CurrentTrackID.StrTrackID);
            }
            catch
            {
            }

            return msg;
        }
    }
}
