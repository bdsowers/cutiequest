using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMM.RDG
{
    public class RoomData
    {
        private string mName;
        private string[] mCategories;
        private float mProbability;
        private int mWidth;
        private int mHeight;
        private int[,] mTiles;
        private List<Vector2Int> mExits = new List<Vector2Int>();

        public string name { get { return mName; } }
        public string[] categories { get { return mCategories; } }
        public float probability { get { return mProbability; } set { mProbability = value; } }
        public int width { get { return mWidth; } }
        public int height { get { return mHeight; } }
        public List<Vector2Int> exits { get { return mExits; } }

        public RoomData(string dataBlob)
        {
            ParseRoomDataBlob(dataBlob);
        }

        public int Tile(int x, int y)
        {
            return mTiles[x, y];
        }

        public Vector2Int RandomExit(System.Random rng = null)
        {
            int exitIndex = rng == null ? Random.Range(0, exits.Count) : rng.Next(0, exits.Count);
            return exits[exitIndex];
        }

        public bool HasCategory(string category)
        {
            return System.Array.IndexOf(mCategories, category) != -1;
        }

        private void ParseRoomDataBlob(string dataBlob)
        {
            // Format:
            // name
            // category
            // probability
            // 11211
            // 20002
            // 10001
            // 11211

            // 0 => nothing
            // 1 => walkable
            // 2 => wall
            // 3 => exit point
            // Anything else => game-specific decoration
            List<string> lines = new List<string>(dataBlob.Split(new char[] { '\n' }));
            lines.RemoveAll(i => (i == null || i.Trim().Length == 0));

            mName = lines[0].Trim();
            mCategories = lines[1].Trim().Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < mCategories.Length; ++i)
            {
                mCategories[i] = mCategories[i].Trim();
            }

            mProbability = float.Parse(lines[2].Trim());

            mWidth = lines[3].Trim().Length;
            mHeight = lines.Count - 3;

            mTiles = new int[mWidth, mHeight];
            for (int y = 0; y < mHeight; ++y)
            {
                string line = lines[y + 3].Trim();
                for (int x = 0; x < mWidth; ++x)
                {
                    string tile = line.Substring(x, 1);
                    mTiles[x, y] = int.Parse(tile);
                }
            }

            FindExits();
        }

        private void FindExits()
        {
            mExits.Clear();
            for (int x = 0; x < mWidth; ++x)
            {
                for (int y = 0; y < mHeight; ++y)
                {
                    if (mTiles[x,y] == RandomDungeonTileData.EXIT_TILE)
                    {
                        mExits.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        public void FlipHorizontal()
        {
            for (int x = 0; x < mWidth / 2; ++x)
            {
                for (int y = 0; y < mHeight; ++y)
                {
                    int temp = mTiles[x, y];
                    mTiles[x, y] = mTiles[mWidth - x - 1, y];
                    mTiles[mWidth - x - 1, y] = temp;
                }
            }
            FindExits();
        }

        public void FlipVertical()
        {
            for (int x = 0; x < mWidth; ++x)
            {
                for (int y = 0; y < mHeight / 2; ++y)
                {
                    int temp = mTiles[x, y];
                    mTiles[x, y] = mTiles[x, mHeight - y - 1];
                    mTiles[x, mHeight - y - 1] = temp;
                }
            }
            FindExits();
        }

        public void Rotate90()
        {
            int targetWidth = mHeight;
            int targetHeight = mWidth;

            int[,] newTileData = new int[mHeight, mWidth];

            for (int sourceY = 0; sourceY < mHeight; ++sourceY)
            {
                int targetX = targetWidth - 1 - sourceY;

                for (int sourceX = 0; sourceX < mWidth; ++sourceX)
                {
                    int targetY = sourceX;

                    newTileData[targetX, targetY] = mTiles[sourceX, sourceY];
                }
            }

            mTiles = newTileData;
            mWidth = targetWidth;
            mHeight = targetHeight;

            FindExits();
        }
    }

}