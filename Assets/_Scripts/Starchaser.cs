using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starchaser : MonoBehaviour
{
    public enum State
    {
        findFallenStar,
        travelFallenStar,
        findTradingPost,
        travelTradingPost,
        findSpaceship,
        travelSpaceship
    }

    private GridManager gridManager_;
    private State state;
    private List<Tile> openList;
    private List<Tile> closedList;
    private List<Tile> tilePath;

    private float maxStamina;
    private float stamina;
    private float moveSpeed;

    private int pathIndex;

    public int x;
    public int y;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    // Start is called before the first frame update
    public void Init(int x_, int y_)
    {
        maxStamina = 100f;
        stamina = maxStamina;
        state = State.findFallenStar;
        gridManager_ = GameObject.Find("gridManager").GetComponent<GridManager>();
        x = x_;
        y = y_;
        tilePath = new List<Tile>();
        pathIndex = 0;
        moveSpeed = 5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case State.findFallenStar:
                resetPath();
                tilePath = findPath(x, y, gridManager_.GetFallenStarTile().x, gridManager_.GetFallenStarTile().y);
                state = State.travelFallenStar;
                break;

            case State.travelFallenStar:
                Movement();
                if (pathIndex >= tilePath.Count)
                    state = State.findTradingPost;
                break;

            case State.findTradingPost:
                resetPath();
                tilePath = findPath(x, y, gridManager_.GetTradingPostTile().x, gridManager_.GetTradingPostTile().y);
                state = State.travelTradingPost;
                break;

            case State.travelTradingPost:
                Movement();
                carryFallenStar();
                stamina -= 2f;
                if (pathIndex >= tilePath.Count)
                {
                    gridManager_.resetFallenStar();
                    state = State.findSpaceship;
                }
                if (stamina < 0f)
                {
                    dropFallenStar();
                    state = State.findSpaceship;
                }
                break;

            case State.findSpaceship:
                resetPath();
                tilePath = findPath(x, y, gridManager_.GetSpaceshipTile().x, gridManager_.GetSpaceshipTile().y);
                state = State.travelSpaceship;
                break;

            case State.travelSpaceship:
                Movement();
                if (pathIndex >= tilePath.Count)
                {
                    state = State.findFallenStar;
                    stamina = maxStamina;
                }
                break;
        }
    }

    private void carryFallenStar()
    {
        gridManager_.GetFallenStar().transform.position = transform.position;
    }

    private void dropFallenStar()
    {
        gridManager_.SetFallenStarTile(tilePath[pathIndex - 1]);
        gridManager_.GetFallenStar().transform.position = tilePath[pathIndex - 1].transform.position;
    }

    private void Movement()
    {
        if (tilePath == null)
            return;
        else
        {
            if (pathIndex < tilePath.Count)
            {
                float step = moveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, tilePath[pathIndex].transform.position, step);

                if (Vector3.Distance(transform.position, tilePath[pathIndex].transform.position) < 0.01f)
                {
                    transform.position = tilePath[pathIndex].transform.position;
                    x = tilePath[pathIndex].x;
                    y = tilePath[pathIndex].y;
                    pathIndex++;
                }
            }
        }
    }

    private void resetPath()
    {
        tilePath = null;
        pathIndex = 0;
    }

    private List<Tile> findPath(int startX, int startY, int endX, int endY)
    {
        Tile startNode = gridManager_.GetTile(startX, startY);
        Tile endNode = gridManager_.GetTile(endX, endY);

        openList = new List<Tile> { startNode };
        closedList = new List<Tile>();

        for (int x = 0; x < gridManager_.GetWidth(); x++)
        {
            for (int y = 0; y < gridManager_.GetHeight(); y++)
            {
                Tile pathNode = gridManager_.GetTile(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalcFCost();
                pathNode.cameFromTile = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalcDistCost(startNode, endNode);
        startNode.CalcFCost();

        while (openList.Count > 0)
        {
            Tile currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalcPath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Tile neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                    continue;

                if (neighbourNode.blocked)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalcDistCost(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromTile = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalcDistCost(neighbourNode, endNode);
                    neighbourNode.CalcFCost();

                    if(!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        return null;
    }

    private List<Tile> GetNeighbourList(Tile currentNode)
    {
        List<Tile> neighbourList = new List<Tile>();

        if(currentNode.x - 1 >= 0)
        {
            neighbourList.Add(gridManager_.GetTile(currentNode.x - 1, currentNode.y));

            if (currentNode.y - 1 >= 0)
                neighbourList.Add(gridManager_.GetTile(currentNode.x - 1, currentNode.y - 1));

            if (currentNode.y + 1 < gridManager_.GetHeight())
                neighbourList.Add(gridManager_.GetTile(currentNode.x - 1, currentNode.y + 1));
        }

        if (currentNode.x + 1 < gridManager_.GetWidth())
        {
            neighbourList.Add(gridManager_.GetTile(currentNode.x + 1, currentNode.y));

            if (currentNode.y - 1 >= 0)
                neighbourList.Add(gridManager_.GetTile(currentNode.x + 1, currentNode.y - 1));

            if (currentNode.y + 1 < gridManager_.GetHeight())
                neighbourList.Add(gridManager_.GetTile(currentNode.x + 1, currentNode.y + 1));
        }

        if (currentNode.y - 1 >= 0)
            neighbourList.Add(gridManager_.GetTile(currentNode.x, currentNode.y - 1));

        if  (currentNode.y + 1 < gridManager_.GetHeight())
            neighbourList.Add(gridManager_.GetTile(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    private List<Tile> CalcPath(Tile endNode)
    {
        List<Tile> path = new List<Tile>();
        path.Add(endNode);
        Tile currentNode = endNode;
        while(currentNode.cameFromTile != null)
        {
            path.Add(currentNode.cameFromTile);
            currentNode = currentNode.cameFromTile;
        }
        path.Reverse();
        return path;
    }

    private int CalcDistCost(Tile a, Tile b)
    {
        int xDist = Mathf.Abs(a.x - b.x);
        int yDist = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDist - yDist);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDist, yDist) + MOVE_STRAIGHT_COST * remaining;
    }

    private Tile GetLowestFCostNode(List<Tile> pathNodeList)
    {
        Tile lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = pathNodeList[i];
        }
        return lowestFCostNode;
    }
}
