using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameCardController : MonoBehaviour
{
    [SerializeField]
    private Image m_CardImage;

    [SerializeField]
    private Animator m_AnimController;

    private UnityAction m_OnDrawComplete;

    #region Public Methods
    /// <summary>
    /// Initiating The Card
    /// </summary>
    /// <param name="sprite">Card's UI Sprite</param>
    public void Init(Sprite sprite) {
        m_CardImage.sprite = sprite;
        gameObject.transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Drawing a card from the deck
    /// </summary>
    /// <param name="drawType"></param>
    /// <param name="OnDrawCompleted"></param>
    public void DrawCard(GameManager.EDrawType drawType, UnityAction OnDrawCompleted = null) {
        m_OnDrawComplete = OnDrawCompleted;
        m_AnimController.SetTrigger(drawType.ToString());
    }

    public void SendCardToWinner(Player.EPlayerType from,Player.EPlayerType to) {
        if (from == to)
        {
            m_AnimController.SetTrigger("Player_To_Player");
        }
        else if (from == Player.EPlayerType.player)
        {
            m_AnimController.SetTrigger("Player_To_PC");
        }
        else {
            m_AnimController.SetTrigger("PC_To_Player");
        }
    }
    /// <summary>
    /// On Draw animation is completed - triggerd by the animation
    /// </summary>
    public void OnDrawCompleted() {
        m_OnDrawComplete?.Invoke();
    }

    //Reset the Card
    public void Reset()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
