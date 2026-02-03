using UnityEngine;

namespace ShootEmUp
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "ShootEmUp/Character Config", order = 0)]
    public sealed class CharacterConfig : ScriptableObject
    {
        [SerializeField] private bool isPlayer;
        [SerializeField] private int defaultHealth = 1;
        [SerializeField] private float speed = 5f;

        public int DefaultHealth => defaultHealth;
        public bool IsPlayer => isPlayer;
        public float Speed => speed;
    }
}
