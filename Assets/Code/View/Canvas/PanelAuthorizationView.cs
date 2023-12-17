using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class PanelAuthorizationView : PanelView
    {
        [SerializeField] private GameObject _panelLogin;
        [SerializeField] private GameObject _panelRegister;

        [SerializeField] private Button _buttonLoginAuth;
        [SerializeField] private Button _buttonRegisterAuth;

        [SerializeField] private Button _buttonLoginAuthNo;
        [SerializeField] private Button _buttonRegisterAuthNo;

        [Header("Login")]
        public TMP_InputField EmailInputField;
        public TMP_InputField PasswordLoginInputField;
        public TextMeshProUGUI WarningLogintext;
        public TextMeshProUGUI ConfirmLogintext;
        public Button ButtonLoginAuth => _buttonLoginAuth;
        public Button ButtonRegisterAuth => _buttonRegisterAuth;

        [Header("Register")]
        public TMP_InputField UsernameInputField;
        public TMP_InputField PhoneInputField;
        public TMP_InputField EmailRegisterInputField;
        public TMP_InputField PasswordRegisterInputField;
        public TMP_InputField PasswordRegisterVerifyInputField;
        public TextMeshProUGUI WarningRegistertext;
        public Button ButtonLoginAuthNo => _buttonLoginAuthNo;
        public Button ButtonRegisterAuthNo => _buttonRegisterAuthNo;
    }
}