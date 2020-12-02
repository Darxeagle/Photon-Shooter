using Photon.Pun;
using PhotonShooter.Scripts.Gameplay.Actors;
using PhotonShooter.Scripts.Gameplay.Config;
using UnityEngine;
using Zenject;

namespace PhotonShooter.Scripts.Gameplay.Entities
{
    public class AProjectile : MonoBehaviourPunCallbacks
    {
        [Inject] private LevelView levelView;
        [Inject] private LevelController levelController;
        [Inject] private ItemList itemList;
        
        public Character Character { get; private set; }
        public WeaponConfig WeaponConfig { get; private set; }

        public void Launch(Transform launchTransform, Character character, WeaponConfig weaponConfig)
        {
            if (!photonView.IsMine) return;
            
            Character = character;
            WeaponConfig = weaponConfig;

            LaunchInternal(launchTransform);
        }

        protected virtual void LaunchInternal(Transform launchTransform)
        {
            
        }

        protected bool HandleCollision(Collider collider)
        {
            Character collidedCharacter = null;
            
            if (!photonView.IsMine) return false;
            if (!collider.CompareTag("Level") && !collider.CompareTag("Character")) return false;
            
            if (collider.CompareTag("Character"))
            {
                collidedCharacter = collider.GetComponent<Character>();
                if (collidedCharacter.photonView.ViewID == Character.photonView.ViewID) return false;
                
                levelController.ProjectileCollision(this, collidedCharacter);
                return true;
            }

            return true;
        }

        protected void CreateSubProjectile(GameObject prefab)
        {
            levelView.CreateProjectile(transform, prefab, Character, WeaponConfig);
        }

        protected void CreateHitProjectile()
        {
            CreateSubProjectile(WeaponConfig.hitProjectilePrefab);
        }
    }
}