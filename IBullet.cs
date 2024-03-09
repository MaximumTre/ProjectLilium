using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public interface IBullet
    {
        public void InitBullet(Damage damage, List<string> damageTags, float force);
    }
}
