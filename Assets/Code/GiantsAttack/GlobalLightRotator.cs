using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    [DefaultExecutionOrder(1001)]
    public class GlobalLightRotator : MonoBehaviour
    {
        [SerializeField] private bool _doWork;
        [SerializeField] private Vector3 _lightEulers;
        [SerializeField] private float _intensity = 1f;
        
        private void Start()
        {
            if (!_doWork)
                return;
            EnvironmentState.CurrentGlobalLight.transform.eulerAngles = _lightEulers;
            EnvironmentState.CurrentGlobalLight.intensity = _intensity;
        }
    }
}