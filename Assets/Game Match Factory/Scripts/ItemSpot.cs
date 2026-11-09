using UnityEngine;

public class ItemSpot : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform itemParent;

    [Header(" Settings ")]
    private Item item;
    public Item Item => item;

    public void Populate(Item item) //điền vào
    {
        this.item = item;
        item.transform.SetParent(transform);

        item.AssignSpot(this);
    }

    public void Clear()
    {
        this.item = null;
    }

    public void BumpDown()
    {
        animator.Play("Bump", 0, 0);
    }

    public bool IsEmpty() => item == null;

}
