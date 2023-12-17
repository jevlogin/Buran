using UnityEngine;

namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class InputInitialization : ICleanup
    {
        private readonly Camera _camera;
        private readonly PlayerInputActions _playerInputActions;
        #region Fields

        private IUserInputProxy _inputHorizontal;
        private IUserInputProxy _inputVertical;
        private IUserInputBool _inputMouse;
        //private IUserInputTouch _inputTouch;
        private MobileInputTouch _inputTouch;

        #endregion


        #region ClassLifeCycles

        public InputInitialization(Camera camera)
        {
            _camera = camera;
            _playerInputActions = new PlayerInputActions();

            _inputHorizontal = new PCInputHorizontal(_playerInputActions.Player.MoveLeftAndRight);
            _inputVertical = new PCInputVertical(_playerInputActions.Player.MoveLeftAndRight);
            //_inputMouse = new PCInputMouse();
            _inputTouch = new MobileInputTouch(_playerInputActions.Player.PrimaryContact, 
                            _playerInputActions.Player.PrimaryPosition, _camera);
        }

        public void Cleanup()
        {
            _inputTouch.Cleanup();
        }

        #endregion


        #region Methods

        public (IUserInputProxy inputHorizontal, IUserInputProxy inputVertical) GetInput()
        {
            (IUserInputProxy inputHorizontal, IUserInputProxy inputVertical) result = (_inputHorizontal, _inputVertical);
            return result;
        }

        public IUserInputBool GetInputMouse()
        {
            return _inputMouse;
        }

        //public IUserInputTouch GetTouchMobile()
        //{
        //    return _inputTouch;
        //}

         public MobileInputTouch GetTouchMobile()
        {
            return _inputTouch;
        }

        #endregion
    }
}