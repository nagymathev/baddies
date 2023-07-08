using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Code.Utility
{
    public class Singleton<T>: MonoBehaviour 
        where T : Singleton<T>
    {
        private static T instance;
        public static T Instance => instance;
        public bool IsInitialized = false;

        static Singleton()
        {
            // Static constructor to initialize the instance
        }
        
        public Singleton()
        {
            IsInitialized = true;
        }

        public Singleton([CanBeNull] Action<T> initializer): this()
        {
            initializer ??= (x => {});
            initializer(instance);
        }
    }

}