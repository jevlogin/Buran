using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData/PlayerData", order = 51)]
    internal sealed class PlayerData : ScriptableObject
    {
        #region Fields

        [Header("�������� ������"), Space(20), SerializeField] private PlayerStruct _playerStruct;
        [Header("���������� ������")] private PlayerComponents _playerComponents;
        [Header("�������������� ��������� ��� ������"), SerializeField] private PlayerSettings _playerSettings;

        #endregion


        #region Properties

        internal PlayerStruct PlayerStruct => _playerStruct;
        internal PlayerComponents PlayerComponents => _playerComponents;
        internal PlayerSettings PlayerSettings => _playerSettings;

        #endregion
    }
}