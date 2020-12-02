using System.Collections.Generic;
using PhotonShooter.Scripts.Gameplay.Config;
using UniRx;
using UnityEngine;
using Object = System.Object;

namespace PhotonShooter.Scripts.Gameplay.Actors
{
    public class CharacterModel
    {
        public const int MaxHp = 100;
        public const int MaxWeapons = 3;
        
        private List<WeaponModel> weaponModels = new List<WeaponModel>();
        
        public ReactiveProperty<int> Hp { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> Kills  { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> Deaths  { get; } = new ReactiveProperty<int>();
        public IReadOnlyList<WeaponModel> WeaponModels => weaponModels;
        public ReactiveProperty<int> SelectedWeaponIndex { get; } = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<WeaponModel> SelectedWeapon => SelectedWeaponIndex.Select(i => weaponModels[i]).ToReactiveProperty();

        public CharacterModel(IEnumerable<WeaponConfig> availableWeapons)
        {
            foreach (var weaponConfig in availableWeapons)
            {
                weaponModels.Add(new WeaponModel(weaponConfig));
            }
        }

        public void Reset()
        {
            Hp.Value = MaxHp;

            var initialWeaponIndex = Random.Range(0, MaxWeapons);
            for (int i = 0; i < MaxWeapons; i++)
            {
                if (i == initialWeaponIndex) weaponModels[i].Pickup();
                else weaponModels[i].Empty();
            }

            SelectWeapon(initialWeaponIndex);
        }

        public void DealDamage(int damage)
        {
            Hp.Value = Mathf.Max(Hp.Value - damage, 0);
        }

        public bool Dead => Hp.Value == 0;

        public void SelectWeapon(int index)
        {
            SelectedWeaponIndex.Value = index;
            for (int i = 0; i < MaxWeapons; i++)
            {
                weaponModels[i].Selected.Value = i == index;
            }
        }

        public void PickupWeapon(WeaponConfig weaponConfig)
        {
            var relatedWeaponIndex = weaponModels.FindIndex(wm => wm.WeaponConfig.id == weaponConfig.id);
            var relatedWeapon = weaponModels[relatedWeaponIndex];

            if (!relatedWeapon.Picked.Value)
            {
                relatedWeapon.Pickup();
                SelectWeapon(relatedWeaponIndex);
            }
            else
            {
                relatedWeapon.AddAmmo();
            }
        }

        public void PickupHealthpack(HealthPackConfig healthPackConfig)
        {
            Hp.Value = Mathf.Min(Hp.Value + healthPackConfig.health, MaxHp);
        }

        public void PickupAmmoPack(AmmoPackConfig ammoPackConfig)
        {
            var relatedWeaponIndex = weaponModels.FindIndex(wm => wm.WeaponConfig.id == ammoPackConfig.weaponId);
            weaponModels[relatedWeaponIndex].AddAmmo();
        }
        
        public Object[] Serialize()
        {
            return new Object[]
            {
                Hp.Value,
                Kills.Value,
                Deaths.Value,
                SelectedWeaponIndex.Value
            };
        }
        
        public void Deserialize(Object[] data)
        {
            Hp.Value = (int) data[0];
            Kills.Value = (int) data[1];
            Deaths.Value = (int) data[2];
            SelectWeapon((int) data[3]);
        }
    }
}