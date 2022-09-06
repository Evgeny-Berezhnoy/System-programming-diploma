using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

using UI;

namespace Network
{
    public class LobbyWindowManager : MonoBehaviour
    {
        #region Fields

        [Header("Settings")]
        [SerializeField, Range(2, 8)] private uint _matchSize = 4;

        [Header("Scripts")]
        [SerializeField] private LoginController _loginController;
        [SerializeField] private LobbyView _lobby;

        #endregion

        #region Unity events

        private void Start()
        {
            _lobby.Initialize();
            _lobby.SubscribeMatchListRefreshCallback(RefreshMatchList);
            _lobby.SubscribeMatchCreationCallback(CreateMatch);
            _lobby.SubscribeMatchSelectionCallback(SelectMatch);

            _loginController.Initialize();
            _loginController.SubscribeLoginCallback(OnLogin);
        }

        #endregion

        #region Methods

        private void OnLogin()
        {
            _lobby.Show();

            RefreshMatchList();
        }

        private void RefreshMatchList()
        {
            NetworkManager.singleton.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnRefreshMatchList);
        }

        private void OnRefreshMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            if (!success)
            {
                Debug.LogError("An error arised while getting the list of matches: " + extendedInfo);
            };

            _lobby.MatchListUpdate(matches);
        }

        private void CreateMatch(string matchName)
        {
            NetworkManager.singleton.matchMaker.CreateMatch(matchName, _matchSize, true, "", "", "", 0, 0, OnCreateMatch);
        }

        private void OnCreateMatch(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (!success)
            {
                Debug.LogError("An error arised while creating the match: " + extendedInfo);

                return;
            };

            StartMatch(matchInfo, true);
        }

        private void SelectMatch(MatchInfoSnapshot matchInfo)
        {
            NetworkManager.singleton.matchMaker.JoinMatch(matchInfo.networkId, "", "", "", 0, 0, OnSelectMatch);
        }

        private void OnSelectMatch(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (!success)
            {
                Debug.LogError("An error arised while selecting the match: " + extendedInfo);

                return;
            };

            StartMatch(matchInfo, false);
        }

        private void StartMatch(MatchInfo matchInfo, bool isHost)
        {
            if (isHost)
            {
                NetworkManager.singleton.StartHost(matchInfo);
            }
            else
            {
                NetworkManager.singleton.StartClient(matchInfo);
            };
        }

        #endregion
    }
}