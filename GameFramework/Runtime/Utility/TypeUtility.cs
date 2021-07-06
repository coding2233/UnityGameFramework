using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Wanderer.GameFramework
{
    public class TypeUtility
    {
        private static List<Type> _allAssemblyTypes;
        /// <summary>
        /// 所有程序集的类型
        /// </summary>
        /// <value></value>
        public static List<Type> AllAssemblyTypes
        {
            get
            {
                if (_allAssemblyTypes == null)
                {
                    _allAssemblyTypes = new List<Type>();
                    //获取所有程序的类型
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        _allAssemblyTypes.AddRange(assemblies[i].GetTypes());
                    }
                }
                return _allAssemblyTypes;
            }
        }

        private static List<Type> _assemblyTypes;
        /// <summary>
        /// 所有程序集的类型
        /// </summary>
        /// <value></value>
        public static List<Type> AssemblyTypes
        {
            get
            {
                if (_assemblyTypes == null)
                {
                    _assemblyTypes = new List<Type>();
                    _assemblyTypes.AddRange(typeof(TypeUtility).Assembly.GetTypes());
                }
                return _assemblyTypes;
            }
        }

    }
}

