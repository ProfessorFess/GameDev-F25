using System;
using System.Collections.Generic;
using UnityEngine;

public class Solitaire : MonoBehaviour
{
    public GameObject cardPrefab;
    public String[] suits = { "C", "D", "H", "S" };
    public String[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
    public Sprite[] cardFaces;
    public GameObject[] foundationPositions;
    public GameObject[] tableauPositions;
    public List<string> deck;
    public List<string>[] foundations;
    public List<string>[] tableaus;
    public List<string> foundation0 = new List<string>();
    public List<string> foundation1 = new List<string>();
    public List<string> foundation2 = new List<string>();
    public List<string> foundation3 = new List<string>();
    public List<string> tableau0 = new List<string>();
    public List<string> tableau1 = new List<string>();
    public List<string> tableau2 = new List<string>();
    public List<string> tableau3 = new List<string>();
    public List<string> tableau4 = new List<string>();
    public List<string> tableau5 = new List<string>();
    public List<string> tableau6 = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tableaus = new List<string>[] { tableau0, tableau1, tableau2, tableau3, tableau4, tableau5, tableau6 };
        foundations = new List<string>[] { foundation0, foundation1, foundation2, foundation3 };
        PlayGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlayGame()
    {
        deck = GenerateDeck();
        foreach (string card in deck)
        {
            Debug.Log(card);
        }
        //shuffle
        Deal();
    }

    List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                newDeck.Add(suit + rank);
            }
        }
        return newDeck;
    }

    void Deal()
    {

    }
}
