using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{

    public class Room {
        public string split;
        private Vector2 pt1;
        private Vector2 pt2;
        private Room sister;
        private bool isConnected = false;
        private bool isInstantiated = false;

        public Room(float x1, float y1, float x2, float y2) {
            this.pt1 = new Vector2(x1, y1);
            this.pt2 = new Vector2(x2, y2);

        } 
        public float getArea() {
            return Mathf.Abs(pt1.x - pt2.x) * Mathf.Abs(pt1.y - pt2.y);
        }

        public Vector2 getPt1(){
            return pt1;
        }

        public Vector2 getPt2(){
            return pt2;

        }

        public float getLength() {
            return Mathf.Abs(pt2.x - pt1.x);

        }

        public float getWidth() {
            return Mathf.Abs(pt1.y - pt2.y);

        }

        public Room getSister() {
            return sister;
        }
        
        public void setSister(Room s) {
            this.sister = s;
        }


        public bool connectStatus() {
            return isConnected;
        }

        public void  setConnectStatus(bool status) {
            this.isConnected = status;
        }

        public Vector2 getCenter(){
            return new Vector2((getPt1().x + getPt2().x) / 2, (getPt1().y + getPt2().y) / 2);
        }

        public bool instantiationStatus() {
            return isInstantiated;
        }

        public void  setInstantStatus(bool status) {
            this.isInstantiated = status;
        }



    }

    [Range(1, 15)]
    public int maxRooms;
    public static int amountRooms = 0;

    [Range(50, 100)]
    public int minLength;
    [Range(50, 100)]
    public int minWidth;
    private int minArea;

    public GameObject roomPrefab;
    public GameObject corridorPrefab;

    public Room[] expandRoomsArr(Room[] rooms){
        Room[] newArr = new Room[rooms.Length* 2];
        for(int i = 0; i <  rooms.Length; i++){
            newArr[i] = rooms[i];
        }

        return newArr;

    }
    public Room[] generateRooms() {
        minArea = minLength * minWidth;
        
        Room[] r = new Room[256];
        r[1] = new Room(-300.0f, 300.0f, 300.0f, -300.0f);
        generateRooms(r, 1);
        return r;
    }

    private void generateRooms(Room[] rooms, int index) {
        Room parent = rooms[index];

        Vector2 parentPt1 = parent.getPt1();
        Vector2 parentPt2 = parent.getPt2();

        Room child1, child2;

        // Make sure that too-small children won't be made
        if ((parent.getLength() *  parent.getWidth()) / 2 < minArea) {
            return;
        }
        //horizontal split
        if (Random.Range(0.0f, 1.0f) < .5){
            float hSplit = Random.Range(parentPt2.y + minLength * 1.3f, parentPt1.y - minLength * 1.3f);

            // create children
            child1 = new Room(parentPt1.x, parentPt1.y, parentPt2.x, hSplit);
            child1.split = "horizontal";
            child2 = new Room(parentPt1.x, hSplit, parentPt2.x, parentPt2.y);
            child2.split = "horizontal";

        }

        //vertical split
        else{  

            float vSplit = Random.Range(parentPt1.x + minLength * 1.3f, parentPt2.x - minLength * 1.3f);

            // create children
            child1 = new Room(parentPt1.x, parentPt1.y, vSplit, parentPt2.y);
            child1.split = "vertical";
            child2 = new Room(vSplit, parentPt1.y, parentPt2.x, parentPt2.y);  
            child2.split = "vertical";
         
        }
        
        // add to tree and generate rooms on children

        if (index * 2 + 1 >= rooms.Length){
            rooms = expandRoomsArr(rooms);
        }

        // Preventing the creation of rooms with too small widths and lengths 
        if ((int) Mathf.Min(child2.getLength(), child1.getLength()) < minLength || ((int) Mathf.Min(child1.getWidth(), child2.getWidth()) < minWidth)) {
            return;
        }

        //left child
        child1.setSister(child2);

        rooms[index * 2] = child1;
        generateRooms(rooms, index*2);

        //right child
        child2.setSister(child1);
        rooms[index * 2 + 1] = child2;
        generateRooms(rooms, index*2 + 1 );



    }

        private bool isLeaf(Room[] rooms, int index){
            if (rooms[index] == null)
                return false;
            if (index*2 < rooms.Length){
                if (rooms[index*2] != null)
                    return false;
            }
            return true;

        }
        public List<int> getFinalRooms(Room[] rooms){
            List<int> leaves = new List<int>();
            int count =  0;

            for(int i = 0; i < rooms.Length; i++){
                if(rooms[i] != null)
                    count++;
                if (isLeaf(rooms, i) ){
                    leaves.Add(i);
                }
            }
            print("Rooms in tree: " + count);
            return leaves;
        }  


        public void placeRooms(List<int> leafIndexes, Room[] rooms){
            for(int i = 0; i <  maxRooms && i  < leafIndexes.Count; i++){
                
                Room r = rooms[leafIndexes[i]];
                float x =  (r.getPt1().x + r.getPt2().x) / 2;
                float y =  (r.getPt1().y + r.getPt2().y) / 2;
                r.setInstantStatus(true);

                GameObject go = Instantiate(roomPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                go.transform.localScale = new Vector3(r.getLength() * .9f, r.getWidth() *.9f, 1);
                
            }
        }

        public void createRooms(){
            Room[] rooms = generateRooms();
            print("Length of initial rooms array: " + rooms.Length);
            List<int> finalRooms = getFinalRooms(rooms);
            print("Number of bottom row nodes: " + finalRooms.Count);
            placeRooms(finalRooms, rooms);
            connectRooms(finalRooms, rooms);
        }

        public void addCorridor(Room room1, Room room2, string split){
            float corridorSize =  1/4f * Mathf.Min(minWidth, minLength);
            float x, y, corridorLength;
            Room smaller, bigger;

            if (!(room1.instantiationStatus()) || !(room2.instantiationStatus())) {
                return;
            }

            if (split.Equals("horizontal")){
                Room top, bottom;
                // Gets the room on top
                if (room1.getCenter().y > room2.getCenter().y) {
                    top = room1;
                    bottom = room2;
                } else {
                    top = room2;
                    bottom = room1;
                }

                corridorLength = (top.getCenter().y - bottom.getCenter().y) * 0.6f;

                // Get the room with the shorter length
                if (room1.getLength() < room2.getLength()) {
                    smaller = room1;
                    bigger = room2;
                } else {
                    smaller = room2;
                    bigger = room2;
                }

                // Generates an x value within the bounds of smaller room
                // for the corridor center point
                x = Random.Range(smaller.getPt1().x + corridorSize, smaller.getPt2().x - corridorSize);
                y = bottom.getPt1().y;

                // Create corridor GameObject and scale it to size
                GameObject go = Instantiate(corridorPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;          
                go.transform.localScale = new Vector3(corridorSize, corridorLength, 1);

            }
            else{
                Room left, right;
                // Gets the room on left and right
                if (room1.getCenter().x < room2.getCenter().x) {
                    left = room1;
                    right = room2;
                } else {
                    left = room2;
                    right = room1;
                }

                corridorLength = (right.getCenter().x - left.getCenter().x) * .6f;

                // Get the room with the shorter width
                if (room1.getWidth() < room2.getWidth()) {
                    smaller = room1;
                    bigger = room2;
                } else {
                    smaller = room2;
                    bigger = room2;
                }

                // Generates an x value within the bounds of smaller room
                // for the corridor center point
                y = Random.Range(smaller.getPt2().y + corridorSize, smaller.getPt1().y- corridorSize);
                x = left.getPt2().x;
                
                // Create corridor GameObject and scale it to size
                GameObject go = Instantiate(corridorPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;          
                go.transform.localScale = new Vector3(corridorLength, corridorSize, 1);

            }


        }


        public void connectRooms(List<int> finalRooms, Room[] rooms){
            // Connect sisters
            
            Room room1, room2;
            for(int i = 0; i < finalRooms.Count && i <  maxRooms - 1; i+=1){
                room1 = rooms[finalRooms[i]];

                if (room1.connectStatus() == true) {
                    continue;
                }

                room2 = room1.getSister();
                addCorridor(room1, room2, room1.split); 
                room1.setConnectStatus(true);
                room2.setConnectStatus(true);

            }


        }

        void Start(){
            createRooms();
        }

}