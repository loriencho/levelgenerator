using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{

    public class Room {
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

    [Range(100, 500)]
    public int minArea;

    public GameObject roomPrefab;

    public Room[] expandRoomsArr(Room[] rooms){
        Room[] newArr = new Room[rooms.Length* 2];
        for(int i = 0; i <  rooms.Length; i++){
            newArr[i] = rooms[i];
        }

        return newArr;

    }
    public Room[] generateRooms() {
        
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

        //horizontal split
        if (Random.Range(0.0f, 1.0f) < .5){
            float hSplit = (parentPt1.y + parentPt2.y) / 2;

            // create children
            child1 = new Room(parentPt1.x, parentPt1.y, parentPt2.x, hSplit);
            child2 = new Room(parentPt1.x, hSplit, parentPt2.x, parentPt2.y);

        }

        //vertical split
        else{  

            float vSplit = (parentPt1.x + parentPt2.x) / 2;
            // create children
            child1 = new Room(parentPt1.x, parentPt1.y, vSplit, parentPt2.y);
            child2 = new Room(vSplit, parentPt1.y, parentPt2.x, parentPt2.y);           
        }
        
        // check area of children
        if (child1.getArea() < minArea){
            print("Area too small");
            print(child1.getArea());
            print(child2.getArea());
            print(minArea);

            return;
        }

        else{
            // add to tree and generate rooms on children
            //left child

            if (index * 2 + 1 >= rooms.Length){
                rooms = expandRoomsArr(rooms);
            }

            rooms[index * 2] = child1;
            generateRooms(rooms, index*2);


            //right child
            rooms[index * 2 + 1] = child2;
            generateRooms(rooms, index*2 + 1 );



        }

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
        public List<Room> getFinalRooms(Room[] rooms){
            List<Room> bottomNodes = new List<Room>();
            int count =  0;

            for(int i = 0; i < rooms.Length; i++){
                if(rooms[i] != null)
                    count++;
                if (isBottomNode(rooms, i) ){
                    bottomNodes.Add(rooms[i]);
                }
            }
            print("Rooms in tree: " + count);
            return bottomNodes;
        }  


        public void placeRooms(List<Room> rooms){
            for(int count = 0; count<  maxRooms; count++){
                
                if(rooms.Count <= 0)
                    break;
                
                int i = Random.Range(0, rooms.Count-1);

                print("Tried to index at: " + i);
                print("Room count: " + rooms.Count);

                float x =  (rooms[i].getPt1().x + rooms[i].getPt2().x) / 2;
                float y =  (rooms[i].getPt1().y + rooms[i].getPt2().y) / 2;

                GameObject go = Instantiate(roomPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                go.transform.localScale = new Vector3(rooms[i].getLength(), rooms[i].getWidth(), 1);
                rooms.RemoveAt(i);


            }

            print(400);


        }


        public void createRooms(){
            Room[] rooms = generateRooms();
            print("Length of initial rooms array: " + rooms.Length);
            List<Room> finalRooms = getFinalRooms(rooms);
            print("Number of bottom row nodes: " + finalRooms.Count);

            placeRooms(finalRooms);
        }


        void Start(){
            createRooms();
        }

}