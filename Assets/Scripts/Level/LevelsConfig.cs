using UnityEngine;

namespace ShootEmUp
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "ShootEmUp/Levels Config", order = 3)]
    public sealed class LevelsConfig : ScriptableObject
    {
        [SerializeField] private LevelConfig[] levels;

        public int LevelCount => levels?.Length ?? 0;
        
        public LevelConfig GetLevel(int index)
        {
            return levels != null && index >= 0 && index < levels.Length ? levels[index] : null;
        }
    }
}
