using UnityEngine;

public class FamilyTreeLink : MonoBehaviour
{
    [SerializeField] private new LineRenderer renderer;
    
    public void Initialize(Material material)
    {
        renderer.material = material;
    }

    public void SetPoints(Vector2 father, Vector2 mother, Vector2[] children)
    {
        Vector2 mid = (father + mother) / 2f;

        Vector3[] positions = new Vector3[2 * children.Length + 4]; //4 for mother and father.
        renderer.positionCount = positions.Length;
        
        positions[0] = father;
        positions[1] = mother;
        positions[2] = mid;

        if (children.Length > 0)
        {
            Vector2 cmid = (mid + new Vector2(mid.x, children[0].y)) / 2f;
            positions[3] = cmid;

            for (int i = 0; i < children.Length; i++)
            {
                positions[2 * i + 4] = children[i];
                positions[2 * i + 5] = cmid;
            }
        }

        
        
        renderer.SetPositions(positions);
    }
}
