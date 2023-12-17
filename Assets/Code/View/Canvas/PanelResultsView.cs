using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class PanelResultsView : PanelView
    {
        [SerializeField] private WebPlayer _webPlayer;
        [SerializeField] private TextMeshProUGUI _textDistanceTraveled;
        [SerializeField] private TextMeshProUGUI _textSpeed;
        [SerializeField] private TextMeshProUGUI _textElapsedTime;
        [SerializeField] private TextMeshProUGUI _textPeopleOfReached;
        [SerializeField] private Slider _sliderTravelElapsed;


        internal Slider _sliderTravel => _sliderTravelElapsed;
        internal WebPlayer WebPlayer => _webPlayer;
        internal TextMeshProUGUI TextDistanceTraveled => _textDistanceTraveled;
        internal TextMeshProUGUI TextSpeed => _textSpeed;
        internal TextMeshProUGUI TextElapsedTime => _textElapsedTime;
        internal TextMeshProUGUI TextPeopleOfReached => _textPeopleOfReached;
    }
}