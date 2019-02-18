using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class BorderCalculation
{

    private List<node> treatSquare(int upperLeftCorner, int width)
    {
        List<node> borders = new List<node>();
        float terrainWidth = Mathf.Sqrt(GameManager.instance.Nodes.Length);
        int index = upperLeftCorner;
        for (int rows = 0; rows < width; rows++)
        {
            // do the columns
            for (int i = 0; i < width; i++)
            {
                // do the downs
                borders.Add(GameManager.instance.Nodes[index]);

                index += 1;
            }

            index -= 4;
            index += (int)terrainWidth;
        }

        return borders;
    }


    public List<List<node>> GetInitBorderCalculation(Vector3 pointPivot)
    {
        List<node> Borders = new List<node>();
        int width=4;
        int index = 0;
        var listNodeList = new List<List<node>>();
        float terrainWidth = Mathf.Sqrt(GameManager.instance.Nodes.Length);
        UnityEngine.Debug.Log("this is the " + " groupe, positions : " + terrainWidth);

        for (int z = 0; z < (terrainWidth/4); z++)
        {
            for (int i = 0; i < terrainWidth; i += ((int)terrainWidth * 4))
            {

                for (int q = 0; q < terrainWidth; q += 4)
                {
                    // next rectangle horizontaly
                    listNodeList.Add(treatSquare(index, width));
                    index += 4;

                }
                index += ((int)terrainWidth * 3);
            }

        }
       
        //foreach (var item in listNodeList)
        //{
        //    foreach (var item2 in item)
        //    {
        //        Borders.Add(item2);
        //    }
            
        //}
        foreach (var item in GameManager.instance.Nodes)
        {
            float dist = Vector3.Distance(pointPivot, item.transform.position);

            if (dist < 100)
            {
                //NodesList.Add(item);
                //Borders.Add(item);
                

            }

        }

       
        return listNodeList;
    }
    // the owner has to be set right (not implemented yet)!!! 
    public List<node> CornerDraw(List<List<node>> input, Owner owner)
    {
        int x = 0;
        int y = 0;
        int listWidth = (int)Math.Sqrt(input.Count);
        List<node> borders = new List<node>();
      
        for (; x < input.Count; x++)
        {
            if (input[x][0].GetOwner == owner)
            {
                //left
                if (x>=1)
                {
                    if (input[x - 1][0].GetOwner != owner)
                    {
                        borders.Add(input[x][4]);
                        borders.Add(input[x][8]);
                    }
                }       
                //right
                if (input[x + 1][0].GetOwner != owner)
                {
                    borders.Add(input[x][7]);
                    borders.Add(input[x][11]);
                }
                //up
                if (x>=listWidth)
                {
                    if (input[x - listWidth][0].GetOwner != owner)
                    {
                        borders.Add(input[x][1]);
                        borders.Add(input[x][2]);
                    }
                }
                //down
                if (input[x + listWidth][0].GetOwner != owner)
                {
                    borders.Add(input[x][13]);
                    borders.Add(input[x][14]);
                }

                //check up down left and right for owners
                // if not owner then create a line renderer on corner
                
            }
            
        }
        
        return borders;
    }
    
    
}

