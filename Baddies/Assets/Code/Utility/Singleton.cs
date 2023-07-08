using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Code.Utility
{
    public class Singleton<T>: MonoBehaviour 
        where T : Singleton<T>
    {
        private static Lazy<T> instance = new Lazy<T>(() => FindObjectOfType<T>() ?? Instantiate(new GameObject(typeof(T).Name)).AddComponent<T>());
        public static T Instance => instance.Value;

        static Singleton()
        {
            // Static constructor to initialize the instance
        }
    }

}