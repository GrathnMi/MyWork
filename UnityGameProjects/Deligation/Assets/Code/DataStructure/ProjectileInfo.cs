using System.Collections;
using System.Collections.Generic;

namespace ShipData
{
    public class ProjectileInfo
    {
        public enum projTypes { missile, laser, plasma }
        private projTypes _type;
        public projTypes GetProjType { get { return _type; } }


        private float _damage;
        public float Damage { get { return _damage; } }

        public ProjectileInfo(projTypes type, float damage = 10.0f)
        {
            _type = type;
            _damage = damage;
        }



    }
}
