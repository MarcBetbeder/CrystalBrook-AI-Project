using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Card : MonoBehaviour, IComparable<Card>
{
    [SerializeField] private Suit suit;
    [SerializeField] private Value value;
    [SerializeField] private string id;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color pressColor;
    [SerializeField] private Color highlightColor;

    private SpriteRenderer sr;
    private BoxCollider2D bc;

    private bool moving = false;

    private void Start()
    {
        Initialise();
    }

    public void Initialise()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    public Suit GetSuit()
    {
        return suit;
    }

    public Value GetValue()
    {
        return value;
    }

    public string GetID()
    {
        return id;
    }

    public void SetMoving(bool tf)
    {
        moving = tf;
    }

    public bool IsMoving()
    {
        return moving;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = defaultColor;
    }

    private void OnMouseDown()
    {
        sr.color = pressColor;
    }

    private void OnMouseUpAsButton()
    {
        PlayCard();
    }

    public void PlayCard()
    {
        FindObjectOfType<GameManager>().PlayMainPlayerCard(this);
        SetColliderActive(false);
    }

    public void SetColliderActive(bool tf)
    {
        if (tf)
        {
            sr.color = defaultColor;
        }
        bc.enabled = tf;
    }

    public void HighlightCard(bool tf)
    {
        if (tf)
        {
            sr.color = highlightColor;
        } else
        {
            sr.color = defaultColor;
        }
    }

    public override string ToString()
    {
        return id;
    }

    public int CompareTo(Card other)
    {
        if (GetSuit() > other.GetSuit())
        {
            return 1;
        } else if (GetSuit() < other.GetSuit())
        {
            return -1;
        } else
        {
            if (GetValue() > other.GetValue())
            {
                return 1;
            } else if (GetValue() < other.GetValue())
            {
                return -1;
            } else
            {
                return 0;
            }
        }
    }
}
