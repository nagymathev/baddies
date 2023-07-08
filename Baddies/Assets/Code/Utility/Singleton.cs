using System;
using JetBrains.Annotations;

namespace Code.Utility
{
    public class Singleton<T> where T : class, new()
    {
        private static T instance = new T();
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