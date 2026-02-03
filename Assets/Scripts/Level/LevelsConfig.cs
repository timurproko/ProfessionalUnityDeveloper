using UnityEngine;

namespace ShootEmUp
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "ShootEmUp/Levels Config", order = 3)]
    public sealed class LevelsConfig : ScriptableObject
    {
        [SerializeField] private LevelConfig[] levels = System.Array.Empty<LevelConfig>();

        public int LevelCount => levels != null ? levels.Length : 0;
        public LevelConfig GetLevel(int index) => levels != null && index >= 0 && index < levels.Length ? levels[index] : null;
    }
}
