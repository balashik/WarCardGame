using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField]
    private Deck m_Deck;

    private List<Card> m_ShuffledDeck;

    private void GenerateDeck() {
        m_ShuffledDeck = new List<Card>();
        foreach (Card c in m_Deck.Cards) {
            //Instantiate(c.CardObject, transform);
            m_ShuffledDeck.Add(c);
        }    
    }

    // Generic Shuffling to the deck
    private void ShuffleDeck() {
        int n = m_ShuffledDeck.Count;
        System.Random rnd = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            Card value = m_ShuffledDeck[k];
            m_ShuffledDeck[k] = m_ShuffledDeck[n];
            m_ShuffledDeck[n] = value;
        }
    }


    public void DistribudeCards(Player[] players) {
        while (m_ShuffledDeck.Count > 0) {
            foreach (Player p in players) {
                p.Deck.Enqueue(PullCard());
            }
        }
    }

    private Card PullCard() {
        Card tempCard = m_ShuffledDeck[0];
        m_ShuffledDeck.RemoveAt(0);
        return tempCard;
    }

    public void Init() {
        GenerateDeck();
        ShuffleDeck();
    }
}
