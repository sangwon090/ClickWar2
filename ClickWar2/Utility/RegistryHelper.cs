﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ClickWar2.Utility
{
    public class RegistryHelper
    {
        public static string GroupName
        { get; set; } = "ClickWar2";

        protected static RegistryKey GetSubKey()
        {
            var reg = Registry.CurrentUser.OpenSubKey(GroupName, true);

            if (reg == null)
                reg = Registry.CurrentUser.CreateSubKey(GroupName, RegistryKeyPermissionCheck.ReadWriteSubTree);


            return reg;
        }

        public static string GetData(string key, string defaultResult)
        {
            var reg = GetSubKey();


            var val = reg.GetValue(key);

            if (val == null)
                return defaultResult;

            return (string)val;
        }

        public static bool GetDataAsBool(string key, bool defaultResult)
        {
            var reg = GetSubKey();


            var val = reg.GetValue(key);

            if (val == null)
                return defaultResult;

            return (GetData(key, defaultResult.ToString()).ToLower() == "true");
        }

        public static void SetData<T>(string key, T data)
        {
            var reg = GetSubKey();


            reg.SetValue(key, data);
        }
    }
}
