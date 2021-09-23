using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class TimerManager : GameFrameworkModule
    {
        private List<Timer> _timers = new List<Timer>();

        public Timer Start()
        {
            var timerGo = new GameObject();
            timerGo.hideFlags = HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(timerGo);
            Timer timer = timerGo.AddComponent<Timer>();
            timer.SetStartTime();
            _timers.Add(timer);
            return timer;
        }

        public float Stop(Timer timer)
        {
            float time = -1;
            if (timer != null&& timer.gameObject!=null)
            {
                time = timer.GetLiveTime();
                GameObject.Destroy(timer.gameObject);
            }
            return time;
        }

        public override void OnClose()
        {
        }
    }
}