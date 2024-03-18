using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatObstacle : MonoBehaviour, IBoatObstacle
    {
        [SerializeField] private ParticleSystem _particle;

        public void Hit()
        {
            if (_particle != null)
            {
                _particle.transform.parent = transform.parent;
                _particle.gameObject.SetActive(true);
                _particle.Play();
            }
            gameObject.SetActive(false);
        }
    }
}