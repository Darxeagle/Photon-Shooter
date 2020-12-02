using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Config
{
    [CreateAssetMenu(fileName = "New Ammo Pack Config", menuName = "Photon Shooter/Ammo Pack Config")]
    public class AmmoPackConfig : ItemConfig
    {
        public int weaponId;
    }
}