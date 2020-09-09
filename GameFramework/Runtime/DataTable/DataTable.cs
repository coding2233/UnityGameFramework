//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #配置文件信息# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月30日 21点58分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
	internal sealed class DataTable : IDataTable
	{
		//所有的数据行
		private readonly Dictionary<int, TableData> _allDataRows;
		private readonly Dictionary<string,int> _tableDataKeys;

		public DataTable()
		{
			_allDataRows = new Dictionary<int, TableData>();
			_tableDataKeys = new Dictionary<string, int>();
		}

		/// <summary>
		/// 解析数据
		/// </summary>
		/// <param name="dataText"></param>
		internal bool ParseData(string dataText)
        {
			if(string.IsNullOrEmpty(dataText))
			 return false;

			_tableDataKeys.Clear();
			
			string[] lines = dataText.Split('\n');
			if(lines.Length>4)
			{
				//keys
				string[] args = lines[1].Trim().Split('\t');
				if(args==null||args.Length<2)
					return false;
				for (int i = 1; i < args.Length; i++)
				{
					if(_tableDataKeys.ContainsKey(args[i]))
						return false;

					_tableDataKeys.Add(args[i],i-1);
				}
				//type
				string[] types = lines[2].TrimEnd().Split('\t');
				
				for (int i = 4; i < lines.Length; i++)
				{
					if(string.IsNullOrEmpty(lines[i]))
						continue;
					args = lines[i].TrimEnd().Split('\t');
					if(args.Length<2||args[0].Trim()=="#")
						continue;

					List<TableData> datas=new List<TableData>();
					for (int j = 1; j < args.Length; j++)
					{
						if(j<types.Length)
						{
						    TableData tableData =TableDataPool.Get(types[j],args[j]);
							datas.Add(tableData);
						}
					}
					//依靠id来作为关键词
					if(datas.Count>0)
					{
						int id =(int)datas[0];
						TableData table= TableDataPool.Get().SetData(datas);
						//设置索引
						table.SetIndexKeys(_tableDataKeys);
						_allDataRows[id]=table;
					}
				}

				if(_allDataRows.Count>0)
				{
					return true;
				}
			}

			return false;
        }
	    
		/// <summary>
		/// 释放
		/// </summary>
		internal void Release()
		{
			foreach (var item in _allDataRows.Values)
			{
				TableDataPool.Release(item);
			}
			_allDataRows.Clear();
			_tableDataKeys.Clear();
		}

		/// <summary>
		/// 总数
		/// </summary>
		public int Count => _allDataRows.Count;

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <param name="id">id</param>
		/// <returns></returns>
		public TableData this[int id] => GetDataRow(id);

		/// <summary>
		/// 检测是否有当前id的数据
		/// </summary>
		/// <param name="id">id</param>
		/// <returns></returns>
		public bool HasDataRow(int id)
		{
			return _allDataRows.ContainsKey(id);
		}

		/// <summary>
		/// 获取某一行的数据
		/// </summary>
		/// <param name="id">id</param>
		/// <returns></returns>
		public TableData GetDataRow(int id)
		{
			TableData dataRow = null;
			_allDataRows.TryGetValue(id, out dataRow);
			return dataRow;
		}

		// /// <summary>
		// /// 获取所有的数据
		// /// </summary>
		// /// <returns></returns>
		// public TableData[] GetAllDataRows()
		// {
		// 	int index = 0;
		// 	TableData[] allDataRows = new TableData[_allDataRows.Count];
		// 	foreach (var item in _allDataRows)
		// 	{
		// 		allDataRows[index++] = item.Value;
		// 	}
		// 	return allDataRows;
		// }

		/// <summary>
		/// 获取所有符合条件的数据表行。
		/// </summary>
		/// <param name="condition">要检查的条件。</param>
		/// <returns>所有符合条件的数据表行。</returns>
		public TableData[] GetAllDataRows(Predicate<TableData> condition)
		{
			List<TableData> results = new List<TableData>();
			foreach (var dataRow in _allDataRows)
			{
				TableData dr = dataRow.Value;
				if (condition(dr))
				{
					results.Add(dr);
				}
			}

			return results.ToArray();
		}

		/// <summary>
		/// 获取所有排序后的数据表行。
		/// </summary>
		/// <param name="comparison">要排序的条件。</param>
		/// <returns>所有排序后的数据表行。</returns>
		public TableData[] GetAllDataRows(Comparison<TableData> comparison)
		{
			List<TableData> allDataRows = new List<TableData>();
			foreach (var dataRow in _allDataRows)
			{
				allDataRows.Add(dataRow.Value);
			}

			allDataRows.Sort(comparison);
			return allDataRows.ToArray();
		}

		#region  IEnumerable
        public IEnumerator<int> GetEnumerator()
        {
            return _allDataRows.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
             return _allDataRows.Keys.GetEnumerator();
        }
		#endregion
    }

}