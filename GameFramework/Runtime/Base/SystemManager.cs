﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #系统管理类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年8月4日 14点52分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Wanderer.GameFramework
{
    public sealed class SystemManager : GameFrameworkModule
    {
        public Assembly GetAssembly { get; private set; }
        public Type[] GetTypes { get; private set; }

        public SystemManager()
        {
            GetAssembly = typeof(SystemManager).Assembly;
            GetTypes = GetAssembly.GetTypes();
        }

        public override void OnClose()
        {

        }
    }
}
