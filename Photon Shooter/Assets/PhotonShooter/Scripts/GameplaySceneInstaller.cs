using PhotonShooter.Scripts.Gameplay;
using PhotonShooter.Scripts.Gameplay.Config;
using UnityEngine;
using UnityEngine.Serialization;

namespace PhotonShooter.Scripts
{
    using PhotonShooter.Scripts.Connection;
    using Zenject;

    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private ItemList itemList;
        
        public override void InstallBindings()
        {
            itemList.Initialize();
            
            Container.Bind<LevelController>().AsSingle();
            Container.BindInstance(itemList).AsSingle();
        }
    }
}