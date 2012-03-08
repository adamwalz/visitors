using System.Linq;
using UnityEngine;
using System.Collections;

public class Sliceable : MonoBehaviour
{
    public bool CreateCollisionMeshs = true;
    public bool DelayCreateCollisionMeshs = true;
    public Material CutMaterial;
    public int CutMaterialSlot = 1;
    public Plane CutPlane;
    public int MeshColliderTriangles = 50;

    public bool hasSliced;
    public bool isAwaitingCreation;
    public bool hasCreatedData;
    private MeshUtilities.VirtualMesh resultPositiveMesh;
    private MeshUtilities.VirtualMesh resultNegativeMesh;
    private MeshUtilities.VirtualMesh resultPositiveHull;
    private MeshUtilities.VirtualMesh resultNegativeHull;

    void Awake()
    {
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            return;

        var virtualMesh = new MeshUtilities.VirtualMesh(meshFilter.mesh);

        UnityThreadHelper.TaskDistributor.Dispatch(UnityThreading.Dispatcher.CreateSafeAction(
        () =>
        {
            var sliceSettings = new MeshUtilities.SliceSettings();
            sliceSettings.SliceAreaSettings.SubMeshId = CutMaterialSlot;

            var splitResult = MeshUtilities.Tools.SplitMesh(CutPlane, virtualMesh, sliceSettings);
            if (!splitResult.Success)
            {
                UnityThreadHelper.Dispatcher.Dispatch(() => SetSliceResultData(null, null, null, null));
                return;
            }

            MeshUtilities.VirtualMesh positiveHull = null;
            MeshUtilities.VirtualMesh negativeHull = null;

            if (CreateCollisionMeshs && !DelayCreateCollisionMeshs)
            {
                positiveHull = MeshUtilities.Tools.CreateHull(splitResult.PositiveResult, MeshColliderTriangles);
                negativeHull = MeshUtilities.Tools.CreateHull(splitResult.NegativeResult, MeshColliderTriangles);
            }

            UnityThreadHelper.Dispatcher.Dispatch(() => SetSliceResultData(splitResult.PositiveResult, splitResult.NegativeResult, positiveHull, negativeHull));
        },
        exception =>
        {
            UnityThreadHelper.Dispatcher.Dispatch(() => SetSliceResultData(null, null, null, null));
        }));
    }

    private void SetSliceResultData(MeshUtilities.VirtualMesh positiveMesh, MeshUtilities.VirtualMesh negativeMesh, MeshUtilities.VirtualMesh positiveHull, MeshUtilities.VirtualMesh negativeHull)
    {
        hasCreatedData = true;
        resultPositiveMesh = positiveMesh;
        resultNegativeMesh = negativeMesh;
        resultPositiveHull = positiveHull;
        resultNegativeHull = negativeHull;

        if (isAwaitingCreation)
            CreateSlices();
    }

    public void CreateSlices()
    {
        if (hasSliced)
            return;

        if (!hasCreatedData)
        {
            isAwaitingCreation = true;
            return;
        }

        hasSliced = true;

        if (resultPositiveMesh == null && resultNegativeMesh == null)
        {
            GameObject.Destroy(gameObject.GetComponent<Sliceable>());
            return;
        }

        var positive = (GameObject)GameObject.Instantiate(gameObject);
        positive.name = gameObject.name;
        var negative = (GameObject)GameObject.Instantiate(gameObject);
        negative.name = gameObject.name;

        GameObject.Destroy(positive.GetComponent<Sliceable>());
        GameObject.Destroy(negative.GetComponent<Sliceable>());

        positive.GetComponent<MeshFilter>().mesh = resultPositiveMesh.ToMesh();
        negative.GetComponent<MeshFilter>().mesh = resultNegativeMesh.ToMesh();

        if (CutMaterial != null)
        {
            var materials = positive.renderer.materials;
            if (CutMaterialSlot + 1 > positive.renderer.materials.Length)
                System.Array.Resize(ref materials, CutMaterialSlot + 1);
            materials[CutMaterialSlot] = CutMaterial;
            positive.renderer.materials = materials;

            materials = negative.renderer.materials;
            if (CutMaterialSlot + 1 > negative.renderer.materials.Length)
                System.Array.Resize(ref materials, CutMaterialSlot + 1);
            materials[CutMaterialSlot] = CutMaterial;
            negative.renderer.materials = materials;
        }

        positive.rigidbody.AddForce(CutPlane.normal * 200);
        negative.rigidbody.AddForce(-CutPlane.normal * 200);

        if (CreateCollisionMeshs && DelayCreateCollisionMeshs)
        {
            if (DelayCreateCollisionMeshs)
            UnityThreadHelper.TaskDistributor.Dispatch(UnityThreading.Dispatcher.CreateSafeAction(
            () =>
            {
                var positiveHull = MeshUtilities.Tools.CreateHull(resultPositiveMesh, MeshColliderTriangles);
                var negativeHull = MeshUtilities.Tools.CreateHull(resultNegativeMesh, MeshColliderTriangles);

                UnityThreadHelper.Dispatcher.Dispatch(() =>
                {
                    if (positiveHull != null)
                    {
                        var positiveCollider = positive.GetComponent<MeshCollider>();
                        if (positiveCollider == null)
                        {
                            GameObject.Destroy(positive.GetComponent("Collider"));
                            positiveCollider = positive.AddComponent<MeshCollider>();
                        }
                        
                        positiveCollider.sharedMesh = positiveHull.ToMesh();
                        positiveCollider.convex = true;
                    }

                    if (negativeHull != null)
                    {
                        var negativeCollider = negative.GetComponent<MeshCollider>();
                        if (negativeCollider == null)
                        {
                            GameObject.Destroy(negative.GetComponent("Collider"));
                            negativeCollider = negative.AddComponent<MeshCollider>();
                        }
                        
                        negativeCollider.sharedMesh = negativeHull.ToMesh();
                        negativeCollider.convex = true;
                    }
                });
            },
            exception =>
            {
                // nop
            }));
        }
        
        if (resultPositiveHull != null && resultNegativeHull != null)
        {
            var positiveCollider = positive.GetComponent<MeshCollider>();
            var negativeCollider = negative.GetComponent<MeshCollider>();

            if (positiveCollider == null && negativeCollider == null)
            {
                GameObject.Destroy(positive.GetComponent("Collider"));
                GameObject.Destroy(negative.GetComponent("Collider"));

                positiveCollider = positive.AddComponent<MeshCollider>();
                negativeCollider = negative.AddComponent<MeshCollider>();
            }

            if (positiveCollider != null)
            {
                positiveCollider.sharedMesh = resultPositiveHull.ToMesh();
                positiveCollider.convex = true;
            }

            if (negativeCollider != null)
            {
                negativeCollider.sharedMesh = resultNegativeHull.ToMesh();
                negativeCollider.convex = true;
            }
        }

        GameObject.Destroy(gameObject);
    }
}
