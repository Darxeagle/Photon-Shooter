using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Config
{
    [CreateAssetMenu(fileName = "New Health Pack Config", menuName = "Photon Shooter/Health Pack Config")]
    public class HealthPackConfig : ItemConfig
    {
        public int health;
    }
}