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
        Vector2 mid = (father + mother) / 2;

        Vector3[] positions = new Vector3[2 * children.Length + 3]; //3 for mother and father.
        renderer.positionCount = positions.Length;
        
        positions[0] = father;
        positions[1] = mother;
        positions[2] = mid;

        for (int i = 0; i < children.Length; i++)
        {
            positions[2 * i + 3] = children[i];
            positions[2 * i + 4] = mid;
        }
        
        renderer.SetPositions(positions);
    }
}
