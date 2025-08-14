using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySceneUIManager : MonoBehaviour
{
    [SerializeField] private Image m_healthBar;
    [SerializeField] private Image m_avatar;

    private Tweener m_stickHPTweener;
    private void Awake()
    {
        GameController.updatePlayerHpEvent += OnUpdateStickHP;
    }

    private void OnDestroy()
    {
        GameController.updatePlayerHpEvent -= OnUpdateStickHP;
    }
    private void OnUpdateStickHP(int last, int current)
    {
        m_stickHPTweener?.Kill();
        float healthFraction = (float)current / (float)SaveModel.maxPlayerHP;
        m_healthBar.DOFillAmount(healthFraction, 0.5f);
    }
}
