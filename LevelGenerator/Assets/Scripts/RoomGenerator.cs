using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{

    public class Room {
        public string split;
        private Vector2 pt1;
        private Vector2 pt2;

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
            float hSplit = Random.Range(parentPt2.y + minLength, parentPt1.y - minLength);

            // create children
            child1 = new Room(parentPt1.x, parentPt1.y, parentPt2.x, hSplit);
            child1.split = "horizontal";
            child2 = new Room(parentPt1.x, hSplit, parentPt2.x, parentPt2.y);
            child2.split = "horizontal";

        }

        //vertical split
        else{  

            float vSplit = Random.Range(parentPt1.x + minLength, parentPt2.x - minLength);

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
        rooms[index * 2] = child1;
        generateRooms(rooms, index*2);

        //right child
        rooms[index * 2 + 1] = child2;
        generateRooms(rooms, index*2 + 1 );



    }

        private bool isBottomNode(Room[] rooms, int index){
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
                if (isBottomNode(rooms, i) ){
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
            float x, y;
            Room smaller, bigger;
            if (split.Equals("horizontal")){
                y = (room2.getPt1().y + room1.getPt2().y) / 2;

                if (room1.getLength() < room2.getLength()){
                    bigger = room2;
                    smaller = room1;
                }
                else {
                    bigger = room1;
                    smaller = room2;
                    
                }

                x = Random.Range(smaller.getPt1().x  + corridorSize, smaller.getPt2().x - corridorSize);
        
            }
            else{
                x = (room2.getPt1().x + room1.getPt2().x) / 2;
                if (room1.getWidth() < room2.getWidth()){
                    bigger = room2;
                    smaller = room1;
                }
                else {
                    bigger = room1;
                    smaller = room2;
                    
                }

                y = Random.Range(smaller.getPt1().y  + corridorSize, smaller.getPt2().y - corridorSize);
            }

            GameObject go = Instantiate(corridorPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;          
        }


        public void connectRooms(List<int> finalRooms, Room[] rooms){
            // Connect sisters
            
            Room room1, room2;
            for(int i = 0; i  < finalRooms.Count && i <  maxRooms; i+=2){
                if (i + 1 == finalRooms.Count || i+1 >=  maxRooms )
                    break;

                room1 = rooms[finalRooms[i]];
                room2 = rooms[finalRooms[i+1]];

                addCorridor(room1, room2, room1.split); 
            }


        }

        void Start(){
            createRooms();
        }

}