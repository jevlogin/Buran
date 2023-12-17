using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [CreateAssetMenu(fileName = "AmmunitionData", menuName = "AmmunitionData/AmmunitionData", order = 51)]
    internal sealed class AmmunitionData : ScriptableObject
    {
        [SerializeField, Header("�������� ���������")] private AmmunitionStruct _ammunitionStruct;
        [Header("���������� ���������")] private AmmunitionComponents _components;
        [SerializeField, Header("�������������� ��������� ���������")] private AmmunitionSettings _settings;

        public AmmunitionStruct AmmunitionStruct => _ammunitionStruct;
        public AmmunitionComponents Components => _components;
        public AmmunitionSettings AmmunitionSettings => _settings;
    }
}