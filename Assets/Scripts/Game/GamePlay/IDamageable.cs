using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.GamePlay
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        void Die();
        int GetHealth();
        void AddHealth(int amount);
    }
}
