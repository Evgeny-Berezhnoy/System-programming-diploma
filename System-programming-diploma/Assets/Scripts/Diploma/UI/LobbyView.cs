using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

namespace UI
{
    public class LobbyView : MonoBehaviour
    {
        #region Events

        private event Action _onMatchListRefresh;
        private event Action<string> _onMatchCreation;
        private event Action<MatchInfoSnapshot> _onMatchSelection;

        #endregion

        #region Fields

        [Header("UI")]
        [SerializeField] private RectTransform _matchContainer;
        [SerializeField] private Button _createMatchButton;
        [SerializeField] private Button _refreshMatchListButton;
        [SerializeField] private InputField _createMatchValue;

        [Header("Prefabs")]
        [SerializeField] private MatchLinkView _prefab;

        private Dictionary<NetworkID, MatchLinkView> _cachedMatches = new Dictionary<NetworkID, MatchLinkView>();

        #endregion

        #region Unity events

        private void OnDestroy()
        {
            _createMatchButton.onClick.RemoveAllListeners();
            _refreshMatchListButton.onClick.RemoveAllListeners();

            _cachedMatches.Clear();

            var matchCreationHandlers =
                _onMatchCreation
                    ?.GetInvocationList()
                    .Cast<Action<string>>()
                    .ToList();

            if (matchCreationHandlers != null)
            {
                for (int i = 0; i < matchCreationHandlers.Count; i++)
                {
                    _onMatchCreation -= matchCreationHandlers[i];
                };
            };

            var matchSelectionHandlers =
                _onMatchSelection
                    ?.GetInvocationList()
                    .Cast<Action<MatchInfoSnapshot>>()
                    .ToList();

            if (matchSelectionHandlers != null)
            {
                for (int i = 0; i < matchSelectionHandlers.Count; i++)
                {
                    _onMatchSelection -= matchSelectionHandlers[i];
                };
            };
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            _cachedMatches = new Dictionary<NetworkID, MatchLinkView>();

            _createMatchButton.onClick.RemoveAllListeners();
            _createMatchButton.onClick.AddListener(() => CreateMatch());

            _refreshMatchListButton.onClick.RemoveAllListeners();
            _refreshMatchListButton.onClick.AddListener(() => RefreshMatchList());
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SubscribeMatchListRefreshCallback(Action action)
        {
            _onMatchListRefresh += action;
        }

        public void SubscribeMatchCreationCallback(Action<string> action)
        {
            _onMatchCreation += action;
        }

        public void SubscribeMatchSelectionCallback(Action<MatchInfoSnapshot> action)
        {
            _onMatchSelection += action;
        }

        public void MatchListUpdate(List<MatchInfoSnapshot> matches)
        {
            if (matches != null)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    var matchInfo = matches[i];

                    UpdateMatch(matchInfo);
                };
            };

            var deleteMatchList = new List<MatchInfoSnapshot>();

            foreach(var cachedMatch in _cachedMatches)
            {
                if(matches == null
                    || matches.FirstOrDefault(x => x.networkId == cachedMatch.Key) == default(MatchInfoSnapshot))
                {
                    deleteMatchList.Add(cachedMatch.Value.Information);
                };
            };

            for(int i = 0; i < deleteMatchList.Count; i++)
            {
                DeleteMatch(deleteMatchList[i]);  
            };
        }

        private void AddMatch(MatchInfoSnapshot matchInfo)
        {
            var matchLinkView = Instantiate(_prefab, _matchContainer);

            _cachedMatches.Add(matchInfo.networkId, matchLinkView);

            matchLinkView.SubscribeJoinMatchCallback(value => JoinMatch(value));
        }

        private void UpdateMatch(MatchInfoSnapshot matchInfo)
        {
            if (!_cachedMatches.ContainsKey(matchInfo.networkId))
            {
                AddMatch(matchInfo);
            };

            _cachedMatches[matchInfo.networkId].Information = matchInfo;
        }

        private void DeleteMatch(MatchInfoSnapshot matchInfo)
        {
            if (!_cachedMatches.ContainsKey(matchInfo.networkId)) return;

            _cachedMatches.TryGetValue(matchInfo.networkId, out var room);
            
            if (_cachedMatches.Remove(matchInfo.networkId))
            {
                Destroy(room.gameObject);
            };
        }

        private void CreateMatch()
        {
            _onMatchCreation?.Invoke(_createMatchValue.text);
        }

        private void RefreshMatchList()
        {
            _onMatchListRefresh?.Invoke();
        }

        private void JoinMatch(MatchInfoSnapshot matchInformation)
        {
            _onMatchSelection?.Invoke(matchInformation);
        }

        #endregion
    }
}