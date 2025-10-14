using System;
using UnityEngine;

namespace Attributes
{
    /// <summary>
    /// Display a field as read-only in the inspector.
    /// CustomPropertyDrawers will not work when this attribute is used.
    /// </summary>
    public class ReadonlyAttribute : PropertyAttribute
    {
        
    }
}