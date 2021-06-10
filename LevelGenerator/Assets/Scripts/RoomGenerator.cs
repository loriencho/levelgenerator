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

    }

    [Range(1, 15)]
    public int maxRooms;

    [Range(50, 100)]
    public int minArea;

    public GameObject room;

    public Room[] generateRooms() {
        
        Room[] r = new Room[maxRooms*100 + 1];
        r[1] = new Room(-300.0f, 300.0f, 300.0f, -300.0f);
        generateRooms(r, 1, 0);
        return r;
    }

    private void generateRooms(Room[] rooms, int index, int numRooms) {
        if (numRooms >= maxRooms)
            return;

        Room parent = rooms[index];

        Vector2 parentPt1 = parent.getPt1();
        Vector2 parentPt2 = parent.getPt2();

        Room child1, child2;

        //horizontal split
        if (Random.value < .5){
            float hSplit = Mathf.Abs(parentPt1.y - parentPt2.y) / 2;

            // create children
            child1 = new Room(parentPt1.x, parentPt1.y, parentPt2.x, hSplit);
            child2 = new Room(parentPt1.x, hSplit, parentPt2.x, parentPt2.y);

        }

        //vertical split
        else{  
            float vSplit = Mathf.Abs(parentPt1.x - parentPt2.x) / 2;
            // create children
            child1 = new Room(parentPt1.x, parentPt1.y, vSplit, parentPt2.y);
            child2 = new Room(vSplit, parentPt1.y, parentPt2.x, parentPt2.y);           
        }
        
        // check area of children
        if (child1.getArea() < minArea){
            return;
        }

        else{
            // add to tree and generate rooms on children

            //left child
            rooms[index * 2] = child1;
            generateRooms(rooms, index*2, numRooms + 1);
            //right child
            rooms[index * 2 + 1] = child2;
            generateRooms(rooms, index*2 + 1, numRooms +1 );


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
            for(int i = 0; i <  rooms.Count; i ++){
                
                float x =  (rooms[i].getPt1().x + rooms[i].getPt2().x) / 2;
                float y =  (rooms[i].getPt1().y + rooms[i].getPt2().y) / 2;

                Instantiate(room, new Vector3(x, y, 0), Quaternion.identity);

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


