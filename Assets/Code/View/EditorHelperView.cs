using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class EditorHelperView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _canvasGroupHelper;

        private void Awake()
        {
#if UNITY_EDITOR
            _canvasGroupHelper.gameObject.SetActive(true);
#else
            _canvasGroupHelper.gameObject.SetActive(false);
#endif
        }
    }
}