using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tile tile_;
    [SerializeField] private Spaceship spaceship_;
    [SerializeField] private TradingPost tradingPost_;
    [SerializeField] private FallenStar fallenStar_;
    [SerializeField] private Starchaser starchaser_;
    [SerializeField] private Transform cam_;

    private int width, height;
    private List<Tile> unblockedTiles_;
    private List<Tile> blockedTiles_;
    private Tile[,] grid;
    private Tile spaceshipTile;
    private Tile tradingPostTile;
    private Tile fallenStarTile;
    private FallenStar fallenStarVar;

    private void Start()
    {
        width = 16;
        height = 9;
        unblockedTiles_ = new List<Tile>();
        blockedTiles_ = new List<Tile>();
        grid = new Tile[width, height];
        GenerateGrid();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(mousePos.x + 0.5f);
            int y = Mathf.FloorToInt(mousePos.y + 0.5f);
            if(grid[x, y].blocked)
            {
                grid[x, y].blocked = false;
                blockedTiles_.Remove(grid[x, y]);
                unblockedTiles_.Add(grid[x, y]);
            }
            else
            {
                grid[x, y].blocked = true;
                unblockedTiles_.Remove(grid[x, y]);
                blockedTiles_.Add(grid[x, y]);
            }
        }
    }

    private bool randomBool()
    {
        if (Random.value >= 0.8)
            return true;

        return false;
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tile_, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                if (randomBool())
                {
                    spawnedTile.Init(true, x, y);
                    blockedTiles_.Add(spawnedTile);
                }
                else
                {
                    spawnedTile.Init(false, x, y);
                    unblockedTiles_.Add(spawnedTile);
                }

                grid[x, y] = spawnedTile;
            }
        }

        spaceshipTile = unblockedTiles_[Random.Range(0, unblockedTiles_.Count)];
        unblockedTiles_.Remove(spaceshipTile);
        var spawnedSpaceship = Instantiate(spaceship_, new Vector3(spaceshipTile.transform.position.x, spaceshipTile.transform.position.y), Quaternion.identity);
        spawnedSpaceship.name = "Spaceship";
        spawnedSpaceship.Init(spaceshipTile.x, spaceshipTile.y);

        tradingPostTile = unblockedTiles_[Random.Range(0, unblockedTiles_.Count)];
        unblockedTiles_.Remove(tradingPostTile);
        var spawnedTradingPost = Instantiate(tradingPost_, new Vector3(tradingPostTile.transform.position.x, tradingPostTile.transform.position.y), Quaternion.identity);
        spawnedTradingPost.name = "TradingPost";
        spawnedTradingPost.Init(tradingPostTile.x, tradingPostTile.y);

        Tile starchaserTile = unblockedTiles_[Random.Range(0, unblockedTiles_.Count)];
        unblockedTiles_.Remove(starchaserTile);
        var spawnedStarchaser = Instantiate(starchaser_, new Vector3(starchaserTile.transform.position.x, starchaserTile.transform.position.y), Quaternion.identity);
        spawnedStarchaser.name = "Starchaser";
        spawnedStarchaser.Init(starchaserTile.x, starchaserTile.y);

        fallenStarTile = unblockedTiles_[Random.Range(0, unblockedTiles_.Count)];
        var spawnedFallenStar = Instantiate(fallenStar_, new Vector3(fallenStarTile.transform.position.x, fallenStarTile.transform.position.y), Quaternion.identity);
        spawnedFallenStar.name = "FallenStar";
        spawnedFallenStar.Init(fallenStarTile.x, fallenStarTile.y);
        fallenStarVar = spawnedFallenStar;

        cam_.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10f);
    }

    public void resetFallenStar()
    {
        fallenStarTile = unblockedTiles_[Random.Range(0, unblockedTiles_.Count)];
        fallenStarVar.transform.position = fallenStarTile.transform.position;
        fallenStarVar.Init(fallenStarTile.x, fallenStarTile.y);
    }

    public Tile GetTile(int x_, int y_)
    {
        return grid[x_, y_];
    }

    public Tile GetSpaceshipTile()
    {
        return spaceshipTile;
    }
    public Tile GetTradingPostTile()
    {
        return tradingPostTile;
    }
    public Tile GetFallenStarTile()
    {
        return fallenStarTile;
    }

    public void SetFallenStarTile(Tile tile)
    {
        fallenStarTile = tile;
    }    

    public FallenStar GetFallenStar()
    {
        return fallenStarVar;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}
