using System;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [Serializable]
    internal sealed class AmmunitionSettings
    {
        [SerializeField] private Bullet _bulletPlayerPrefab;
        [SerializeField] private EnemyBullet _bulletEnemyPrefab;
        [SerializeField] private string _nameBullet;

        public string NameBullet => _nameBullet;
        internal Bullet BulletPrefab => _bulletPlayerPrefab;
        internal EnemyBullet BulletEnemyPrefab => _bulletEnemyPrefab;
    }
}