using System.Collections;
using System.Collections.Generic;
using Radishmouse;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_SkillLinks : MonoBehaviour
{
    [SerializeField] private GameObject parentSkill;
    [SerializeField] private List<GameObject> childrenSkills;
    [SerializeField] private float linksThickness;
    [SerializeField] private bool init;
    [SerializeField] private Color linksColor;

    private List<UILineRenderer> lineRenderer = new List<UILineRenderer>();

    private RectTransform parentRectTransform;
    private List<RectTransform> childrenRectTransforms = new List<RectTransform>();
    
    // Start is called before the first frame update
    void Start()
    {
        init = false;
        ResetLinks();
        DrawLinks();
        UpdateLinks();
    }

    [Button]
    private void DrawLinks()
    {
        for (int i = 0; i < childrenSkills.Count; i++)
        {
            var link = new GameObject().transform;
            link.transform.parent = gameObject.transform;
            link.name = $"Link_{i+1}";
            link.gameObject.AddComponent<CanvasRenderer>();
            lineRenderer.Add(link.gameObject.AddComponent<UILineRenderer>());
            lineRenderer[i].center = false;
            lineRenderer[i].color = linksColor;
        }

        parentRectTransform = parentSkill.GetComponent<RectTransform>();
        foreach (var childSkill in childrenSkills)
        {
            childrenRectTransforms.Add(childSkill.GetComponent<RectTransform>());
        }
        
        if (childrenSkills.Count>0 && parentSkill!=null)
            init = true;
    }
    
    private void UpdateLinks()
    {
        for (int i = 0; i < childrenSkills.Count; i++)
        {
            lineRenderer[i].points[0] = parentSkill.transform.position;
            lineRenderer[i].points[1] = childrenSkills[i].transform.position;
            lineRenderer[i].thickness = linksThickness;
        }
    }

    [Button]
    private void ResetLinksButton()
    {
        init = false;
        
        for (int i = 0; i < lineRenderer.Count; i++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        childrenRectTransforms.Clear();
        parentRectTransform = null;
        lineRenderer.Clear();
    }
 
    
    private void ResetLinks()
    {
        init = false;
        
        for (int i = 0; i < lineRenderer.Count; i++)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        
        childrenRectTransforms.Clear();
        parentRectTransform = null;
        lineRenderer.Clear();
    }
    
}
