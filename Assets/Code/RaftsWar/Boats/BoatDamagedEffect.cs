using System.Collections;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatDamagedEffect : MonoBehaviour, IBoatDamagedEffect
    {
        [SerializeField] private BoatDamageSettings _damageSettings;
        private Coroutine _working;
        
        public Boat Boat { get; set;}

        public void Play()
        {
            Stop();
            _working =StartCoroutine(Flicking());
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        public void Restore(BoatPart part)
        {
            part.SetCurrentView();
        }

        private IEnumerator Flicking()
        {
            for (var i = 0; i < _damageSettings.count; i++)
            {
                On();
                yield return new WaitForSeconds(_damageSettings.delay);
                Off();
                yield return new WaitForSeconds(_damageSettings.delay);
            }

            void On()
            {
                Boat.RootPart.SetTempView(_damageSettings.DamagedView);
                foreach (var part in Boat.Parts)
                    part.SetTempView(_damageSettings.DamagedView);
            }
            void Off()
            {
                Boat.RootPart.SetCurrentView();
                foreach (var part in Boat.Parts)
                    part.SetCurrentView();
            }
        }
    }
}