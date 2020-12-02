using Photon.Pun;
using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Entities
{
    public class Arrow : AProjectile
    {
        private Rigidbody rigidbody;
        private Vector3 speed;
        
        protected override void LaunchInternal(Transform launchTransform)
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.position = launchTransform.position;
            rigidbody.velocity = launchTransform.forward * WeaponConfig.projectileSpeed;
        }
        
        private void Update()
        {
            if (!photonView.IsMine) return;
            rigidbody.rotation = Quaternion.LookRotation(rigidbody.velocity);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine) return;
            
            if (HandleCollision(other))
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}