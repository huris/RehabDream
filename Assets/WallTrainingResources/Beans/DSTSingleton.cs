using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DSTFrame
{
    public abstract class DSTSingleton<T> where T:DSTSingleton<T>
    {
        protected static T instance = null;
        public DSTSingleton()
        {
        }
        public static T Instance()
        {
            if (instance==null)
            {
                //instance = new DSTSingleton<T>();
                ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
                if (ctor == null) throw new Exception("ctor() not found!");
                instance = ctor.Invoke(null) as T;
            }
            return instance;
        }


    }
}
