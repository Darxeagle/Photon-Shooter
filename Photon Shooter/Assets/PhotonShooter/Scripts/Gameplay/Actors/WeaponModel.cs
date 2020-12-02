using Photon.Pun;
using PhotonShooter.Scripts.Gameplay.Config;
using UniRx;
using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Actors
{
    public class WeaponModel
    {
        public WeaponConfig WeaponConfig { get; private set; }
        public ReactiveProperty<bool> Picked { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> Selected { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<int> Ammo { get; } = new ReactiveProperty<int>(0);
        public float LastShotTime { get; private set; } = float.NegativeInfinity;

        public WeaponModel(WeaponConfig weaponConfig)
        {
            WeaponConfig = weaponConfig;
        }

        public void Empty()
        {
            Picked.Value = false;
            Ammo.Value = 0;
            LastShotTime = 0;
        }

        public void Pickup()
        {
            Picked.Value = true;
            Ammo.Value = WeaponConfig.maxAmmo / 2;
            LastShotTime = -WeaponConfig.shootInterval;
        }

        public void AddAmmo()
        {
            Ammo.Value = Mathf.Min(Ammo.Value + WeaponConfig.maxAmmo / 4, WeaponConfig.maxAmmo);
        }
        
        public bool CanShoot()
        {
            return Ammo.Value > 0 && Time.fixedTime > LastShotTime + WeaponConfig.shootInterval;
        }
        
        public void ShootAmmo()
        {
            LastShotTime = Time.fixedTime;
            Ammo.Value -= 1;
        }
    }
}