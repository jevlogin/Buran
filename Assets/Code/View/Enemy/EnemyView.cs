using System;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [Serializable]
    internal class EnemyView : MonoBehaviour, ICollisionDetect
    {
        public event Action<Collider2D> OnCollisionEnterDetect;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(!collision.gameObject.TryGetComponent<EnemyBullet>(out var enemyBullet))
            {
                OnCollisionEnterDetect?.Invoke(collision.collider);
            }
        }
    }
}