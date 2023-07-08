using System;
using UnityEngine;

namespace Code.GameState
{
    public class GameState
    {
        public uint money;
        
        public GameState()
        {
            money = 1000;
            // Register that when we spawn minion we reduce our money
            EventManager.Instance.OnMinionSpawned += (_, x) => money -= 1;
        }
    }

    public class StateBehaviour : MonoBehaviour
    {
        public GameState State { get; private set; }

        private void Start()
        {
            State = new GameState();
        }
    }
}