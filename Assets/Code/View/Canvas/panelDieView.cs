using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class panelDieView : PanelView
    {
        [SerializeField] private TextMeshProUGUI _userName;
        [SerializeField] private TextMeshProUGUI _travelDistance;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _maxDiscount;
        [SerializeField] private TextMeshProUGUI _positionResults;
        [SerializeField] private TextMeshProUGUI _textSupport;
        [SerializeField] private Button _buttonSignIn;
        [SerializeField] private Button _buttonRegister;
        [SerializeField] private Button _buttonContinue;
        [SerializeField] private GameObject _panelDieAuthNo;
        [SerializeField] private GameObject _panelDieAuthYes;


        public GameObject panelDieAuthNo => _panelDieAuthNo;
        public GameObject panelDieAuthYes => _panelDieAuthYes;
        public Button ButtonContinue => _buttonContinue;
        public Button ButtonSignIn => _buttonSignIn;
        public Button ButtonRegister => _buttonRegister;
        public TextMeshProUGUI Score => _scoreText;
        public TextMeshProUGUI Money => _moneyText;
        public TextMeshProUGUI UserName => _userName;
        public TextMeshProUGUI TravelDistance => _travelDistance;
        public TextMeshProUGUI MaxDiscount => _maxDiscount;
        public TextMeshProUGUI PositionResults => _positionResults;
        public TextMeshProUGUI TextSupport  => _textSupport;
    }
}