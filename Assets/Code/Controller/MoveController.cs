﻿using System;
using System.Collections.Generic;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal class MoveController : ICleanup, IExecute, IFixedExecute, IEventActionGeneric<float>, IInitialization
    {
        private readonly IUserInputProxy _inputHorizontal;
        private readonly IUserInputProxy _inputVertical;
        private readonly SceneController _sceneController;
        private Transform _playerTransform;
        private Speed _speed;
        private readonly Camera _camera;
        private readonly Rigidbody2D _rigidbodyPlayer;
        private readonly PlayerModel _playerModel;
        private readonly List<PanelView> _panelViews;
        private Vector3 _move;

        private float _vertical;
        private float _horizontal;
        private bool _isStopControl;


        private bool _isMovingUp = false;
        private float _moveDuration;
        private float _moveStartTime;
        private float _currentStatePosition;
        private bool _isMovingFreeControl;

        internal Action DisableEnergyBlock;
        private float _timeToDisableEnergyBlock;
        private bool _isDisableEnergyBlock;
        private bool _isBlockReset;
        private PanelHUDView _panelHUD;
        private float _previousSpeed;
        private float _cameraWidth;

        internal event Action<bool> TheShipTookOff;
        internal event Action OnChangeBlockReset;

        internal event Action<float> OnChangeSpeedMovement;
        public event Action<float> OnChangePositionAxisY;


        public MoveController((IUserInputProxy inputHorizontal, IUserInputProxy inputVertical) getInput,
            Rigidbody2D rigidbodyPlayer, Transform playerTransform, Speed speed, PlayerModel playerModel,
            Camera camera, SceneController sceneController, List<PanelView> panelViews)
        {
            _inputHorizontal = getInput.inputHorizontal;
            _inputVertical = getInput.inputVertical;
            _playerTransform = playerTransform;
            _speed = speed;
            _camera = camera;
            _rigidbodyPlayer = rigidbodyPlayer;
            _playerModel = playerModel;
            _panelViews = panelViews;
            _inputHorizontal.AxisOnChange += HorizontalAxisOnChange;
            _inputVertical.AxisOnChange += VerticalAxisOnChange;
            _sceneController = sceneController;
            _sceneController.IsStopControl += OnChangeIsStopControl;
            _sceneController.TakeOffOfTheShip += OnChangeTakeOffOfTheShip;

            _timeToDisableEnergyBlock = _playerModel.Settings.TimeForShipToTakeOff / 1.3f;
            _cameraWidth = _camera.orthographicSize * 2 * _camera.aspect;
        }

        private void OnChangeTakeOffOfTheShip(bool value)
        {
            MoveUpForDuration(_playerModel.Settings.TimeForShipToTakeOff);
        }

        private void OnChangeIsStopControl(bool value)
        {
            _isStopControl = value;
        }

        private void VerticalAxisOnChange(float value)
        {
            _vertical = value;
        }

        private void HorizontalAxisOnChange(float value)
        {
            _horizontal = value;
        }

        public void Cleanup()
        {
            _inputHorizontal.AxisOnChange -= HorizontalAxisOnChange;
            _inputVertical.AxisOnChange -= VerticalAxisOnChange;
            _sceneController.IsStopControl -= OnChangeIsStopControl;
            _sceneController.TakeOffOfTheShip -= OnChangeTakeOffOfTheShip;
        }

        public void Execute(float deltaTime)
        {
            if (_isStopControl && !_isMovingUp)
                return;

            if (_isMovingUp)
            {
                float elapsedTime = Time.time - _moveStartTime;

                if (elapsedTime > _timeToDisableEnergyBlock && !_isBlockReset)
                {
                    _isBlockReset = true;
                    OnChangeBlockReset?.Invoke();
                }

                if (!_isDisableEnergyBlock && elapsedTime > _playerModel.Settings.TimeForShipToTakeOff)
                {
                    _isDisableEnergyBlock = true;
                    _sceneController.DisableEnergyBlock?.Invoke();
                }

                if (elapsedTime < _moveDuration)
                {
                    float t = elapsedTime / _moveDuration;
                    float distanceToMove = _playerModel.Settings.SpeedAtTakeShip * deltaTime;

                    Vector3 targetPosition = _playerTransform.position + new Vector3(0f, distanceToMove, 0f);
                    _playerTransform.position = Vector3.Lerp(_playerTransform.position, targetPosition, t);
                }
                else
                {
                    _isMovingUp = false;
                    _isMovingFreeControl = true;
                    TheShipTookOff?.Invoke(true);
                }
            }
        }

        private void MoveUpForDuration(float duration)
        {
            if (_isMovingUp) return;

            _isMovingFreeControl = false;
            _isMovingUp = true;
            _moveDuration = duration;
            _moveStartTime = Time.time;
        }

        public void FixedExecute(float fixedDeltatime)
        {
            float deltaY = _playerTransform.position.y - _currentStatePosition;
            _currentStatePosition = _playerTransform.position.y;

            //TODO - переделать рассчет высоты с учетом скорости корабля...
            float realHeight = 0;
            if (deltaY > 0)
            {
                realHeight = deltaY * _playerModel.PlayerStruct.RealSpeedShipModel * fixedDeltatime;
            }
            else
            {
                if (!_isStopControl)
                {
                    realHeight = _playerModel.PlayerStruct.RealSpeedShipModel / _playerModel.PlayerStruct.ScaleFactor * fixedDeltatime;
                }
            }

            OnChangePositionAxisY?.Invoke(realHeight);

            var movement = new Vector3(_horizontal, _vertical, 0.0f).normalized;

            if (!_isStopControl && _isMovingFreeControl)
            {
                movement *= _speed.CurrentSpeed * fixedDeltatime;
                var newPosition = _playerTransform.position + movement;
                _rigidbodyPlayer.MovePosition(newPosition);
            }

            if (_isMovingFreeControl || _isMovingUp)
            {
                float targetSpeed = _playerModel.PlayerStruct.RealSpeedShipModel * fixedDeltatime
                    + _playerModel.PlayerStruct.SpeedScale * _playerModel.PlayerStruct.Player.Speed.CurrentSpeed;
                float smoothSpeed = Mathf.Lerp(_previousSpeed, targetSpeed, _playerModel.PlayerStruct.VelocitySmooth);

                _previousSpeed = smoothSpeed;

                OnChangeSpeedMovement?.Invoke(_previousSpeed);
            }
        }

        public void Initialization()
        {
            foreach (var panel in _panelViews)
            {
                if (panel is PanelHUDView panelHUD)
                    _panelHUD = panelHUD;
            }
        }
    }
}