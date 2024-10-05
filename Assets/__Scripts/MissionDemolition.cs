using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameMode {
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour {
    static private MissionDemolition S;
    [Header("Inscribed")]
    public Text             uitLevel;
    public Text             uitShots;
    public Vector3          castlePos;
    public GameObject[]     castles;
    public int              maxShots = 20;

    [Header("Dynamic")]
    public int              level;
    public int              levelMax;
    public int              shotsTaken;
    public GameObject       castle;
    public GameMode         mode = GameMode.idle;
    public string           showing = "Show Slingshot";

    void Start() {
        S = this;

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel() {
        if (castle != null) {
            Destroy(castle);
        }
        Projectile.DESTROY_PROJECTILES();

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);

        Invoke("SwitchToSlingshotView", 3f);
    }

    void SwitchToSlingshotView() {
        FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
    }

    void UpdateGUI() {
        uitLevel.text = "Level: "+(level+1)+" of "+levelMax;
        uitShots.text = "Shots Taken: "+shotsTaken + " / " + maxShots;
    }

    void Update() {
        UpdateGUI();

        if ((mode == GameMode.playing) && Goal.goalMet) {
            mode = GameMode.levelEnd;
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);
            Invoke("NextLevel", 2f);
        }   

        if (shotsTaken >= maxShots && !Goal.goalMet) {
            if (AllProjectilesAsleep()) {
                SceneManager.LoadScene("GameOverMenu");
            }
        }
    }

    bool AllProjectilesAsleep() {
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile proj in projectiles) {
            if (proj.awake) {
                return false;
            }
        }
        return true;
    }


    void NextLevel() {
        level++;
        
        if (level == levelMax) {
            if (shotsTaken < maxShots) {
                SceneManager.LoadScene("VictoryMenu");
            } else {
                SceneManager.LoadScene("GameOverMenu");
            }
        } else {
            StartLevel();
        }
    }

    static public void SHOT_FIRED() {
        S.shotsTaken++;
    }

    static public GameObject GET_CASTLE() {
        return S.castle;
    }
}
