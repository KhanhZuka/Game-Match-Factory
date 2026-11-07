using System;
using UnityEngine;

public class ItemSpotsManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform itemSpot;

    [Header(" Settings ")]
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    [SerializeField] private Vector3 itemLocalScaleOnSpot;


    private void Awake()
    {
        InputManager.itemClicked += OnItemClicked;
    }

    private void OnDestroy()
    {
        InputManager.itemClicked -= OnItemClicked;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnItemClicked(Item item)
    {
        // 1. Turn the item as a child of item spot
        item.transform.SetParent(itemSpot);

        // 2. Scale the item down, set its local position 0,0,0
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScaleOnSpot;

        // 3. Disable its shadow
        item.DisableShadows();

        // 4. Disable its collider / physics
        item.DisablePhysics();
    }
}
