using System;
using PhotonShooter.Scripts.Connection;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PhotonShooter.Scripts.Gameplay.Ui
{
    public class ExitUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        [Inject] private MatchManager matchManager;

        public bool IsOpen { get; private set; } = false;

        private void Start()
        {
            panel.SetActive(false);
            yesButton.OnClickAsObservable().Subscribe(_ =>
            {
                matchManager.LeaveMatch();
                panel.SetActive(false);
                IsOpen = false;
            }).AddTo(this);
            noButton.OnClickAsObservable().Subscribe(_ =>
            {
                panel.SetActive(false);
                Screen.lockCursor = true;
                IsOpen = false;
            }).AddTo(this);
        }

        private void Update()
        {
            if (!IsOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                panel.SetActive(true);
                Screen.lockCursor = false;
                IsOpen = true;
            }
        }
    }
}