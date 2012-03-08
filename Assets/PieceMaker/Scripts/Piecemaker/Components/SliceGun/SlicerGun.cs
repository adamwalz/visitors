using System.Linq;
using UnityEngine;
using System.Collections;

public class SlicerGun : MonoBehaviour
{
    public Transform Projectile;
    public float FireDelay = 100.0f;
    public AudioClip ReadyAudio;

    private GameObject slicePlaneGameObject;
    private bool gunIsWaiting;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        slicePlaneGameObject = this.transform.FindChild("SlicePlane").gameObject;
        slicePlaneGameObject.SetActiveRecursively(false);

        Screen.lockCursor = true;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 500, 20), "Left Mouse Button: Slice target");
        GUI.Label(new Rect(10, 30, 500, 20), "'Q' Key: Rotate slicer counter clock wise");
        GUI.Label(new Rect(10, 50, 500, 20), "'E' Key: Rotate slicer clock wise");
    }
	
	void Update()
    {
        if (Input.GetKey(KeyCode.Q))
            this.transform.RotateAroundLocal(Vector3.forward, Time.deltaTime * 2);
        if (Input.GetKey(KeyCode.E))
            this.transform.RotateAroundLocal(Vector3.forward, Time.deltaTime * -2);

        if (Input.GetButton("Fire1") && !gunIsWaiting && !slicePlaneGameObject.active)
        {
            if (ReadyAudio != null)
                audioSource.PlayOneShot(ReadyAudio);
            slicePlaneGameObject.SetActiveRecursively(true);
        }
        else if (Input.GetButtonUp("Fire1") && Projectile != null && !gunIsWaiting)
        {
            slicePlaneGameObject.SetActiveRecursively(false);
            StartCoroutine(Fire());
        }
	}

    private IEnumerator Fire()
    {
        Instantiate(Projectile, transform.position, transform.rotation);
        gunIsWaiting = true;
        yield return new WaitForSeconds(FireDelay / 1000);
        gunIsWaiting = false;
    }
}
