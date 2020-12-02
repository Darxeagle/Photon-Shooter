using Photon.Pun;
using PhotonShooter.Scripts.Connection;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PhotonShooter.Scripts
{
    public class ProjectController
    {
        [Inject] private ConnectionManager connectionManager;
        [Inject] private MatchManager matchManager;

        [Inject]
        public void Initialize()
        {
            connectionManager.Connect();
            
            matchManager.StateReactive.Where(state => state.IsPlayingState()).Subscribe(_ =>
            {
                PhotonNetwork.LoadLevel(1);
                Screen.lockCursor = true;
            });
            
            matchManager.StateReactive.Where(state => state.IsLeftMatchState()).Subscribe(_ =>
            {
                PhotonNetwork.LoadLevel(0);
                Screen.lockCursor = false;
            });
        }
    }
}