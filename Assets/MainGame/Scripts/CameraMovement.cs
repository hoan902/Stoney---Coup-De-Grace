using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private CinemachineCamera m_cinemaCam;
    [SerializeField] private CinemachineFollowZoom m_cineFollowZoomCam;
    [SerializeField] private CinemachineImpulseSource m_cinemaShakeCam;

    private CinemachineBrain m_camBrain;
    private Camera m_unityCamera;

    private bool m_stopCamera;
    private Transform m_player;
    private Transform m_cameraTransform;
    private float m_currentFixedUpdateTime;

    private void Awake()
    {
        m_stopCamera = false;
        m_camBrain = GetComponent<CinemachineBrain>();
        m_camBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Linear;
        m_currentFixedUpdateTime = Time.fixedDeltaTime;

        GameController.cameraFollowTarget += SetupFollowTarget;
        GameController.cameraZoomEff += CameraZoomDramaticEff;
        GameController.shakeCameraEvent += OnCameraShake;
    }

    private void OnDestroy()
    {
        GameController.cameraFollowTarget -= SetupFollowTarget;
        GameController.cameraZoomEff -= CameraZoomDramaticEff;
        GameController.shakeCameraEvent -= OnCameraShake;
    }

    private IEnumerator Start()
    {
        m_unityCamera = GetComponent<Camera>();
        m_cameraTransform = m_unityCamera.transform;
        yield return null;
    }
    private void SetupFollowTarget(Transform player)
    {
        m_player = player;
        m_cinemaCam.Target.TrackingTarget = m_player;
    }

    private void CameraZoomDramaticEff()
    {
        DOTween.To(() => m_cineFollowZoomCam.FovRange.y,x => m_cineFollowZoomCam.FovRange.y = x, 40f, 0.5f)
            .OnComplete(() =>
            {
                DOTween.To(() => m_cineFollowZoomCam.FovRange.y, x => m_cineFollowZoomCam.FovRange.y = x, 60f, 0.2f);
            });
    }
    private void OnCameraShake()
    {
        m_cinemaShakeCam.ImpulseDefinition.ImpulseDuration = 0.2f;
        m_cinemaShakeCam.ImpulseDefinition.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Explosion;
        m_cinemaShakeCam.GenerateImpulse();
    }
}
