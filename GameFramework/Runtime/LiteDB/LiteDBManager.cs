using LiteDB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class LiteDBManager : GameFrameworkModule
    {
        private Dictionary<string, LiteDBMapper> _litedbs = new Dictionary<string, LiteDBMapper>();

        public LiteDBMapper Select(string dbPath)
        {
            LiteDBMapper mapper;
            if (!_litedbs.TryGetValue(dbPath, out mapper))
            {
                mapper = new LiteDBMapper(dbPath);
                _litedbs.Add(dbPath, mapper);
            }
            return mapper;
        }


        public override void OnClose()
        {
            foreach (var item in _litedbs.Values)
            {
                item.Dispose();
            }
            _litedbs.Clear();
        }
    }

    public class LiteDBMapper:System.IDisposable
    {
        private LiteDatabase _db;
        public LiteDatabase LiteDB => _db;

        public LiteDBMapper(string path)
        {
            _db = new LiteDatabase(path);
        }

        public ILiteCollection<T> GetCollection<T>()
        {
            return _db.GetCollection<T>();
        }

        public ILiteQueryable<T> Query<T>()
        {
            return GetCollection<T>().Query();
        }

        public T[] QueryAll<T>()
        {
            return GetCollection<T>().Query().ToArray();
        }

        public void Insert<T>(T t)
        {
            GetCollection<T>().Insert(t);
        }

        public void Upload(string key, string file)
        {
            var storage =  _db.GetStorage<string>();
            storage.Upload(key, file);
        }

        public void Download(string key, string localPath)
        {
            var storage = _db.GetStorage<string>();
            storage.Download(key, localPath,true);
        }

        public void SetData<T>(T value) where T: ILiteData
        {
            var col = _db.GetCollection<T>(GetTableName<T>());
            var query = col.Query().Where(x => x.Id==value.Id);
            if (query.Count() > 0)
            {
                col.Update(value);
            }
            else
            {
                col.Insert(value);
            }
        }

        public T GetData<T>(int id, T defaultValue = default(T)) where T : ILiteData
        {
            var col = _db.GetCollection<T>(GetTableName<T>());
            var value = col.Query().Where(x =>x.Id==id);
            if (value.Count() > 0)
            {
                return value.First();
            }

            return defaultValue;
        }

        public void SetCustomerData<T>(string key, T value)
        {
            var col = _db.GetCollection<CustomerData<T>>(GetTableName<T>());
            CustomerData<T> customerData;
            var query = col.Query().Where(x => x.Key.Equals(key));
            if (query.Count() > 0)
            {
                customerData = query.First();
                customerData.Data = value;
                col.Update(customerData);
            }
            else
            {
                customerData = new CustomerData<T>()
                {
                    Key = key,
                    Data = value
                };
                col.Insert(customerData);
            }
        }

        public T GetCustomerData<T>(string key, T defaultValue = default(T))
        {
            var col = _db.GetCollection<CustomerData<T>>(GetTableName<T>());
            var value = col.Query().Where(x => x.Key.Equals(key));
            if (value.Count() > 0)
            {
                return value.First().Data;
            }

            return defaultValue;
        }
        private string GetTableName<T>()
        {
            string tableName = Regex.Replace(typeof(T).Name, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]", "");
            tableName = $"{tableName}";
            return tableName;
        }


        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
            }
        }
    }

}