using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHp : Status
{
    [Header("Boss DropItem Details")]
    [SerializeField] private int _dropItemAmount = 1;
    [SerializeField] [Range(0f, 1f)] private float _healthDropItemPercentage = 0f; //the percentage that indicates when the boss will drop the item

    [Header("Death Drop Item Details")]
    [SerializeField] private GameObject _endDropItem;

    private float healthPercentCheckpoint = 1f;

    #region Getters and Setters
    public int dropItemAmount
    {
        get { return _dropItemAmount; }
        private set { }
    }

    public float healthDropItemPercentage
    {
        get { return _healthDropItemPercentage; }
        private set { }
    }

    public GameObject endDropItem
    {
        get { return _endDropItem; }
        private set { _endDropItem = value; }
    }
    #endregion

    #region Unity
    void Start()
    {
        SetHealthPercentCheckpoint();
    }

    void Update()
    {
        if (noHealth)
        {
            Debug.Log(gameObject.name + " Boss is destroyed");

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.ResumeBackgroundMusic();
            }

            healthBar.SetActiveState(false);

            InstantiateItem(endDropItem);

            Destroy(gameObject);
        }
        else
        {
            if (currentHealthPercentage <= healthPercentCheckpoint)
            {
                Debug.Log("Reached HealthCheckpoint: " + healthPercentCheckpoint);

                for (int i = 0; i < dropItemAmount; i++) CreateDropItem();

                SetHealthPercentCheckpoint();
            }
        }
    }
    #endregion

    #region Boss DropItem Details
    public void SetDropItemAmount(int dropItemAmount) { this.dropItemAmount = dropItemAmount;}

    public void SetHealthDropItemPercentage(float healthDropItemPercentage) { this.healthDropItemPercentage = healthDropItemPercentage; }
    #endregion

    #region Health Percentage
    public void SetHealthPercentCheckpoint()
    {
        if (healthDropItemPercentage > 0f && healthDropItemPercentage < 1f)
        {
            healthPercentCheckpoint -= healthDropItemPercentage;
        }
        else healthPercentCheckpoint = 1f;
    }
    #endregion
}
