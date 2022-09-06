using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

namespace UI
{
    public class MatchLinkView : MonoBehaviour
    {
        #region Events

        private event Action<MatchInfoSnapshot> _onButtonClicked;

        #endregion

        #region Fields

        [SerializeField] private Button _joinMatchButton;

        [SerializeField] private Text _matchName;
        [SerializeField] private Text _occupied;
        [SerializeField] private Text _capacity;

        private MatchInfoSnapshot _matchInformation;

        #endregion

        #region Properties

        public MatchInfoSnapshot Information
        {
            get => _matchInformation;
            set
            {
                _matchInformation = value;

                _matchName.text = _matchInformation.name;
                _occupied.text  = _matchInformation.currentSize.ToString();
                _capacity.text  = _matchInformation.maxSize.ToString();

                _joinMatchButton.interactable = _matchInformation.currentSize < _matchInformation.maxSize;
            }
        }

        #endregion

        #region Unity events

        private void Start()
        {
            _joinMatchButton.onClick.AddListener(() => JoinMatch());
        }

        private void OnDestroy()
        {
            _joinMatchButton.onClick.RemoveAllListeners();

            var handlers =
                _onButtonClicked
                    ?.GetInvocationList()
                    .Cast<Action<MatchInfoSnapshot>>()
                    .ToList();

            if (handlers != null)
            {
                for (int i = 0; i < handlers.Count; i++)
                {
                    _onButtonClicked -= handlers[i];
                };
            };
        }

        #endregion

        #region Methods

        public void SubscribeJoinMatchCallback(Action<MatchInfoSnapshot> action)
        {
            _onButtonClicked += action;
        }

        public void JoinMatch()
        {
            _onButtonClicked?.Invoke(_matchInformation);
        }

        #endregion
    }
}