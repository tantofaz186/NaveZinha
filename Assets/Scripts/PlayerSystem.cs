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
        // Shoot(ref state);
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
    // void Shoot(ref SystemState state)
    // {
    //    
    // }
}