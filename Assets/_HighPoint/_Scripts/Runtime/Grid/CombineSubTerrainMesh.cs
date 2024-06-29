using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombineSubTerrainMesh : MonoBehaviour
{
    [SerializeField] Material _mat;

    EventBinding<GameStateChangedEvent> GameStateChanged;

    const int CHUNK_SIZE = 800;

    void OnEnable()
    {
        // GameStateChanged = new EventBinding<GameStateChangedEvent>(HandleGameStateChanged);
        // Bus<GameStateChangedEvent>.Register(GameStateChanged);

        HexGrid.OnTerrainFinalized += CombineSubTerrainMeshes;
    }

    void OnDisable()
    {
        // Bus<GameStateChangedEvent>.Unregister(GameStateChanged);

        HexGrid.OnTerrainFinalized -= CombineSubTerrainMeshes;
    }

    // void HandleGameStateChanged(GameStateChangedEvent @event)
    // {
    //     switch (@event.State)
    //     {
    //         case GameState.:
    //             CombineSubTerrainMeshes();
    //             break;
    //     }
    // }

    void CombineSubTerrainMeshes()
    {
        //Find all the dots GameObjects
        var allSubTerrain = GameObject.FindGameObjectsWithTag("SubTerrain").ToList();

        var chunks = ListExtensions.ChunkBy(allSubTerrain, CHUNK_SIZE);

        for (int i = 0; i < chunks.Count; i++)
        {
            var terrainInChunk = chunks[i];

            GameObject mesh = new("SubTerrain Chunk " + i);
            mesh.transform.parent = transform;
            mesh.AddComponent<MeshFilter>();
            mesh.AddComponent<MeshRenderer>();

            CombineMesh(terrainInChunk, mesh);
        }
    }

    void CombineMesh(List<GameObject> meshes, GameObject output)
    {
        //Create CombineInstance from the amount of dots
        CombineInstance[] cInstance = new CombineInstance[meshes.Count];

        //Initialize CombineInstance from MeshFilter of each dot
        for (int i = 0; i < meshes.Count; i++)
        {
            //Get current Mesh Filter and initialize each CombineInstance 
            MeshFilter cFilter = meshes[i].GetComponent<MeshFilter>();

            //Get each Mesh and position
            cInstance[i].mesh = cFilter.sharedMesh;
            cInstance[i].transform = cFilter.transform.localToWorldMatrix;
            //Hide each MeshFilter or Destroy the GameObject 
            cFilter.gameObject.SetActive(false);
        }

        // Create new GameObject that will contain the new combined Mesh

        MeshRenderer mr = output.GetComponent<MeshRenderer>();
        mr.material = _mat;
        MeshFilter mf = output.GetComponent<MeshFilter>();

        //Create new Mesh then combine it
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(cInstance);
    }
}
