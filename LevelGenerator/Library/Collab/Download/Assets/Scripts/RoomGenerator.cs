using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


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

    [Range(1, 1000)]
    public int maxRooms;
    public static int amountRooms = 0;

    [Range(50, 100)]
    public int minLength;
    [Range(50, 100)]
    public int minWidth;
    private int minArea;

    public GameObject roomPrefab;
    public GameObject corridorPrefab;
    public Transform dungeon;

    public Tilemap roomMap;
    public Tile corridorTile, floorTile;


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
            if (rooms[index] == null){
                return false;

            }
            if (index*2 < rooms.Length){

                if (rooms[index*2] != null)
                    return false;
            }
            return true;

        }


        public List<int> getLeaves(Room[] rooms){
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
                float cx =  (r.getPt1().x + r.getPt2().x) / 2;
                float cy =  (r.getPt1().y + r.getPt2().y) / 2;
                float halfLength =  r.getLength() / 2;
                float halfWidth = r.getWidth() / 2;
                r.setInstantStatus(true);

                GameObject go = Instantiate(roomPrefab, new Vector3(cx, cy, 0), Quaternion.identity, dungeon) as GameObject;
                go.transform.localScale = new Vector3(r.getLength() * .9f, r.getWidth() *.9f, 1);
                

            }
        }

        public void addCorridor(Room room1, Room room2){
            float corridorSize =  1/3f * Mathf.Min(minWidth, minLength);
            float x, y, corridorLength, corridorWidth;

            Vector2 room1Pt1 = room1.getPt1();
            Vector2 room1Pt2 = room1.getPt2();
            Vector2 room2Pt1 = room2.getPt1();
            Vector2 room2Pt2 = room2.getPt2();


            if (!(room1.instantiationStatus()) || !(room2.instantiationStatus())) {
                return;
            }

            // Determine to make corridor straight or bent

            bool roomOneWithinRoomTwoVBounds = (room1Pt1.y < room2Pt1.y) && (room1Pt2.y > room2Pt2.y);
            bool roomTwoWithinRoomTOneVBounds = (room2Pt1.y < room1Pt1.y) && (room2Pt2.y > room1Pt2.y);
            bool roomOneWithinRoomTwoHBounds = (room1Pt1.x > room2Pt1.x) && (room1Pt2.x < room2Pt2.x);
            bool roomTwoWithinRoomOneHBounds = (room2Pt1.x > room1Pt1.x) && (room2Pt2.x < room1Pt2.x);

            // If vertically straight
            if (roomOneWithinRoomTwoVBounds || roomTwoWithinRoomTOneVBounds) {
                // Generate x value from within contained room
                x = Random.Range(Mathf.Max(room1Pt1.x, room2Pt1.x) + corridorSize, Mathf.Min(room1Pt2.x, room2Pt2.x) - corridorSize);
                y = (room1.getCenter().y + room2.getCenter().y) / 2;

                // Distance between centers is width
                corridorWidth = Mathf.Abs(room1.getCenter().y - room2.getCenter().y); 
                
                GameObject go = Instantiate(corridorPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;          
                go.transform.localScale = new Vector3(corridorSize, 100, 1);
                return;
            }

            // If horizontally straight 
            else if (roomOneWithinRoomTwoHBounds || roomOneWithinRoomTwoHBounds) {
                // Generate y value from within contained room
                y = Random.Range(Mathf.Max(room1Pt2.y, room2Pt2.y) + corridorSize, Mathf.Min(room1Pt1.y, room2Pt1.y) - corridorSize);
                x = (room1.getCenter().x + room2.getCenter().x) / 2;

                // Distance between centers is length
                corridorLength = Mathf.Abs(room1.getCenter().x - room2.getCenter().x); 

                GameObject go = Instantiate(corridorPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;          
                go.transform.localScale = new Vector3(100, corridorSize, 1);
                return;
            }

            else {
                Room roomA, roomB;
                if (Random.value < .5) {
                    roomA = room1;
                    roomB = room2;
                } else {
                    roomA = room2;
                    roomB = room1;
                }

                Vector2 roomAPt1 = roomA.getPt1();
                Vector2 roomAPt2 = roomA.getPt2();
                Vector2 roomBPt1 = roomB.getPt1();
                Vector2 roomBPt2 = roomB.getPt2();

                // Generate corner x from room A
                x = Random.Range(roomAPt1.x + corridorSize, roomAPt2.x - corridorSize);
                // Generate corner y from room B
                y = Random.Range(roomBPt2.y + corridorSize, roomAPt1.y - corridorSize);

                // Length of horizontal corridor is x distance from room B
                corridorWidth = Mathf.Abs(roomA.getCenter().y - y);

                // Width of vertical corridor is y distance from room A
                corridorLength = Mathf.Abs(roomB.getCenter().x - x);


                // Center of horizontal corridor: (Average of xc and xb , by)
                Vector3 hCor = new Vector3((x + roomB.getCenter().x) / 2, roomB.getCenter().y, 1.0f);
                // Center of vertical corridor: (xa, Average of yc and ya)
                Vector3 vCor  = new Vector3(roomA.getCenter().x, (y+ roomA.getCenter().y) / 2, 1.0f);

                
                GameObject hc = Instantiate(corridorPrefab, hCor, Quaternion.identity) as GameObject;          
                hc.transform.localScale = new Vector3(corridorLength, corridorSize, 1);
                GameObject vc = Instantiate(corridorPrefab, vCor, Quaternion.identity) as GameObject;          
                vc.transform.localScale = new Vector3(corridorSize, corridorWidth, 1);
                
                return;

            }



        }

        public void connectSisters(List<int> leaves, Room[] rooms){
            
            Room room1, room2;
            for(int i = 0; i < leaves.Count && i <  maxRooms - 1; i+=1){
                room1 = rooms[leaves[i]];

                if (room1.connectStatus() == true) {
                    continue;
                }

                room2 = room1.getSister();
                addCorridor(room1, room2); 
                room1.setConnectStatus(true);
                room2.setConnectStatus(true);
            }
            
            // while parent index is not 0 or less than 0, conenct parents


        }
        
        public int getParent(int childIndex){
            if (childIndex % 2 == 0) // left child
                return childIndex / 2;
            return (childIndex - 1) / 2; //right child

        }

        public int getSister(int roomIndex){
            if(roomIndex % 2 == 0)
                return roomIndex + 1;
            else
                return roomIndex - 1;

        }
        
        // returns indexes of all children
        public List<int> getAllLeafChildren(int currentIndex, Room[] rooms){
            List<int> leafChildren = new List<int>();

            if (rooms[currentIndex] == null){
                return leafChildren;
            }

            int leftChild = currentIndex * 2; 
            int rightChild  =currentIndex * 2 + 1;

            if (isLeaf(rooms, leftChild)) {
                if (rooms[leftChild].instantiationStatus())
                    leafChildren.Add(leftChild);
            } else {
                leafChildren.AddRange(getAllLeafChildren(leftChild, rooms));
            }

            if (isLeaf(rooms, rightChild)) {
                if (rooms[rightChild].instantiationStatus()) {
                    leafChildren.Add(rightChild);            
                } 
            } else {
                leafChildren.AddRange(getAllLeafChildren(rightChild, rooms));
            }

            return leafChildren;

        }


        public void connectParents(int childIndex, Room[] rooms){
            Room parent1, parent2; 
            if (childIndex == 2 || childIndex == 3) // at root 
            {    
                print("Reached root");

                parent1= rooms[childIndex];
                parent2 = parent1.getSister();
            }

            else {
                parent1 = rooms[getParent(childIndex)];

                if (parent1 == null){
                    print("Parent1 == null. Parent1 is " + getParent(childIndex));
                    print("Parent1 == null. Child is " + childIndex);

                }
                if (parent1.connectStatus())
                    return;
                
                // Connect parent with its sister!
                parent2 = rooms[getSister(getParent(childIndex))];
                if (parent2 == null) { 
                    print("The parent index is " + getParent(childIndex));
                    print("The child index is " + childIndex);

                    print("Null at " + getSister(getParent(childIndex)));
                    return;  
                    }

            }


            string split = parent1.split;
            List<int> parent1Children = getAllLeafChildren(getParent(childIndex), rooms);
            List<int> parent2Children = getAllLeafChildren(getSister(getParent(childIndex)), rooms);

            bool parent1NoChildren = (parent1Children.Count == 0);
            bool parent2NoChildren = (parent2Children.Count == 0);
            int currentParent = childIndex;

            if (parent1NoChildren && parent2NoChildren)
                return;
            else if (parent1NoChildren) {
                while(parent1NoChildren) {
                    print("Going up");
                    currentParent = getParent(currentParent);
                    parent1Children = getAllLeafChildren(currentParent, rooms);
                    parent1NoChildren = (parent1Children.Count == 0);
                }
            }
            else if (parent2NoChildren){
                while(parent2NoChildren) {
                    print("Going up");

                    currentParent = getParent(currentParent);
                    parent2Children = getAllLeafChildren(currentParent, rooms);
                    parent2NoChildren = (parent2Children.Count == 0);
                }
            }
            
            // for(int i = 0; i < parent1Children.Count; i++){
            //     for(int j = 0; j < parent2Children.Count; j++){
            //         if(canConnect(rooms[parent1Children[i]], rooms[parent2Children[j]], split)){
            //             addCorridor(rooms[parent1Children[i]], rooms[parent2Children[j]], split);
                    
            //             // break out of both for loops
            //             i = parent1Children.Count;
            //             break;
            //         } else{

            //             if(i== parent1Children.Count-1 && j== parent2Children.Count-1)
            //                 print("Could not count");

            //         }
            //     }

            // }

            parent1.setConnectStatus(true);
            parent2.setConnectStatus(true);
            
            if (childIndex == 2 || childIndex == 3){
                return;
            }
            print("This is the childIndex, " + childIndex + ", this is hte parent index, " + getParent(childIndex));
            connectParents(getParent(childIndex), rooms);

        }

        public void createRooms(){
            Room[] rooms = generateRooms();
            print("Length of initial rooms array: " + rooms.Length);
            List<int> leaves = getLeaves(rooms);
            print("Number of bottom row nodes: " + leaves.Count);
            placeRooms(leaves, rooms);
            connectSisters(leaves, rooms);
            // for (int i =0; i < leaves.Count && i <maxRooms; i++){
            //     connectParents(leaves[i], rooms);
            // }
            
        }
        
        void Start(){
            createRooms();
        }

}