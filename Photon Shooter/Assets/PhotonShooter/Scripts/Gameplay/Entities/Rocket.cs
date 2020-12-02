using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Entities
{
    public class Rocket : AProjectile
    {
        private void Update()
        {
            if (!photonView.IsMine) return;

            transform.position += (transform.forward * (WeaponConfig.projectileSpeed * Time.deltaTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine) return;
            
            if (HandleCollision(other))
            {
                CreateHitProjectile();
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}