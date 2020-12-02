using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Entities
{
    public class Explosion : AProjectile
    {
        protected override void LaunchInternal(Transform launchTransform)
        {
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => PhotonNetwork.Destroy(gameObject)).AddTo(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other);
        }
        
    }
}