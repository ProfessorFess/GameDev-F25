using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreeCellSolitaire : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite emptyPlace;
    public string[] suits = { "C", "D", "H", "S" };
    public string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public Sprite[] cardFaces;
    public Sprite cardBack; // not really used in FreeCell, but kept for compatibility

    public GameObject[] foundationPositions;  // size 4
    public GameObject[] tableauPositions;     // size 8
    public GameObject[] freeCellPositions;    // size 4
    public GameObject winPanel;             // UI panel to show when the player wins (optional)

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
    public List<string> tableau7 = new List<string>();

    // Each FreeCell holds at most one card name or null
    private string[] freeCells = new string[4];

    private System.Random rng = new System.Random();
    private Vector3 cardOffset = new Vector3(0f, -0.3f, -0.1f);
    private bool gameWon = false;

    void Start()
    {
        tableaus = new List<string>[]
        {
            tableau0, tableau1, tableau2, tableau3,
            tableau4, tableau5, tableau6, tableau7
        };
        foundations = new List<string>[] { foundation0, foundation1, foundation2, foundation3 };

        PlayGame();
    }

    void Update()
    {
    }

    void PlayGame()
    {
        deck = GenerateDeck();
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
        // shuffle
        newDeck = newDeck.OrderBy(x => rng.Next()).ToList();
        return newDeck;
    }

    // FreeCell deal:
    //  - 8 tableau columns
    //  - First 4 have 7 cards, last 4 have 6 cards
    //  - All cards face up, no remaining stock/waste
    void Deal()
    {
        Debug.Log("Dealing FreeCell cards...");

        if (tableauPositions.Length != 8)
        {
            Debug.LogError("FreeCellSolitaire: tableauPositions length must be 8.");
        }

        int deckIndex = 0;

        for (int t = 0; t < 8; t++)
        {
            int cardsToDeal = (t < 4) ? 7 : 6;

            for (int i = 0; i < cardsToDeal; i++)
            {
                string card = deck[deckIndex++];
                tableaus[t].Add(card);

                GameObject tabPosition = tableauPositions[t];
                Vector3 pos = tabPosition.transform.position + cardOffset * i;

                CreateCard(card, pos, tabPosition.transform, true); // all face up in FreeCell
            }
        }

        // no deck left in play for FreeCell (we won't draw later)
        if (deckIndex < deck.Count)
        {
            Debug.LogWarning("FreeCellSolitaire: Deck has leftover cards after dealing; check logic.");
        }

        deck.Clear();
    }

    void CreateCard(string cardName, Vector3 position, Transform parent, bool isFaceUp)
    {
        GameObject newCard = Instantiate(cardPrefab, position, Quaternion.identity, parent);
        newCard.name = cardName;

        Sprite cardFace = cardFaces.FirstOrDefault(s => s.name == cardName);
        var cardSprite = newCard.GetComponent<CardSprite>();
        cardSprite.cardFace = cardFace;
        cardSprite.isFaceUp = isFaceUp;
    }

    // ------------------ MOVE LOGIC ------------------ //

    public bool IsValidMove(GameObject cardObject, GameObject targetObject)
    {
        if (cardObject == null || targetObject == null || cardObject == targetObject)
            return false;

        // Determine which pile we clicked on
        ResolveTarget(targetObject,
            out GameObject clickedTag,
            out int foundationIndex,
            out int tabIndex,
            out int freeCellIndex);

        string sourceTag = cardObject.transform.parent.tag;
        string cardName = cardObject.name;

        // Debug info so we can see what's happening
Debug.Log(
    "IsValidMove: card=" + cardName +
    ", sourceTag=" + sourceTag +
    ", targetTag=" + clickedTag.tag +
    ", foundationIndex=" + foundationIndex +
    ", tabIndex=" + tabIndex +
    ", freeCellIndex=" + freeCellIndex +
    ", freeCellChildren=" + (clickedTag.CompareTag("FreeCell") ? clickedTag.transform.childCount : -1)
);

        // FreeCell rule: we only move single exposed cards, no stacks.
        if (sourceTag == "Tableau" && !IsLastInTab(cardObject))
        {
            Debug.Log("Invalid: trying to move non-last card from tableau (no multi-stack yet).");
            return false;
        }

        // 1) Moving to a FreeCell (simplified)
        if (clickedTag.CompareTag("FreeCell"))
        {
            // FreeCell must be empty (we don't rely on freeCells[] here, just the transform)
            bool isEmpty = (clickedTag.transform.childCount == 0);
            Debug.Log("FreeCell target, isEmpty = " + isEmpty);
            return isEmpty;
        }

        // 2) Moving to a Tableau
        if (clickedTag.CompareTag("Tableau") && tabIndex >= 0)
        {
            // We can move from Tableau, FreeCell, or Foundation to Tableau
            if (sourceTag == "Tableau" || sourceTag == "FreeCell" || sourceTag == "Foundation")
            {
                bool canPlace = CanPlaceOnTableau(cardName, tabIndex);
                Debug.Log("Can place on tableau[" + tabIndex + "]: " + canPlace);
                return canPlace;
            }
            return false;
        }

        // 3) Moving to a Foundation
        if (clickedTag.CompareTag("Foundation") && foundationIndex >= 0)
        {
            // From Tableau or FreeCell is allowed
            if (sourceTag == "Tableau" || sourceTag == "FreeCell")
            {
                bool canPlace = CanPlaceOnFoundation(cardName, foundationIndex);
                Debug.Log("Can place on foundation[" + foundationIndex + "]: " + canPlace);
                return canPlace;
            }
            // Optionally allow from Foundation? Usually foundations go one way, but some variants allow undo.
            return false;
        }

        Debug.Log("IsValidMove: no matching rule, returning false.");
        return false;
    }

    public void PlaceCard(GameObject cardObject, GameObject targetObject)
    {
        if (cardObject == null || targetObject == null || cardObject == targetObject)
            return;

        ResolveTarget(targetObject,
            out GameObject clickedTag,
            out int foundationIndex,
            out int tabIndex,
            out int freeCellIndex);

        string sourceTag = cardObject.transform.parent.tag;
        string cardName = cardObject.name;

        // Remove from source data structure
        if (sourceTag == "Tableau")
        {
            // remove from its tableau list
            foreach (List<string> tableau in tableaus)
            {
                if (tableau.Count > 0 && tableau.Last() == cardName)
                {
                    tableau.RemoveAt(tableau.Count - 1);
                    break;
                }
            }
        }
        else if (sourceTag == "Foundation")
        {
            foreach (List<string> foundation in foundations)
            {
                if (foundation.Count > 0 && foundation.Last() == cardName)
                {
                    foundation.RemoveAt(foundation.Count - 1);
                    break;
                }
            }
        }

        // Add to destination
        if (clickedTag.CompareTag("Tableau") && tabIndex >= 0)
        {
            tableaus[tabIndex].Add(cardName);

            // Position card: place below the top card in that tableau
            if (clickedTag == targetObject) // clicked empty tableau space
            {
                if (tableaus[tabIndex].Count == 1)
                {
                    cardObject.transform.position = targetObject.transform.position + new Vector3(0f, 0f, -0.03f);
                }
                else
                {
                    cardObject.transform.position = targetObject.transform.position + cardOffset * (tableaus[tabIndex].Count - 1);
                }
            }
            else // clicked on top card in that tableau
            {
                cardObject.transform.position = targetObject.transform.position + cardOffset;
            }

            cardObject.transform.parent = clickedTag.transform;
        }
        else if (clickedTag.CompareTag("Foundation") && foundationIndex >= 0)
        {
            foundations[foundationIndex].Add(cardName);
            cardObject.transform.position = clickedTag.transform.position + new Vector3(0f, 0f, -0.03f);
            cardObject.transform.parent = clickedTag.transform;

            // Ensure the newest card in the foundation renders on top
            SpriteRenderer sr = cardObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = foundations[foundationIndex].Count;
            }

            CheckForWin();
        }
        else if (clickedTag.CompareTag("FreeCell"))
        {
            // Just move the card into this FreeCell; we rely on transform.childCount in IsValidMove
            cardObject.transform.position = clickedTag.transform.position + new Vector3(0f, 0f, -0.03f);
            cardObject.transform.parent = clickedTag.transform;
        }
        else
        {
            Debug.LogWarning("PlaceCard: destination did not match Tableau/Foundation/FreeCell.");
        }
    }

    // ------------------ WIN DETECTION ------------------ //

    private void CheckForWin()
    {
        if (gameWon) return;

        int totalCardsInFoundations = 0;
        foreach (List<string> foundation in foundations)
        {
            totalCardsInFoundations += foundation.Count;
        }

        // Standard FreeCell win: all 52 cards are in the foundations
        if (totalCardsInFoundations == 52)
        {
            gameWon = true;
            Debug.Log("FreeCell: You win!");

            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
        }
    }

    public bool HasWon()
    {
        int totalCardsInFoundations = 0;
        foreach (List<string> foundation in foundations)
        {
            totalCardsInFoundations += foundation.Count;
        }
        return totalCardsInFoundations == 52;
    }


    // ------------------ RESTART / NEW GAME ------------------ //

    public void RestartGame()
    {
        // Destroy all card GameObjects in the scene
        GameObject[] allCards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in allCards)
        {
            Destroy(card);
        }

        // Clear logical state
        if (deck != null)
        {
            deck.Clear();
        }

        if (tableaus != null)
        {
            foreach (List<string> tab in tableaus)
            {
                tab.Clear();
            }
        }

        if (foundations != null)
        {
            foreach (List<string> foundation in foundations)
            {
                foundation.Clear();
            }
        }

        // Clear freecell tracking array (even though we don't rely on it anymore)
        if (freeCells != null)
        {
            for (int i = 0; i < freeCells.Length; i++)
            {
                freeCells[i] = null;
            }
        }

        // Reset win state and hide win panel
        gameWon = false;
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // Deal a fresh game
        PlayGame();
    }

    // ------------------ HELPERS ------------------ //

    public bool IsLastInTab(GameObject cardObject)
    {
        foreach (List<string> tab in tableaus)
        {
            if (tab.Count > 0 && tab.Last() == cardObject.name)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAlternatingColor(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        char suit1 = card1[0];
        char suit2 = card2[0];
        bool isRed1 = (suit1 == 'D' || suit1 == 'H');
        bool isRed2 = (suit2 == 'D' || suit2 == 'H');
        return isRed1 != isRed2;
    }

    public bool IsSameSuit(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        return card1[0] == card2[0];
    }

    public bool IsOneRankHigher(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        int rank1 = Array.IndexOf(ranks, card1.Substring(1));
        int rank2 = Array.IndexOf(ranks, card2.Substring(1));
        return rank1 == rank2 + 1;
    }

    public bool IsOneRankLower(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        int rank1 = Array.IndexOf(ranks, card1.Substring(1));
        int rank2 = Array.IndexOf(ranks, card2.Substring(1));
        return rank1 + 1 == rank2;
    }

    public bool CanPlaceOnFoundation(string card, int foundationIndex)
    {
        if (foundations[foundationIndex].Count == 0)
        {
            // must be an Ace if foundation is empty
            return card.Substring(1) == "A";
        }
        string topCard = foundations[foundationIndex].Last();
        return IsSameSuit(card, topCard) && IsOneRankHigher(card, topCard);
    }

    public bool CanPlaceOnTableau(string card, int tableauIndex)
    {
        if (tableaus[tableauIndex].Count == 0)
        {
            // FreeCell rule: ANY card can go on an empty tableau
            return true;
        }
        string topCard = tableaus[tableauIndex].Last();
        return IsAlternatingColor(card, topCard) && IsOneRankLower(card, topCard);
    }

    // For clicks: map whatever was clicked (card or pile) to the pile GameObject and indices
    void ResolveTarget(
        GameObject toLocation,
        out GameObject clickedTag,
        out int foundationIndex,
        out int tableauIndex,
        out int freeCellIndex)
    {
        clickedTag = toLocation.transform.CompareTag("Card")
            ? toLocation.transform.parent.gameObject
            : toLocation;

        foundationIndex = -1;
        tableauIndex = -1;
        freeCellIndex = -1;

        if (clickedTag.CompareTag("Foundation"))
        {
            foundationIndex = Array.IndexOf(foundationPositions, clickedTag);
        }
        else if (clickedTag.CompareTag("Tableau"))
        {
            tableauIndex = Array.IndexOf(tableauPositions, clickedTag);
        }
        else if (clickedTag.CompareTag("FreeCell"))
        {
            freeCellIndex = Array.IndexOf(freeCellPositions, clickedTag);
        }
    }
}