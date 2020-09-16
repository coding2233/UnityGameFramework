using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    [System.AttributeUsage(AttributeTargets.Class)]
    public class DebuggerWindowAttribute :Attribute
    {
        public string Title{get;private set;}

        public DebuggerWindowAttribute(string title)
        {
            Title=title;
        }
    }

}
