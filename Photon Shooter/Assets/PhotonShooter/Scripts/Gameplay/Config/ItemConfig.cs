using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Config
{
    public class ItemConfig : ScriptableObject
    {
        [HideInInspector] public int id;
        public GameObject viewPrefab;
        public float respawnTime;
    }
}