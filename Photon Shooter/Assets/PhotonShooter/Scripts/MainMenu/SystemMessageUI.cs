using PhotonShooter.Scripts.Connection;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PhotonShooter.Scripts.MainMenu
{
    public class SystemMessageUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text messageText;
        [SerializeField] private Button okButton;

        [Inject] private MatchManager matchManager;


        private void Start()
        {
            okButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (panel.activeSelf) panel.SetActive(false);
            }).AddTo(this);

            ShowMatchState(matchManager.State);
        }

        private void ShowMatchState(MatchState matchState)
        {
            switch (matchState)
            {
                case MatchState.EnemyLeftMatch:
                    ShowMessage("Enemy left the match");
                    break;
                case MatchState.LeftMatch:
                    ShowMessage("You left the match");
                    break;
                case MatchState.MatchOverWinPlayer:
                    ShowMessage("You win!");
                    break;
                case MatchState.MatchOverWinEnemy:
                    ShowMessage("You lost!");
                    break;
            }
        }

        private void ShowMessage(string message)
        {
            panel.SetActive(true);
            messageText.text = message;
        }
    }
}