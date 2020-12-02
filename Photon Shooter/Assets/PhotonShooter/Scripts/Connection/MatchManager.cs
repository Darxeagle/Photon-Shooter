using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PhotonShooter.Scripts.Connection
{
    public class MatchManager : IMatchmakingCallbacks, IInRoomCallbacks
    {
        private ReactiveProperty<MatchState> state = new ReactiveProperty<MatchState>();

        public MatchState State => state.Value;
        public IReadOnlyReactiveProperty<MatchState> StateReactive => state;
        

        public MatchManager()
        {
            PhotonNetwork.AddCallbackTarget(this);
            SetState(MatchState.Disconnected);
        }

        public void FindMatch()
        {
            if (State.IsCanCreateMatchState())
            {
                SetState(MatchState.WaitingForRoom);
                PhotonNetwork.NetworkingClient.OpJoinRandomOrCreateRoom(null,
                    new EnterRoomParams()
                    {
                        RoomOptions = new RoomOptions()
                        {
                            MaxPlayers = 2,
                            EmptyRoomTtl = 0
                        }
                    });
            }
        }

        public void CancelWaiting()
        {
            if (State == MatchState.WaitingForEnemy)
            {
                SetState(MatchState.CancellingWait);
                PhotonNetwork.NetworkingClient.OpLeaveRoom(true);
            }
        }
        
        public void LeaveMatch(MatchState fromState = MatchState.LeavingMatch)
        {
            if (State == MatchState.Running)
            {
                SetState(fromState);
                PhotonNetwork.NetworkingClient.OpLeaveRoom(true);
            }
        }

        private void SetState(MatchState value)
        {
            state.Value = value;
        }

        #region Matchmaking callbacks
        
        public void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom");
            if (State == MatchState.WaitingForRoom)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
                {
                    SetState(MatchState.Running);
                }
                else
                {
                    SetState(MatchState.WaitingForEnemy);
                }
            }
        }
        
        public void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom");
            switch (State)
            {
                case MatchState.CancellingWait:
                    SetState(MatchState.CancelledWait);
                    break;
                case MatchState.EnemyLeavingMatch:
                    SetState(MatchState.EnemyLeftMatch);
                    break;
                case MatchState.LeavingMatch:
                    SetState(MatchState.LeftMatch);
                    break;
                case MatchState.MatchLeavingWinPlayer:
                    SetState(MatchState.MatchOverWinPlayer);
                    break;
                case MatchState.MatchLeavingWinEnemy:
                    SetState(MatchState.MatchOverWinEnemy);
                    break;
            }
        }
        
        
        public void OnCreatedRoom()
        {
            Debug.Log("OnCreatedRoom");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogFormat("OnCreateRoomFailed {0} {1}", returnCode, message);
        }
        
        public void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogFormat("OnJoinRoomFailed {0} {1}", returnCode, message);
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("OnJoinRandomFailed with message {0} :: {1}", returnCode, message);
        }
        
        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            Debug.Log("OnFriendListUpdate");
        }

        #endregion
        
        #region InRoom callbacks

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("OnPlayerEnteredRoom");
            if (State == MatchState.WaitingForEnemy)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    SetState(MatchState.Running);
                }
            }
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("OnPlayerLeftRoom");
            if (State == MatchState.Running)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    LeaveMatch(MatchState.EnemyLeavingMatch);
                }
            }
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            Debug.Log("OnRoomPropertiesUpdate");
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            Debug.Log("OnRoomPropertiesUpdate");
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log("OnRoomPropertiesUpdate");
        }
        
        #endregion
    }
}