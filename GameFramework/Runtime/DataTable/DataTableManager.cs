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

namespace Wanderer.GameFramework
{
    public sealed class DataTableManager : GameFrameworkModule
    {
		private readonly Dictionary<string, DataTable> _allDataTables;
		private ResourceManager _resource;
		private ObjectPool<DataTable> _dataTablePool=new ObjectPool<DataTable>(null,(dataTable)=>{
			dataTable.Release();
		});

	    public DataTableManager()
	    {
		    _allDataTables = new Dictionary<string, DataTable>();
		    _resource = GameFrameworkMode.GetModule<ResourceManager>();
	    }

	    /// <summary>
	    /// 加载数据表
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="data">配置表的数据</param>
	    /// <returns></returns>
		public async void LoadDataTable(string dataTablePath)
	    {
			if(_allDataTables.ContainsKey(dataTablePath))
			 	return;
		    string data= (await _resource.LoadAsset<TextAsset>(dataTablePath)).text;
			DataTable dataTable = _dataTablePool.Get();
			if(dataTable.ParseData(data))
			{
				_allDataTables.Add(dataTablePath,dataTable);
			}
			else
			{
				_dataTablePool.Release(dataTable);
			}
		}

		/// <summary>
		/// 是否有当前的配置表
		/// </summary>
		/// <param name="dataTablePath"></param>
		/// <returns></returns>
		public bool HasDataTable(string dataTablePath)
		{
			return _allDataTables.ContainsKey(dataTablePath);
		}

	    /// <summary>
	    /// 获取数据表
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    public IDataTable GetDataTable(string dataTablePath)
	    {
		    DataTable dataTable = null;
		    if (_allDataTables.TryGetValue(dataTablePath, out dataTable))
			    return dataTable;
		    return null;
	    }

		/// <summary>
		/// 移除DataTable
		/// </summary>
		/// <param name="dataTablePath"></param>
		public void RemoveDataTable(string dataTablePath)
		{
			if (_allDataTables.TryGetValue(dataTablePath, out DataTable dataTable))
			{
				_dataTablePool.Release(dataTable);
			}
		}

		public override void OnClose()
        {
			foreach (var item in _allDataTables.Values)
			{
				_dataTablePool.Release(item);
			}
	        _allDataTables.Clear();
        }
    }
}
