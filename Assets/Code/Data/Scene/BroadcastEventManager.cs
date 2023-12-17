using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    public sealed class BroadcastEventManager : MonoBehaviour
    {
        #region Fields

        public event Action<Dictionary<string, ResultsUser>> OnParsedResults;
        public event Action OnStartGame;
        public event Action OnSuccessPostRegister;
        public event Action OnSuccessPostResults;
        public event Action<bool, string> OnAuthGame;
        public event Action<FirebaseUser> OnRegisterUser;
        public event Action<FirebaseUser> OnSignedUser;
        public event Action<GameUser> OnGetSignedGameUser;
        public event Action<string> OnError;

        #endregion


        public void StartGame()
        {
            OnStartGame?.Invoke();
        }

        internal void OnRequestFailedAuth(string error)
        {
            OnAuthGame?.Invoke(false, error);
        }

        internal void OnRequestSuccessAuth(string data)
        {
            FirebaseAuth.OnAuthStateChanged(gameObject.name, nameof(OnAuthUserSigned), nameof(DisplayInfo));
            OnAuthGame?.Invoke(true, data);
        }

        internal void OnRequestSuccessRegister(string data)
        {
            FirebaseAuth.OnAuthStateChanged(gameObject.name, nameof(InvokeNewRegisterUser), nameof(DisplayInfo));
        }

        internal void OnAuthUserSigned(string user)
        {
            var parsedUser = JsonConvert.DeserializeObject<FirebaseUser>(user);
            OnSignedUser?.Invoke(parsedUser);
        }

        private void InvokeNewRegisterUser(string user)
        {
            var parsedUser = JsonConvert.DeserializeObject<FirebaseUser>(user);
            OnRegisterUser?.Invoke(parsedUser);
        }

        public void DisplayErrorObject(string error)
        {
            var parsedError = JsonConvert.DeserializeObject<FirebaseError>(error);
            DisplayError(parsedError.message);
        }

        public void DisplayError(string error)
        {
            Debug.LogError(error);
            OnError?.Invoke(error);
        }

        public void DisplayInfo(string info)
        {
            Debug.Log(info);
        }

        internal void OnRequestSuccessPostJsonUserData(string data)
        {
            OnSuccessPostRegister?.Invoke();
        }

        internal void OnRequestSuccessUpdateResultsJSON(string data)
        {
            GetResultsTable();
        }

        internal void GetResultsTable()
        {
            FirebaseDatabase.GetJSON(ManagerPath.RESULTS_TABLE,
                            gameObject.name, nameof(OnRequestSuccessGetResultsJSON), nameof(DisplayErrorObject));
        }

        internal void OnRequestSuccessGetResultsJSON(string data)
        {
            var parsedResults = JsonConvert.DeserializeObject<Dictionary<string, ResultsUser>>(data);
            OnParsedResults?.Invoke(parsedResults);
        }

        internal void OnRequestSuccessUpdateJSONResultsUser(string data)
        {
            Debug.Log($"Данные результатов пользователя, были успешно обновлены.\n {data}");
        }

        internal void OnRequestSuccessUpdateJSONUser(string data)
        {
            Debug.Log($"Данные пользователя, были успешно обновлены.\n {data}");
        }

        internal void OnRequestSuccessGetGameUserJSON(string user)
        {
            var gameUser = JsonConvert.DeserializeObject<GameUser>(user);
            if (gameUser == null)
                Debug.LogError($"No DeserializeObject {gameUser}");
            else
                OnGetSignedGameUser?.Invoke(gameUser);
        }
    }
}