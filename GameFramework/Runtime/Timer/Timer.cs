using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class Timer : MonoBehaviour
    {
        float _realtimeSinceStartup;
        float _liveTime;
        private Action _onInvokeCallback;
        public void SetStartTime()
        {
            _realtimeSinceStartup = Time.realtimeSinceStartup;
        }

        public float GetLiveTime()
        {
            _liveTime = Time.realtimeSinceStartup - _realtimeSinceStartup;
            return _liveTime;
        }

        public void RunAction(float interval,Action onInvokeCallback, float repeatRate=0.0f)
        {
            _onInvokeCallback = onInvokeCallback;
            if (repeatRate > 0)
            {
                InvokeRepeating("InvokeAction", interval, repeatRate);
            }
            else
            {
                Invoke("InvokeAction",interval);
            }
        }


        private void InvokeAction()
        {
            _onInvokeCallback?.Invoke();
        }
    }
}
