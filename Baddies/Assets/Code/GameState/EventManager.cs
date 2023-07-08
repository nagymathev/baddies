using System;
using Code.Utility;
using JetBrains.Annotations;
using UnityEngine;

namespace Code.GameState
{
    public class EventManager: Singleton<EventManager>
    {
        public EventManager([CanBeNull] Action<EventManager> initializer) : base(initializer)
        { }

        public EventManager()
        { }

        /// <summary>
        /// Callback when we spawn a minion <br />
        /// Probably want to feed in the properties of the minion spawned not the actual MonoBehaviour <br />
        /// Args(caller, minion)
        /// </summary>
        public event Action<object, Enemy> OnMinionSpawned;
        
        public void MinionSpawned(object caller, Enemy minion)
        {
            OnMinionSpawned?.Invoke(caller, minion);
        }
    }
}