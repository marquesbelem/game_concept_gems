using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour {
    private Rigidbody2D rgdb;
    private Collider2D collider2D;
    private void Start () {
        rgdb = GetComponent<Rigidbody2D> ();
        collider2D = GetComponent<Collider2D> ();
        DestroyComponents ();
    }
    public void UpdateName (int x, int y) {
        gameObject.name = "[" + x + "][" + y + "]";
    }

    public Gem MineGem () {
        return gameObject.transform.GetChild (0).GetComponent<Gem> ();
    }

    public void UpdateMineGem () {
        GameObject[] gems = GameObject.FindGameObjectsWithTag ("Gem");

        for (int i = 0; i < gems.Length; i++) {
            if (Mathf.Round (gems[i].transform.TransformPoint (Vector3.zero).x) == Mathf.Round (gameObject.transform.position.x) &&
                Mathf.Round (gems[i].transform.TransformPoint (Vector3.zero).y) == Mathf.Round (gameObject.transform.position.y)) {
                gems[i].transform.SetParent (gameObject.transform);
            }
        }
    }

    private void DestroyComponents () {
        Destroy (rgdb, 2);
        Destroy (collider2D, 2);
    }

}