using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum EPlayerGameState {
        playing,
        lost
    }

    public enum EPlayerType {
        pc,
        player
    }

    public EPlayerGameState GameState { get; private set; }

    private Queue<Card> m_Deck;
    public Queue<Card> Deck
    {
        get
        {
            return m_Deck;
        }
        set
        {
            m_Deck = value;
        }
    }
    public UnityEvent OnDeckEmpty { get; set; } = new UnityEvent();

    [SerializeField]
    private Transform m_OpenCardsArea;

    [SerializeField]
    private Text m_CardsLeftAmount;

    [SerializeField]
    private CardPoolingController m_PoolingController;

    [SerializeField]
    private EPlayerType m_PlayerType;

    public EPlayerType PlayerType
    {
        get
        {
            return m_PlayerType;
        }
    }

    private List<GameCardController> m_ActiveGameCards;
    public Transform OpenCardsArea
    {
        get
        {
            return m_OpenCardsArea;
        }
    }

    #region Public Methods

    /// <summary>
    /// sets the player on its initial state
    /// </summary>
    public void Init() {
        Deck = new Queue<Card>();
        m_ActiveGameCards = new List<GameCardController>();
        GameState = EPlayerGameState.playing;
    }

    /// <summary>
    /// drawing a card from players deck
    /// </summary>
    /// <returns>top card of the player</returns>
    public Card DrawCard(GameManager.EDrawType drawType, UnityAction OnDrawCompleted = null)
    {
        if (Deck.Count == 0)
        {
            GameState = EPlayerGameState.lost;
            OnDeckEmpty.Invoke();
        }
        Card c = Deck.Dequeue();
        GameCardController  gameCardController = GenerateCard(c);
        gameCardController.DrawCard(drawType, OnDrawCompleted);
        m_ActiveGameCards.Add(gameCardController);
        return c;
    }

    public void SendCardsToWinner(EPlayerType winnerType) {
        foreach (GameCardController gameCard in m_ActiveGameCards) {
            gameCard.SendCardToWinner(m_PlayerType, winnerType);
        }
        m_CardsLeftAmount.text = m_Deck.Count.ToString();
        m_ActiveGameCards.Clear();
    }
    #endregion

    #region Private Methods
    private GameCardController GenerateCard(Card c)
    {

        GameCardController gameCardController = m_PoolingController.CreateCard(c);
        gameCardController.Init(c.CardImage);
        return gameCardController;
    }

    #endregion
}
