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

    public interface IDataTable<T> where T : class, IDataTableRow,new()
    {
		/// <summary>
		/// 总数
		/// </summary>
	    int Count { get; }
		/// <summary>
		/// 获取数据行
		/// </summary>
		/// <param name="id">数据id</param>
		/// <returns></returns>
		T this[int id] { get; }
		/// <summary>
		/// 是否存在Id的数据行
		/// </summary>
		/// <param name="id">数据id</param>
		/// <returns></returns>
		bool HasDataRow(int id);
	    /// <summary>
	    /// 获取Id的数据行
	    /// </summary>
	    /// <param name="id"></param>
	    /// <returns></returns>
	    T GetDataRow(int id);
	    /// <summary>
	    /// 获取所有的数据行
	    /// </summary>
	    /// <returns></returns>
	    T[] GetAllDataRows();
	}
}
