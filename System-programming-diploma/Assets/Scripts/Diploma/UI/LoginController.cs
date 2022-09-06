using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoginController : MonoBehaviour
    {
        #region Events

        private event Action _onLogin;

        #endregion

        #region Fields

        [SerializeField] private InputField _playerName;
        [SerializeField] private Button _loginButton;

        #endregion

        #region Unity events

        private void OnDestroy()
        {
            Clear();
        }

        #endregion

        #region Methods
        
        public void Initialize()
        {
            Clear();
            
            _loginButton.onClick.AddListener(() => Login());
        }

        public void SubscribeLoginCallback(Action action)
        {
            _onLogin += action;
        }

        private void Login()
        {
            if (string.IsNullOrWhiteSpace(_playerName.text)) return;
            
            PlayerPrefs.SetString("Player name", _playerName.text);

            gameObject.SetActive(false);

            _onLogin?.Invoke();
        }

        private void Clear()
        {
            _loginButton.onClick.RemoveAllListeners();

            var loginHandlers =
                _onLogin
                    ?.GetInvocationList()
                    .Cast<Action>()
                    .ToList();

            if (loginHandlers != null)
            {
                for (int i = 0; i < loginHandlers.Count; i++)
                {
                    _onLogin -= loginHandlers[i];
                };
            };
        }

        #endregion
    }
}
