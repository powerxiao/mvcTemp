// ---------------------------------------------------------------------
// <copyright file="MenuManager.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-05-28 09:28:51
// * History: Last Modified By wansan 2015-05-28 10:02:51
// </copyright>
// ---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Better.Infrastructures.Log;
using Better517Na.UserLogin.Model;

/// <summary>
/// The $safeprojectname$.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    public static class MenuManager
    {
        /// <summary>
        /// GetUserMenus
        /// </summary>
        /// <param name="htmlHelper">htmlHelper</param>
        /// <param name="menu">menu</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString GetUserMenus(this HtmlHelper htmlHelper, string menu)
        {
            string webName = "您的网站名称";
            MStaffInfo mstaffInfo = HttpContext.Current.Session["$safeprojectname$_UserInfo"] as MStaffInfo;
            TrackIdManager.GetInstance(mstaffInfo.Staff_id);
            UserLoginServiceHelper.UserLoginServiceHelper userHelper = new UserLoginServiceHelper.UserLoginServiceHelper();

            // 获取菜单并放入session
            DataTable dtpower = null;

            var sessionName = "deptPower|$safeprojectname$Module_Menu";

            if (HttpContext.Current.Session[sessionName] != null)
            {
                dtpower = HttpContext.Current.Session[sessionName] as DataTable;
            }

            if (dtpower == null)
            {
                dtpower = userHelper.GetStaffPower(mstaffInfo.StaffType, mstaffInfo.Staff_id, mstaffInfo.Department_id, webName).Tables[0];
            }

            HttpContext.Current.Session[sessionName] = dtpower;

            DataRow[] mainMenu = dtpower.Select(" Node_Type = '-1' and IsMenu=1 ", "NodeSort");
            StringBuilder sb = new StringBuilder(2048);
            string node_Code = string.Empty;
            string title = string.Empty;
            string urlpage = string.Empty;
            string target = string.Empty;

            string token = HttpContext.Current.Session["Token"] as string;

            // 菜单展示
            string[] showMenuParam = null;

            showMenuParam = menu.Split('-');
            for (int i = 0; i < mainMenu.Length; i++)
            {
                node_Code = mainMenu[i]["Node_Code"].ToString();
                title = mainMenu[i]["Node_Name"].ToString();
                ////获得菜单项
                DataRow[] subMenu = dtpower.Select(string.Format(" PNode_Code='{0}' and IsMenu=1", node_Code), "NodeSort");
                ////添加菜单显示
                if (node_Code == "01" || (subMenu != null && subMenu.Length > 0))
                {
                    char[] visibledMenu = mainMenu[i]["visibled_Flag"].ToString().PadLeft(4, '0').ToCharArray();
                    if (visibledMenu[mstaffInfo.StaffType - 1] == '1')
                    {
                        sb.AppendFormat("<li><a href=\"#\" class=\"buy\" rel=\"buy\"><em></em>{0}</a>", title);

                        if (showMenuParam.Length > 1)
                        {
                            if (int.Parse(showMenuParam[0]) == i)
                            {
                                sb.AppendFormat("<ul style='display:block'>");
                            }
                            else
                            {
                                sb.AppendFormat("<ul>");
                            }
                        }
                        else
                        {
                            sb.AppendFormat("<ul>");
                        }
                        ////添加菜单项显示
                        for (int k = 0; k < subMenu.Length; k++)
                        {
                            node_Code = subMenu[k]["Node_Code"].ToString();
                            urlpage = subMenu[k]["Form_Name"].ToString();
                            title = subMenu[k]["Node_Name"].ToString();

                            if (showMenuParam.Length > 1)
                            {
                                if (int.Parse(showMenuParam[0]) == i && int.Parse(showMenuParam[1]) == k)
                                {
                                    sb.AppendFormat(" <li class='color0098c6'><a href=\"/{0}\">{1}</a></li>", urlpage, title);
                                }
                                else
                                {
                                    sb.AppendFormat(" <li><a href=\"/{0}\">{1}</a></li>", urlpage, title);
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" <li><a href=\"/{0}\">{1}</a></li>", urlpage, title);
                            }
                        }

                        sb.AppendFormat("</ul></li>");
                    }
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        /// <summary>
        /// 获取当前用户显示的大菜单节点
        /// </summary>
        /// <returns>大菜单节点</returns>
        public static string GetMenuIndex()
        {
            string menuIndex = string.Empty;
            if (HttpContext.Current.Session["MenuIndex"] == null)
            {
                HttpContext.Current.Session["MenuIndex"] = menuIndex = "h1";
            }
            else
            {
                menuIndex = HttpContext.Current.Session["MenuIndex"].ToString();
            }

            return menuIndex;
        }

        /// <summary>
        /// 设置当前用户显示的大菜单节点
        /// </summary>
        /// <param name="menuItem">menuItem</param>
        public static void SetMenuIndex(string menuItem)
        {
            if (string.IsNullOrEmpty(menuItem))
            {
                menuItem = "h0";
            }

            HttpContext.Current.Session["MenuIndex"] = menuItem;
        }
    }
}
