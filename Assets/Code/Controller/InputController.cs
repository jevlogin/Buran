using System.Collections.Generic;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class InputController : IExecute, ICleanup
    {
        #region Fields

        private readonly InputInitialization _inputInitialization;
        private readonly IUserInputProxy _horizontal;
        private readonly IUserInputProxy _vertical;
        //private readonly IUserInputBool _inputMouse;
        private readonly IUserInputTouch _inputTouch;
        private readonly List<ICleanup> _listCleanup;

        #endregion


        #region ClassLifeCycles

        public InputController(InputInitialization inputInitialization)
        {
            _inputInitialization = inputInitialization;
            _horizontal = inputInitialization.GetInput().inputHorizontal;
            _vertical = inputInitialization.GetInput().inputVertical;
            //_inputMouse = inputInitialization.GetInputMouse();
            _inputTouch = inputInitialization.GetTouchMobile();

            _listCleanup = new List<ICleanup>();
            AddedToCleanupList();
        }

        public void Cleanup()
        {
            foreach (var clean in _listCleanup)
            {
                clean.Cleanup();
            }
            _listCleanup.Clear();
        }

        #endregion


        #region IExecute

        public void Execute(float deltatime)
        {
            _horizontal.GetAxis();
            _vertical.GetAxis();
            //_inputMouse.GetButtonDown();
        }

        #endregion


        #region Methods

        private void AddedToCleanupList()
        {
            if (_inputTouch is ICleanup cleanupTouch)
            {
                _listCleanup.Add(cleanupTouch);
            };
            if (_horizontal is ICleanup cleanupHorizontal)
            {
                _listCleanup.Add(cleanupHorizontal);
            }
            if (_vertical is ICleanup cleanupVertical)
            {
                _listCleanup.Add(cleanupVertical);
            }
            if (_inputInitialization is ICleanup cleanupInputInitialization)
            {
                _listCleanup.Add(cleanupInputInitialization);
            }
        }

        #endregion
    }
}