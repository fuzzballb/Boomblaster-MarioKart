using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour {

    //For placing the image of the mini map.
    public GUIStyle miniMap;
    //Two transform variables, one for the player's and the enemy,
    public Transform player;
    public List<Transform> enemies;
    //Icon images for the player and enemy(s) on the map.
    public GUIStyle playerIcon;
    public GUIStyle enemyIcon;
    //Offset variables (X and Y) - where you want to place your map on screen.
     float mapOffSetX = 100;
     float mapOffSetY = 100;
    //The width and height of your map as it'll appear on screen,
     float mapWidth = Screen.height / 2;
     float mapHeight = Screen.height / 2;
    //Resolution (both width and height) of your terrain.
    float sceneWidth;
    float sceneHeight;
    //The size of your player and enemy's icon as it would appear on the map.
    public float iconSize = 10;
    public float iconHalfSize;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    iconHalfSize = iconSize/2;
	}

    private float GetMapPos(float pos, float mapSize, float sceneSize) {
        //Debug.Log(pos * mapSize / sceneSize);
        return pos * mapSize/sceneSize;
    }



    void OnGUI()
    {
        sceneWidth = Screen.width;
        sceneHeight = Screen.height;
        //Everything about the map.

        //GUI.BeginGroup(new Rect(mapOffSetX, mapOffSetY, mapWidth, mapHeight));
        //GUI.Button(new Rect(mapOffSetX, mapOffSetY, mapWidth, mapHeight), "test");
        //GUI.BeginGroup(new Rect(Screen.width / 2 -100, Screen.height / 2 - 100, 800, 600));
        if (player != null)
        {
            //var pX = GetMapPos(player.transform.position.x + 800, mapWidth, sceneWidth);
            //var pZ = GetMapPos(player.transform.position.z + 800, mapHeight, sceneHeight);

            var pX = player.transform.position.x + 200;
            var pZ = player.transform.position.z - 200;


            var playerMapX = pX - iconHalfSize;
            var playerMapZ = (pZ * -1) - iconHalfSize;
            //Debug.Log(new Rect(playerMapX, playerMapZ, iconSize, iconSize));
            GUI.Box(new Rect(playerMapX, playerMapZ, iconSize, iconSize), "P", playerIcon);
            //GUI.Button(new Rect(playerMapX, playerMapZ, iconSize, iconSize), "test");
        }
        foreach(var enemy in enemies)
        {
            if (enemy != null) //  && enemy != player
            {
                var sX = enemy.transform.position.x + 200;
                var sZ = enemy.transform.position.z - 200;
                var enemyMapX = sX - iconHalfSize;
                var enemyMapZ = ((sZ * -1) - iconHalfSize);

                GUI.Box(new Rect(enemyMapX, enemyMapZ, iconSize, iconSize), "E", playerIcon);
                //GUI.Box(new Rect(enemyMapX, enemyMapZ, iconSize, iconSize), "Enemy", enemyIcon);
            }
        }
        //GUI.EndGroup();
    }
}
