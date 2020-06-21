using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    public Gem[, ] Grid;
    private Gem _selected;
    private Vector3 _posStart;
    private Vector3 _posEnd;
    private Gem _targetGem;
    private Gem _currentGem;
    private bool _movement;
    private List<Gem> _matchsX;
    private List<Gem> _matchsY;
    private bool _spawn;

    private void Awake () {
        if (Instance == null)
            Instance = this;
        else
            Destroy (gameObject);
    }
    void Start () {
        Gem.OnMouseOverGemEventHandler += OnMouseOverGem;
        InvokeRepeating ("UpdateGrid", 0f, 2f);
        InvokeRepeating ("DestroyClones", 2f, 8f);

    }
    private void OnDisable () {
        Gem.OnMouseOverGemEventHandler -= OnMouseOverGem;
    }

    private void Update () {
        if (_movement) {
            _currentGem.transform.position = Vector2.MoveTowards (_currentGem.transform.position, _posEnd, 2 * Time.deltaTime);
            _targetGem.transform.position = Vector2.MoveTowards (_targetGem.transform.position, _posStart, 2 * Time.deltaTime);
        }
    }

    void ChangeRgdbStatus (bool status) {
        foreach (Gem gem in Grid) {
            gem.ChangeSimulated (status);
        }
    }

    IEnumerator Movement () {
        ChangeRgdbStatus (false);
        _movement = true;
        _selected.SetMarkSelected (false);

        yield return new WaitForSeconds (1f);

        ChangeRgdbStatus (true);
        _movement = false;

        //yield return StartCoroutine (UpdateGrid ());
        // UpdateGrid ();

        yield return StartCoroutine (CheckAllGrid ());

        //  UpdateGrid ();
    }

    public void ChecksGemsPrize (Gem gem) {
        // Debug.Log ("ChecksGemsPrize");
        int row = Setup.Instance.Row;
        int columns = Setup.Instance.Columns;
        int count = 0;

        _matchsX = new List<Gem> { gem };
        _matchsY = new List<Gem> { gem };

        int left = gem.X - 1;
        while (left >= 0 && Grid[left, gem.Y].ID == gem.ID) {
            _matchsX.Add (Grid[left, gem.Y]);
            left--;
        }

        int right = gem.X + 1;
        while (right < columns && Grid[right, gem.Y].ID == gem.ID) {
            _matchsX.Add (Grid[right, gem.Y]);
            right++;
        }

        if (_matchsX.Count >= 3) {
            foreach (Gem g in _matchsX)
                Destroy (g.gameObject);

            _matchsX.Clear ();
            _spawn = true;
        }

        int down = gem.Y - 1;
        int up = gem.Y + 1;

        while (down >= 0 && Grid[gem.X, down].ID == gem.ID) {
            _matchsY.Add (Grid[gem.X, down]);
            down--;
        }

        while (up < row && Grid[gem.X, up].ID == gem.ID) {
            _matchsY.Add (Grid[gem.X, up]);
            up++;
        }

        if (_matchsY.Count >= 3) {
            foreach (Gem g in _matchsY)
                Destroy (g.gameObject);

            _matchsY.Clear ();
            _spawn = true;
        }
    }

    void UpdateGrid () {

        int row = Setup.Instance.Row;
        int columns = Setup.Instance.Columns;

        for (int c = 0; c < columns; c++) {
            for (int r = 0; r < row; r++) {
                string name = "[" + c + "][" + r + "]";

                GameObject container = GameObject.Find (name);
                Gem gem = container.GetComponent<Container> ().UpdateMineGem ();
                if (gem != null) {
                    gem.UpdatePosition (c, r);
                    Grid[c, r] = gem;
                }
            }
        }

        // Debug.Log ("Update Grid");
    }
    IEnumerator CheckAllGrid () {
        int row = Setup.Instance.Row;
        int columns = Setup.Instance.Columns;
        for (int c = 0; c < columns; c++) {
            for (int r = 0; r < row; r++) {
                ChecksGemsPrize (Grid[c, r]);
            }
        }

        yield return new WaitForSeconds (.2f);
        if (_spawn) {
            DestroyClones ();
            Setup.Instance.SpwanGem ();
        }
        yield return new WaitForSeconds (1f);
        _spawn = false;
        // Debug.Log ("Logo apos do return CheckALL grid");
    }

    private void InvokeCheckGrid () {
        StartCoroutine (CheckAllGrid ());

        // StartCoroutine (UpdateGrid ());
    }

    private void DestroyClones () {
        Debug.Log ("Get Clones");
        GameObject[] clones = GameObject.FindGameObjectsWithTag ("Clone");

        for (int i = 0; i < clones.Length; i++) {
            if (clones[i].transform.position.y > 4)
                clones[i].GetComponent<Gem> ().IAmDestroy ();
        }
    }
    void OnMouseOverGem (Gem gem) {
        if (_selected == gem) {
            //clicando duas vezes no mesmo item 
            _selected.SetMarkSelected (false);
            _selected = null;
            return;
        }

        if (_selected == null) {
            //Selecionou 
            _selected = gem;
            _posStart = _selected.transform.position;
            _selected.SetMarkSelected (true);

        } else {

            float x = Mathf.Abs (gem.X - _selected.X);
            float y = Mathf.Abs (gem.Y - _selected.Y);

            if (x + y == 1) {
                //Realizar a troca

                _currentGem = _selected;
                _targetGem = gem;
                _posEnd = gem.transform.position;

                StartCoroutine (Movement ());

            } else {
                Debug.Log ("Esse Movimento não é permitido");
                _selected.SetMarkSelected (false);
            }
            _selected = null;
        }
    }

}