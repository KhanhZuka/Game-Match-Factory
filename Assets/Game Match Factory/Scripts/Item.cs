using UnityEngine;

[RequireComponent (typeof(Rigidbody), typeof(SphereCollider))]
public class Item : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Renderer renderer;
    private Material baseMaterial;

    private void Awake()
    {
        baseMaterial = renderer.material;
    }

    public void DisableShadows()
    {

    }

    public void DisablePhysics()
    {
        //rig.isKinematic = true;
        //collider.enabled = false;

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }

    public void Select(Material outlineMaterial)
    {
        renderer.materials = new Material[] {baseMaterial, outlineMaterial };
    }

    public void Deselect()
    {
        renderer.materials = new Material[] { baseMaterial };
    }
}
