using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PhotonShooter.Scripts.Connection;
using PhotonShooter.Scripts.Gameplay.Actors;
using PhotonShooter.Scripts.Gameplay.Config;
using PhotonShooter.Scripts.Gameplay.Entities;
using UniRx;
using Zenject;

namespace PhotonShooter.Scripts.Gameplay
{
    public class LevelController : IOnEventCallback
    {
        [Inject] private LevelView levelView;
        [Inject] private ItemList itemList;
        [Inject] private MatchManager matchManager;
        
        #region Events
        
        public const byte DealDamageCode = 0x00;
        public const byte PickupWeaponCode = 0x01;
        public const byte PickupHealthPackCode = 0x02;
        public const byte PickupAmmoPackCode = 0x03;
        public const byte AddKillCode = 0x05;
        public const byte MatchOverCode = 0x06;
        
        private static RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        private static SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        #endregion
        
        private const int MaxKills = 10;
        private List<Character> characters = new List<Character>();
        
        private ReactiveProperty<Character> playerCharacter = new ReactiveProperty<Character>(null);
        public IReadOnlyReactiveProperty<Character> PlayerCharacter => playerCharacter;
        private ReactiveProperty<Character> enemyCharacter = new ReactiveProperty<Character>(null);
        public IReadOnlyReactiveProperty<Character> EnemyCharacter => enemyCharacter;

        public void Initialize()
        {
            PhotonNetwork.AddCallbackTarget(this);
            var playerCharacter = levelView.CreatePlayerCharacter();
            characters.Add(playerCharacter);
            this.playerCharacter.Value = playerCharacter;
            RespawnCharacter(playerCharacter);
        }

        public void AddCharacter(Character character)
        {
            if (!characters.Contains(character))
            {
                characters.Add(character);
                if (character.photonView.IsMine) playerCharacter.Value = character;
                else enemyCharacter.Value = character;
            }
        }
        
        public void RemoveCharacter(Character character)
        {
            if (characters.Contains(character))
            {
                characters.Remove(character);
                if (character != playerCharacter.Value) enemyCharacter.Value = null;
            }
        }

        private void RespawnCharacter(Character character)
        {
            if (!character.photonView.IsMine) return;
            
            character.CharacterModel.Reset();
            levelView.MoveToSpawn(character);
        }

        public void ShootWeapon(Character character)
        {
            if (!character.photonView.IsMine) return;
            if (!character.CharacterModel.SelectedWeapon.Value.CanShoot()) return;
            
            character.CharacterModel.SelectedWeapon.Value.ShootAmmo();
            levelView.CreateProjectile(character.BulletOrigin, 
                character.CharacterModel.SelectedWeapon.Value.WeaponConfig.projectilePrefab,
                character,
                character.CharacterModel.SelectedWeapon.Value.WeaponConfig);
        }
        
        public void ProjectileCollision(AProjectile projectile, Character character)
        {
            if (!projectile.photonView.IsMine) return;
            
            var content = new int[]
            {
                projectile.Character.photonView.ViewID,
                character.photonView.ViewID,
                projectile.WeaponConfig.damage
            };
            
            PhotonNetwork.RaiseEvent(DealDamageCode, content, raiseEventOptions, sendOptions);
        }
        
