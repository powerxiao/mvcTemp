// ---------------------------------------------------------------------
// <copyright file="IpLocator.cs" company="517Na">
// * Copyright (c) 2014 517Na科技有限公司 版权所有.
// * Author : wansan
// * Create : create by wansan 2015-05-28 10:10:04
// * History: Last Modified By wansan 2015-05-28 11:11:44
// </copyright>
// ---------------------------------------------------------------------
/************************************************************
IP数据库、手机归属地查询软件完整源代码（C#）
  Author: rssn
  Email : rssn@163.com
  QQ    : 126027268
  Blog  : http://blog.csdn.net/rssn_net/
 ************************************************************/
using System;
using System.IO;

/// <summary>
/// The $safeprojectname$ namespace.
/// </summary>
namespace $namespaceName$
{
    /// <summary>
    /// IpLocator类
    /// </summary>
    public class IpLocator
    {
        /// <summary>
        /// Gets the ip location.
        /// </summary>
        /// <param name="fn">The function.</param>
        /// <param name="ips">The ips.</param>
        /// <returns>IpLocation.</returns>
        /// <exception cref="System.Exception">文件不存在!</exception>
        public static IpLocation GetIpLocation(string fn, string ips)
        {
            if (!File.Exists(fn))
            {
                throw new Exception("文件不存在!");
            }

            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader fp = new BinaryReader(fs);

            // 读文件头,获取首末记录偏移量
            int fo = fp.ReadInt32();
            int lo = fp.ReadInt32();

            // IP值
            uint ipv = IpStringToInt(ips);

            // 获取IP索引记录偏移值
            int rcipOffset = GetIndexOffset(fs, fp, fo, lo, ipv);
            fs.Seek(rcipOffset, System.IO.SeekOrigin.Begin);

            IpLocation ipl;
            if (rcipOffset >= 0)
            {
                fs.Seek(rcipOffset, System.IO.SeekOrigin.Begin);

                // 读取开头IP值
                ipl.IpStart = fp.ReadUInt32();

                // 转到记录体
                fs.Seek(ReadInt24(fp), System.IO.SeekOrigin.Begin);

                // 读取结尾IP值
                ipl.IpEnd = fp.ReadUInt32();
                ipl.Country = GetString(fs, fp);
                ipl.City = GetString(fs, fp);
            }
            else
            {
                // 没找到
                ipl.IpStart = 0;
                ipl.IpEnd = 0;
                ipl.Country = "未知国家";
                ipl.City = "未知地址";
            }

            fp.Close();
            fs.Close();
            return ipl;
        }

        /// <summary>
        /// 函数功能: 采用“二分法”搜索索引区, 定位IP索引记录位置
        /// </summary>
        /// <param name="fs">The fs.</param>
        /// <param name="fp">The fp.</param>
        /// <param name="ufo">The ufo.</param>
        /// <param name="ulo">The ulo.</param>
        /// <param name="ipv">The ipv.</param>
        /// <returns>System.Int32.</returns>
        public static int GetIndexOffset(FileStream fs, BinaryReader fp, int ufo, int ulo, uint ipv)
        {
            int fo = ufo, lo = ulo;
            int mo;    // 中间偏移量
            uint mv;    // 中间值
            uint fv, lv; // 边界值
            uint llv;   // 边界末末值
            fs.Seek(fo, System.IO.SeekOrigin.Begin);
            fv = fp.ReadUInt32();
            fs.Seek(lo, System.IO.SeekOrigin.Begin);
            lv = fp.ReadUInt32();

            // 临时作它用,末记录体偏移量
            mo = ReadInt24(fp);
            fs.Seek(mo, System.IO.SeekOrigin.Begin);
            llv = fp.ReadUInt32();

            // 边界检测处理
            if (ipv < fv)
            {
                return -1;
            }
            else if (ipv > llv)
            {
                return -1;
            }

            do
            {
                // 使用"二分法"确定记录偏移量
                int temMo = (lo - fo) / 7;
                temMo = temMo / 2;
                temMo = temMo * 7;
                mo = fo + temMo;
                fs.Seek(mo, System.IO.SeekOrigin.Begin);
                mv = fp.ReadUInt32();
                if (ipv >= mv)
                {
                    fo = mo;
                }
                else
                {
                    lo = mo;
                }

                if (lo - fo == 7)
                {
                    mo = lo = fo;
                }
            }
            while (fo != lo);

            return mo;
        }

