using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;


namespace WORLDGAMEDEVELOPMENT
{
    internal class CanvasController : IController, ICleanup, IInitialization, ILateExecute, IExecute, IEventPaused
    {
        private CanvasModel _canvasModel;
        private SceneModel _sceneModel;
        private InputInitialization _inputInitialization;
        private PlayerModel _playerModel;
        private PanelHUDView _panelHUD;
        private PanelMainMenuView _panelMainMenu;
        private PanelResultsView _panelResults;
        private List<IEventAction> _listEvent = new();
        private List<Button> _allButtonClearList = new();
        private bool _isPaused;
        private bool _isGameStarted;

        internal event Action<EventCanvas> StartGame;
        public event Action<bool> OnPause;

        private Timer _timerToLeftInGame;
        private Timer _timerLevelLeft;
        private float _distanceTravel;
        private Dictionary<PanelView, bool> _viewsStateActive;
        private GameUser _registerUser;
        private SceneControllerUIView _sceneControllerUIView;
        private panelDieView _panelDie;
        private PanelAuthorizationView _authorizationView;

        public CanvasController(CanvasModel canvasModel, PlayerModel playerModel, SceneModel sceneModel, InputInitialization inputInitialization)
        {
            _canvasModel = canvasModel;
            _sceneModel = sceneModel;
            _inputInitialization = inputInitialization;

            _sceneModel.SceneStruct.BroadcastEventManager.OnStartGame += OnStartGame;
            _sceneModel.SceneStruct.BroadcastEventManager.OnAuthGame += OnAuthGame;

            _sceneModel.SceneStruct.BroadcastEventManager.OnRegisterUser += OnRegisterUser;
            _sceneModel.SceneStruct.BroadcastEventManager.OnSignedUser += OnSignedUser;
            _sceneModel.SceneStruct.BroadcastEventManager.OnSuccessPostRegister += OnSuccessPostRegister;
            _sceneModel.SceneStruct.BroadcastEventManager.OnParsedResults += OnParsedResults;
            _sceneModel.SceneStruct.BroadcastEventManager.OnError += OnError;
            _sceneModel.SceneStruct.BroadcastEventManager.OnGetSignedGameUser += OnGetSignedGameUser;

            _playerModel = playerModel;
            _playerModel.PlayerStruct.Player.Health.OnChangeHealth += OnChangeHealth;
            _playerModel.PlayerStruct.Player.Shield.OnChangeShield += OnChangeShield;
            _playerModel.PlayerStruct.Player.Expirience.OnChangeExpirience += OnChangeExpirience;
            _playerModel.PlayerStruct.Player.IsDeadPlayer += IsDeadPlayer;

            _timerToLeftInGame = new Timer();
            _timerToLeftInGame.OnChangeFullTime += OnChangeTimeToLeftInGame;

            CreatePanelTypeFromByCanvasViewPanelViews();

            foreach (var item in _canvasModel.CanvasStruct.CanvasView.transform.GetComponentsInChildren<Button>())
            {
                _allButtonClearList.Add(item);
            }
        }


        private void CreatePanelTypeFromByCanvasViewPanelViews()
        {
            foreach (var panel in _canvasModel.CanvasStruct.CanvasView.panelViews)
            {
                panel.gameObject.SetActive(false);

                if (panel is SceneControllerUIView sceneControllerUIView)
                {
                    _sceneControllerUIView = sceneControllerUIView;
                }
                if (panel is panelDieView panelDie)
                {
                    _panelDie = panelDie;
                    _panelDie.ButtonContinue.onClick.AddListener(ContinueAfterDead);
                    _allButtonClearList.Add(_panelDie.ButtonContinue);
                }
                if (panel is PanelHUDView panelHUD)
                {
                    _panelHUD = panelHUD;
                }
                if (panel is PanelMainMenuView panelMainMenu)
                {
                    _panelMainMenu = panelMainMenu;
                }
                if (panel is PanelResultsView panelResults)
                {
                    _panelResults = panelResults;
                }
                if (panel is PanelAuthorizationView authorizationView)
                {
                    _authorizationView = authorizationView;
                }
            }
        }

