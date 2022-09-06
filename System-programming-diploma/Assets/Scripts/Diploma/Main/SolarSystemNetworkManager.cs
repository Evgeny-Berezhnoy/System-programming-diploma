using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        #region Unity events

        private void Start()
        {
            StartMatchMaker();
        }

        private void OnDestroy()
        {
            StopMatchMaker();
        }

        #endregion

        #region Base methods

        public override void OnServerAddPlayer(
            NetworkConnection conn,
            short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player =
                Instantiate(
                    playerPrefab,
                    spawnTransform.position,
                    spawnTransform.rotation);

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        #endregion
    }
}