        public void ItemSpawnerCollision(ItemSpawner itemSpawner, Character character)
        {
            if (!itemSpawner.photonView.IsMine) return;
            
            itemSpawner.TakeItem();

            switch (itemSpawner.ItemConfig)
            {
                case WeaponConfig weaponConfig:
                    var wcontent = new int[]
                    {
                        character.photonView.ViewID,
                        weaponConfig.id
                    };
                    PhotonNetwork.RaiseEvent(PickupWeaponCode, wcontent, raiseEventOptions, sendOptions);
                    break;
                case HealthPackConfig healthPackConfig:
                    var hcontent = new int[]
                    {
                        character.photonView.ViewID,
                        healthPackConfig.id
                    };
                    PhotonNetwork.RaiseEvent(PickupHealthPackCode, hcontent, raiseEventOptions, sendOptions);
                    break;
                case AmmoPackConfig ammoPackConfig:
                    var acontent = new int[]
                    {
                        character.photonView.ViewID,
                        ammoPackConfig.id
                    };
                    PhotonNetwork.RaiseEvent(PickupAmmoPackCode, acontent, raiseEventOptions, sendOptions);
                    break;
            }
        }
        
        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case DealDamageCode:
                    var dcontent = (int[])photonEvent.CustomData;
                    DealDamageToCharacter(
                        characters.First(c => c.photonView.ViewID == dcontent[1]),
                        characters.First(c => c.photonView.ViewID == dcontent[0]),
                        dcontent[2]);
                    break;
                case PickupWeaponCode:
                    var wcontent = (int[])photonEvent.CustomData;
                    CharacterPickupWeapon(
                        characters.First(c => c.photonView.ViewID == wcontent[0]),
                        itemList.GetItem<WeaponConfig>(wcontent[1]));
                    break;
                case PickupHealthPackCode:
                    var hcontent = (int[]) photonEvent.CustomData;
                    CharacterPickupHealthpack(
                        characters.First(c => c.photonView.ViewID == hcontent[0]),
                        itemList.GetItem<HealthPackConfig>(hcontent[1]));
                    break;
                case PickupAmmoPackCode:
                    var acontent = (int[])photonEvent.CustomData;
                    CharacterPickupAmmopack(
                        characters.First(c => c.photonView.ViewID == acontent[0]),
                        itemList.GetItem<AmmoPackConfig>(acontent[1]));
                    break;
                case AddKillCode:
                    var kcontent = (int[])photonEvent.CustomData;
                    CharacterAddKill(characters.First(c => c.photonView.ViewID == kcontent[0]));
                    break;
                case MatchOverCode:
                    var ocontent = (int[])photonEvent.CustomData;
                    MatchOver(ocontent[0]);
                    break;
            }
        }

        private void DealDamageToCharacter(Character character, Character attacker, int damage)
        {
            if (!character.photonView.IsMine) return;
            
            character.CharacterModel.DealDamage(damage);
            if (character.CharacterModel.Dead)
            {
                character.CharacterModel.Deaths.Value += 1;
                RespawnCharacter(character);
                
                var content = new int[]
                {
                    attacker.photonView.ViewID
                };
                PhotonNetwork.RaiseEvent(AddKillCode, content, raiseEventOptions, sendOptions);
            }
        }
        
        private void CharacterPickupWeapon(Character character, WeaponConfig weaponConfig)
        {
            if (!character.photonView.IsMine) return;
            character.CharacterModel.PickupWeapon(weaponConfig);
        }

        private void CharacterPickupHealthpack(Character character, HealthPackConfig healthPackConfig)
        {
            if (!character.photonView.IsMine) return;
            character.CharacterModel.PickupHealthpack(healthPackConfig);
        }
        
        private void CharacterPickupAmmopack(Character character, AmmoPackConfig ammoPackConfig)
        {
            if (!character.photonView.IsMine) return;
            character.CharacterModel.PickupAmmoPack(ammoPackConfig);
        }

        private void CharacterAddKill(Character character)
        {
            if (!character.photonView.IsMine) return;
            character.CharacterModel.Kills.Value += 1;
            
            if (character.CharacterModel.Kills.Value >= MaxKills)
            {
                var content = new int[]
                {
                    character.photonView.ViewID
                };
                PhotonNetwork.RaiseEvent(MatchOverCode, content, raiseEventOptions, sendOptions);
            }
        }

        private void MatchOver(int winnerId)
        {
            if (PlayerCharacter.Value.photonView.ViewID == winnerId)
            {
                matchManager.LeaveMatch(MatchState.MatchLeavingWinPlayer);
            }
            else
            {
                matchManager.LeaveMatch(MatchState.MatchOverWinEnemy);
            }
        }

        public void Destroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}