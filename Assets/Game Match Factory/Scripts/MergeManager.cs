using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [Header(" Go Up Settings ")]
    [SerializeField] private float goUpDistance;
    [SerializeField] private float goUpDuration;
    [SerializeField] private LeanTweenType goUpEasing;

    [Header(" Smash Settings ")]
    [SerializeField] private float smashDuration;
    [SerializeField] private LeanTweenType smashEasing;

    [Header(" Effects ")]
    [SerializeField] private ParticleSystem mergeParticles;

    private void Awake()
    {
        ItemSpotsManager.mergeStarted += OnMergeStarted;
    }

    private void OnDestroy()
    {
        ItemSpotsManager.mergeStarted -= OnMergeStarted;
    }

    private void OnMergeStarted(List<Item> items)
    {
        for(int i = 0; i < items.Count; i++)
        {
            Vector3 targetPos = items[i].transform.position + items[i].transform.up * goUpDistance;

            Action callback = null;

            if (i == 0)
                callback = () => SmashItems(items);

            LeanTween.move(items[i].gameObject, targetPos, goUpDuration)
                .setEase(goUpEasing)
                .setOnComplete(callback);
        }
    }

    private void SmashItems(List<Item> items)
    {
        // Sort the items from left to right
        items.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        float targetX = items[1].transform.position.x;

        LeanTween.moveX(items[0].gameObject, targetX, smashDuration)
            .setEase(smashEasing)
            .setOnComplete(() => FinalizeMerge(items));

        LeanTween.moveX(items[2].gameObject, targetX, smashDuration)
            .setEase(smashEasing);
    }

    private void FinalizeMerge(List<Item> items)
    {
        for (int i = 0; i < items.Count; i++)
            Destroy(items[i].gameObject);

        ParticleSystem particles = Instantiate(mergeParticles, items[1].transform.position, Quaternion.identity, transform);
        particles.Play();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
