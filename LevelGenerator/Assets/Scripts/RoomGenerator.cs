using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{

    public class Room {
        private Vector2 pt1;
        private Vector2 pt2;

        private bool hasChildren = false;

        public Room(float x1, float y1, float x2, float y2) {
            this.pt1 = new Vector2(x1, y1);
            this.pt2 = new Vector2(x2, y2);

        } 
        public float getMinLength() {
            return Mathf.Min(Mathf.Abs(pt1.x - pt2.x), Mathf.Abs(pt1.y - pt2.y));
        }

        public Vector2 getPt1(){
            return pt1;
        }

        public Vector2 getPt2(){
            return pt2;

        }

        public bool getHasChildren() {
            return hasChildren;

        }

        public void setHasChildren(bool b) {
            hasChildren = b;
        }

    }

    [Range(1, 15)]
    public int maxRooms;

    [Range(100, 300)]
    public int minRoomLength;

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

        // Checking to see if room too small to make children
        if (parent.getMinLength() / 2 < minRoomLength) {
            return; 
        }

        //horizontal split
        if (Random.value < .5){
            float hSplit = Random.Range (parentPt1.y + minRoomLength, parentPt1.y - minRoomLength);

            // create children
            child1 = new Room(parentPt1.x, parentPt1.y, parentPt2.x, hSplit);
            child2 = new Room(parentPt1.x, hSplit, parentPt2.x, parentPt2.y);

        }

        //vertical split
        else{  
            float vSplit = Random.Range (parentPt1.x + minRoomLength, parentPt1.x - minRoomLength);
            // create children
            child1 = new Room(parentPt1.x, parentPt1.y, vSplit, parentPt2.y);
            child2 = new Room(vSplit, parentPt1.y, parentPt2.x, parentPt2.y);           
        }

        // add to tree and generate rooms on children

        //left child
        rooms[index].setHasChildren(true);
        rooms[index * 2] = child1;
        generateRooms(rooms, index*2, numRooms + 1);
        //right child
        rooms[index * 2 + 1] = child2;
        generateRooms(rooms, index*2 + 1, numRooms +1 );


        

    }

        private bool isLeaf(Room[] rooms, int index){
            if (rooms[index] == null) {
                return false;
            }
            return rooms[index].getHasChildren();

        }
        public List<Room> getFinalRooms(Room[] rooms){
            List<Room> leaves = new List<Room>();
            int count =  0;

            for(int i = 0; i < rooms.Length; i++){
                if(rooms[i] != null)
                    count++;
                if (isLeaf(rooms, i) ){
                    leaves.Add(rooms[i]);
                }
            }
            print("Rooms in tree: " + count);
            return leaves;
        }  


        public void placeRooms(List<Room> rooms){

            for(int i = 0; i <  rooms.Count; i ++){
                for (int x  = (int) rooms[i].getPt1().x; x <= rooms[i].getPt2().x; x++) {
                    //for (int y =  (int) rooms[i].getPt1().y; y >= rooms[i].getPt2().y; y--) {
                    print("test");
                    Instantiate(room, new Vector3(x, y, 0), Quaternion.identity);
                    }
                }

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


