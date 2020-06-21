using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Classe responsavel por setar características para as gemas*/
public class Gem : MonoBehaviour {

    public int ID;

    public int X {
        get;
        private set;
    }

    public int Y {
        get;
        private set;
    }

    private Rigidbody2D rgdb;
    private SpriteRenderer spriteRenderer;

    private void Start () {
        rgdb = GetComponent<Rigidbody2D> ();
        spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    public void SetMarkSelected (bool selected) {
        if (selected)
            spriteRenderer.color = new Color (0.3f, 0.3f, 0.3f, 1f);
        else
            spriteRenderer.color = new Color (255f, 255f, 255f, 1f);
    }

    public void SetMarkChoices (bool selected) {
        if (selected)
            spriteRenderer.color = new Color (0.6f, 0.9f, 0.3f, 1f);
        else
            spriteRenderer.color = new Color (255f, 255f, 255f, 1f);
    }

    public void ChangeSimulated (bool status) {
        rgdb.simulated = status;
    }
    public void UpdatePosition (int x, int y) {
        X = x;
        Y = y;
        gameObject.name = "Gem: " + ID.ToString ();
    }

    private void OnMouseDown () {
        if (OnMouseOverGemEventHandler != null) {
            OnMouseOverGemEventHandler (this);
        }
    }

    public delegate void OnMouseOverGem (Gem gem);
    public static event OnMouseOverGem OnMouseOverGemEventHandler;
}