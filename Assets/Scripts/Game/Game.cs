using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public int scoreCurrent;
    public List<int> scorePhase;
    public int phaseCurrent = 0;
    private float _time;
    public Text timeText;
    public Text scoreText;
    public Text scorePhaseText;
    public Text msg;
    public GameObject panelMsg;
    private bool _finish;
    private void Awake () {
        if (Instance == null)
            Instance = this;
        else
            Destroy (gameObject);
    }
    void Start () {
        _time = Time.time;
        Gem.OnMouseOverGemEventHandler += OnMouseOverGem;
        InvokeRepeating ("InvokeCheckGrid", 3f, 2f);
        scorePhaseText.text = "/ " + scorePhase[phaseCurrent].ToString ();
    }
    private void OnDisable () {
        Gem.OnMouseOverGemEventHandler -= OnMouseOverGem;
    }

    private void Update () {

        Timer ();

        if (_movement) {
            if (_currentGem != null)
                _currentGem.transform.position = Vector2.MoveTowards (_currentGem.transform.position, _posEnd, 2 * Time.deltaTime);
            if (_targetGem != null)
                _targetGem.transform.position = Vector2.MoveTowards (_targetGem.transform.position, _posStart, 2 * Time.deltaTime);
        }
    }

    #region Timer and Phases
    void Timer () {
        if (!_finish) {
            float t = Time.time - _time;
            string minutos = ((int) t / 60).ToString ();
            string seconds = (t % 60).ToString ("f0");

            timeText.text = minutos + ":" + seconds;

            if ((int) t / 60 == 2) {
                _finish = true;
                panelMsg.SetActive (true);
                msg.text = "Time is over";
            }

            CheckPhase ();
        }
    }

    void CheckPhase () {
        if (scoreCurrent >= scorePhase[phaseCurrent]) {
            scoreCurrent = 0;
            scoreText.text = scoreCurrent.ToString ();

            if (phaseCurrent == scorePhase.Count - 1) {
                msg.text = "Completed all the stages";
                panelMsg.SetActive (true);
                _finish = true;
            } else {
                phaseCurrent++;
                scorePhaseText.text = "/ " + scorePhase[phaseCurrent].ToString ();
                _time = Time.time;
            }
        }
    }

    public void RestartGame () {
        SceneManager.LoadScene ("Game");
    }

    public void RestartGrid () {
        Gem[] gems = FindObjectsOfType<Gem> ();
        for (int i = 0; i < gems.Length; i++) {
            Destroy (gems[i].gameObject);
        }

        Setup.Instance.InitGrid ();
    }
    #endregion

    #region  Checks Grid and Gem
    void ChangeRgdbStatus (bool status) {
        Gem[] gems = FindObjectsOfType<Gem> ();
        for (int i = 0; i < gems.Length; i++) {
            gems[i].ChangeSimulated (status);
        }
    }

    IEnumerator Movement () {
        ChangeRgdbStatus (false);
        _movement = true;
        _selected.SetMarkSelected (false);
        AudioManager.Instance.PlaySound ("Swap");
        yield return new WaitForSeconds (1f);

        ChangeRgdbStatus (true);
        _movement = false;

        UpdateGrid ();

        ChecksGemsPrize (_currentGem);
        ChecksGemsPrize (_targetGem);

        yield return new WaitForSeconds (.2f);
        if (_spawn)
            Setup.Instance.SpwanGem ();

        yield return new WaitForSeconds (1f);
        _spawn = false;

    }

    public void ChecksGemsPrize (Gem gem) {
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
            foreach (Gem g in _matchsX) {
                if (g != null) {
                    Setup.Instance.PostionsSpawn.Add (g.gameObject.transform.position.x);
                    Destroy (g.gameObject);
                    AudioManager.Instance.PlaySound ("Clear");
                    scoreCurrent = scoreCurrent + 5;
                    scoreText.text = scoreCurrent.ToString ();
                }
            }

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
            foreach (Gem g in _matchsY) {
                if (g != null) {
                    Setup.Instance.PostionsSpawn.Add (g.gameObject.transform.position.x);
                    Destroy (g.gameObject);
                    AudioManager.Instance.PlaySound ("Clear");
                    scoreCurrent = scoreCurrent + 5;
                    scoreText.text = scoreCurrent.ToString ();
                }
            }

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
        if (_spawn)
            Setup.Instance.SpwanGem ();

        yield return new WaitForSeconds (1f);
        _spawn = false;
    }

    private void InvokeCheckGrid () {
        UpdateGrid ();
        StartCoroutine (CheckAllGrid ());
    }

    void OnMouseOverGem (Gem gem) {
        if (_selected == gem) {
            //clicando duas vezes no mesmo item 
            _selected.SetMarkSelected (false);
            _selected = null;
            AudioManager.Instance.PlaySound ("Select");
            return;
        }

        if (_selected == null) {
            //Selecionou 
            _selected = gem;
            _posStart = _selected.transform.position;
            _selected.SetMarkSelected (true);
            AudioManager.Instance.PlaySound ("Select");

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
    #endregion
}