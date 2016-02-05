// ---------------------------------------------------------------------
// <copyright file="UserManager.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-09-29 11:46:02
// * History: Last Modified By wansan 2015-09-29 11:58:44
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Better.Infrastructures.Log;
using Better517Na.UserLogin.Model;

/// <summary>
/// The $safeprojectname$.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// 获取用户对象
        /// </summary>
        /// <returns>用户对象</returns>
        public static MStaffInfo GetUser()
        {
            MStaffInfo modelStaff = HttpContext.Current.Session["$safeprojectname$_UserInfo"] as MStaffInfo;
            if (HttpContext.Current.Session["$safeprojectname$_UserInfo"] != null)
            {
                return HttpContext.Current.Session["$safeprojectname$_UserInfo"] as MStaffInfo;
            }

            TrackID.GetInstance(HttpContext.Current.User.Identity.Name);
            UserLoginServiceHelper.UserLoginServiceHelper userHelper = new UserLoginServiceHelper.UserLoginServiceHelper();
            MStaffInfo staff = userHelper.GetStaffInfoModel(HttpContext.Current.User.Identity.Name);
            HttpContext.Current.Session["$safeprojectname$_UserInfo"] = modelStaff;
            return modelStaff;
        }
    }
}
