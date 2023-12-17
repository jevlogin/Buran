using System;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [Serializable]
    internal class Bullet : AmmunitionView, IDamageable
    {
        [SerializeField, Range(0, 10)] private float _maxLifetimeOutsideThePool;
        [SerializeField] internal float Speed;
        private float _damage = 10.0f;
        private bool _isDead;
        private float lifeTime = 0.0f;

        public Rigidbody2D Rigidbody2D { get; internal set; }

        public float LifeTime
        {
            get => lifeTime;
            set
            {
                lifeTime = value;
                IsDead = false;
            }
        }
        public float MaxLifeTimeOutsideThePool
        {
            get
            {
                return _maxLifetimeOutsideThePool;
            }
        }

        public float Damage { get => _damage; set => _damage = value; }
        public bool IsDead { get => _isDead; set => _isDead = value; }

        //private void Awake()
        //{
        //    Rigidbody2D = gameObject.GetOrAddComponent<Rigidbody2D>();
        //    OnCollisionEnterDetect += Bullet_OnCollisionEnterDetect;
        //    IsDead = false;
        //}

        //private void OnDestroy()
        //{
        //    OnCollisionEnterDetect -= Bullet_OnCollisionEnterDetect;
        //}

        //private void Bullet_OnCollisionEnterDetect(Collider2D collider)
        //{
        //    if (!IsDead)
        //    {
        //        if (collider.TryGetComponent<IDamageable>(out var damageable))
        //        {
        //            TakeDamage(damageable.Damage);
        //        }
        //    }
        //}

        public virtual void TakeDamage(float damage)
        {
            LifeTime += MaxLifeTimeOutsideThePool;
            IsDead = true;
        }
    }
}