using System;
using System.Collections.Generic;
using UnityEngine;

namespace WORLDGAMEDEVELOPMENT
{
    internal class PlayerController : ICleanup, IExecute, IFixedExecute, ILateExecute, IAddedModel, IInitialization
    {
        #region Fields

        internal readonly PlayerInitialization PlayerInitialization;
        private readonly InputInitialization _inputInitialization;
        private readonly Camera _camera;
        private readonly SceneController _sceneController;
        private readonly CanvasView _canvasView;
        private MoveController _moveController;
        private RotationController _rotationController;
        private List<IController> _controllers;
        private bool _stopControl;
        private float _timeFreezeDead;
        internal event Action IsAlivePlayer;

        #endregion


        #region Properties

        internal RotationController RotationController
        {
            get
            {
                if (_rotationController == null)
                {
                    _rotationController = new RotationController(_inputInitialization.GetInput(), PlayerInitialization.PlayerModel.Components.PlayerTransform,
                                                _camera, _sceneController);
                }
                return _rotationController;
            }
        }

        internal MoveController MoveController
        {
            get
            {
                if (_moveController == null)
                {
                    _moveController = new MoveController(_inputInitialization.GetInput(), PlayerInitialization.PlayerModel.Components.RigidbodyPlayer,
                                               PlayerInitialization.PlayerModel.Components.PlayerTransform,
                                               PlayerInitialization.PlayerModel.PlayerStruct.Player.Speed,
                                               PlayerInitialization.PlayerModel, _camera,
                                               _sceneController, _canvasView.panelViews);
                }
                return _moveController;
            }
        }

        #endregion


        #region ClassLifeCycles

        public PlayerController(InputInitialization inputInitialization, PlayerInitialization playerInitialization, Camera camera, SceneController sceneController, CanvasView canvasView)
        {
            _inputInitialization = inputInitialization;
            PlayerInitialization = playerInitialization;
            _camera = camera;
            _sceneController = sceneController;
            _sceneController.IsStopControl += OnCnageIsStopControl;
            _sceneController.DisableEnergyBlock += DisableEnergyBlock;
            PlayerInitialization.PlayerModel.PlayerStruct.Player.EnableShield += EnableShield;
            PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.OnChangeShield += Shield_OnChangeShield;

            _canvasView = canvasView;

            PlayerInitialization.PlayerModel.PlayerStruct.Player.IsDeadPlayer += IsDeadPlayerAndRestartPosition;

            _controllers = new()
            {
                MoveController,
                RotationController,
            };

            MoveController.OnChangeBlockReset += OnChangeBlockReset;
            MoveController.DisableEnergyBlock += DisableEnergyBlock;
        }

        private void Shield_OnChangeShield(Shield shield)
        {
            var particle = PlayerInitialization.PlayerModel.Components.ShieldView.ShieldParticle.main;
            if (shield.CurrentValue <= 30)
            {
                particle.startColor = Color.red;
            }
            else
            {
                particle.startColor = Color.blue;
            }
        }

        private void EnableShield()
        {
            PlayerInitialization.PlayerModel.Components.ShieldView.PlayShield();
            PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.ResetLastDamage();
        }

        public void UpdateShield(float deltatime)
        {
            if (!PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.IsRemaining)
                return;

            PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.TimeSinceLastDamage += deltatime;

            if (PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.TimeSinceLastDamage >= PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.ShieldRecoveryTime)
            {
                float recoveryRate = 0.3f;

                float recoveryAmount = PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.MaxValue * recoveryRate * deltatime;

                var value = Mathf.Min(PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.MaxValue,
                        PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.CurrentValue + recoveryAmount);

                if (!(value <= PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.CurrentValue))
                {
                    PlayerInitialization.PlayerModel.PlayerStruct.Player.Shield.CurrentValue = value;
                }
            }
        }

        private void IsDeadPlayerAndRestartPosition()
        {
            _stopControl = true;
            //Можно например стартовать прямо со стартовой площадки...
            PlayerInitialization.PlayerModel.PlayerStruct.Player.transform.position = new Vector3(0, 0, 0);
            PlayerInitialization.PlayerModel.PlayerStruct.Player.transform.rotation = Quaternion.identity;
            _timeFreezeDead = 0.0f;
        }