        private void OnSignedUser(FirebaseUser user)
        {
            if (_registerUser == null)
            {
                var pathSignedUser = Path.Combine(ManagerPath.USERS, user.uid);
                FirebaseDatabase.GetJSON(pathSignedUser,
                    _sceneModel.SceneStruct.BroadcastEventManager.gameObject.name,
                    nameof(_sceneModel.SceneStruct.BroadcastEventManager.OnRequestSuccessGetGameUserJSON),
                    nameof(_sceneModel.SceneStruct.BroadcastEventManager.DisplayErrorObject));
            }
        }

        private void OnGetSignedGameUser(GameUser gameUser)
        {
            _registerUser = gameUser;

            if (_registerUser.TravelDistance > _distanceTravel)
            {
                _panelDie.TextSupport.gameObject.SetActive(true);
            }
            UpdateUserSettings();

            var userData = JsonConvert.SerializeObject(_registerUser);

            FirebaseDatabase.UpdateJSON(Path.Combine(ManagerPath.USERS, _registerUser.uid), userData,
                 _sceneModel.SceneStruct.BroadcastEventManager.gameObject.name,
               nameof(_sceneModel.SceneStruct.BroadcastEventManager.OnRequestSuccessPostJsonUserData),
               nameof(_sceneModel.SceneStruct.BroadcastEventManager.DisplayErrorObject));
        }

        private void OnError(string error)
        {
            if (_authorizationView.WarningLogintext.gameObject.activeSelf)
                _authorizationView.WarningLogintext.text = error;
            if (_authorizationView.WarningRegistertext.gameObject.activeSelf)
                _authorizationView.WarningRegistertext.text = error;
        }

        private void OnParsedResults(Dictionary<string, ResultsUser> results)
        {
            var currentPosition = 1;
            currentPosition = SetCurrentPositionByResultsTable(results, currentPosition);

            _panelDie.UserName.text = !string.IsNullOrEmpty(_registerUser?.displayName) ? _registerUser.displayName : _registerUser.email;

            var money = _registerUser.MoneyCount / 10.0f;

            _panelDie.Money.text = money.ToString();
            _panelDie.MaxDiscount.text = _registerUser.MaxDiscount.ToString();
            _panelDie.TravelDistance.text = _registerUser.TravelDistance.ToString();
            _panelDie.Score.text = _registerUser.MoneyCount.ToString();

            _panelDie.PositionResults.text = currentPosition.ToString();

            SwitchPanelDieByRegisterOrSignInUser();
        }

        private int SetCurrentPositionByResultsTable(Dictionary<string, ResultsUser> results, int currentPosition)
        {
            foreach (var resultsUser in results.Values)
            {
                if (_registerUser.uid == resultsUser.UserId)
                    continue;
                if (_registerUser.TravelDistance < resultsUser.TravelDistance)
                    currentPosition++;
            }

            return currentPosition;
        }

        private void OnSuccessPostRegister()
        {
            ResultsUser userResult = new ResultsUser
            {
                UserId = _registerUser.uid,
                TravelDistance = _registerUser.TravelDistance
            };

            var dataUserResult = JsonConvert.SerializeObject(userResult);
            var pathDataUserResult = Path.Combine(ManagerPath.RESULTS_TABLE, userResult.UserId);

            FirebaseDatabase.UpdateJSON(pathDataUserResult, dataUserResult,
               _sceneModel.SceneStruct.BroadcastEventManager.gameObject.name,
               nameof(_sceneModel.SceneStruct.BroadcastEventManager.OnRequestSuccessUpdateResultsJSON),
               nameof(_sceneModel.SceneStruct.BroadcastEventManager.DisplayErrorObject));
        }

