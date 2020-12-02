using PhotonShooter.Scripts.Connection;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PhotonShooter.Scripts.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject connectingState;
        [SerializeField] private GameObject readyState;
        [SerializeField] private GameObject matchmakingState;
        [SerializeField] private Button playButton;
        [SerializeField] private Button cancelMatchmakingButton;

        [Inject] private ConnectionManager connectionManager;
        [Inject] private MatchManager matchManager;

        private void Start()
        {
            Observable.CombineLatest<bool, MatchState, (bool, MatchState)> 
                (connectionManager.Connecting, matchManager.StateReactive, (b, state) => (b, state))
                .Subscribe(tuple =>
                {
                    connectingState.SetActive(tuple.Item1 || tuple.Item2==MatchState.WaitingForRoom);
                    matchmakingState.SetActive(tuple.Item2==MatchState.WaitingForEnemy);
                    readyState.SetActive(!tuple.Item1 && tuple.Item2.IsCanCreateMatchState());
                }).AddTo(this);

            playButton.OnClickAsObservable().Subscribe(_ => matchManager.FindMatch()).AddTo(this);
            cancelMatchmakingButton.OnClickAsObservable().Subscribe(_ => matchManager.CancelWaiting()).AddTo(this);
        }
    }
}