using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float speed = 200.0f;
    private RaycastHit2D hit; 

    void FixedUpdate()
    {

        Vector3 v = Vector3.zero; 
        bool move = false;
        if (Input.GetKey(KeyCode.UpArrow)  ||  Input.GetKey(KeyCode.W)) {
            v = Vector3.up * Time.deltaTime * speed;
            move=true;
    }       
        else if (Input.GetKey(KeyCode.DownArrow)  ||  Input.GetKey(KeyCode.S)){
            v = Vector3.down * Time.deltaTime * speed;
            move=true;

        }
        else if (Input.GetKey(KeyCode.LeftArrow)  ||  Input.GetKey(KeyCode.A)) {
            v = Vector3.left * Time.deltaTime * speed;
            move=true;

            }

        else if (Input.GetKey(KeyCode.RightArrow)  ||  Input.GetKey(KeyCode.D)) {
            v =Vector3.right * Time.deltaTime * speed;
            move=true;


        }
        if (move){
            transform.Translate(v);
            hit = Physics2D.Raycast(transform.position, -transform.forward);
            
            if (hit.collider == null)
                transform.Translate(-v);
            else if ( hit.collider.gameObject.name.StartsWith("Item") ){
                hit.collider.gameObject.SetActive(false);
                print("Set active");

            }

        }


        
    }
}