        private void OnRegisterUser(FirebaseUser user)
        {
            _registerUser = ChangeGameUser(user);

            Debug.Log($"Пользователь после получения с сервера и назначения данных - {_registerUser}");

            var dataUser = JsonUtility.ToJson(_registerUser);
            var pathDataUser = Path.Combine(ManagerPath.USERS, _registerUser.uid);

            FirebaseDatabase.PostJSON(pathDataUser, dataUser,
                _sceneModel.SceneStruct.BroadcastEventManager.gameObject.name,
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.OnRequestSuccessPostJsonUserData),
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.DisplayErrorObject));
        }

        private GameUser ChangeGameUser(FirebaseUser user)
        {
            _registerUser.uid = user.uid;
            _registerUser.email = user.email;
            _registerUser.isEmailVerified = user.isEmailVerified;
            UpdateUserSettings();

            return _registerUser;
        }

        private void UpdateUserSettings()
        {
            _registerUser.MoneyCount = _panelHUD.Bonus;
            _registerUser.TravelDistance = _distanceTravel;

            var money = _registerUser.MoneyCount / 10.0f;
            _registerUser.MaxDiscount += money;
        }

        private void OnAuthGame(bool isAuthorization, string data = "")
        {
            if (isAuthorization)
            {
                _authorizationView.WarningLogintext.color = Color.green;
                _authorizationView.WarningLogintext.text = $"Успешная авторизация!";

                SwitchPanelDieByRegisterOrSignInUser();
            }
            else
            {
                _authorizationView.WarningLogintext.color = Color.red;
                _authorizationView.WarningLogintext.text = $"Авторизация не удалась.";
            }
        }

        public void Initialization()
        {
            _viewsStateActive = new Dictionary<PanelView, bool>();

            _panelMainMenu.ButtonQuit.onClick.AddListener(ApplicationQuit);

            ChangedAuthRules();

            UpdateHUDPlayer();
        }


        #region ICleanup

        public void Cleanup()
        {
            _playerModel.PlayerStruct.Player.Health.OnChangeHealth -= OnChangeHealth;
            _playerModel.PlayerStruct.Player.Shield.OnChangeShield -= OnChangeShield;
            _playerModel.PlayerStruct.Player.Expirience.OnChangeExpirience -= OnChangeExpirience;
            _playerModel.PlayerStruct.Player.IsDeadPlayer -= IsDeadPlayer;

            _sceneModel.SceneStruct.BroadcastEventManager.OnStartGame -= OnStartGame;
            _sceneModel.SceneStruct.BroadcastEventManager.OnAuthGame -= OnAuthGame;
            _sceneModel.SceneStruct.BroadcastEventManager.OnRegisterUser -= OnRegisterUser;
            _sceneModel.SceneStruct.BroadcastEventManager.OnSuccessPostRegister -= OnSuccessPostRegister;
            _sceneModel.SceneStruct.BroadcastEventManager.OnParsedResults -= OnParsedResults;
            _sceneModel.SceneStruct.BroadcastEventManager.OnError -= OnError;
            _sceneModel.SceneStruct.BroadcastEventManager.OnSignedUser -= OnSignedUser;
            _sceneModel.SceneStruct.BroadcastEventManager.OnGetSignedGameUser -= OnGetSignedGameUser;

            foreach (var eventAction in _listEvent)
            {
                if (eventAction is EnemyController enemyController)
                {
                    enemyController.AddScoreByAsteroidDead -= AddScoreByAsteroidDead;
                }

                if (eventAction is MoveController moveController && moveController is IEventActionGeneric<float> playerEvent)
                {
                    playerEvent.OnChangePositionAxisY -= PlayerEvent_EventFloatGeneric;
                }
            }
            foreach (var button in _allButtonClearList)
            {
                button.onClick.RemoveAllListeners();
            }
            _allButtonClearList.Clear();
        }

        #endregion


        private void ChangedAuthRules()
        {
            _authorizationView.ButtonLoginAuth.onClick.AddListener(LoginUser);
            _authorizationView.ButtonRegisterAuthNo.onClick.AddListener(RegisterUserWithEmailAndPassword);

            _allButtonClearList.Add(_authorizationView.ButtonLoginAuth);
            _allButtonClearList.Add(_authorizationView.ButtonRegisterAuthNo);
        }

        private void RegisterUserWithEmailAndPassword()
        {

            if (_authorizationView.PasswordRegisterInputField.text != _authorizationView.PasswordRegisterVerifyInputField.text)
            {
                _authorizationView.WarningRegistertext.text = "Пароли не совпадают!";
                return;
            }
            else if (_authorizationView.PasswordRegisterInputField.text.Length < 6)
            {
                _authorizationView.WarningRegistertext.text = "Пароль должен быть больше 6 символов!";
                return;
            }
            else if (string.IsNullOrEmpty(_authorizationView.PhoneInputField.text))
            {
                _authorizationView.WarningRegistertext.text = "Заполните поле телефон!";
                _authorizationView.PhoneInputField.text = "+7";
                return;
            }
            else if (_authorizationView.PhoneInputField.text.Length < 10)
            {
                _authorizationView.WarningRegistertext.text = "Телефон должен быть больше 9 символов!";
                return;
            }
            else
            {
                _authorizationView.WarningRegistertext.text = string.Empty;
            }

            _registerUser = new GameUser
            {
                displayName = _authorizationView.UsernameInputField.text,
                phoneNumber = _authorizationView.PhoneInputField.text,
                TravelDistance = _distanceTravel,
                MoneyCount = _panelHUD.Bonus,
                MaxDiscount = 1000,
            };

            Debug.Log($"Пользователь при регистрации - {_registerUser}");

            FirebaseAuth.CreateUserWithEmailAndPassword(_authorizationView.EmailRegisterInputField.text,
                _authorizationView.PasswordRegisterInputField.text,
                _sceneModel.SceneStruct.BroadcastEventManager.gameObject.name,
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.OnRequestSuccessRegister),
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.DisplayErrorObject));
        }

        private void LoginUser()
        {
            FirebaseAuth.SignInWithEmailAndPassword(_authorizationView.EmailInputField.text,
                _authorizationView.PasswordLoginInputField.text,
                _sceneModel.SceneStruct.BroadcastEventManager.gameObject.name,
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.OnRequestSuccessAuth),
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.OnRequestFailedAuth));
        }

        private void OnStartGame()
        {
            if (!_isGameStarted)
            {
                ButtonStartGame();
            }
            else
            {
                ResumeGame();
            }
        }

        private void ContinueAfterDead()
        {
            PauseOrResume(false);
            _panelDie.gameObject.SetActive(!_panelDie.gameObject.activeSelf);
            if (_panelDie.TextSupport.gameObject.activeSelf)
            {
                _panelDie.TextSupport.gameObject.SetActive(false);
            }
        }

        private void IsDeadPlayer()
        {
            PauseOrResume(true);

            if (_registerUser == null)
            {
                SwitchPanelDieByRegisterOrSignInUser();
            }
            else
            {
                UpdateUserSettings();

                SendUpdateResultsToFirebase();
                SendUpdateUserToFirebase();

                _sceneModel.SceneStruct.BroadcastEventManager.GetResultsTable();
            }
        }

        private void SendUpdateUserToFirebase()
        {
            var userData = JsonConvert.SerializeObject(_registerUser);
            var pathDataUser = Path.Combine(ManagerPath.USERS, _registerUser.uid);

            FirebaseDatabase.UpdateJSON(pathDataUser, userData,
                _sceneModel.SceneStruct.BroadcastEventManager.gameObject.name,
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.OnRequestSuccessUpdateJSONUser),
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.DisplayErrorObject));
        }

        private void SendUpdateResultsToFirebase()
        {
            var tempResults = new ResultsUser
            {
                UserId = _registerUser.uid,
                TravelDistance = _registerUser.TravelDistance
            };
            var resultsData = JsonConvert.SerializeObject(tempResults);
            var pathDataResults = Path.Combine(ManagerPath.RESULTS_TABLE, _registerUser.uid);

            FirebaseDatabase.UpdateJSON(pathDataResults, resultsData,
                _sceneModel.SceneStruct.BroadcastEventManager.gameObject.name,
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.OnRequestSuccessUpdateJSONResultsUser),
                nameof(_sceneModel.SceneStruct.BroadcastEventManager.DisplayErrorObject));
        }

        /// <summary>
        /// Сбросили деньги на панели HUD после смерти игрока. Игрок начинает накопление сначала.
        /// </summary>
        private void ResetHUDMoney()
        {
            _panelHUD.Bonus = 0.0f;
            UpdateHUDPlayer();
        }

        private void SwitchPanelDieByRegisterOrSignInUser()
        {
            if (_registerUser != null)
            {
                Debug.Log($"Пользователь авторизован.");
                _authorizationView.gameObject.SetActive(false);
                ResetHUDMoney();

                _panelDie.gameObject.SetActive(true);
                _panelDie.panelDieAuthYes.gameObject.SetActive(true);
                _panelDie.panelDieAuthNo.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log($"Пользователь не авторизован.");
                _panelDie.gameObject.SetActive(true);

                _panelDie.panelDieAuthYes.SetActive(false);
                _panelDie.panelDieAuthNo.SetActive(true);
            }
        }

        private void OnChangeExpirience(Expirience exp)
        {
            _panelHUD.Expirience.Update(exp.MaxValue, exp.CurrentValue);
            _panelHUD.TextPlayerLevel.text = _playerModel.PlayerStruct.Player.Expirience.CurrentLevel.ToString();
        }

        private void OnChangeShield(Shield shield)
        {
            _panelHUD.Shield.Update(shield.MaxValue, shield.CurrentValue);
        }

        private void OnChangeHealth(Health health)
        {
            _panelHUD.Health.Update(health.MaxHealth, health.CurrentHealth);
        }

        private void OnChangeTimeToLeftInGame(string time)
        {
            _panelResults.TextElapsedTime.text = time;
        }

        internal void Add(IEventAction eventAction)
        {
            _listEvent.Add(eventAction);

            if (eventAction is EnemyController enemyController)
            {
                enemyController.AddScoreByAsteroidDead += AddScoreByAsteroidDead;
            }

            if (eventAction is MoveController moveController && moveController is IEventActionGeneric<float> playerEvent)
            {
                playerEvent.OnChangePositionAxisY += PlayerEvent_EventFloatGeneric;
                moveController.OnChangeSpeedMovement += OnChangeSpeedMovement;
            }
        }

        private void AddScoreByAsteroidDead(float value)
        {
            _panelHUD.Bonus += value;
            UpdateHudBonusText();
        }

        private void OnChangeSpeedMovement(float speed)
        {
            _panelResults.TextSpeed.text = speed.ToString("F0");
        }

        private void PlayerEvent_EventFloatGeneric(float value)
        {
            _distanceTravel += value;
            _panelResults.TextDistanceTraveled.text = _distanceTravel.ToString(format: "F0");

            _panelResults._sliderTravel.value = _distanceTravel / _playerModel.PlayerStruct.DistanceToMars;
        }

        private void UpdateHUDPlayer()
        {
            OnChangeShield(_playerModel.PlayerStruct.Player.Shield);
            OnChangeHealth(_playerModel.PlayerStruct.Player.Health);
            OnChangeExpirience(_playerModel.PlayerStruct.Player.Expirience);
            UpdateHudBonusText();
        }

        private void UpdateHudBonusText()
        {
            _panelHUD.TextScore.text = _panelHUD.Bonus.ToString();
        }

        private void ButtonStartGame()
        {
            _panelMainMenu.gameObject.SetActive(true);
            _panelResults.gameObject.SetActive(true);
            _sceneControllerUIView.gameObject.SetActive(true);

            _isGameStarted = true;

            StartGame?.Invoke(EventCanvas.StartGame);

            if (_isPaused)
            {
                _isPaused = false;
                PauseOrResume(_isPaused);
            }

            _panelHUD.gameObject.SetActive(true);

            StartGame?.Invoke(EventCanvas.StartShip);
        }

        private void ResumeGame()
        {
            PauseOrResume(!_isPaused);
        }

        public void LateExecute(float deltatime)
        {
            //TODO - переделать - убрать в Сценеконтроллер, и подписываться на него...
            if (Input.GetKeyDown(KeyCode.Escape) && !_isPaused)
            {
                ApplicationQuit();
            }
        }

        private void PauseOrResume(bool value)
        {
            _isPaused = value;
            SwitchPanelView(_isPaused);
            OnPause?.Invoke(_isPaused);
        }

        public void Execute(float deltatime)
        {
            _timerToLeftInGame.Execute(deltatime);
        }

        #region ApplicationQuit

        private void ApplicationQuit()
        {
            PauseOrResume(!_isPaused);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
            OpenLinks.GoBackPage();
#else
            Application.Quit();
#endif
        }

        #endregion


        /// <summary>
        /// disable all panels except the death panel
        /// </summary>
        /// <param name="isPaused">pause in the game</param>
        private void SwitchPanelView(bool isPaused)
        {
            foreach (var panelView in _canvasModel.CanvasStruct.CanvasView.panelViews)
            {
                if (panelView is panelDieView)
                    continue;
                if (isPaused)
                {
                    _viewsStateActive[panelView] = panelView.gameObject.activeSelf;
                    panelView.gameObject.SetActive(!isPaused);
                }
                else
                {
                    panelView.gameObject.SetActive(_viewsStateActive[panelView]);
                }
            }
        }
    }
}