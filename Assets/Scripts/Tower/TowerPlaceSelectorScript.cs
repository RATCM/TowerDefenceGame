using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerPlaceSelectorScript : MonoBehaviour
{
    [HideInInspector] public TowerObject SelectedTower;
    [HideInInspector] public float TowerAngle;
    [HideInInspector] private GameObject LinePrefab;
    [HideInInspector] private (GameObject,GameObject) Lines;
    [HideInInspector] private List<GameObject> TowerPrefabs;
    [HideInInspector] private int prefabIndex = 0;

    [HideInInspector] private (LineRenderer, LineRenderer) lineRenderers;
    void Start()
    {
        LinePrefab = UnityManager.GetPrefab("LaserRay");
        TowerPrefabs = UnityManager.GetAllPrefabsOfTag("Tower").ToList();
        SelectedTower = TowerPrefabs[0].GetComponent<TowerObject>();
    }
    void CreateAngleLaser()
    {
        var angle = ((DefenceTower)SelectedTower).MaxTargetingAngle;
        if(Lines.Item1 == null) // instantiate lines
        {
            Lines.Item1 = Instantiate(LinePrefab, transform);
            Lines.Item2 = Instantiate(LinePrefab, transform);

            lineRenderers.Item1 = Lines.Item1.GetComponent<LineRenderer>();
            lineRenderers.Item2 = Lines.Item2.GetComponent<LineRenderer>();

            lineRenderers.Item1.SetPositions(new Vector3[] { Vector3.zero, Vector2.up.Rotate(((DefenceTower)SelectedTower).MaxTargetingAngle/2) });
            lineRenderers.Item2.SetPositions(new Vector3[] { Vector3.zero, Vector2.up.Rotate(-((DefenceTower)SelectedTower).MaxTargetingAngle/2) });
        }
    }
    public void SetNextTower()
    {
        if (prefabIndex == TowerPrefabs.Count-1)
            prefabIndex = 0;
        else
            prefabIndex++;
        UpdateTower();
    }

    public void SetPreviousTower()
    {
        if (prefabIndex == 0)
            prefabIndex = TowerPrefabs.Count-1;
        else
            prefabIndex--;
        UpdateTower();
    }

    void UpdateTower()
    {
        SelectedTower = TowerPrefabs[prefabIndex].GetComponent<TowerObject>();

        lineRenderers.Item1.SetPositions(new Vector3[] { Vector3.zero, Vector2.up.Rotate(((DefenceTower)SelectedTower).MaxTargetingAngle/2)});
        lineRenderers.Item2.SetPositions(new Vector3[] { Vector3.zero, Vector2.up.Rotate(-((DefenceTower)SelectedTower).MaxTargetingAngle/2)});
    }

    void Update()
    {
        if (SelectedTower == null)
            return;

        if (SelectedTower is DefenceTower && Lines.Item1 == null)
            CreateAngleLaser();
    }
}