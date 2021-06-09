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

   [SerializeField]
    private Room start;   
    [Range(1, 15)]
    private static int maxRooms;
    [Range(50, 100)]
    private static int minArea;

    public static void generateRooms() {
        Room[] r = new Room[maxRooms*2 + 1];
        r[1] = new Room(-50.0f, 50.0f, 50.0f, -50.0f);
        generateRooms(r, 1);
    }

    private static void generateRooms(Room[] rooms, int index) {

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
            generateRooms(rooms, index*2);
            //right child
            rooms[index * 2 + 1] = child2;
            generateRooms(rooms, index*2 + 1);


        }

    }


}


