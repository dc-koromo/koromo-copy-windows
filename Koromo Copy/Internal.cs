﻿/***

   Copyright (C) 2018. dc-koromo. All Rights Reserved.
   
   Author: Koromo Copy Developer

***/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Koromo_Copy
{
    /// <summary>
    /// 오브젝트의 데이터를 가져오거나설정합니다.
    /// </summary>
    public class Internal
    {
        #region Low Level

        /// <summary>
        /// 모든 조건을 포함하는 바인딩 옵션
        /// </summary>
        public const BindingFlags DefaultBinding = BindingFlags.NonPublic |
                         BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        /// <summary>
        /// 매우 간단한 내용만 보여주는 바인딩 옵션
        /// </summary>
        public const BindingFlags CommonBinding = BindingFlags.Instance | BindingFlags.Public;

        public static List<FieldInfo> get_all_fields(Type t, BindingFlags flags)
        {
            if (t == null)
                return new List<FieldInfo>();
            
            var list = t.GetFields(flags).ToList();
            list.AddRange(get_all_fields(t.BaseType, flags));
            return list;
        }

        public static List<FieldInfo> enum_recursion(object obj, string[] bb, int ptr)
        {
            if (bb.Length == ptr)
            {
                return get_all_fields(obj.GetType(), DefaultBinding);
            }
            return enum_recursion(obj.GetType().GetField(bb[ptr], DefaultBinding).GetValue(obj), bb, ptr + 1);
        }

        public static List<FieldInfo> enum_recursion(object obj, string[] bb, int ptr, BindingFlags option)
        {
            if (bb.Length == ptr)
            {
                return obj.GetType().GetFields(option).ToList();
            }
            var x = obj.GetType().GetField(bb[ptr], DefaultBinding);
            return enum_recursion(obj.GetType().GetField(bb[ptr], DefaultBinding).GetValue(obj), bb, ptr + 1, option);
        }

        public static object get_recursion(object obj, string[] bb, int ptr)
        {
            if (bb.Length == ptr)
            {
                return obj;
            }
            return get_recursion(obj.GetType().GetField(bb[ptr], DefaultBinding).GetValue(obj), bb, ptr + 1);
        }

        public static void set_recurion(object obj, string[] bb, int ptr)
        {
            if (bb.Length - 2 == ptr)
            {
                obj.GetType().GetField(bb[ptr]).SetValue(obj,
                    Convert.ChangeType(bb[ptr + 1], obj.GetType().GetField(bb[ptr], DefaultBinding).GetValue(obj).GetType()));
                return;
            }
            set_recurion(obj.GetType().GetField(bb[ptr]).GetValue(obj), bb, ptr + 1);
        }

        public static List<MethodInfo> enum_methods(object obj, string[] bb, int ptr, BindingFlags option)
        {
            if (bb.Length == ptr)
            {
                return obj.GetType().GetMethods(option).ToList();
            }
            var x = obj.GetType().GetField(bb[ptr], DefaultBinding);
            return enum_methods(obj.GetType().GetField(bb[ptr], DefaultBinding).GetValue(obj), bb, ptr + 1, option);
        }

        public static object call_method(object obj, string[] bb, int ptr, BindingFlags option, object[] param)
        {
            if (bb.Length - 1 == ptr)
            {
                if (obj is Control)
                    return (obj as Control).Send(() => { return obj.GetType().GetMethods(option).Where(y => y.Name == bb[ptr]).ToList()[0].Invoke(obj, param); });
                else
                    return obj.GetType().GetMethods(option).Where(y => y.Name == bb[ptr]).ToList()[0].Invoke(obj, param);
            }
            var x = obj.GetType().GetField(bb[ptr], DefaultBinding);
            return call_method(obj.GetType().GetField(bb[ptr], DefaultBinding).GetValue(obj), bb, ptr + 1, option, param);
        }

        #endregion

    }
}