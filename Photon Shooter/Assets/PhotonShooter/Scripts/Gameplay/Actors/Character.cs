using System;
using Photon.Pun;
using PhotonShooter.Scripts.Gameplay.Config;
using UniRx;
using UnityEngine;
using Zenject;
using Object = System.Object;

namespace PhotonShooter.Scripts.Gameplay.Actors
{
    public class Character : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] private CharacterInput characterInput;
        [SerializeField] private GameObject camera;
        [SerializeField] private Transform weaponPlace;
        [SerializeField] public Transform BulletOrigin;

        [Inject] private LevelController levelController;
        [Inject] private LevelView levelView;
        [Inject] private ItemList itemList;

        private GameObject weaponView;
        
        public CharacterModel CharacterModel { get; private set; }

        [Inject]
        public void Constructor()
        {
            CharacterModel = new CharacterModel(itemList.GetWeaponConfigs());
        }

        private void Start()
        {
            characterInput.enabled = photonView.IsMine;
            camera.SetActive(photonView.IsMine);
            levelController.AddCharacter(this);

            CharacterModel.SelectedWeaponIndex.Subscribe(indx =>
            {
                if (weaponView != null) Destroy(weaponView);
                weaponView = Instantiate(CharacterModel.SelectedWeapon.Value.WeaponConfig.viewPrefab, weaponPlace);
                weaponView.transform.localPosition = Vector3.zero;
                weaponView.transform.localRotation = Quaternion.identity;
            }).AddTo(this);
            
            if (photonView.IsMine)
            {
                characterInput.WeaponSelected.Subscribe(index =>
                {
                    if (CharacterModel.WeaponModels[index].Picked.Value)
                        CharacterModel.SelectWeapon(index);
                }).AddTo(this);
            }
        }

        private void Update()
        {
            if (!photonView.IsMine) return;
            
            if (characterInput.FirePressed && CharacterModel.SelectedWeapon.Value.CanShoot())
            {
                levelController.ShootWeapon(this);
            }
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(CharacterModel.Serialize());
            }
            else
            {
                CharacterModel.Deserialize((Object[]) stream.ReceiveNext());
            }
        }

        protected void OnDestroy()
        {
            levelController.RemoveCharacter(this);
        }
    }
}