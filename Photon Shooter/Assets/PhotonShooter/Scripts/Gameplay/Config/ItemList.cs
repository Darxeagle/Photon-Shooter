using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhotonShooter.Scripts.Gameplay.Config
{
    [CreateAssetMenu(fileName = "Item Manager", menuName = "Photon Shooter/Item Manager")]
    public class ItemList : ScriptableObject
    {
        [SerializeField] private List<ItemConfig> itemList;
        private Dictionary<int, ItemConfig> itemDictionary;

        public void Initialize()
        {
            itemDictionary = new Dictionary<int, ItemConfig>();
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].id = i;
                itemDictionary.Add(i, itemList[i]);
            }
        }
        
        public ItemConfig GetItem(int itemId)
        {
            return itemDictionary[itemId];
        }

        public T GetItem<T>(int itemId) where T : ItemConfig
        {
            return itemDictionary[itemId] as T;
        }

        public IEnumerable<WeaponConfig> GetWeaponConfigs()
        {
            return itemList.OfType<WeaponConfig>();
        }
    }
}