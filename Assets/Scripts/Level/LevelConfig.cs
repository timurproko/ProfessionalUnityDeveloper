using UnityEngine;

namespace ShootEmUp
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ShootEmUp/Level Config", order = 2)]
    public sealed class LevelConfig : ScriptableObject
    {
        [SerializeField] private int totalEnemiesToSpawn = 20;
        [SerializeField] private int maxEnemiesPerWave = 5;
        [SerializeField] private int poolPrewarmCount = 7;

        public int TotalEnemiesToSpawn => totalEnemiesToSpawn;
        public int MaxEnemiesPerWave => maxEnemiesPerWave;
        public int PoolPrewarmCount => poolPrewarmCount;
    }
}
