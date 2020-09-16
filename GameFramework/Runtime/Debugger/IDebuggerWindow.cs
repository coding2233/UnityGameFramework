using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public interface IDebuggerWindow 
    {
        void OnInit(params object[] args);

        void OnEnter();
     
        void OnDraw();

        void OnExit();

        void OnClose();
    }

}
