using Unity.Entities;
using UnityEngine;

public partial class InputSystem : SystemBase
{
    private Controls _controls;
    
   protected override void OnCreate()
   {
       if (!SystemAPI.TryGetSingleton(out InputComponent _))
       {
           EntityManager.CreateEntity(typeof(InputComponent));
       }

       _controls = new Controls();
       _controls.Enable();
   } 
   protected override void OnUpdate()
       {
          Vector2 movement = _controls.Player.Move.ReadValue<Vector2>();
          Vector2 mousePosition = _controls.Player.MousePos.ReadValue<Vector2>();
          bool shoot = _controls.Player.Shoot.IsPressed();
          
          SystemAPI.SetSingleton(new InputComponent
          {
              Movement = movement,
              MousePosition = mousePosition,
              Shoot = shoot
          });
       }
}
