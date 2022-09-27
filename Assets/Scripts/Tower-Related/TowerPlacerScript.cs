using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerPlacerScript : MonoBehaviour
{
    [SerializeField] private GameObject TextToModifyIn;
    public bool FollowPointer = false;

    private GameObject newTower;
    private SpriteRenderer newTowerSR;
    private Collider2D newTowerC2;

    private bool SkipClickOnce;
    //private bool Placeable = false;

    private void FixedUpdate()
    {
        /*
        if (!FollowPointer)
        {
            return;
        }
        Placeable = newTowerC2.IsTouchingLayers(7);
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (!FollowPointer)
        {
            return;
        }

        // Follow mouse.
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newTower.transform.position = mousePosition;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Global.PointerState = "";
            Destroy(newTower);
            FollowPointer = false;
        }
        /*
        if (Placeable)
        {
            newTowerSR.color = new Color(0f, 1f, 0f, 0.745f);
        }
        else
        {
            newTowerSR.color = new Color(1f, 0f, 0f, 0.745f);
        }
        */

        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            if (SkipClickOnce)
            {
                SkipClickOnce = false;
            }
            else
            {
                newTowerSR.color = new Color(1f, 1f, 1f, 1f);
                newTowerSR.sortingLayerName = "Towers";
                newTower = null;
                newTowerSR = null;
                FollowPointer = false;
            }  
        }
    }
    public void UpdateSelectedTower()
    {
        FollowPointer = false;
        SkipClickOnce = true;

        Destroy(newTower);
        newTower = Instantiate(Global.SelectedTower, new Vector3(0, 0, 0), Quaternion.identity);

        newTowerSR = newTower.GetComponent<SpriteRenderer>();
        newTowerSR.color = new Color(1f, 1f, 1f, 0.745f);
        newTowerSR.sortingLayerName = "PlacedTowers";

        newTowerC2 = newTower.GetComponent<Collider2D>();

        // Set tower name and description in left bottom bar.
        string towerName = Global.SelectedTower.GetComponent<TowerObject>().TowerName;
        string towerDescription = Global.SelectedTower.GetComponent<TowerObject>().TowerDescription;

        TextToModifyIn.GetComponent<TextMeshProUGUI>().text = $"{towerName}\n{towerDescription}";

        FollowPointer = true;
    }
}
