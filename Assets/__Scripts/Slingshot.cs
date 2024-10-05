using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour {
    [Header("Inscribed")]
    public GameObject   projectilePrefab;
    public LineRenderer rubberBand;
    public AudioClip    stretchSound;
    public AudioClip    snapSound;
    private AudioSource audioSource;
    public float        velocityMult = 10f;

    public GameObject   projLinePrefab;
    public GameObject    leftAnchor;
    public GameObject    rightAnchor;

    [Header("Dynamic")]
    public GameObject   launchPoint;
    public Vector3      launchPos;
    public GameObject   projectile;
    public bool         aimingMode;

    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
        
        rubberBand = GetComponent<LineRenderer>();
        rubberBand.positionCount = 3;
        rubberBand.startWidth = 0.3f;
        rubberBand.endWidth = 0.3f;

        audioSource = gameObject.AddComponent<AudioSource>();
    }
    void OnMouseEnter() {
        launchPoint.SetActive(true);
    }

    void OnMouseExit() {
        launchPoint.SetActive(false);
    }

    void OnMouseDown() {
        aimingMode = true;
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        audioSource.PlayOneShot(stretchSound);
    }

    void Update() {
        if (!aimingMode) {
            rubberBand.enabled = false;
            return;
        }
        rubberBand.enabled = true;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        rubberBand.SetPosition(0, leftAnchor.transform.position + Vector3.up * 0.7f);
        rubberBand.SetPosition(1, projPos);
        rubberBand.SetPosition(2, rightAnchor.transform.position + Vector3.up * 0.7f);

        if (Input.GetMouseButtonUp(0)) {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile;
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();

            audioSource.PlayOneShot(snapSound       );
        }
    }
}
