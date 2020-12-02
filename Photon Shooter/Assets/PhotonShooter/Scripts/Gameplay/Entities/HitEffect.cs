using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Entities
{
    public class HitEffect : AProjectile
    {
        protected override void LaunchInternal(Transform launchTransform)
        {
            Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ => PhotonNetwork.Destroy(gameObject)).AddTo(this);
        }
    }
}