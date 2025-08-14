using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private CinemachineCamera m_cinemaCam;

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
    }

    private void OnDestroy()
    {
        GameController.cameraFollowTarget -= SetupFollowTarget;
        GameController.cameraZoomEff -= CameraZoomDramaticEff;
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
        m_unityCamera.DOFieldOfView(40, 0.4f).OnComplete(() =>
        {
            m_unityCamera.DOFieldOfView(60, 0.2f);
        });
        m_unityCamera.DOShakeRotation(0.4f, 1f).OnComplete((() =>
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = m_currentFixedUpdateTime * Time.timeScale;
        }));
    }
}
