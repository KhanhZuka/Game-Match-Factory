using System;
using UnityEngine;
using System.Collections.Generic;

public class ItemSpotsManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform itemSpotsParent;
    private ItemSpot[] spots;

    [Header(" Settings ")]
    [SerializeField] private Vector3 itemLocalPositionOnSpot;
    [SerializeField] private Vector3 itemLocalScaleOnSpot;
    private bool isBusy; //xem spot còn trống không

    [Header(" Data ")]
    private Dictionary<EItemName, ItemMergeData> itemMergeDataDictionary = new Dictionary<EItemName, ItemMergeData>();

    [Header(" Animation Settings ")]
    [SerializeField] private float animationDuration;
    [SerializeField] private LeanTweenType animationEasing;

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
        if (isBusy)
        {
            Debug.Log("ItemSpotManager is busy...");
            return;
        }

        if (!IsFreeSpotAvailable())
        {
            Debug.LogWarning("No free spots available ! Gameover ma boiii");
            return;
        }

        //We're busy
        isBusy = true;

        HandleItemClicked(item);
    }

    private void HandleItemClicked(Item item)
    {
        if(itemMergeDataDictionary.ContainsKey(item.ItemName))
            HandleItemMergeDataFound(item);
        else
            MoveItemToFirstFreeSpot(item);
    }

    private void HandleItemMergeDataFound(Item item)
    {
        ItemSpot idealSpot = GetIdealSpotFor(item);

        itemMergeDataDictionary[item.ItemName].Add(item);

        TryMoveItemToIdealSpot(item, idealSpot);
    }

    private ItemSpot GetIdealSpotFor(Item item) // ham nay tra ve vi tri ben phai cua item cung loai
    {
        List<Item> items = itemMergeDataDictionary[item.ItemName].items;
        List<ItemSpot> itemSpots = new List<ItemSpot>();

        for(int i = 0; i < items.Count; i++)
        {
            itemSpots.Add(items[i].Spot);
        }

        //We have a list of occupied spots by the items similar to item

        //If we only have one spot, we should simply  grab the spot next to it
        if(itemSpots.Count >= 2)
            itemSpots.Sort((a,b) => b.transform.GetSiblingIndex().CompareTo(a.transform.GetSiblingIndex())); // sap xep giam dan vi tri cua cac item

        int idealSpotIndex =  itemSpots[0].transform.GetSiblingIndex() + 1; // lay vi tri ben phai cua item cung loai

        return spots[idealSpotIndex];
    }

    private void TryMoveItemToIdealSpot(Item item, ItemSpot idealSpot)
    {
        if (!idealSpot.IsEmpty()) // neu vi tri ben phai ko trong
        {
            HandleIdealSpotFull(item, idealSpot);
            return;
        }

        MoveItemToSpot(item, idealSpot, () => HandleItemReachedSpot(item)); // neu vi tri ben phai trong thi di chuyen item vao
    }

    private void MoveItemToSpot(Item item, ItemSpot targetSpot, Action completeCallback)
    {
        targetSpot.Populate(item);

        // 2. Scale the item down, set its local position 0,0,0
        //item.transform.localPosition = itemLocalPositionOnSpot;
        //item.transform.localScale = itemLocalScaleOnSpot;
        //item.transform.localRotation = Quaternion.identity;

        LeanTween.moveLocal(item.gameObject, itemLocalPositionOnSpot, animationDuration)
            .setEase(animationEasing);

        LeanTween.scale(item.gameObject, itemLocalScaleOnSpot, animationDuration)
            .setEase(animationEasing);

        LeanTween.rotateLocal(item.gameObject, Vector3.zero, animationDuration)
            .setOnComplete(completeCallback);

        // 3. Disable its shadow
        item.DisableShadows();

        // 4. Disable its collider / physics
        item.DisablePhysics();
    }

    private void HandleItemReachedSpot(Item item, bool checkForMerge = true)
    {
        item.Spot.BumpDown();

        if (!checkForMerge) return;

        if (itemMergeDataDictionary[item.ItemName].CanMergeItems())
            MergeItems(itemMergeDataDictionary[item.ItemName]);
        else
            CheckForGameover();
    }

    private void MergeItems(ItemMergeData itemMergeData)
    {
        List<Item> items = itemMergeData.items;

        //Remove the item merge data from the dictionary
        itemMergeDataDictionary.Remove(itemMergeData.itemName);

        for (int i = 0; i < items.Count; i++)
        {
            items[i].Spot.Clear();
            Destroy(items[i].gameObject);
        }

        if(itemMergeDataDictionary.Count <= 0)
            isBusy = false;
        else
            MoveAllItemToTheLeft(HandleAllItemsMovedToTheLeft);

        // TODO: Remove this line after moving the items to the left
        //isBusy = false;
    }

    private void MoveAllItemToTheLeft(Action completeCallback)
    {
        bool callbackTriggered = false;

        for (int i = 3; i < spots.Length; i++)
        {
            ItemSpot spot = spots[i];

            if (spot.IsEmpty())
                continue;

            Item item = spot.Item;

            ItemSpot targetSpot = spots[i - 3];

            if (!targetSpot.IsEmpty())
            {
                Debug.LogWarning($"{targetSpot.name} is full");
                isBusy = false;
                return;
            }

            spot.Clear();

            completeCallback += () => HandleItemReachedSpot(item, false);
            MoveItemToSpot(item, targetSpot, completeCallback);

            callbackTriggered = true;
        }

        if (!callbackTriggered)
        {
            completeCallback?.Invoke();
        }
    }

    private void HandleAllItemsMovedToTheLeft()
    {
        isBusy = false;
    }

    private void HandleIdealSpotFull(Item item, ItemSpot idealSpot)
    {
        MoveAllItemsToTheRightFrom(idealSpot, item);
    }

    private void MoveAllItemsToTheRightFrom(ItemSpot idealSpot, Item itemToPlace)
    {
        int spotIndex = idealSpot.transform.GetSiblingIndex();

        for(int i = spots.Length - 2; i >= spotIndex; i--)
        {
            ItemSpot spot = spots[i];

            if (spots[i].IsEmpty())
                continue;

            Item item = spots[i].Item;

            spot.Clear();

            ItemSpot targetSpot = spots[i + 1];

            if (!targetSpot.IsEmpty())
            {
                Debug.LogError("ERROR, this should not happen");
                isBusy = false;
                return;
            }

            MoveItemToSpot(item, targetSpot, ()=>HandleItemReachedSpot(item,false));
        }

        MoveItemToSpot(itemToPlace, idealSpot,() => HandleItemReachedSpot(itemToPlace));
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        ItemSpot targetSpot = GetFreeSpot();

        if(targetSpot == null)
        {
            Debug.LogError("Target spot is null => This should not happen!!");
            return;
        }

        CreateItemMergeData(item);

        MoveItemToSpot(item,targetSpot, () => HandleFirstItemReachedSpot(item));
    }

    private void HandleFirstItemReachedSpot(Item item)
    {
        item.Spot.BumpDown();

        CheckForGameover();
    }

    private void CheckForGameover()
    {
        if (GetFreeSpot() == null)
            Debug.LogWarning("Gameover !!!");
        else
            isBusy = false;
    }

    private void CreateItemMergeData(Item item)
    {
        itemMergeDataDictionary.Add(item.ItemName, new ItemMergeData(item));
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
