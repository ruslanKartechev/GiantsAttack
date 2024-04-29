using System.Collections;
using GameCore.Cam;
using GameCore.Core;
using UnityEngine;

namespace GiantsAttack
{
    public class ChaseLevelStartAction : MonoBehaviour, IStartLevelAction
    {
        [SerializeField] private GameObject _runaway;
        [SerializeField] private Transform _cameraPoint;
        [SerializeField] private Animator _cameraAnimator;
        [SerializeField] private float _camToPeopleTime = 1f;
        [SerializeField] private float _camToHalicopterDelay = 3f;
        [SerializeField] private float _camToHelicopterTime = 2f;
        [SerializeField] private float _endDelay = 1f;

        public IHelicopter Player { get; set; }
        public IMonster Enemy { get; set; }
        private bool CamToPeopleSet { get; set; }

        private void Start()
        {
            _runaway.GetComponent<IRunaway>().Init();
        }

        public void Execute(System.Action stagesBeginCallback)
        {
            StartCoroutine(Working(stagesBeginCallback));
        }

        private IEnumerator Working(System.Action stagesBeginCallback)
        {
            GCon.UIFactory.GetStartMenu().Hide(() => {});
            CameraContainer.PlayerCamera.MoveToPointToParent(_cameraPoint, _camToPeopleTime, () =>
            {
                CamToPeopleSet = true;
            } );
            _cameraAnimator.enabled = true;
            Player.Mover.StopLoiter(true);
            yield return new WaitForSeconds(_camToHalicopterDelay);
            GCon.UIFactory.GetGameplayMenu().Show(() => {});
            CameraContainer.PlayerCamera.Parent(null);
            CameraContainer.PlayerCamera.MoveToPointToParent(Player.CameraPoints.InsidePoint, 
                _camToHelicopterTime, () => { });
            yield return new WaitForSeconds(_camToHelicopterTime);
            yield return new WaitForSeconds(_endDelay);
            stagesBeginCallback.Invoke();
        }
    }
}