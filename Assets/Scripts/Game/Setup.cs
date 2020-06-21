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
    public float spacing = 1.6f;
    private void Awake () {
        if (Instance == null)
            Instance = this;
        else
            Destroy (gameObject);
    }

    void Start () {
        InitGrid ();
    }

    //x => calunas
    //y => linhas
    void InitGrid () {
        Game.Instance.Grid = new Gem[Columns, Row];

        for (int c = 0; c < Columns; c++) {
            for (int r = 0; r < Row; r++) {

                float cWidth = c * spacing;
                Vector3 pos = new Vector3 (cWidth, r, 0);

                GameObject container = Instantiate (_goContainer, pos, Quaternion.identity, _goGrid.transform);
                container.GetComponent<Container> ().UpdateName (c, r);

                int idx = Random.Range (0, _gems.Count - 1);
                GameObject clone = Instantiate (_gems[idx], container.transform.position, Quaternion.identity, container.transform);
                clone.GetComponent<Gem> ().UpdatePosition (c, r);

                Game.Instance.Grid[c, r] = container.GetComponent<Container> ().MineGem ();
            }
        }
    }

    public void SpwanGem () {
        for (int c = 0; c < Columns; c++) {
            for (int r = 0; r < Row; r++) {
                float cWidth = c * spacing;
                Vector3 pos = new Vector3 (cWidth, 7f, 0);

                int idx = Random.Range (0, _gems.Count - 1);
                GameObject clone = Instantiate (_gems[idx], pos, Quaternion.identity, _goGrid.transform);
                clone.transform.localScale = new Vector3 (.6f, .6f, .6f);
                clone.tag = "Clone"; 
            }
        }
    }

}