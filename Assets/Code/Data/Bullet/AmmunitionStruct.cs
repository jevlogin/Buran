using System;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [Serializable]
    internal struct AmmunitionStruct
    {
        [Header("Настройки для игрока")]
        [SerializeField] private int _poolSize;
        [SerializeField] private float _refireTimer;
        [SerializeField] private AmmunitionType _typeAmmunitionPlayer;

        internal Bullet Bullet;
        internal Pool<Bullet> PoolBullet;
        internal BulletPool PoolBulletGeneric;


        [Header("Настройки для противников")]
        [SerializeField] private AmmunitionType _typeAmmunitionEnemy;
        [SerializeField] private float _speedBulet;

        internal EnemyBullet EnemyBullet;
        internal Pool<Bullet> PoolEnemyBullet;
        internal BulletPool PoolEnemyBulletGeneric;

        internal AmmunitionType AmmunitionType => _typeAmmunitionPlayer;
        internal float RefireTimer { readonly get => _refireTimer; private set => _refireTimer = value; }
        internal readonly int PoolSize => _poolSize;

        public readonly float SpeedBulet => _speedBulet;
    }
}