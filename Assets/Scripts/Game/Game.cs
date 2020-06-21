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
    List<Gem> matchsX;
    List<Gem> matchsY;
    private void Awake () {
        if (Instance == null)
            Instance = this;
        else
            Destroy (gameObject);
    }
    void Start () {
        Gem.OnMouseOverGemEventHandler += OnMouseOverGem;
    }
    private void OnDisable () {
        Gem.OnMouseOverGemEventHandler -= OnMouseOverGem;
    }

    public void ChecksGemsPrize () {
        int row = Setup.Instance.Row;
        int columns = Setup.Instance.Columns;
        int count = 0;

        matchsX = new List<Gem> ();
        matchsY = new List<Gem> ();

        for (int r = 0; r < row; r++) {
            for (int c = 0; c < columns; c++) {
                if (c < columns - 1) {
                    if (Grid[r, c].ID == Grid[r, c + 1].ID) {
                        if (!matchsX.Contains (Grid[r, c]))
                            matchsX.Add (Grid[r, c]);

                        matchsX.Add (Grid[r, c + 1]);

                        if (matchsX.Count >= 3) {
                            foreach (Gem g in matchsX) {

                                Destroy (g.gameObject);
                            }

                            matchsX.Clear ();
                        }
                    }
                }

                if (r < row - 1) {
                    if (Grid[r, c].ID == Grid[r + 1, c].ID) {
                        if (!matchsY.Contains (Grid[r, c]))
                            matchsY.Add (Grid[r, c]);

                        matchsY.Add (Grid[r + 1, c]);

                        if (matchsY.Count >= 3) {
                            foreach (Gem g in matchsY)
                                Destroy (g.gameObject);

                            matchsY.Clear ();
                        }
                    }
                }
            }
        }

        /*int left = gem.X - 1;
        int right = gem.X + 1;

        matchsX = new List<Gem> { gem };

        Debug.Log("ID - " + gem.ID);

        while (left >= 0) {
            if (Grid[left, gem.Y].ID == gem.ID)
                matchsX.Add (Grid[left, gem.Y]);

            left--;
        }

        while (right < row) {
            if (Grid[right, gem.Y].ID == gem.ID)
                matchsX.Add (Grid[right, gem.Y]);

            right++;
        }

        Debug.Log ("MatchsX count: " + matchsX.Count);
        if (matchsX.Count >= 2) {
            Debug.Log ("X Combinou");
        }

        int down = gem.Y - 1;
        int up = gem.Y + 1;

        matchsY = new List<Gem> { gem };

        while (down >= 0) {
            if (Grid[gem.X, down].ID == gem.ID)
                matchsY.Add (Grid[gem.X, down]);

            down--;
        }

        while (up < row) {
            if (Grid[gem.X, up].ID == gem.ID)
                matchsY.Add (Grid[gem.X, up]);

            up++;
        }

        Debug.Log ("MatchsY count: " + matchsY.Count);
        if (matchsY.Count >= 2) {
            Debug.Log ("Y Combinou");
        }*/
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

        yield return UpdateGrid ();
        //ChecksGemsPrize ();
    }

    IEnumerator UpdateGrid () {
        int row = Setup.Instance.Row;
        int columns = Setup.Instance.Columns;

        for (int r = 0; r < row; r++) {
            for (int c = 0; c < columns; c++) {
                string name = "[" + r + "][" + c + "]";

                GameObject container = GameObject.Find (name);
                container.GetComponent<Container> ().UpdateMineGem ();

                yield return new WaitForSeconds (.5f);
                Gem gem = container.GetComponent<Container> ().MineGem ();
                gem.UpdatePosition (r, c);
                Game.Instance.Grid[r, c] = gem;
            }
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