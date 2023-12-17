using System.Runtime.InteropServices;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class GameController : MonoBehaviour, ICleanup
    {
        #region DLL_Import

        [DllImport("__Internal")]
        public static extern void ShowButonStart();

        #endregion


        [SerializeField] private Data _data;
        private Controllers _controllers;



        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Start()
        {
            _controllers = new Controllers();
            var cameraFactory = new CameraFactory(_data.CameraData);
            var cameraInitialization = new CameraInitialization(cameraFactory);

            var inputInitialization = new InputInitialization(cameraInitialization.CameraModel.Components.CameraView.Camera);
            _controllers.Add(new InputController(inputInitialization));

            var sceneFactory = new SceneFactory(_data.SceneData);
            var sceneInitialization = new SceneInitialization(sceneFactory);
            var sceneController = new SceneController(sceneInitialization.SceneModel);
            _controllers.Add(sceneController);

            var playerFactory = new PlayerFactory(_data.PlayerData);
            var playerInitialization = new PlayerInitialization(playerFactory,
                sceneInitialization.SceneModel.SceneStruct.StartSceneView.StartSpaceTransform);

            var canvasFactory = new CanvasFactory(_data.CanvasData);
            var canvasInitialization = new CanvasInitialization(canvasFactory);
            var canvasController = new CanvasController(canvasInitialization.CanvasModel, playerInitialization.PlayerModel, 
                                        sceneInitialization.SceneModel, inputInitialization);
            sceneController.Add(canvasController);

            var newUIInputController = new UIInputController(inputInitialization, canvasInitialization.CanvasModel);
            _controllers.Add(newUIInputController);

            sceneController.Add(canvasInitialization.CanvasModel);
            _controllers.Add(canvasController);

            sceneController.Add(playerInitialization.PlayerModel);

            var ammunitionFactory = new AmmunitionFactory(_data.AmmunitionData);
            var ammunitionInitialization = new AmmunitionInitialization(ammunitionFactory);

            var enemyFactory = new EnemyFactory(_data.EnemyData);
            var enemyInitialization = new EnemyInitialization(enemyFactory);

            var playerController = new PlayerController(inputInitialization, playerInitialization, 
                cameraInitialization.CameraModel.Components.CameraView.Camera, sceneController, canvasInitialization.CanvasModel.CanvasStruct.CanvasView);
            sceneController.Add(playerController);
            _controllers.Add(playerController);
            canvasController.Add(playerController.MoveController);
          
            var cameraController = new CameraController(cameraInitialization.CameraModel,
                playerInitialization.PlayerModel.Components.PlayerTransform, sceneController, playerController);

            sceneController.Add(cameraController);
            _controllers.Add(cameraController);

            var playerShooterController = new PlayerShooterController(playerInitialization, ammunitionInitialization.AmmunitionFactoryModel, sceneController);

            _controllers.Add(playerShooterController);

            var enemyController = new EnemyController(enemyInitialization.Model, sceneController, playerInitialization.PlayerModel, ammunitionInitialization);
            _controllers.Add(enemyController);
            canvasController.Add(enemyController);

            var particleController = new ParticleController(playerInitialization.PlayerModel, sceneController, playerController);
            _controllers.Add(particleController);

            var audioFactory = new AudioFactory(_data.AudioData);
            var audioInitialization = new AudioInitialization(audioFactory);
            var audioController = new AudioController(audioInitialization.AudioModel, playerInitialization.PlayerModel,
                playerShooterController, enemyController, canvasController, canvasInitialization.CanvasModel, sceneController);
            _controllers.Add(audioController);

            var VFXFactory = new VFXFactory(_data.VFXData);
            var VFXInitialization = new VFXInitialization(VFXFactory);
            var vfxController = new VFXController(VFXInitialization.Model, enemyController, playerShooterController);
            _controllers.Add(vfxController);

            var backgroundFactory = new BackgroundFactory(_data.BackgroundData);
            var backgroundInitialization = new BackgroundInitialization(backgroundFactory);
            var backgroundController = new BackgroundController(backgroundInitialization.BackgroundModel, 
                cameraInitialization.CameraModel.Components.CameraView.Camera, sceneController);
            _controllers.Add(backgroundController);

            var updateCharachterController = new UpdateCharachterController(playerInitialization, canvasInitialization);
            _controllers.Add(updateCharachterController);

            _controllers.Initialization();


#if UNITY_EDITOR
            Debug.Log($"Не трогаем кнопку ShowButonStart");
#elif UNITY_WEBGL
            ShowButonStart();
#endif

        }

        private void Update()
        {
            _controllers.Execute(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _controllers.FixedExecute(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            _controllers.LateExecute(Time.deltaTime);
        }

        public void Cleanup()
        {
            _controllers.Cleanup();
        }
    }
}