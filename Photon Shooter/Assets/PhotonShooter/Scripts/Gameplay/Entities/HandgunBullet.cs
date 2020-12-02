using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Entities
{
    public class HandgunBullet : AProjectile
    {
        protected override void LaunchInternal(Transform launchTransform)
        {
            var ray = new Ray();
            var hits = Physics.RaycastAll(transform.position, transform.forward);
            var hitPosition = transform.position + transform.forward * 1000f;

            foreach (var raycastHit in hits)
            {
                if (HandleCollision(raycastHit.collider))
                {
                    hitPosition = raycastHit.point;
                    break;
                }
            }
            
            transform.localScale = new Vector3(1f, 1f, Vector3.Distance(transform.position, hitPosition));
            transform.position = hitPosition;
            CreateHitProjectile();
            Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ => PhotonNetwork.Destroy(gameObject)).AddTo(this);
        }
    }
}