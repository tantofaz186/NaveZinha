using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public partial class PlayerSystem : SystemBase
{
    private Entity _playerEntity;
    private Entity _inputEntity;

    private PlayerComponent _playerComponent;
    private InputComponent _inputComponent;

    protected override void OnUpdate()
    {
        _playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        _inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();

        _playerComponent = EntityManager.GetComponentData<PlayerComponent>(_playerEntity);
        _inputComponent = EntityManager.GetComponentData<InputComponent>(_inputEntity);

        Move(ref CheckedStateRef);
        Shoot(ref CheckedStateRef);
    }

    void Move(ref SystemState state)
    {
        LocalTransform playerTransform = EntityManager.GetComponentData<LocalTransform>(_playerEntity);
        playerTransform.Position += new float3(_inputComponent.Movement.y, 0, -_inputComponent.Movement.x) * _playerComponent.MoveSpeed *
                                    SystemAPI.Time.DeltaTime;
        Vector2 dir = (Vector2)_inputComponent.MousePosition - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.Position);

        float angle = math.degrees(math.atan2(dir.y, dir.x));
        playerTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.down);
        EntityManager.SetComponentData(_playerEntity, playerTransform);
    }

    void Shoot(ref SystemState state)
    {
        if (_inputComponent.Shoot)
        {
            for (int i = 0; i < _playerComponent.NumOfBulletsToSpawn; i++)
            {
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);

                Entity bulletEntity = EntityManager.Instantiate(_playerComponent.BulletPrefab);

                ECB.AddComponent(bulletEntity, new BulletComponent
                {
                    Speed = 5f,
                    Damage = 10f,
                    Size = 3f
                });

                ECB.AddComponent(bulletEntity, new BulletLifeTimeComponent
                {
                    RemainingLifeTime = 5f
                });

                LocalTransform playerTransform = EntityManager.GetComponentData<LocalTransform>(_playerEntity);
                LocalTransform bulletTransform = EntityManager.GetComponentData<LocalTransform>(bulletEntity);
                float randomOffset = UnityEngine.Random.Range(-_playerComponent.BulletSpread, _playerComponent.BulletSpread);
                bulletTransform.Rotation = playerTransform.Rotation;
                bulletTransform.Position = (playerTransform.Position + (playerTransform.Forward() * -1) * 1.5f) +
                                           randomOffset * playerTransform.Right();
                bulletTransform.Scale = 0.5f;
                ECB.SetComponent(bulletEntity, bulletTransform);
                ECB.Playback(EntityManager);

                ECB.Dispose();
            }
        }
    }
}