using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class EnemyBullet : Bullet
    {
        private void Awake()
        {
            Rigidbody2D = gameObject.GetOrAddComponent<Rigidbody2D>();
            OnCollisionEnterDetect += Bullet_OnCollisionEnterDetect;
            IsDead = false;
        }

        private void OnDestroy()
        {
            OnCollisionEnterDetect -= Bullet_OnCollisionEnterDetect;
        }
        private void Bullet_OnCollisionEnterDetect(Collider2D collider)
        {
            if (collider.TryGetComponent<EnemyView>(out _) || collider.TryGetComponent<EnemyBullet>(out _))
                return;

            if (!IsDead)
            {
                if (collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    TakeDamage(damageable.Damage);
                }
            }
        }

        public override void TakeDamage(float damage)
        {
            LifeTime += MaxLifeTimeOutsideThePool;
            IsDead = true;
        }
    }
}