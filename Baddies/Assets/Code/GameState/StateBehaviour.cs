using System;
using UnityEngine;

namespace Code.GameState
{
    public class StateBehaviour : MonoBehaviour
    {
        public uint money;
        
        // public GameState State { get; private set; }

        private void Start()
        {
            // State = new GameState();
            money = 1000;
            // Register that when we spawn minion we reduce our money
            EventManager.Instance.OnMinionSpawned += (_, x) => money -= 1;
        }
    }
}