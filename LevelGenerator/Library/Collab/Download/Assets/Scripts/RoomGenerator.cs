using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    
    [SerializeField]
    private Room start;   
    [Range(1, 15)]
    private int maxRooms;
    [Range(50, 100)]
    private int area;

    public class Room {
        private Vector2 pt1;
        private Vector pt2;

        public Room(float x1, float y1, float x2, float y2) {
            this.pt1 = new Vector2(x1, y1);
            this.pt2 = new Vector2(x2, y2);

        } 
        public getArea() {
            return Math.abs(pt1.x - pt2.x) * Math.abs(pt1.y - pt2.y);
        }


    }


    public static generateRooms() {
        Room[] r = new Room[maxRooms*2 + 1]];
        r[1] = new Room()
        
    }

    private generateRooms(Room[] rooms) {





    }


}


