using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

public class CubeGrid : MonoBehaviour
{
    public int GridDimX = 15;
    public int GridDimY = 10;
    public float GridSpacing = 1.2f;
    public GameObject CubePrefab;

    public bool CalculatedCluster;
    [SerializeField] private Color[] m_ColorChoices;
    private List<ClickableCube> m_Cubes;

    //Using a 2D array to represent the grid
    ClickableCube[,] m_Grid;
    bool[,] m_VisitedCells;

    public List<CubeCluster> m_CubeClusters;
    public List<CubeCluster> m_FloatingClusters;
    private ClickableCube m_StartingCube;


    void Start()
    {
        
        //Create a 2D array to hold the cubes, then generate the cubes in it
        m_Grid = new ClickableCube[GridDimX, GridDimY];
        m_Cubes = new List<ClickableCube>();
        //Create a grid of visited cells
        m_VisitedCells = new bool[GridDimX, GridDimY];

        GenerateCubes();

        m_CubeClusters = new List<CubeCluster>();
        m_FloatingClusters= new List<CubeCluster>();
        CalculatedCluster = false;
    }

    void Update()
    {
        if (m_CubeClusters.Count > 0)
        {
            foreach (var cluster in m_CubeClusters)
            {
                if (cluster.GetNumOfCubes() > 2)
                {
                    cluster.DeactivateCube();
                }
            }
            FindFloatingClusters();
        }
        
        if (m_FloatingClusters.Count > 1)
        {
            for (int i = 1; i < m_FloatingClusters.Count; i++)
            {
                m_FloatingClusters[i].DeactivateCube();
                FindFloatingClusters();
            }
        }
    }

    public void RecalcuateClusters(ClickableCube cube)
    {
        //Clear all clusters
        m_CubeClusters.Clear();

        //Clear the visited cells so we can start a new search
        ClearVisitedCells();

        //This tracks which coordinates we need to visit
        List<IntVector2> coordsToVisit = new List<IntVector2>(); 

        IntVector2 startCoords = cube.m_Index;

        //Create a new cluster
        CubeCluster cubeCluster = new CubeCluster();
        m_CubeClusters.Add(cubeCluster);
        
        //Start searching over this cluster
        coordsToVisit.Add(startCoords);
        
        while (coordsToVisit.Count > 0)
        {
            //This section gets the next coordinates to process and removes it from the list
            //
            //Note: by changing this section we can change the type of search we are doing.
            //Currently we are treating the list like a stack.  This will make the search happen in 
            //a depth-first fashion.  If we changed it to act like a queue, the search would turn
            //into a breadth-first search.  Finally, if we changed it to act like a priority queue, it
            //would turn into a heuristic search.
            int indexToRemove = coordsToVisit.Count - 1;
            IntVector2 currentCoords = coordsToVisit[indexToRemove];
            coordsToVisit.RemoveAt(indexToRemove);
        
            //Ignore these coords if we've already visited them
            if (m_VisitedCells[currentCoords.x, currentCoords.y])
            {
                continue;
            }
        
            //Track this coordinate as visited
            m_VisitedCells[currentCoords.x, currentCoords.y] = true;
        
            //Check if the cube here is activated.  If it isn't we can ignore it and move on to
            //this next one.
            ClickableCube currentCube = m_Grid[currentCoords.x, currentCoords.y];
            
            if (!currentCube.Activated)
            {
                continue;
            }
        
            if (currentCube.GetColor() == cube.GetColor())
            {
                cubeCluster.AddCube(currentCube);
            }
        
            //Add next coordinates to visit.  We are using a helper function to determine whether to 
            //visit the coords, and calling it once for each direction that should be visited.
            AddCoordsIfNeeded(currentCoords, new IntVector2(1, 0), ref coordsToVisit, cube); //right
            AddCoordsIfNeeded(currentCoords, new IntVector2(-1, 0), ref coordsToVisit, cube); //left
            AddCoordsIfNeeded(currentCoords, new IntVector2(0, 1), ref coordsToVisit, cube); //up 
            AddCoordsIfNeeded(currentCoords, new IntVector2(0, -1), ref coordsToVisit, cube); //down
        }
    }
    public void FindFloatingClusters()
    {
        //Clear all clusters
        m_FloatingClusters.Clear();

        //Clear the visited cells so we can start a new search
        ClearVisitedCells();

        //This tracks which coordinates we need to visit
        List<IntVector2> coordsToVisit = new List<IntVector2>();

        IntVector2 startCoords;

        while (FindNonVisitedCoord(out startCoords))
        {
            //Create a new cluster
            CubeCluster cubeCluster = new CubeCluster();

            m_FloatingClusters.Add(cubeCluster);

            //Start searching over this cluster
            coordsToVisit.Add(startCoords);

            while (coordsToVisit.Count > 0)
            {
                //This section gets the next coordinates to process and removes it from the list
                //
                //Note: by changing this section we can change the type of search we are doing.
                //Currently we are treating the list like a stack.  This will make the search happen in 
                //a depth-first fashion.  If we changed it to act like a queue, the search would turn
                //into a breadth-first search.  Finally, if we changed it to act like a priority queue, it
                //would turn into a heuristic search.
                int indexToRemove = coordsToVisit.Count - 1;
                IntVector2 currentCoords = coordsToVisit[indexToRemove];
                coordsToVisit.RemoveAt(indexToRemove);

                //Ignore these coords if we've already visited them
                if (m_VisitedCells[currentCoords.x, currentCoords.y])
                {
                    continue;
                }

                //Track this coordinate as visited
                m_VisitedCells[currentCoords.x, currentCoords.y] = true;

                //Check if the cube here is activated.  If it isn't we can ignore it and move on to
                //this next one.
                ClickableCube currentCube = m_Grid[currentCoords.x, currentCoords.y];
                if (!currentCube.Activated)
                {
                    continue;
                }

                //Add the cube to the cluster
                cubeCluster.AddCube(currentCube);

                //Add next coordinates to visit.  We are using a helper function to determine whether to 
                //visit the coords, and calling it once for each direction that should be visited.
                AddCoordsIfNeeded(currentCoords, new IntVector2(1, 0), ref coordsToVisit); //right
                AddCoordsIfNeeded(currentCoords, new IntVector2(-1, 0), ref coordsToVisit); //left
                AddCoordsIfNeeded(currentCoords, new IntVector2(0, 1), ref coordsToVisit); //up 
                AddCoordsIfNeeded(currentCoords, new IntVector2(0, -1), ref coordsToVisit); //down
            }
        }
    }

