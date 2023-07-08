using System;
using Code.Utility;
using JetBrains.Annotations;
using UnityEngine;

namespace Code.GameState
{
    public class EventManager: Singleton<EventManager>
    {

        public EventManager()
        { }

        /// <summary>
        /// Callback when we spawn a minion <br />
        /// Probably want to feed in the properties of the minion spawned not the actual MonoBehaviour <br />
        /// Args(caller, minion)
        /// </summary>
        /// TODO: change caller type
        public event Action<object, Enemy> OnMinionSpawned;
        
        /// <summary>
        /// Callback when we spawn a minion <br />
        /// Probably want to feed in the properties of the minion spawned not the actual MonoBehaviour <br />
        /// Args(caller, minion, killer)
        /// </summary>
        /// TODO: change killer type
        public event Action<object, Enemy, GameObject> OnMinionKilled;
        
        public void MinionSpawned(object caller, Enemy minion)
        {
            OnMinionSpawned?.Invoke(caller, minion);
        }
        
        public void MinionKilled(object caller, Enemy minion, [CanBeNull] GameObject killer)
        {
            OnMinionKilled?.Invoke(caller, minion, killer);
        }
    }
}