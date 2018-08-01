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
using UnityEngine;

namespace HotFix.Taurus
{
    public sealed class DataTableManager : GameFrameworkModule
    {

		private readonly Dictionary<int, DataTableBase> _allDataTabvles;
		private GameFramework.Taurus.ResourceManager _resource;

	    public DataTableManager()
	    {
		    _allDataTabvles = new Dictionary<int, DataTableBase>();
		    _resource = GameFramework.Taurus.GameFrameworkMode.GetModule<GameFramework.Taurus.ResourceManager>();
	    }
        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleName">assetBundle的名称</param>
        /// <param name="dataTablePath">配置表的数据</param>
        /// <returns></returns>
        public void LoadDataTable<T>(string assetBundleName,string dataTablePath) where T :class, IDataTableRow,new()
	    {
		    string data= _resource.LoadAsset<TextAsset>(assetBundleName,dataTablePath).text;
		    DataTable<T> dataTable = new DataTable<T>();
		    string[] rows = data.Split('\n');
		    foreach (var item in rows)
		    {
				//排除多余的数据
			    if (string.IsNullOrEmpty(item) || rows.Length == 0 || rows[0] == "#")
				    continue;
			    dataTable.AddDataRow(item);
		    }
		    int hasCode = typeof(T).GetHashCode();
		    _allDataTabvles[hasCode] = dataTable;
		}

	    /// <summary>
	    /// 获取数据表
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    public IDataTable<T> GetDataTable<T>() where T : class, IDataTableRow, new()
	    {
		    DataTableBase dataTable = null;
		    int hashCode = typeof(T).GetHashCode();
		    if (_allDataTabvles.TryGetValue(hashCode, out dataTable))
			    return (IDataTable<T>)dataTable;
		    return null;
	    }



		public override void OnClose()
        {
	        _allDataTabvles.Clear();

        }
    }
}
