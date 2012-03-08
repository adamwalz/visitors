using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlicerProjectile : MonoBehaviour
{
    public bool CreateCollisionMeshs = true;
    public bool DelayCreateCollisionMeshs = true;
    public Material CutMaterial;
    public int CutMaterialSlot = 1;
    public float Speed = 50.0f;
    public float LifeTime = 2.0f;
    public int MeshColliderTriangles = 50;
    public AudioClip FireAudio;
    public AudioClip EnterAudio;
    public AudioClip ExitAudio;

    private AudioSource audioSource;
    private List<GameObject> slicingObjects = new List<GameObject>();

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (FireAudio != null)
            audioSource.PlayOneShot(FireAudio);
    }

    IEnumerator Start()
    {
        if (FireAudio != null)
            Instantiate(FireAudio, transform.position, transform.rotation);

        rigidbody.AddForce(transform.forward * Speed, ForceMode.VelocityChange);

        yield return new WaitForSeconds(LifeTime);

        foreach (var slicingObject in slicingObjects.ToArray())
            FinalizeSlice(slicingObject);
        Destroy(gameObject);
    }

    private void FinalizeSlice(GameObject slicingObject)
    {
        var sliceable = slicingObject.GetComponent<Sliceable>();
        if (sliceable == null)
            return;

        sliceable.CreateSlices();
        slicingObjects.Remove(slicingObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sliceable")
        {
            var sliceable = other.gameObject.GetComponent<Sliceable>();
            if (sliceable != null)
                return;
            sliceable = other.gameObject.AddComponent<Sliceable>();

            var relativePosition = other.gameObject.transform.InverseTransformPoint(this.transform.position);
            var relativeDirection = other.gameObject.transform.InverseTransformDirection(this.transform.up).normalized;
            var cutPlane = new Plane(relativeDirection, relativePosition);

            sliceable.CutMaterial = CutMaterial;
            sliceable.CutMaterialSlot = CutMaterialSlot;
            sliceable.CreateCollisionMeshs = CreateCollisionMeshs;
            sliceable.DelayCreateCollisionMeshs = DelayCreateCollisionMeshs;
            sliceable.MeshColliderTriangles = MeshColliderTriangles;
            sliceable.CutPlane = cutPlane;

            slicingObjects.Add(other.gameObject);

            if (EnterAudio != null)
                audioSource.PlayOneShot(EnterAudio);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Sliceable")
        {
            if (ExitAudio != null)
                audioSource.PlayOneShot(ExitAudio);
            FinalizeSlice(other.gameObject);
        }
    }
}
