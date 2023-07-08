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
            money = 1000;
            // Register that when we spawn minion we reduce our money
            EventManager.Instance.OnMinionSpawned += (_, x) => money -= 1;
        }
        
        public bool CanSpawnMinion(object caller, Enemy minion)
        {
            // TODO: Proper rules for spawning minions
            return money > 0;
        }
    }
}