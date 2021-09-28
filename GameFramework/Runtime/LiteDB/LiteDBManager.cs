using LiteDB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class LiteDBManager : GameFrameworkModule
    {
        private Dictionary<string, LiteDBMapper> _litedbs = new Dictionary<string, LiteDBMapper>();


        public LiteDBMapper Select(string dbName)
        {
            LiteDBMapper mapper;
            if (!_litedbs.TryGetValue(dbName, out mapper))
            {
                string dbPath = Path.Combine(Application.persistentDataPath, $"{dbName}.litedb");
                mapper = new LiteDBMapper(dbPath);
                _litedbs.Add(dbName, mapper);
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

        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
            }
        }
    }

}