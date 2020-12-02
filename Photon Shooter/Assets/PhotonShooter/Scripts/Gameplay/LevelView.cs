using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using PhotonShooter.Scripts.Gameplay.Actors;
using PhotonShooter.Scripts.Gameplay.Config;
using PhotonShooter.Scripts.Gameplay.Entities;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace PhotonShooter.Scripts.Gameplay
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private GameObject characterPrefab;

        [Inject] private LevelController levelController;
        
        private List<Transform> characterSpawns;
        
        
        private void Start()
        {
            characterSpawns = GetComponentsInChildren<CharacterSpawn>().Select(ps => ps.transform).ToList();
            levelController.Initialize();
        }

        public Character CreatePlayerCharacter()
        {
            var spawnPoint = RandomSpawnPosition();
            var playerCharacter = PhotonNetwork.Instantiate(characterPrefab.name, spawnPoint.position, spawnPoint.rotation)
                .GetComponent<Character>();
            return playerCharacter;
        }

        public void MoveToSpawn(Character character)
        {
            var spawnPoint = RandomSpawnPosition();
            character.transform.position = spawnPoint.transform.position;
            character.transform.rotation = spawnPoint.transform.rotation;
        }

        private Transform RandomSpawnPosition()
        {
            var index = Random.Range(0, characterSpawns.Count);
            return characterSpawns[index];
        }

        public void CreateProjectile(Transform launchTransform, GameObject prefab, Character character, WeaponConfig weaponConfig)
        {
            var projectile = PhotonNetwork.Instantiate(prefab.name, launchTransform.position, launchTransform.rotation)
                .GetComponent<AProjectile>();
            projectile.Launch(launchTransform, character, weaponConfig);
        }

        private void OnDestroy()
        {
            levelController.Destroy();
        }
    }
}