using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance;

    public enum EDrawType
    {
        RegularDraw,
        WarDraw
    }
    public static GameManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject Go = new GameObject();
                m_Instance = Go.AddComponent<GameManager>();
            }
            return m_Instance;
        }
    }
    public DeckManager deckManager;

    [SerializeField]
    private Player[] m_Players;

    [SerializeField]
    private GameObject MainMenu;

    [SerializeField]
    private GameObject GameCanvas;

    [SerializeField]
    private Button DrawButton;

    [SerializeField]
    private Button WarButton;

    [SerializeField]
    private Text WinningText;

    private List<Card>[] m_PlayersWarCards;

    private int m_DrawCount;

    #region MonoBehaviour
    private void Start()
    {
        m_PlayersWarCards = new List<Card>[m_Players.Length];
        for(int i=0; i<m_Players.Length;i++) {
            m_PlayersWarCards[i] = new List<Card>();
        }
    }

    private void OnEnable()
    {
        foreach (Player p in m_Players)
        {
            p.OnDeckEmpty.AddListener(CheckWinner);
        }
    }

    private void OnDisable()
    {
        foreach (Player p in m_Players)
        {
            p.OnDeckEmpty.RemoveListener(CheckWinner);
        }
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// Starts the player Turn
    /// </summary>
    public void Turn() {
        DrawButton.gameObject.SetActive(false);
        Draw(EDrawType.RegularDraw,()=> {
            int winningHand = CheckWinningHand();
            if (winningHand == -1)
            {
                WarButton.gameObject.SetActive(true);

            }
            else
            {
                AwardWinner(winningHand);
                DrawButton.gameObject.SetActive(true);
            }
        });
        
    }

    /// <summary>
    /// Starts the Game
    /// </summary>
    public void StartGame() {
        MainMenu.gameObject.SetActive(false);
        foreach (Player p in m_Players) {
            p.Init();
        }
        deckManager.Init();
        deckManager.DistribudeCards(m_Players);
        GameCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// War events that starts the war series
    /// </summary>
    public void War() {
        Draw(EDrawType.WarDraw,()=> {
            Draw(EDrawType.RegularDraw, OnWarAnimationComplete);
        });
    }

    #endregion

    #region Private Methods

    private void OnWarAnimationComplete() {

        int winningHand = CheckWinningHand();
        if (winningHand != -1)
        {
            AwardWinner(winningHand);
            DrawButton.gameObject.SetActive(true);
            WarButton.gameObject.SetActive(false);
        }
    }

    private void Draw(EDrawType drawType, UnityAction OnDrawComplete = null)
    {
        m_DrawCount = 0;
        for (int i = 0; i < m_Players.Length; i++)
        {
            Card c = m_Players[i].DrawCard(drawType, ()=> {
                m_DrawCount++;
            });
            m_PlayersWarCards[i].Add(c);

        }
        StartCoroutine(WaitForAllDraws(OnDrawComplete));
    }

    private IEnumerator WaitForAllDraws(UnityAction OnDrawComplete) {
        while (m_DrawCount != m_Players.Length) {
            yield return null;
        }
        OnDrawComplete?.Invoke();

    }

    private void AwardWinner(int winnerIndex) {
        foreach (List<Card> playersHands in m_PlayersWarCards) {
            playersHands.ForEach(c => m_Players[winnerIndex].Deck.Enqueue(c));
        }
        foreach (Player p in m_Players) {
            p.SendCardsToWinner(m_Players[winnerIndex].PlayerType);
        }
        ResetCurrentJackpot();
    }

    /// <summary>
    /// check has the highest hand and returns
    /// </summary>
    /// <returns>index of winner if all equeal returns -1</returns>
    private int CheckWinningHand() {
        List<int> currentHands = new List<int>();
        foreach (List<Card> cardList in m_PlayersWarCards) {
            currentHands.Add(cardList.Last().Value);
        }
        int max = currentHands.Max();
        int min = currentHands.Min();

        if (max == min)
        {
            return -1;
        }
        else {
            return currentHands.IndexOf(max);
        }
    }

    private void ResetCurrentJackpot() {
        for (int i = 0; i < m_PlayersWarCards.Length; i++) {
            m_PlayersWarCards[i] = new List<Card>();
        }
    }

    private void CheckWinner() {
        int winnerCount=0;
        Player winner = null;
        foreach (Player p in m_Players) {
            if (p.GameState == Player.EPlayerGameState.playing) {
                winner = p;
                winnerCount++;
            }
        }
        if (winnerCount == 1)
        {
            WinningText.text = string.Format("{0} is the WINNER", winner.name);
            MainMenu.SetActive(false);
            GameCanvas.SetActive(true);
        }
    }

    #endregion

}