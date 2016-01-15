using UnityEngine;
using System.Collections;

public class CombineMeshesAndTextures : MonoBehaviour
{

    public GameObject[] objectsToCombine; // The objects to combine, each should have a mesh filter and renderer with a single material.
    public bool useMipMaps = true;
    public TextureFormat textureFormat = TextureFormat.RGB24;

    void Start()
    {
        Combine();
    }

    /*
     * Combines all object textures into a single texture then creates a material used by all objects.
     * The materials properties are based on those of the material of the object at position[0].
     *
     * Also combines any meshes marked as static into a single mesh.
     */
    private void Combine()
    {

        int size;
        int originalSize;
        int pow2;
        Texture2D combinedTexture;
        Material material;
        Texture2D texture;
        Mesh mesh;
        Hashtable textureAtlas = new Hashtable();

        if (objectsToCombine.Length > 1)
        {
            originalSize = objectsToCombine[0].GetComponent<Renderer>().material.mainTexture.width;
            pow2 = GetTextureSize(objectsToCombine);
            size = pow2 * originalSize;
            combinedTexture = new Texture2D(size, size, textureFormat, useMipMaps);

            // Create the combined texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                texture = (Texture2D)objectsToCombine[i].GetComponent<Renderer>().material.mainTexture;
                if (!textureAtlas.ContainsKey(texture))
                {
                    combinedTexture.SetPixels((i % pow2) * originalSize, (i / pow2) * originalSize, originalSize, originalSize, texture.GetPixels());
                    textureAtlas.Add(texture, new Vector2(i % pow2, i / pow2));
                }
            }
            combinedTexture.Apply();
            material = new Material(objectsToCombine[0].GetComponent<Renderer>().material);
            material.mainTexture = combinedTexture;

            // Update texture co-ords for each mesh (this will only work for meshes with coords betwen 0 and 1).
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 offset;
                if (textureAtlas.ContainsKey(objectsToCombine[i].GetComponent<Renderer>().material.mainTexture))
                {
                    offset = (Vector2)textureAtlas[objectsToCombine[i].GetComponent<Renderer>().material.mainTexture];
                    for (int u = 0; u < mesh.uv.Length; u++)
                    {
                        uv[u] = mesh.uv[u] / (float)pow2;
                        uv[u].x += ((float)offset.x) / (float)pow2;
                        uv[u].y += ((float)offset.y) / (float)pow2;
                    }
                }
                else
                {
                    // This happens if you use the same object more than once, don't do it :)
                }

                mesh.uv = uv;
                objectsToCombine[i].GetComponent<Renderer>().material = material;
            }

            // Combine each mesh marked as static
            int staticCount = 0;
            CombineInstance[] combine = new CombineInstance[objectsToCombine.Length];
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                if (objectsToCombine[i].isStatic)
                {
                    staticCount++;
                    combine[i].mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                    combine[i].transform = objectsToCombine[i].transform.localToWorldMatrix;
                }
            }

            // Create a mesh filter and renderer
            if (staticCount > 1)
            {
                MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
                filter.mesh = new Mesh();
                filter.mesh.CombineMeshes(combine);
                renderer.material = material;

                // Disable all the static object renderers
                for (int i = 0; i < objectsToCombine.Length; i++)
                {
                    if (objectsToCombine[i].isStatic)
                    {
                        objectsToCombine[i].GetComponent<MeshFilter>().mesh = null;
                        objectsToCombine[i].GetComponent<Renderer>().material = null;
                        objectsToCombine[i].GetComponent<Renderer>().enabled = false;
                    }
                }
            }

            Resources.UnloadUnusedAssets();
        }
    }

    private int GetTextureSize(GameObject[] o)
    {
        ArrayList textures = new ArrayList();
        // Find unique textures
        for (int i = 0; i < o.Length; i++)
        {
            if (!textures.Contains(o[i].GetComponent<Renderer>().material.mainTexture))
            {
                textures.Add(o[i].GetComponent<Renderer>().material.mainTexture);
            }
        }
        if (textures.Count == 1) return 1;
        if (textures.Count < 5) return 2;
        if (textures.Count < 17) return 4;
        if (textures.Count < 65) return 8;
        // Doesn't handle more than 64 different textures but I think you can see how to extend
        return 0;
    }
}