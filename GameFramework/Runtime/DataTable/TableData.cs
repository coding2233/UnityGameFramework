using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class TableData : IEquatable<TableData>,IEnumerable<TableData>
    {
        #region  field
        private List<TableData> _instanceArray;
        private bool _instanceBool;
        private int _instanceInt;
        private long _instanceLong;
        private float _instanceFloat;
        private double _instanceDouble;
        private string _instanceString;
        private Vector2 _instanceVector2;
        private Vector3 _instanceVector3;
        private Color _instanceColor;

        #endregion
#region  TableData Typs
        private TableDataType _type;
        public TableDataType TableType
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// 是否为数组
        /// </summary>
        /// <value></value>
        public bool IsArray
        {
            get
            {
                return _type ==TableDataType.Table_Array;
            }
        }

#endregion

#region  properties
         //索引key
        private Dictionary<string,int> _indexKeys;
        #endregion


        #region  外部接口

        public TableData this[int index] => GetData(index);

        public TableData this[string key]=>GetData(key);
        #endregion

        #region  internal
        internal void SetIndexKeys(Dictionary<string,int> indexKeys)
        {
            _indexKeys=indexKeys;
        }

        internal void Release()
        {
            if(_type==TableDataType.Table_Array)
            {
                ReleaseArray(_instanceArray);
            }
            _type=TableDataType.Table_none;
            _indexKeys = null;
            _instanceString=null;
        }
        internal TableData SetData(bool data)
        {
            _instanceBool= data;
            _type=TableDataType.Table_bool;
            return this;
        }
        internal TableData SetData(int data)
        {
            _instanceInt= data;
            _type=TableDataType.Table_int;
            return this;
        }
        internal TableData SetData(long data)
        {
            _instanceLong= data;
            _type=TableDataType.Table_long;
            return this;
        }
        internal TableData SetData(float data)
        {
            _instanceFloat= data;
            _type=TableDataType.Table_float;
            return this;
        }
        internal TableData SetData(double data)
        {
            _instanceDouble= data;
            _type=TableDataType.Table_double;
            return this;
        }
        internal TableData SetData(string data)
        {
            _instanceString= data;
            _type=TableDataType.Table_string;
            return this;
        }
        internal TableData SetData(Vector2 data)
        {
            _instanceVector2= data;
            _type=TableDataType.Table_Vector2;
            return this;
        }
        internal TableData SetData(Vector3 data)
        {
            _instanceVector3= data;
            _type=TableDataType.Table_Vector3;
            return this;
        }
        internal TableData SetData(List<TableData> data)
        {
            _instanceArray= data;
            _type=TableDataType.Table_Array;
            return this;
        }

        internal TableData SetData(Color data)
        {
            _instanceColor=data;
            _type=TableDataType.Table_Color;
            return this;
        }
        #endregion

        #region IEnumerator
        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public IEnumerator<TableData> GetEnumerator()
        {
            if(_type==TableDataType.Table_Array)
            {
                return _instanceArray.GetEnumerator();
            }
            return null;
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
           if(_type==TableDataType.Table_Array)
            {
                return _instanceArray.GetEnumerator();
            }
            return null;
        }
        #endregion

       #region  内部函数

        //获取数据
        private TableData GetData(string key)
        {
            if(_indexKeys!=null && _indexKeys.TryGetValue(key,out int index))
            {
                return GetData(index);
            }
            return null;
        }

        //获取数据
        private TableData GetData(int index)
        {
            if(_instanceArray!=null&&_instanceArray.Count>0&&index<_instanceArray.Count)
            {
                return _instanceArray[index];
            }
            return null;
        }

        /// <summary>
        /// 回收数组
        /// </summary>
        private void ReleaseArray(List<TableData> instanceArray)
        {
            if(instanceArray!=null)
            {
                for (int i = 0; i < instanceArray.Count; i++)
                {
                    TableDataPool.Release(instanceArray[i]);
                }
                instanceArray=null;
            }
        }
       #endregion

        public bool Equals(TableData other)
        {
            return false;
        }

        public override string ToString()
        {  
            string str ="";
            switch (_type)
            {
                case TableDataType.Table_Array:
                    for (int i = 0; i < _instanceArray.Count; i++)
                    {
                        str+=$"{_instanceArray[i].ToString()}\n";
                    }
                    break;
                case TableDataType.Table_bool:
                    str = _instanceBool.ToString();
                    break;
                case TableDataType.Table_double:
                    str = _instanceDouble.ToString();
                    break;
                case TableDataType.Table_float:
                    str = _instanceFloat.ToString();
                    break;
                case TableDataType.Table_int:
                    str = _instanceInt.ToString();
                    break;
                case TableDataType.Table_long:
                    str = _instanceLong.ToString();
                    break;
                case TableDataType.Table_string:
                    str=_instanceString;
                    break;
                case TableDataType.Table_Vector2:
                    str = _instanceVector2.ToString();
                    break;
                case TableDataType.Table_Vector3:
                    str = _instanceVector3.ToString();
                    break;
                default:
                    break;
            }
            return str;
        }

        #region Implicit Conversions
        public static implicit operator TableData (Boolean data)
        {
            TableData tableData =  TableDataPool.Get().SetData(data);
            return tableData;
        }

        public static implicit operator TableData (Int32 data)
        {
            TableData tableData =  TableDataPool.Get().SetData(data);
            return tableData;
        }

        public static implicit operator TableData (Int64 data)
        {
            TableData tableData =  TableDataPool.Get().SetData(data);
            return tableData;
        }

        public static implicit operator TableData (float data)
        {
            TableData tableData =  TableDataPool.Get().SetData(data);
            return tableData;
        }

        public static implicit operator TableData (Double data)
        {
            TableData tableData =  TableDataPool.Get().SetData(data);
            return tableData;
        }

        public static implicit operator TableData (string data)
        {
            TableData tableData =  TableDataPool.Get().SetData(data);
            return tableData;
        }

        public static implicit operator TableData (Vector2 data)
        {
            TableData tableData =  TableDataPool.Get().SetData(data);
            return tableData;
        }

        public static implicit operator TableData (Vector3 data)
        {
            TableData tableData =  TableDataPool.Get().SetData(data);
            return tableData;
        }

        public static implicit operator TableData (Color data)
        {
            TableData tableData = TableDataPool.Get().SetData(data);
            return tableData;
        }
        #endregion


        #region Explicit Conversions

        public static explicit operator Boolean (TableData data)
        {
            if (data._type != TableDataType.Table_bool)
                throw new InvalidCastException (
                    "Instance of TableData doesn't hold a Boolean");

            return data._instanceBool;
        }

        public static explicit operator Int32 (TableData data)
        {
            if (data._type != TableDataType.Table_int&&data._type != TableDataType.Table_long)
                throw new InvalidCastException (
                    "Instance of TableData doesn't hold a Int32");

            return data._type==TableDataType.Table_int?data._instanceInt:(int)data._instanceLong;
        }

        public static explicit operator Int64 (TableData data)
        {
             if (data._type != TableDataType.Table_int&&data._type != TableDataType.Table_long)
                throw new InvalidCastException (
                    "Instance of TableData doesn't hold a Int64");

            return data._type==TableDataType.Table_long?data._instanceLong:(long)data._instanceInt;
        }

        public static explicit operator float (TableData data)
        {
            if (data._type != TableDataType.Table_float)
                throw new InvalidCastException (
                    "Instance of TableData doesn't hold a float");

            return data._instanceFloat;
        }
        public static explicit operator double (TableData data)
        {
            if (data._type != TableDataType.Table_double)
                throw new InvalidCastException (
                    "Instance of TableData doesn't hold a double");

            return data._instanceDouble;
        }
        public static explicit operator string (TableData data)
        {
            if (data._type != TableDataType.Table_string)
                throw new InvalidCastException (
                    "Instance of TableData doesn't hold a string");

            return data._instanceString;
        }
        public static explicit operator Vector2 (TableData data)
        {
            if (data._type != TableDataType.Table_Vector2)
                throw new InvalidCastException (
                    "Instance of TableData doesn't hold a Vector2");

            return data._instanceVector2;
        }
        public static explicit operator Vector3 (TableData data)
        {
            if (data._type != TableDataType.Table_Vector3)
                throw new InvalidCastException (
                    "Instance of TableData doesn't hold a Vector3");

            return data._instanceVector3;
        }

        public static explicit operator Color32(TableData data)
        {
            return Color.white;
        }

        #endregion

    }


    public enum TableDataType 
    {
        Table_none,
        Table_bool,
        Table_int,
        Table_long,
        Table_float,
        Table_double,
        Table_string,
        Table_Vector2,
        Table_Vector3,
        Table_Array,
        Table_Color,
    }


    internal static class TableDataPool
    {
        private static readonly ObjectPool<TableData> _objectPool = new ObjectPool<TableData>(null, null);

        public static TableData Get()
        {
            return _objectPool.Get();
        }

        public static TableData Get(string type,string value)
        {
            type=type.ToLower();
            value=value.Trim();
            TableData tableData = Get();
            switch (type)
            {
                case "bool":
                    tableData.SetData(value.ToBool());
                    break;
                case "int":
                    tableData.SetData(value.ToInt32());
                    break;
                case "long":
                    tableData.SetData(value.ToInt64());
                    break;
                case "float":
                    tableData.SetData(value.ToFloat());
                    break;
                case "double":
                    tableData.SetData(value.ToDouble());
                    break;
                case "string":
                    tableData.SetData(value);
                    break;
                case "vector2":
                    tableData.SetData(value.ToVector2());
                    break;
                case "vector3":
                    tableData.SetData(value.ToVector3());
                    break;
                case "color":
                    tableData.SetData(value.ToColor());
                    break;
                default:
                    tableData.SetData(value);
                    break;
            }
            return tableData;
        }

        public static void Release(TableData element)
        {
            element.Release();
            _objectPool.Release(element);
        }


    }

}