        private void DisableEnergyBlock()
        {
            ParticleSystem.MainModule mainModule = PlayerInitialization.PlayerModel.Components.ParticlesStarSystem.main;
            mainModule.gravityModifier = PlayerInitialization.PlayerModel.PlayerStruct.ParticleSpeedAfterTakeOff;

            PlayerInitialization.PlayerModel.Components.RigidbodyEnergyBlock.gameObject.SetActive(false);
            PlayerInitialization.PlayerModel.Components.RigidbodyEnergyBlock.transform.SetParent(PlayerInitialization.PlayerModel.Components.PlayerTransform);

            var position = PlayerInitialization.PlayerModel.Components.PlayerTransform.position + PlayerInitialization.PlayerModel.Settings.TransformPositionEnergyBlock;

            PlayerInitialization.PlayerModel.Components.RigidbodyEnergyBlock.transform.position = position;
            PlayerInitialization.PlayerModel.Components.RigidbodyEnergyBlock.transform.rotation = PlayerInitialization.PlayerModel.Components.PlayerTransform.rotation;
            PlayerInitialization.PlayerModel.Components.RigidbodyEnergyBlock.isKinematic = true;
        }

        private void OnChangeBlockReset()
        {
            PlayParticle();
            ActivateEnergyBlockRigidbody();
        }

        private void OnCnageIsStopControl(bool value)
        {
            if (value)
            {
                PlayerInitialization.PlayerModel.Components.RigidbodyPlayer.isKinematic = true;
            }
            else
            {
                PlayerInitialization.PlayerModel.Components.RigidbodyPlayer.isKinematic = false;
                ActivateEnergyBlockRigidbody();
            }
        }

        private void ActivateEnergyBlockRigidbody()
        {
            PlayerInitialization.PlayerModel.Components.RigidbodyEnergyBlock.transform.SetParent(null);
            PlayerInitialization.PlayerModel.Components.RigidbodyEnergyBlock.isKinematic = false;
        }

        private void PlayParticle()
        {
            if (!PlayerInitialization.PlayerModel.Components.ParticlesStarSystem.gameObject.activeSelf)
            {
                PlayerInitialization.PlayerModel.Components.ParticlesStarSystem.gameObject.SetActive(true);
            }
            if (PlayerInitialization.PlayerModel.Components.ParticlesStarSystem.isStopped
                    || PlayerInitialization.PlayerModel.Components.ParticlesStarSystem.isPaused)
            {
                PlayerInitialization.PlayerModel.Components.ParticlesStarSystem.Play();
            }
        }

        #endregion


        #region ICleanup

        public void Cleanup()
        {
            _moveController.Cleanup();
            PlayerInitialization.PlayerModel.PlayerStruct.Player.EnableShield -= EnableShield;
            MoveController.OnChangeBlockReset -= OnChangeBlockReset;
            _sceneController.IsStopControl -= OnCnageIsStopControl;
            _sceneController.DisableEnergyBlock -= DisableEnergyBlock;
            MoveController.DisableEnergyBlock -= DisableEnergyBlock;
            PlayerInitialization.PlayerModel.PlayerStruct.Player.IsDeadPlayer -= IsDeadPlayerAndRestartPosition;
        }

        #endregion


        #region IExecute

        public void Execute(float deltatime)
        {
            if (!_stopControl)
            {
                foreach (var controller in _controllers)
                {
                    if (controller is IExecute execute)
                    {
                        execute.Execute(deltatime);
                    }
                }
            }
            else
            {
                _timeFreezeDead += deltatime;
                if (_timeFreezeDead > 5.0f)
                {
                    _stopControl = false;
                    IsAlivePlayer?.Invoke();
                }
                else if (_timeFreezeDead > 1.0f && !PlayerInitialization.PlayerModel.PlayerStruct.Player.gameObject.activeSelf)
                {
                    PlayerInitialization.PlayerModel.PlayerStruct.Player.IsCanShoot?.Invoke(true);
                    PlayerInitialization.PlayerModel.PlayerStruct.Player.gameObject.SetActive(true);
                }
            }
        }

        public void LateExecute(float deltatime)
        {
            foreach (var controller in _controllers)
            {
                if (controller is ILateExecute execute)
                {
                    execute.LateExecute(deltatime);
                }
            }
        }

        public void FixedExecute(float fixedDeltatime)
        {
            foreach (var controller in _controllers)
            {
                if (controller is IFixedExecute execute)
                {
                    execute.FixedExecute(fixedDeltatime);
                }
            }

            UpdateShield(fixedDeltatime);
        }

        public void Initialization()
        {
            foreach (var controller in _controllers)
            {
                if (controller is IInitialization init)
                {
                    init.Initialization();
                }
            }

            EnableShield();
        }

        #endregion
    }
}