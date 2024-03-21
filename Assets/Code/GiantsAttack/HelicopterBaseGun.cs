using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterBaseGun : MonoBehaviour, IHelicopterGun
    {
        [SerializeField] private Transform _rotatable;
        [SerializeField] private List<HelicopterGunBarrel> _barrels;

        public List<HelicopterGunBarrel> Barrels => _barrels;
        
        public Transform Rotatable => _rotatable;
    }
}