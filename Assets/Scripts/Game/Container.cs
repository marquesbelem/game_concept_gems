using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour {
    private Rigidbody2D rgdb;
    private Collider2D collider2D;
    private Vector2 direction;
    private void Start () {
        rgdb = GetComponent<Rigidbody2D> ();
        collider2D = GetComponent<Collider2D> ();
        DestroyComponents ();
        // InvokeRepeating ("UpdateMineGem", 2f, 5f);
    }

    /* private void Update () {
          UpdateMineGem ();
     }*/
    public void UpdateName (int x, int y) {
        gameObject.name = "[" + x + "][" + y + "]";
    }

    public Gem MineGem () {
        if (gameObject.transform.childCount > 0)
            return gameObject.transform.GetChild (0).GetComponent<Gem> ();

        return null;
    }

    public Gem UpdateMineGem () {
        RaycastHit2D hit = Physics2D.Raycast (this.gameObject.transform.position, direction);

        if (hit.collider != null) {
            if (Mathf.Round (hit.point.x) == Mathf.Round (gameObject.transform.position.x) &&
                Mathf.Round (hit.point.y) == Mathf.Round (gameObject.transform.position.y)) {
                hit.transform.SetParent (gameObject.transform);
                return hit.transform.GetComponent<Gem> ();
            }
        }

        return null;
    }

    private void DestroyComponents () {
        Destroy (rgdb, 2);
        Destroy (collider2D, 2);
    }

}