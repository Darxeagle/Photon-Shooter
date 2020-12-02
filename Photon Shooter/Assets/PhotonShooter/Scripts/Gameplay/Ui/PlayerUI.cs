using PhotonShooter.Scripts.Gameplay.Actors;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace PhotonShooter.Scripts.Gameplay.Ui
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI playerScoreText;
        [SerializeField] private TextMeshProUGUI enemyScoreText;
        [SerializeField] private Transform weaponPanelsContainer;
        [SerializeField] private Transform weaponPanelPrefab;

        [Inject] private LevelController levelController;

        private void Start()
        {
            levelController.PlayerCharacter.First(c => c != null).Subscribe(c => SetPlayerModel(c.CharacterModel))
                .AddTo(this);

            Observable.Switch(levelController.EnemyCharacter.Select(c =>
                c != null ? c.CharacterModel.Kills : new ReactiveProperty<int>(-1))).Subscribe(v =>
            {
                if (v == -1)
                {
                    enemyScoreText.gameObject.SetActive(false);
                }
                else
                {
                    enemyScoreText.gameObject.SetActive(true);
                    enemyScoreText.text = v.ToString();
                }
            }).AddTo(this);
        }

        private void SetPlayerModel(CharacterModel characterModel)
        {
            characterModel.Hp.Subscribe(v => { healthText.text = v.ToString(); }).AddTo(this);
            characterModel.Kills.Subscribe(v => { playerScoreText.text = v.ToString(); }).AddTo(this);
            
            Observable.Switch(characterModel.SelectedWeapon.Select(wm => wm.Ammo))
                .Subscribe(v => { ammoText.text 
                    = v.ToString()+"/"+characterModel.SelectedWeapon.Value.WeaponConfig.maxAmmo; }).AddTo(this);

            foreach (var weaponModel in characterModel.WeaponModels)
            {
                Instantiate(weaponPanelPrefab, weaponPanelsContainer)
                    .GetComponent<WeaponPanel>().SetModel(weaponModel);
            }
        }
    }
}