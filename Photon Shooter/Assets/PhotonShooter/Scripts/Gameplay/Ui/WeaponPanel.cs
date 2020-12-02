using PhotonShooter.Scripts.Gameplay.Actors;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonShooter.Scripts.Gameplay
{
    public class WeaponPanel : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject selected;
        [SerializeField] private TextMeshProUGUI ammoText;

        public void SetModel(WeaponModel weaponModel)
        {
            backgroundImage.color = weaponModel.WeaponConfig.panelColor;
            
            weaponModel.Selected.Subscribe(v => selected.gameObject.SetActive(v)).AddTo(this);
            weaponModel.Picked.Subscribe(v =>
            {
                backgroundImage.color = new Color(
                    weaponModel.WeaponConfig.panelColor.r,
                    weaponModel.WeaponConfig.panelColor.g,
                    weaponModel.WeaponConfig.panelColor.b,
                    v ? 1f : 0.1f);
                ammoText.gameObject.SetActive(v);
            }).AddTo(this);
        }
    }
}