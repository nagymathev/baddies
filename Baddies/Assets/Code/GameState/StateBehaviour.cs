using Code.Utility;

namespace Code.GameState
{
    public class StateBehaviour : Singleton<StateBehaviour>
    {
        public uint money;
        
        // public GameState State { get; private set; }

        private void Start()
        {
            // State = new GameState();
            money = 10;
            // Register that when we spawn minion we reduce our money
            EventManager.Instance.OnMinionSpawned += (_, x) => money = SafeMinus(money, 1);
            
            EventManager.Instance.OnMinionKilled += (_, x, killer) => money += 2;
        }

        private uint SafeMinus(uint x, uint dx)
        {
            if (x < dx) return 0;
            return x - dx;
        }
        
        public bool CanSpawnMinion(object caller, Enemy minion)
        {
            // TODO: Proper rules for spawning minions
            return money > 0;
        }
    }
}