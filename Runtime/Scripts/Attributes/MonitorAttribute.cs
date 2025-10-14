using System;
using UnityEngine;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class MonitorAttribute : PropertyAttribute
    {
        public MonitorAttribute()
        {
            
        }
    }
}