    //A helper function to add new coordinates to check in our search.
    //It will first create the new coords, then double check if the coordinates are valid before adding 
    //them to the list
    void AddCoordsIfNeeded(IntVector2 coords, IntVector2 checkDir, ref List<IntVector2> coordsToVisit, ClickableCube cube)
    {
        IntVector2 nextCoords = coords + checkDir;

        if (AreCoordsValid(nextCoords))
        {
            if(IsCubeSameColor(nextCoords, cube))
                coordsToVisit.Add(nextCoords);
        }
    }

    private void AddCoordsIfNeeded(IntVector2 coords, IntVector2 checkDir, ref List<IntVector2> coordsToVisit)
    {
        IntVector2 nextCoords = coords + checkDir;

        if (AreCoordsValid(nextCoords))
        {
            coordsToVisit.Add(nextCoords);
        }
    }

    //This is a helper function to check if a set of coordinates are valid.  (out of bounds)
    bool AreCoordsValid(IntVector2 coords)
    {
        return coords.x >= 0 && coords.y >= 0 &&
            coords.x < m_Grid.GetLength(0) && coords.y < m_Grid.GetLength(1);
    }

    //Sets all of the visited cells back to non-visited
    void ClearVisitedCells()
    {
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = 0; y < GridDimY; ++y)
            {
                m_VisitedCells[x, y] = false;
            }
        }
    }

    bool FindNonVisitedCoord(out IntVector2 nonVisitedCoord)
    {
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = 0; y < GridDimY; ++y)
            {
                if (m_Grid[x, y].Activated && !m_VisitedCells[x, y])
                {
                    nonVisitedCoord = new IntVector2(x, y);

                    return true;
                }
            }
        }

        //No non-visited activated coords found.  Set the value to an invalid coordinate
        //and return false
        nonVisitedCoord = new IntVector2(-1, -1);

        return false;
    }

    bool IsCubeSameColor(IntVector2 nextCubeCoord, ClickableCube cube)
    {
        if (m_Grid[nextCubeCoord.x, nextCubeCoord.y].GetColor() == cube.GetColor())
        {
            return true;
        }
        return false;
    }
    
    //Creates the cubes in the right position and puts them in the grid    
    private void GenerateCubes()
    {
        //Starts TOP LEFT instead of BOTTOM LEFT
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = GridDimY - 1; y >= 0; --y)
            {
                Vector3 offset = new Vector3(x * GridSpacing, y * GridSpacing, 0.0f);

                GameObject cubeObj = (GameObject)GameObject.Instantiate(CubePrefab);

                m_Cubes.Add(cubeObj.GetComponent<ClickableCube>());

                cubeObj.transform.position = offset + transform.position;

                cubeObj.transform.parent = transform;

                m_Grid[x, y] = cubeObj.GetComponent<ClickableCube>();

                m_Grid[x, y].m_Index = new IntVector2(x, y);

                int random = Random.Range(0, m_ColorChoices.Length);
                cubeObj.GetComponent<ClickableCube>().SetColor(m_ColorChoices[random]);

                if(y <= 9) cubeObj.SetActive(false);

                DebugUtils.Assert(m_Grid[x, y] != null, "Could not find clickableCube component.");
            }
        }
    }

    public void EnableCube(ClickableCube cubeHit, Color color)
    {
        ClickableCube targetCube = GetCubeToActivate(cubeHit.m_Index, new IntVector2(0, -1)); //down
        if (targetCube == null) targetCube = GetCubeToActivate(cubeHit.m_Index, new IntVector2(1, 0)); //right
        if (targetCube == null) targetCube = GetCubeToActivate(cubeHit.m_Index, new IntVector2(-1, 0)); //left

        if (targetCube)
        {
            targetCube.SetColor(color);
            targetCube.Activated = true;
            targetCube.gameObject.SetActive(true);
            RecalcuateClusters(targetCube);
        }
    }
    
    ClickableCube GetCubeToActivate(IntVector2 coords, IntVector2 checkDir)
    {
        IntVector2 nextCoords = coords + checkDir;
        if (AreCoordsValid(nextCoords))
        {
            if (!m_Grid[nextCoords.x, nextCoords.y].Activated)
            {
                var possibleCube = m_Grid[nextCoords.x, nextCoords.y];
                return possibleCube;
            }
        }
        return null;
    }

    public Color GetClusterColor()
    {
        int random = Random.Range(0, m_ColorChoices.Length);
        return m_ColorChoices[random];
    }
}
