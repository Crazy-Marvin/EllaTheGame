using UnityEngine;

namespace MonKey.Extensions
{
    public class MonKeyDebugMeshDrawer : MonoBehaviour
    {
        public Color OutlineColor;
        public Color MeshColor;

        public GameObject reference;

        private Renderer[] renderers;
        private MeshFilter[] filters;

        private void OnDrawGizmos()
        {
            if (!reference)
                return;

            if (renderers == null)
            {
                renderers = reference.GetComponentsInChildren<Renderer>();
            }

            if (filters == null)
            {
                filters = reference.GetComponentsInChildren<MeshFilter>();
            }

            if (renderers.Length == 0)
                return;

            for (int i = 0; i < renderers.Length; i++)
            {
                Bounds bounds = new Bounds(reference.transform.position, Vector3.zero);
                bounds.Encapsulate(renderers[i].bounds);
                Vector3 offsetForScale = (bounds.center - renderers[i].transform.position)
                                         - (bounds.center - renderers[i].transform.position) * 1.05f;
                Gizmos.color = OutlineColor;

                if (filters.Length <= i)
                    continue;

                Gizmos.DrawMesh(filters[i].sharedMesh, filters[i].transform.position + offsetForScale,
                    filters[i].transform.rotation, filters[i].transform.localScale * 1.05f);
                Gizmos.color = MeshColor;

                Gizmos.DrawMesh(filters[i].sharedMesh, filters[i].transform.position,
                    filters[i].transform.rotation, filters[i].transform.localScale);
            }
        }
    }
}