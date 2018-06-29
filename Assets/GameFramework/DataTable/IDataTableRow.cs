//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #配置文件的行解析# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月28日 11点56分# </time>
//-----------------------------------------------------------------------

namespace GameFramework.Taurus
{
    public interface IDataTableRow
    {
        /// <summary>
        /// Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 解析当前数据的接口
        /// </summary>
        /// <param name="data"></param>
        void ParseRowData(string data);
    }
}
