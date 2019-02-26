using UnityEngine;
using System.Collections;
using LitJson;
using System.IO;
using System;

public class CityGeojson : MonoBehaviour {
	
	// the datas as a string	
	private string jsonString;
	
	// JsonData object, easier to manipulate in order to retrieve the needed informations
	private JsonData itemData;
	
	// the center of the map, in our exemple Paris center
	public double latOrigine = 48.856578;
	public double longOrigine = 2.351828;
	
	// a game object representing a chunk of a road
	public GameObject road;
	
	
	void Start () {

		//read the file and convert the string into a jsondata object
		jsonString = File.ReadAllText(Application.dataPath + "/Resources/voie.geojson");
		itemData = JsonMapper.ToObject(jsonString);

		//for each item, we determine if it's a line string or a multilinestring
		int itemCount = itemData["features"].Count;
		
		for(int i = 0; i<itemCount; i++)
		{
			// if it's a line string, we create a road from the line string coordinates
			if (itemData["features"][i]["geometry"]["type"].ToString() == "LineString")
			{
				GameObject parent = new GameObject(itemData["features"][i]["properties"]["l_courtmin"].ToString());

				CreateRoadFromLineString(itemData["features"][i]["geometry"]["coordinates"],parent);
			}
			// if it's not a linestring, it's a multistring, so i need to create a froad from each line string
			else
			{
				int linesCount = itemData["features"][i]["geometry"]["coordinates"].Count;
				
				GameObject parent = new GameObject(itemData["features"][i]["properties"]["l_courtmin"].ToString());

				// creating a road for each line string
				for (int k = 0; k<linesCount; k++)
				{
					CreateRoadFromLineString(itemData["features"][i]["geometry"]["coordinates"][k],parent);
				}
			}
		}
	}
	
	void CreateRoadFromLineString(JsonData coordinateArray, GameObject parent)
	{
		int pointsCount = coordinateArray.Count;
		double latitude = (double)coordinateArray[0][1];
		double longitude = (double)coordinateArray[0][0];

		Vector3 pos = AddPoint (longitude,latitude);
		
		for (int i = 1; i<pointsCount; i++)
		{
			double newLatitude = (double)coordinateArray[i][1];
			double newLongitude = (double)coordinateArray[i][0];

			Vector3 newPos = AddPoint (newLongitude,newLatitude);
			AddLine(pos,newPos,parent);
			pos = newPos;
		}
	}

	void AddLine(Vector3 startv, Vector3 endv, GameObject parent)
	{
		float x = Mathf.Pow((endv.x-startv.x),2);
		float z = Mathf.Pow((endv.z-startv.z),2);
		float distance = Mathf.Sqrt(x+z);
		GameObject newCube =  Instantiate(road, (startv+endv)/2, Quaternion.identity) as GameObject;
		
		newCube.name = "trunk";
		newCube.transform.LookAt(endv);
		newCube.transform.localScale+= new Vector3(0,0,distance);
		newCube.transform.SetParent(parent.transform);
		
	}
	
	Vector3 AddPoint(double longitude, double latitude)
	{
		double coordX = Math.Acos(Math.Sin(ConvertDegreesToRadians(latOrigine))*Math.Sin(ConvertDegreesToRadians(latOrigine)) + 
		                          Math.Cos(ConvertDegreesToRadians(latOrigine))*Math.Cos(ConvertDegreesToRadians(latOrigine))* Math.Cos(ConvertDegreesToRadians(longOrigine-longitude)))*6371000;
		if(longitude<longOrigine)
		{
			coordX = -coordX;
		}
		
		
		double coordZ = Math.Acos (Math.Sin(ConvertDegreesToRadians(latOrigine))*Math.Sin(ConvertDegreesToRadians(latitude)) +
		                           Math.Cos(ConvertDegreesToRadians(latOrigine))*Math.Cos(ConvertDegreesToRadians(latitude)))*6371000;
		
		if(latitude<latOrigine)
		{
			coordZ = -coordZ;
		}

		Vector3 pos = new Vector3((float)coordX,0,(float)coordZ);
		
		return pos;
		
	}
	
	public static double ConvertDegreesToRadians (double degrees)
	{
		double radians = (Math.PI / 180) * degrees;
		return (radians);
	}
}
