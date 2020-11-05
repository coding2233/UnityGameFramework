//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #ui类标记# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月22日 16点55分# </time>
//-----------------------------------------------------------------------


using System;

namespace Wanderer.GameFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UIViewAttribute : Attribute
    {
        public string AssetBundleName { get; private set; }
        public string ViewPath { get; private set; }

        public UIViewAttribute(string assetBundleName, string viewPath)
        {
            AssetBundleName = assetBundleName;
            ViewPath = viewPath;
        }
    }
}
