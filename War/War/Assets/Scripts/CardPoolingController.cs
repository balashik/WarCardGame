using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardPoolingController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_PoolableObject;

    [SerializeField]
    private int m_PoolSize;

    [SerializeField]
    private int m_MinPoolObjectAmount;

    private Queue<GameObject> m_Pool;

    private Queue<GameObject> m_UsedObjects;

    private void Start()
    {
        m_Pool = new Queue<GameObject>();
        m_UsedObjects = new Queue<GameObject>();
        for (int i = 0; i < m_PoolSize; i++) {
            m_Pool.Enqueue(Instantiate(m_PoolableObject,transform));
        }
    }

    public GameCardController CreateCard(Card c)
    {
        GameObject GO = m_Pool.Dequeue();
        GameCardController gameCardController = GO.GetComponent<GameCardController>();
        m_UsedObjects.Enqueue(GO);
        if (m_Pool.Count == m_MinPoolObjectAmount) {
            Reuse();
        }
        return gameCardController;
    }

    private void Reuse() {
        GameObject GO = m_UsedObjects.Dequeue();
        GameCardController gameCardController = GO.GetComponent<GameCardController>();
        gameCardController.Reset();
        m_Pool.Enqueue(GO);
    }
}
