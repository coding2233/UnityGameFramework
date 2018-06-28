//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #配置文件管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月28日 16点03分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
    public sealed class DataTableManager : GameFrameworkModule
    {
        //
        //private Dictionary<string,I>

        public void LoadDataTable<T>(string dataTable) where T :class, IDataTableRow,new()
        {

        }




        public override void OnClose()
        {
        }
    }
}
