using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class TowerPlaceSelectorScript : MonoBehaviour
{
    [HideInInspector] public TowerObject SelectedTower { get; private set; }
    [HideInInspector] public float TowerAngle;
    [HideInInspector] private GameObject LinePrefab;
    [HideInInspector] private (GameObject,GameObject) Lines;
    [HideInInspector] private (LineRenderer, LineRenderer) lineRenderers;

    void Start()
    {
        LinePrefab = UnityManager.GetPrefab("LaserRay");
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
        else // update lines
        {
            lineRenderers.Item1.SetPositions(new Vector3[] { Vector3.zero, Vector2.up.Rotate(((DefenceTower)SelectedTower).MaxTargetingAngle / 2) });
            lineRenderers.Item2.SetPositions(new Vector3[] { Vector3.zero, Vector2.up.Rotate(-((DefenceTower)SelectedTower).MaxTargetingAngle / 2) });
        }
    }

    public void UpdateTower(TowerObject? tower)
    {
        SelectedTower = tower;
    }

    public void UpdateSprite()
    {
        var hit = Physics2D.RaycastAll(gameObject.transform.position, Vector2.zero);

        Color green = Color.green;
        green.a = 0.5f;
        Color red = Color.red;
        red.a = 0.5f;
        // The colors alpha is adjusted to make it transparent

        if (hit.Any(x => x.collider.gameObject.tag == "Tower") || !hit.Any(x => x.collider.gameObject.name == "Placable Area"))
            gameObject.GetComponent<SpriteRenderer>().color = red;
        else
            gameObject.GetComponent<SpriteRenderer>().color = green;
    }

    public void Move(Vector3 newPos)
    {
        if (gameObject.transform.position == newPos)
            return;

        gameObject.transform.position = newPos;
        UpdateSprite();
    }

    void Update()
    {
        if (SelectedTower == null)
            return;

        if (SelectedTower is DefenceTower )
            CreateAngleLaser();
    }
}
