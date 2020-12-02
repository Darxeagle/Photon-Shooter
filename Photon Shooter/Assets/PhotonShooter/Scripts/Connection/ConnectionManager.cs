using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;

namespace PhotonShooter.Scripts.Connection
{
    public class ConnectionManager : IConnectionCallbacks
    {
        private ReactiveProperty<bool> connecting = new ReactiveProperty<bool>(false);
        public IReadOnlyReactiveProperty<bool> Connecting => connecting;
        
        
        public ConnectionManager()
        {
            PhotonNetwork.AddCallbackTarget(this);    
        }

        public void Connect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                connecting.Value = true;
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = "1";
            }
        }

        #region Connection callbacks
        
        public void OnConnected()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnected() was called by PUN");
            connecting.Value = false;
        }

        public void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            connecting.Value = false;
        }
        
        public void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnRegionListReceived() was called by PUN");
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnCustomAuthenticationResponse() was called by PUN");
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnCustomAuthenticationFailed() was called by PUN");
        }
        
        #endregion
    }
}