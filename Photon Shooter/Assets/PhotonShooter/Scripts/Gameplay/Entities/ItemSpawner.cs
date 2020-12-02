using System.Security.Cryptography;
using Photon.Pun;
using PhotonShooter.Scripts.Gameplay.Actors;
using PhotonShooter.Scripts.Gameplay.Config;
using UniRx;
using UnityEngine;
using Zenject;

namespace PhotonShooter.Scripts.Gameplay.Entities
{
    public class ItemSpawner : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] public int ItemId;
        [SerializeField] private Transform itemPoint;

        [Inject] private ItemList itemList;
        [Inject] private LevelController levelController;
        
        private ReactiveProperty<bool> itemReady = new ReactiveProperty<bool>();
        private float lastTakeTime = float.NegativeInfinity;
        private GameObject spawnedItem;

        public ItemConfig ItemConfig { get; private set; }
        
        private void Start()
        {
            ItemConfig = itemList.GetItem(ItemId);
            itemReady.Subscribe(b =>
            {
                if (b)
                {
                    spawnedItem = Instantiate(ItemConfig.viewPrefab, itemPoint);
                    spawnedItem.transform.localPosition = Vector3.zero;
                    spawnedItem.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    if (spawnedItem != null) Destroy(spawnedItem);
                }
            });
        }

        public void TakeItem()
        {
            if (itemReady.Value)
            {
                itemReady.Value = false;
                lastTakeTime = Time.fixedTime;
            }
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                if (!itemReady.Value && Time.fixedTime > lastTakeTime + ItemConfig.respawnTime)
                {
                    itemReady.Value = true;
                }
            }
            
            itemPoint.transform.Rotate(Vector3.up, 30 * Time.fixedDeltaTime);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (photonView.IsMine && itemReady.Value) HandleCollision(other);
        }
        
        protected bool HandleCollision(Collider collider)
        {
            Character collidedCharacter = null;
            
            if (collider.CompareTag("Character"))
            {
                collidedCharacter = collider.GetComponent<Character>();
                levelController.ItemSpawnerCollision(this, collidedCharacter);
                return true;
            }

            return false;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(itemReady.Value);
                stream.SendNext(lastTakeTime);
            }
            else
            {
                itemReady.Value = (bool)stream.ReceiveNext();
                lastTakeTime = (float)stream.ReceiveNext();
            }
        }
    }
}