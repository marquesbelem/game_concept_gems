using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Classe responsavel pela configuração da grade*/
public class Setup : MonoBehaviour {
    public static Setup Instance { get; private set; }

    [SerializeField] private List<GameObject> _gems;
    [SerializeField] private GameObject _goGrid;
    [SerializeField] private GameObject _goContainer;
    public int Row;
    public int Columns;
    private void Awake () {
        if (Instance == null)
            Instance = this;
        else
            Destroy (gameObject);
    }

    void Start () {
        InitGrid ();
    }
    void InitGrid () {
        Game.Instance.Grid = new Gem[Row, Columns];

        for (int r = 0; r < Row; r++) {
            for (int c = 0; c < Columns; c++) {

                float cWidth = c * .8f;
                Vector3 pos = new Vector3 (cWidth, r, 0);

                GameObject container = Instantiate (_goContainer, pos, Quaternion.identity, _goGrid.transform);
                container.GetComponent<Container> ().UpdateName (r, c);

                int idx = Random.Range (0, _gems.Count - 1);
                GameObject clone = Instantiate (_gems[idx], container.transform.position, Quaternion.identity, container.transform);
                clone.GetComponent<Gem> ().UpdatePosition (r, c);

                Game.Instance.Grid[r, c] = container.GetComponent<Container> ().MineGem ();
            }
        }
    }

}