using UnityEngine;

public class BillboardEff : MonoBehaviour
{
    private Transform m_targetLookAt;
    private void LateUpdate()
    {
        m_targetLookAt = Camera.main.transform;
        transform.LookAt(transform.position + m_targetLookAt.forward);
    }

    public void GetLookAtTarget(Transform target)
    {
        m_targetLookAt = target;
    }
}
