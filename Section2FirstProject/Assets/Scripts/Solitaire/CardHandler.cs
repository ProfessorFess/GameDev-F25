using UnityEngine;

public class CardHandler : MonoBehaviour
{
    public Sprite cardBack;
    public Sprite cardFront;
    private SpriteRenderer spriteRenderer;
    public bool isFaceUp = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFaceUp)
        {
            spriteRenderer.sprite = cardFront;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }
}
