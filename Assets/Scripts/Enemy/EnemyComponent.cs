using Unity.Entities;

namespace Enemy
{
    public struct EnemyComponent : IComponentData
    {
        public float MoveSpeed;
        public float Health;
    }
}