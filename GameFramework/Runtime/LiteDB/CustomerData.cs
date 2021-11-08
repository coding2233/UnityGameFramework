using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class CustomerData<T> : ILiteData
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public T Data { get; set; }
    }
}
