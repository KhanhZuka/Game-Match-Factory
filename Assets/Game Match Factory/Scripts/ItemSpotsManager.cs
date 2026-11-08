using System;
using UnityEngine;

public class ItemSpotsManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform itemSpotsParent;
    private ItemSpot[] spots;

    [Header(" Settings ")]
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    [SerializeField] private Vector3 itemLocalScaleOnSpot;


    private void Awake()
    {
        InputManager.itemClicked += OnItemClicked;

        StoreSpots();
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
        if (!IsFreeSpotAvailable())
        {
            Debug.LogWarning("No free spots available ! Gameover ma boiii");
            return;
        }

        HandleItemClicked(item);
    }

    private void HandleItemClicked(Item item)
    {
        MoveItemToFirstFreeSpot(item);
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        ItemSpot targetSpot = GetFreeSpot();

        if(targetSpot == null)
        {
            Debug.LogError("Target spot is null => This should not happen!!");
            return;
        }

        // 1. Turn the item as a child of item spot
        targetSpot.Populate(item);

        // 2. Scale the item down, set its local position 0,0,0
        item.transform.localPosition = itemLocalPositionOnSpot;
        item.transform.localScale = itemLocalScaleOnSpot;
        item.transform.localRotation = Quaternion.identity;

        // 3. Disable its shadow
        item.DisableShadows();

        // 4. Disable its collider / physics
        item.DisablePhysics();

    }

    private void StoreSpots()
    {
        spots = new ItemSpot[itemSpotsParent.childCount];

        for(int i = 0; i < itemSpotsParent.childCount; i++)
        {
            spots[i] = itemSpotsParent.GetChild(i).GetComponent<ItemSpot>();
        }
    }

    private ItemSpot GetFreeSpot()
    {
        for(int i = 0;i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return spots[i];
        }

        return null;
    }

    private bool IsFreeSpotAvailable()
    {
        for(int i = 0;i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return true;
        }

        return false;
    }
}