        /// <summary>
        /// Determines whether the specified s is numeric.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns><c>true</c> if the specified s is numeric; otherwise, <c>false</c>.</returns>
        public static bool IsNumeric(string s)
        {
            if (s != null && System.Text.RegularExpressions.Regex.IsMatch(s, @"^-?\d+$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Ips the string to int.
        /// </summary>
        /// <param name="strIpString">The ip string.</param>
        /// <returns>System.UInt32.</returns>
        public static uint IpStringToInt(string strIpString)
        {
            uint ipv = 0;
            string[] strIpStringArray = strIpString.Split('.');
            int i;
            uint ipi;
            for (i = 0; i < 4 && i < strIpStringArray.Length; i++)
            {
                if (IsNumeric(strIpStringArray[i]))
                {
                    ipi = (uint)Math.Abs(Convert.ToInt32(strIpStringArray[i]));
                    if (ipi > 255)
                    {
                        ipi = 255;
                    }

                    ipv += ipi << ((3 - i) * 8);
                }
            }

            return ipv;
        }

        /// <summary>
        /// Ints to ip string.
        /// </summary>
        /// <param name="ipv">The ipv.</param>
        /// <returns>System.String.</returns>
        public static string IntToIpString(uint ipv)
        {
            string strIpString = string.Empty;
            strIpString += (ipv >> 24) + "." + ((ipv & 0x00FF0000) >> 16) + "." + ((ipv & 0x0000FF00) >> 8) + "." + (ipv & 0x000000FF);
            return strIpString;
        }

        /// <summary>
        /// 获得真实IP地址
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>System.String.</returns>
        public static string GetIpAddr(System.Web.HttpRequest request)
        {
            string ip = string.Empty;

            try
            {
                ip = request.Headers["x-forwarded-for"];
                if (ip == null || ip.Length == 0 || "unknown".ToUpper() == ip.ToUpper())
                {
                    ip = request.Headers["Proxy-Client-IP"];
                }
                else
                {
                    ip = ip.Split(',')[0];
                }

                if (ip == null || ip.Length == 0 || "unknown".ToUpper() == ip.ToUpper())
                {
                    ip = request.Headers["WL-Proxy-Client-IP"];
                }
                else
                {
                    ip = ip.Split(',')[0];
                }

                if (ip == null || ip.Length == 0 || "unknown".ToUpper() == ip.ToUpper())
                {
                    ip = request.UserHostAddress;
                }
            }
            catch
            {
                ip = request.UserHostAddress;
            }

            return string.IsNullOrEmpty(ip) ? string.Empty : ip;
        }

        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="fp">The fp.</param>
        /// <returns>System.String.</returns>
        private static string ReadString(BinaryReader fp)
        {
            byte[] tempByteArray = new byte[128];
            int i = 0;
            do
            {
                tempByteArray[i] = fp.ReadByte();
            }
            while (tempByteArray[i++] != '\0' && i < 128);

            return System.Text.Encoding.Default.GetString(tempByteArray).TrimEnd('\0');
        }

        /// <summary>
        /// Reads the int24.
        /// </summary>
        /// <param name="fp">The fp.</param>
        /// <returns>System.Int32.</returns>
        private static int ReadInt24(BinaryReader fp)
        {
            if (fp == null)
            {
                return -1;
            }

            int ret = 0;
            ret |= (int)fp.ReadByte();
            ret |= (int)fp.ReadByte() << 8 & 0xFF00;
            ret |= (int)fp.ReadByte() << 16 & 0xFF0000;
            return ret;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="fs">The fs.</param>
        /// <param name="fp">The fp.</param>
        /// <returns>System.String.</returns>
        private static string GetString(FileStream fs, BinaryReader fp)
        {
            byte tag;
            int offset;
            tag = fp.ReadByte();
            if (tag == 0x01)
            {
                // 重定向模式1: 城市信息随国家信息定向
                offset = ReadInt24(fp);
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                return GetString(fs, fp);
            }
            else if (tag == 0x02)
            {
                // 重定向模式2: 城市信息没有随国家信息定向
                offset = ReadInt24(fp);
                int tmpOffset = (int)fs.Position;
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                string tmpString = GetString(fs, fp);
                fs.Seek(tmpOffset, System.IO.SeekOrigin.Begin);
                return tmpString;
            }
            else
            {
                fs.Seek(-1, System.IO.SeekOrigin.Current);
                return ReadString(fp);
            }
        }

        /// <summary>
        /// Struct aIpLocation
        /// </summary>
        public struct IpLocation
        {
            /// <summary>
            /// The ip start
            /// </summary>
            public uint IpStart;

            /// <summary>
            /// The ip end
            /// </summary>
            public uint IpEnd;

            /// <summary>
            /// The country
            /// </summary>
            public string Country;

            /// <summary>
            /// The city
            /// </summary>
            public string City;
        }
    }
}