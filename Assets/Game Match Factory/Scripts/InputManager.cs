using System;
using Unity.Collections;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    public static Action<Item> itemClicked; // Khai báo Action nhận vào Item, cho phép các class khác đăng ký phương thức để nhận thông báo khi Item được click.

    [Header(" Setting")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Item currentItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            HandleDrag();
        else if (Input.GetMouseButtonUp(0))
            HandleMouseUp();
    }

    private void HandleDrag()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100); //ban 1 tia raycast tu camera den cho click chuot va luu vao hit

        if (hit.collider == null)
        {
            DeselectCurrentItem();
            return;
        }

        if (!hit.collider.TryGetComponent(out Item item))
        {
            DeselectCurrentItem();
            return;
        }

        DeselectCurrentItem();

        currentItem = item;
        currentItem.Select(outlineMaterial);
    }

    private void DeselectCurrentItem()
    {
        if(currentItem != null)
            currentItem.Deselect();

        currentItem = null;
    }

    private void HandleMouseUp()
    {
        if (currentItem == null)
            return;

        currentItem.Deselect();

        itemClicked?.Invoke(currentItem); // Kích hoạt sự kiện itemClicked, truyền item vừa click đến tất cả các phương thức đã đăng ký (nếu có).
        currentItem = null;
    }
}
