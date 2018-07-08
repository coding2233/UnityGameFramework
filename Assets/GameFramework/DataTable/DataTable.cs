//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #配置文件信息# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月30日 21点58分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
	internal sealed class DataTable<T> : DataTableBase, IDataTable<T> where T : class,IDataTableRow, new()
	{
		//所有的数据行
		private readonly Dictionary<int, T> _allDataRows;

		internal DataTable()
		{
			_allDataRows = new Dictionary<int, T>();
		}

		/// <summary>
		/// 添加数据
		/// </summary>
		/// <param name="dataRowText"></param>
		public override void AddDataRow(string dataRowText)
		{
			T dataRow = new T();
			dataRow.ParseRowData(dataRowText);
			_allDataRows.Add(dataRow.Id, dataRow);
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
		public T this[int id] => GetDataRow(id);

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
		public T GetDataRow(int id)
		{
			T dataRow = null;
			_allDataRows.TryGetValue(id, out dataRow);
			return dataRow;
		}

		/// <summary>
		/// 获取所有的数据
		/// </summary>
		/// <returns></returns>
		public T[] GetAllDataRows()
		{
			int index = 0;
			T[] allDataRows = new T[_allDataRows.Count];
			foreach (var item in _allDataRows)
			{
				allDataRows[index++] = item.Value;
			}
			return allDataRows;
		}

		/// <summary>
		/// 获取所有符合条件的数据表行。
		/// </summary>
		/// <param name="condition">要检查的条件。</param>
		/// <returns>所有符合条件的数据表行。</returns>
		public T[] GetAllDataRows(Predicate<T> condition)
		{
			List<T> results = new List<T>();
			foreach (var dataRow in _allDataRows)
			{
				T dr = dataRow.Value;
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
		public T[] GetAllDataRows(Comparison<T> comparison)
		{
			List<T> allDataRows = new List<T>();
			foreach (var dataRow in _allDataRows)
			{
				allDataRows.Add(dataRow.Value);
			}

			allDataRows.Sort(comparison);
			return allDataRows.ToArray();
		}

	}
}
