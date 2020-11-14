using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] availableRooms;
    public List<GameObject> currentRooms;
    private float screenWidthInPoints;


    public GameObject[] availableObjects;
    public List<GameObject> objects;

    public float objectsMinDistance = 5f;
    public float objectsMaxDistance = 10f;

    public float objectsMinY = -1.7f;
    public float objectsMaxY = 1.7f;

    public float objectsMinRotation = -45f;
    public float objectsMaxRotation = 45f;
    void Start()
    {
        float height = 2f * Camera.main.orthographicSize;
        screenWidthInPoints = height * Camera.main.aspect;
        StartCoroutine(GeneratorCheck());
    }
    void AddObject(float lastObjectX)
    {
        int randomIndex = Random.Range(0, availableObjects.Length);
        GameObject obj = (GameObject)Instantiate(availableObjects[randomIndex]);
        float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
        float randomY = Random.Range(objectsMinY, objectsMaxY);
        obj.transform.position = new Vector3(objectPositionX, randomY, 0f);
        float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        objects.Add(obj);
    }
    void GenerateObjectsIfRequired()
    {
        float playerX = transform.position.x;
        float removeObjectX = playerX - screenWidthInPoints;
        float addObjectX = playerX + screenWidthInPoints;
        float farthestObjectX = 0;

        List<GameObject> objectsToRemove = new List<GameObject>();
        foreach(var obj in objects)
        {
            float objX = obj.transform.position.x;
            farthestObjectX = Mathf.Max(farthestObjectX, objX);
            if(objX < removeObjectX)
            {
                objectsToRemove.Add(obj);
            }
        }
        foreach (var obj in objectsToRemove)
        {
            objects.Remove(obj);
            Destroy(obj);
        }
        if(farthestObjectX < addObjectX)
        {
            AddObject(farthestObjectX);
        }
    }
    void AddRoom(float fartherstRoomEndX)
    {
        int randomRoomIndex = Random.Range(0 , availableRooms.Length);
        GameObject room = (GameObject)Instantiate(availableRooms[randomRoomIndex]);
        float roomWidth = room.transform.Find("floor").localScale.x;
        float roomCentre = fartherstRoomEndX + roomWidth * 0.5f;
        room.transform.position = new Vector3(roomCentre, 0, 0);
        currentRooms.Add(room);
    }
    void GenerateRoomIfRequired()
    {
        List<GameObject> roomsToRemove = new List<GameObject>();
        bool addRooms = true;
        float playerX = transform.position.x;
        float removeRoomX = playerX - screenWidthInPoints;
        float addRoomX = playerX + screenWidthInPoints;
        float farthestRoomEndX = 0;
        foreach(var room in currentRooms)
        {
            float roomWidth = room.transform.Find("floor").localScale.x;
            float roomStartX = room.transform.position.x - (roomWidth * 0.5f);
            float roomEndX = roomStartX + roomWidth;
            if(roomStartX > addRoomX)
            {
                addRooms = false;
            }
            if(roomEndX < removeRoomX)
            {
                roomsToRemove.Add(room);
            }
            farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX);
        }
        foreach(var room in roomsToRemove)
        {
            currentRooms.Remove(room);
            Destroy(room);
        }
        if(addRooms)
        {
            AddRoom(farthestRoomEndX);
        }
    }
    IEnumerator GeneratorCheck()
    {
        while(true)
        {
            GenerateRoomIfRequired();
            GenerateObjectsIfRequired();
            yield return new WaitForSeconds(0.25f);
        }
    }
}
