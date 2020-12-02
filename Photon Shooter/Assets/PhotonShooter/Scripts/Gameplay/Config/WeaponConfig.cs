using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Config
{
    [CreateAssetMenu(fileName = "New Weapon Config", menuName = "Photon Shooter/Weapon Config")]
    public class WeaponConfig : ItemConfig
    {
        public GameObject projectilePrefab;
        public GameObject hitProjectilePrefab;
        public int damage;
        public float shootInterval;
        public int maxAmmo;
        public float projectileSpeed;
        public Color panelColor;
    }
}