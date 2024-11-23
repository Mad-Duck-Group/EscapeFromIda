using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Searching
{
    public enum BlockTypes
    {
        Invalid = -1,
        Empty = 0,
        DemonWall = 1,
        Potion = 2,
        BonusPotion = 3,
        Exit = 4,
        Key = 5,
        Enemy = 6,
        FireStorm = 7,
        WallTile = 8,
        BarbWallTile = 9,
        IceTile = 10,
        PlayerBlock = 99
    }
    [Flags]
    public enum TerrainTypes
    {
        Invalid = 0,
        Terrain1 = 1 << 0,
        Terrain2 = 1 << 1
    }
    public class OOPMapGenerator : MonoSingleton<OOPMapGenerator>
    {
        [Header("Set MapGenerator")]
        [SerializeField] private bool randomMap = true;
        [SerializeField, ShowIf("randomMap"), MinMaxSlider(1, float.MaxValue)] private Vector2 randomMapWidthRange;
        [SerializeField, ShowIf("randomMap"), MinMaxSlider(1, float.MaxValue)] private Vector2 randomMapHeightRange;
        [SerializeField, HideIf("randomMap"), MinValue(2)] private int width = 2;
        [SerializeField, HideIf("randomMap"), MinValue(2)] private int height = 2;

        [Header("Terrain")]
        [SerializeField] private bool randomTerrain = true;
        [SerializeField, ShowIf("randomTerrain"), MinMaxSlider(1, float.MaxValue)] private Vector2 randomTerrainRange;
        [SerializeField, HideIf("randomTerrain")] private bool byColumn = true;
        [SerializeField, HideIf("randomTerrain")] private int columnSplit;
        [SerializeField, HideIf("randomTerrain")] private int rowSplit;

        [Header("Set Player")]
        [SerializeField] private OOPPlayer player;
        public OOPPlayer Player => player;
        [SerializeField] private Vector2Int playerStartPos;
        
        [Header("Set Exit")]
        [SerializeField] private OOPExit exit;
        public OOPExit Exit => exit;
        
        [FormerlySerializedAs("floorsPrefab")]
        [Header("Set Prefab")]
        [SerializeField] private GameObject[] floorsPrefabTerrain1;
        [SerializeField] private GameObject[] floorsPrefabTerrain2;
        [SerializeField] private GameObject[] wallsPrefab;
        [SerializeField] private GameObject[] demonWallsPrefab;
        [SerializeField] private GameObject[] enemiesPrefab;
        [SerializeField] private ItemGenerationData potionData;
        [SerializeField] private ItemGenerationData keyData;
        [SerializeField] private ItemGenerationData fireStormData;
        [SerializeField] private TileGenerationData wallTileData;
        [SerializeField] private TileGenerationData barbWallTileData;
        [SerializeField] private TileGenerationData iceTileData;

        [Header("Set Transform")]
        [SerializeField] private Transform floorParent;
        [SerializeField] private Transform wallParent;
        [FormerlySerializedAs("itemPotionParent")] [SerializeField] private Transform itemParent;
        [SerializeField] private Transform enemyParent;

        [FormerlySerializedAs("obstacleCount")]
        [FormerlySerializedAs("obsatcleCount")]
        [Header("Set object Count")]
        [SerializeField] private int demonWallCount;
        [SerializeField] private int enemyCount;

        private BlockTypes[,] mapdata;
        public BlockTypes[,] MapData => mapdata;
        public BlockTypes[,] entityData;
        public BlockTypes[,] EntityData => entityData;
        private TerrainTypes[,] terrains;
        public TerrainTypes[,] Terrains => terrains;

        private OOPDemonWall[,] demonWalls;
        public OOPDemonWall[,] DemonWalls => demonWalls;
        private OOPEnemy[,] enemies;
        public OOPEnemy[,] Enemies => enemies;
        private Tile[,] tiles;
        public Tile[,] Tiles => tiles;
        private Item[,] items;
        public Item[,] Items => items;
        
        [Serializable]
        private class TileGenerationData
        {
            public Tile tile;
            public int count;
        }
        
        [Serializable]
        private class ItemGenerationData
        {
            public Item item;
            public int count;
        }

        private Dictionary<BlockTypes, TileGenerationData> tileDictionary;
        private Dictionary<BlockTypes, ItemGenerationData> itemDictionary;

        // Start is called before the first frame update
        void Start()
        {
            if (randomMap)
            {
                width = (int)Random.Range(randomMapWidthRange.x, randomMapWidthRange.y);
                height = (int)Random.Range(randomMapHeightRange.x, randomMapHeightRange.y);
            }
            if (randomTerrain)
            {
                byColumn = Random.Range(0, 100) < 50;
                if (byColumn)
                {
                    columnSplit = (int)Random.Range(randomTerrainRange.x, randomTerrainRange.y);
                    columnSplit = Mathf.Clamp(columnSplit, 1, width - 1);
                }
                else
                {
                    rowSplit = (int)Random.Range(randomTerrainRange.x, randomTerrainRange.y);
                    rowSplit = Mathf.Clamp(rowSplit, 1, height - 1);
                }
            }
            mapdata = new BlockTypes[width, height];
            entityData = new BlockTypes[width, height];
            tiles = new Tile[width, height];
            items = new Item[width, height];
            enemies = new OOPEnemy[width, height];
            demonWalls = new OOPDemonWall[width, height];
            terrains = new TerrainTypes[width, height];
            tileDictionary = new Dictionary<BlockTypes, TileGenerationData>
            {
                {BlockTypes.WallTile, wallTileData},
                {BlockTypes.BarbWallTile, barbWallTileData},
                {BlockTypes.IceTile, iceTileData}
            };
            itemDictionary = new Dictionary<BlockTypes, ItemGenerationData>
            {
                {BlockTypes.Potion, potionData},
                {BlockTypes.Key, keyData},
                {BlockTypes.FireStorm, fireStormData}
            };
            GenerateBoundaries();
            InitializePlayer();
            GenerateEnemy();
            GenerateDemonWall();
            GenerateTile(tileDictionary);
            GenerateItem(itemDictionary);
            InitializeExit();
        }

        private void InitializeExit()
        {
            mapdata[width - 1, height - 1] = BlockTypes.Exit;
            exit.transform.position = new Vector3(width - 1, height - 1, 0);
        }

        private void GenerateTile(Dictionary<BlockTypes, TileGenerationData> tileDictionary)
        {
            foreach (var tileData in tileDictionary)
            {
                GenerateTile(tileData.Key, tileData.Value);
            }
        }
        private void GenerateTile(BlockTypes type, TileGenerationData data)
        {
            var loop = 0;
            // List<Vector2Int> availablePos = new List<Vector2Int>();
            // availablePos = mapdata.Cast<BlockTypes>()
            //     .Select((block, index) => new {block, index})
            //     .Where(x => x.block == BlockTypes.Empty && terrains[x.index % width, x.index / width] == data.tile.TerrainType)
            //     .Select(x => new Vector2Int(x.index % width, x.index / width))
            //     .ToList();
            while (loop < data.count)
            {
                if (GetEmptyCount() < data.count - loop) break;
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);
                if (mapdata[x, y] == BlockTypes.Empty && TerrainMatch(terrains[x, y], data.tile.TerrainType) && entityData[x, y] == BlockTypes.Empty)
                {
                    PlaceTile(x, y, type);
                    loop++;
                }
            }
        }
        
        private bool TerrainMatch(TerrainTypes a, TerrainTypes b)
        {
            return (a & b) != 0;
        }
        
        private void GenerateItem(Dictionary<BlockTypes, ItemGenerationData> itemDictionary)
        {
            foreach (var itemData in itemDictionary)
            {
                GenerateItem(itemData.Key, itemData.Value.count);
            }
        }
        
        private void GenerateItem(BlockTypes type, int count)
        {
            var loop = 0;
            while (loop < count)
            {
                if (GetEmptyCount() < count - loop) break;
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);
                if (mapdata[x, y] == BlockTypes.Empty && entityData[x, y] == BlockTypes.Empty)
                {
                    PlaceItem(x, y, type);
                    loop++;
                }
            }
        }

        private void GenerateEnemy()
        {
            var count = 0;
            while (count < enemyCount)
            {
                if (GetEmptyCount() < enemyCount - count) break;
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);
                if (mapdata[x, y] == BlockTypes.Empty)
                {
                    PlaceEnemy(x, y);
                    count++;
                }
            }
        }
        private void GenerateDemonWall()
        {
            int count = 0;
            while (count < demonWallCount)
            {
                if (GetEmptyCount() < demonWallCount - count) break;
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);
                if (mapdata[x, y] == BlockTypes.Empty && entityData[x, y] == BlockTypes.Empty)
                {
                    PlaceDemonWall(x, y);
                    count++;
                }
            }
        }

        private void InitializePlayer()
        {
            player.positionX = playerStartPos.x;
            player.positionY = playerStartPos.y;
            player.transform.position = new Vector3(playerStartPos.x, playerStartPos.y, -0.1f);
            entityData[playerStartPos.x, playerStartPos.y] = BlockTypes.PlayerBlock;
        }

        private void GenerateBoundaries()
        {
            for (int x = -1; x < width + 1; x++)
            {
                for (int y = -1; y < height + 1; y++)
                {
                    if (x == -1 || x == width || y == -1 || y == height)
                    {
                        int r = Random.Range(0, wallsPrefab.Length);
                        GameObject obj = Instantiate(wallsPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
                        obj.transform.parent = wallParent;
                        obj.name = "Wall_" + x + ", " + y;
                    }
                    else
                    {
                        GenerateTerrain(x, y);
                    }
                }
            }
        }

        private void GenerateTerrain(int x, int y)
        {
            //Generate Terrain 1
            if (byColumn && x <= columnSplit)
            {
                int r = Random.Range(0, floorsPrefabTerrain1.Length);
                GameObject obj = Instantiate(floorsPrefabTerrain1[r], new Vector3(x, y, 1), Quaternion.identity);
                obj.transform.parent = floorParent;
                obj.name = "floor_" + x + ", " + y;
                mapdata[x, y] = BlockTypes.Empty;
                terrains[x, y] = TerrainTypes.Terrain1;
            }
            else if (!byColumn && y <= rowSplit)
            {
                int r = Random.Range(0, floorsPrefabTerrain1.Length);
                GameObject obj = Instantiate(floorsPrefabTerrain1[r], new Vector3(x, y, 1), Quaternion.identity);
                obj.transform.parent = floorParent;
                obj.name = "floor_" + x + ", " + y;
                mapdata[x, y] = BlockTypes.Empty;
                terrains[x, y] = TerrainTypes.Terrain1;
            }
            else
            {
                int r = Random.Range(0, floorsPrefabTerrain2.Length);
                GameObject obj = Instantiate(floorsPrefabTerrain2[r], new Vector3(x, y, 1), Quaternion.identity);
                obj.transform.parent = floorParent;
                obj.name = "floor_" + x + ", " + y;
                mapdata[x, y] = BlockTypes.Empty;
                terrains[x, y] = TerrainTypes.Terrain2;
            }
        }

        public BlockTypes GetMapData(int x, int y)
        {
            if (x >= width || x < 0 || y >= height || y < 0) return BlockTypes.Invalid;
            return mapdata[x, y];
        }
        
        public BlockTypes GetEntityData(int x, int y)
        {
            if (x >= width || x < 0 || y >= height || y < 0) return BlockTypes.Invalid;
            return entityData[x, y];
        }
        
        public Tile GetTile(int x, int y)
        {
            if (x >= width || x < 0 || y >= height || y < 0) return null;
            return tiles[x, y];
        }
        
        public TerrainTypes GetTerrain(int x, int y)
        {
            if (x >= width || x < 0 || y >= height || y < 0) return TerrainTypes.Invalid;
            return terrains[x, y];
        }

        public int GetEmptyCount()
        {
            return mapdata.Cast<BlockTypes>().Count(block => block == BlockTypes.Empty);
        }

        public void PlaceItem(int x, int y, BlockTypes type)
        {
            if (!itemDictionary.ContainsKey(type)) return;
            Item item = Instantiate(itemDictionary[type].item, new Vector3(x, y, 0), Quaternion.identity);
            item.transform.SetParent(itemParent);
            mapdata[x, y] = type;
            items[x, y] = item;
            items[x, y].positionX = x;
            items[x, y].positionY = y;
            item.name = $"Item_{items[x, y].Name} {x}, {y}";
        }
        
        public void PlaceEnemy(int x, int y)
        {
            int r = Random.Range(0, enemiesPrefab.Length);
            GameObject obj = Instantiate(enemiesPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.SetParent(enemyParent);
            entityData[x, y] = BlockTypes.Enemy;
            enemies[x, y] = obj.GetComponent<OOPEnemy>();
            enemies[x, y].positionX = x;
            enemies[x, y].positionY = y;
            obj.name = $"Enemy_{enemies[x, y].Name} {x}, {y}";
        }

        public void PlaceDemonWall(int x, int y)
        {
            int r = Random.Range(0, demonWallsPrefab.Length);
            GameObject obj = Instantiate(demonWallsPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.SetParent(wallParent);
            mapdata[x, y] = BlockTypes.DemonWall;
            demonWalls[x, y] = obj.GetComponent<OOPDemonWall>();
            demonWalls[x, y].positionX = x;
            demonWalls[x, y].positionY = y;
            obj.name = $"DemonWall_{demonWalls[x, y].Name} {x}, {y}";
        }

        private void PlaceTile(int x, int y, BlockTypes type)
        {
            if (!tileDictionary.ContainsKey(type)) return;
            var tile = Instantiate(tileDictionary[type].tile, new Vector3(x, y, 0), Quaternion.identity);
            tile.transform.SetParent(floorParent);
            mapdata[x, y] = type;
            tiles[x, y] = tile;
            tile.PosX = x;
            tile.PosY = y;
        }

        public OOPEnemy[] GetEnemies()
        {
            List<OOPEnemy> list = new List<OOPEnemy>();
            foreach (var enemy in enemies)
            {
                if (enemy)
                {
                    list.Add(enemy);
                }
            }
            return list.ToArray();
        }

        public void MoveEnemies()
        {
            List<OOPEnemy> list = new List<OOPEnemy>();
            foreach (var enemy in enemies)
            {
                if (enemy)
                {
                    list.Add(enemy);
                }
            }
            foreach (var enemy in list)
            {
                enemy.RandomMove();
            }
        }
    }
}