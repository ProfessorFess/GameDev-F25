using System.Collections.Generic;
using UnityEngine;

public class Solitaire : MonoBehaviour
{
    public Sprite[] cardSprites; // Assign in inspector
    public GameObject cardPrefab; // Assign in inspector
    public string[] suits = { "H", "D", "C", "S" };
    public string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public GameObject[] foundationPositions;
    public GameObject[] tableauPositions;
    List<string> deck;
    List<string>[] foundations;
    List<string>[] tableaus;
    List<string> foundation0 = new List<string>();
    List<string> foundation1 = new List<string>();
    List<string> foundation2 = new List<string>();
    List<string> foundation3 = new List<string>();
    List<string> tableau0 = new List<string>();
    List<string> tableau1 = new List<string>();
    List<string> tableau2 = new List<string>();
    List<string> tableau3 = new List<string>();
    List<string> tableau4 = new List<string>();
    List<string> tableau5 = new List<string>();
    List<string> tableau6 = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foundations = new List<string>[] { foundation0, foundation1, foundation2, foundation3 };
        tableaus = new List<string>[] { tableau0, tableau1, tableau2, tableau3, tableau4, tableau5, tableau6 };
        deck = GenerateDeck();
        foreach(string card in deck)
        {
            Debug.Log(card);
        }
        //shuffle
        Deal();
    }

    // Update is called once per frame
    void Update()
    {

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
