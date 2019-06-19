using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMM.RDG
{
    // Separate rooms with 'end' statements
    public class RoomSet
    {
        private List<RoomData> mRooms = new List<RoomData>();
        private List<string> mAllRoomSets = new List<string>();

        public RoomSet()
        {

        }

        public void LoadFromResource(string resource)
        {
            TextAsset asset = Resources.Load<TextAsset>(resource);
            LoadFromStringBlob(asset.text);
        }

        public void LoadFromTextAsset(TextAsset asset)
        {
            LoadFromStringBlob(asset.text);
        }

        public void LoadFromStringBlob(string dataBlob)
        {
            mRooms.Clear();

            List<string> roomDataBlobs = new List<string>(dataBlob.Split(new string[] { "end" }, System.StringSplitOptions.RemoveEmptyEntries));
            roomDataBlobs.RemoveAll(i => (i == null || i.Trim().Length == 0));

            for (int i = 0; i < roomDataBlobs.Count; ++i)
            {
                RoomData room = new RoomData(roomDataBlobs[i]);
                mRooms.Add(room);

                for (int j = 0; j < room.categories.Length; ++j)
                {
                    string category = room.categories[j];
                    if (!mAllRoomSets.Contains(category))
                    {
                        mAllRoomSets.Add(category);
                    }
                }
            }

            GenerateRotations(roomDataBlobs);
        }

        private void GenerateRotations(List<string> roomDataBlobs)
        {
            List<RoomData> newRooms = new List<RoomData>();

            for (int i = 0; i < mRooms.Count; ++i)
            {
                if (mRooms[i].HasCategory("rotate_1"))
                {
                    RoomData rotatedRoom = new RoomData(roomDataBlobs[i]);
                    rotatedRoom.Rotate90();
                    newRooms.Add(rotatedRoom);
                }

                if (mRooms[i].HasCategory("rotate_4"))
                {
                    for (int copy = 0; copy < 3; ++copy)
                    {
                        RoomData rotatedRoom = new RoomData(roomDataBlobs[i]);

                        for (int rotation = 0; rotation <= copy; ++rotation)
                        {
                            rotatedRoom.Rotate90();
                        }

                        newRooms.Add(rotatedRoom);
                    }
                }

                bool flipAll = false;
                if (mRooms[i].HasCategory("flip_all"))
                {
                    flipAll = true;
                }

                if (mRooms[i].HasCategory("flip_hor") || flipAll)
                {
                    RoomData flippedRoom = new RoomData(roomDataBlobs[i]);
                    flippedRoom.FlipHorizontal();
                    newRooms.Add(flippedRoom);
                }

                if (mRooms[i].HasCategory("flip_ver") || flipAll)
                {
                    RoomData flippedRoom = new RoomData(roomDataBlobs[i]);
                    flippedRoom.FlipVertical();
                    newRooms.Add(flippedRoom);
                }

                if (mRooms[i].HasCategory("flip_both") || flipAll)
                {
                    RoomData flippedRoom = new RoomData(roomDataBlobs[i]);
                    flippedRoom.FlipVertical();
                    flippedRoom.FlipHorizontal();
                    newRooms.Add(flippedRoom);
                }
            }

            mRooms.AddRange(newRooms);
        }

        public RoomData RandomRoomOfCategory(string category, RoomData excludeRoom = null, System.Random rng = null)
        {
            List<RoomData> availableRooms = AllRoomsFromCategory(category);
            
            if (excludeRoom != null)
                availableRooms.Remove(excludeRoom);

            availableRooms.RemoveAll(i => (i.HasCategory("limit_1") && i.placeCount > 0));

            if (availableRooms.Count == 0)
                return null;

            float[] probabilities = new float[availableRooms.Count];
            probabilities[0] = availableRooms[0].probability;
            for (int i = 1; i < availableRooms.Count; ++i)
            {
                probabilities[i] = availableRooms[i].probability + probabilities[i - 1];
            }

            float value = rng == null ? Random.Range(0, probabilities[probabilities.Length - 1]) : (float)rng.NextDouble() * probabilities[probabilities.Length-1];
            for (int i = 0; i < availableRooms.Count; ++i)
            {
                if (value <= probabilities[i])
                {
                    return availableRooms[i];
                }
            }

            return availableRooms[availableRooms.Count - 1];
        }

        public List<RoomData> AllRoomsFromCategory(string category)
        {
            List<RoomData> roomsFromSet = new List<RoomData>();

            for (int i = 0; i < mRooms.Count; ++i)
            {
                if (mRooms[i].HasCategory(category))
                {
                    roomsFromSet.Add(mRooms[i]);
                }
            }

            return roomsFromSet;
        }
    }
}