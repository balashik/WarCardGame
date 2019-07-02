using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamingCardController : MonoBehaviour
{
    [SerializeField]
    private Image m_CardImage;

    public Image CardImage
    {
        get
        {
            return m_CardImage;
        }
        set
        {
            m_CardImage = value;
        }
    }
}
