using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCellInput : MonoBehaviour
{
    private FreeCellSolitaire solitaire;
    private GameObject selectedCard = null;

    void Start()
    {
        // Make sure a FreeCellSolitaire component exists in your scene
        solitaire = FindAnyObjectByType<FreeCellSolitaire>();
        if (solitaire == null)
        {
            Debug.LogError("FreeCellInput: Could not find FreeCellSolitaire in the scene.");
        }
    }

    void Update()
    {
        // Input is handled via OnBurst from the Input System
    }

    // Called by the Input System (e.g., on mouse click)
    void OnBurst(InputValue value)
    {
        if (solitaire == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit == null) return;

        // --- CARD CLICKED ---
        if (hit.CompareTag("Card"))
        {
            Debug.Log("Card clicked: " + hit.name);

            // If we already have a selected card, try to move it onto this card's pile
            if (selectedCard != null)
            {
                if (selectedCard == hit.gameObject)
                {
                    // Clicking the same card again: deselect
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }

                // Try to move selectedCard to the pile that this clicked card belongs to
                if (solitaire.IsValidMove(selectedCard, hit.gameObject))
                {
                    solitaire.PlaceCard(selectedCard, hit.gameObject);
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
                else
                {
                    Debug.Log("Move invalid from selected card to clicked card.");
                    return;
                }
            }

            // No card currently selected: select this card if it is face up
            CardSprite cs = hit.gameObject.GetComponent<CardSprite>();
            if (cs != null && cs.isFaceUp)
            {
                Debug.Log("Card selected: " + hit.name);
                selectedCard = hit.gameObject;
                selectedCard.GetComponent<SpriteRenderer>().color = Color.gray;
            }

            return;
        }

        // --- TABLEAU CLICKED ---
        if (hit.CompareTag("Tableau"))
        {
            Debug.Log("Tableau clicked: " + hit.name);
            if (selectedCard != null && solitaire.IsValidMove(selectedCard, hit.gameObject))
            {
                solitaire.PlaceCard(selectedCard, hit.gameObject);
                selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                selectedCard = null;
            }
            else
            {
                Debug.Log("Invalid move to tableau or no card selected.");
            }
            return;
        }

        // --- FOUNDATION CLICKED ---
        if (hit.CompareTag("Foundation"))
        {
            Debug.Log("Foundation clicked: " + hit.name);
            if (selectedCard != null && solitaire.IsValidMove(selectedCard, hit.gameObject))
            {
                solitaire.PlaceCard(selectedCard, hit.gameObject);
                selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                selectedCard = null;
            }
            else
            {
                Debug.Log("Invalid move to foundation or no card selected.");
            }
            return;
        }

        // --- FREECELL CLICKED ---
        if (hit.CompareTag("FreeCell"))
        {
            Debug.Log("FreeCell clicked: " + hit.name);
            if (selectedCard != null && solitaire.IsValidMove(selectedCard, hit.gameObject))
            {
                solitaire.PlaceCard(selectedCard, hit.gameObject);
                selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                selectedCard = null;
            }
            else
            {
                Debug.Log("Invalid move to freecell or no card selected.");
            }
            return;
        }
    }
}