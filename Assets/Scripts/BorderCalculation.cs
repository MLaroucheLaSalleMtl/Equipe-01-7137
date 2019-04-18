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


    public List<List<node>> GetInitBorderCalculation(Vector3 pointPivot, Owner owner)
    {
        List<node> Borders = new List<node>();

        int width = 4;
        int index = 0;
        var listNodeList = new List<List<node>>();
        float terrainWidth = Mathf.Sqrt(GameManager.instance.Nodes.Length);
      //  UnityEngine.Debug.Log("this is the " + " groupe, positions : " + terrainWidth);

        for (int z = 0; z < (terrainWidth / 4); z++)
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


        foreach (var item in GameManager.instance.Nodes)
        {

            float dist = Vector3.Distance(pointPivot, item.transform.position);

            if (dist < 75)
            {
                if (item.GetOwner.Name == "Neutral")
                {
                    item.SetOwner(owner);
                   // UnityEngine.Debug.Log(owner.Name);
                }

            }
            else if (item.GetOwner.Name == "Neutral")
            {
                item.SetOwner(GameManager.owners[2]);
            }


        }


        return listNodeList;
    }

    public List<node> CornerDraw(List<List<node>> input, Owner owner)
    {

        int y = 0;
 
        int listWidth = (int)Math.Sqrt(input.Count);
        List<node> borders = new List<node>();

        for (int x = 0; x < input.Count; x++)
        {
            if (input[x][0].GetOwner == owner)
            {
                //left
                if (x >= 1)
                {
                    if (input[x - 1][0].GetOwner != owner)
                    {
                        borders.Add(input[x][4]);
                        borders.Add(input[x][8]);
                    }
                }
                //right
                if (x + 1 < input.Count)
                {
                    if (input[x + 1][0].GetOwner != owner)
                    {
                        borders.Add(input[x][7]);
                        borders.Add(input[x][11]);
                    }
                }
                //up
                if (x >= listWidth)
                {
                    if (input[x - listWidth][0].GetOwner != owner)
                    {
                        borders.Add(input[x][1]);
                        borders.Add(input[x][2]);
                    }
                }
                //down
                if (x + listWidth < input.Count)
                {
                    if (input[x + listWidth][0].GetOwner != owner)
                    {
                        borders.Add(input[x][13]);
                        borders.Add(input[x][14]);
                    }
                }


                //check up down left and right for owners
                // if not owner then create a line renderer on corner

            }


        }


        List<node> Fnodes = new List<node>();
        node ToAdd = new node();
        node tempNode = new node();
        node initialNode = new node();
        int count = 0;
        int maxCount = borders.Count;
        while (borders.Count > 1)
        {
            float closest = 90f;
            float curr = 100f;

            if (count == 0)
            {
                Fnodes.Add(borders[1]);
                ToAdd = borders[1];
                initialNode = borders[1];
            }
            for (int x = 0; x < borders.Count - 1; x++)
            {

                curr = Vector3.Distance(ToAdd.GetPosition, borders[x].GetPosition);
                if (closest > curr)
                {
                    // UnityEngine.Debug.Log(closest);
                    closest = curr;
                    tempNode = borders[x];
                }
                else if(curr == closest)
                {
                    closest = curr;
                    tempNode = borders[x];
                }


            }
            ToAdd = tempNode;
            Fnodes.Add(ToAdd);
            borders.Remove(ToAdd);
            count++;
            if (count == (maxCount + 40))
            {
                break;
            }
        }
        Fnodes.Add(initialNode);


        return Fnodes;
    }

    public void GenLineRenderers(List<List<node>> input)
    {
        foreach (var listNodes in input)
        {
            foreach (var item in listNodes)
            {
                item.gameObject.AddComponent<LineRenderer>();
                break;
            }
        }
    }

    public void UpdateDraw(List<node> cornerDraw, Owner owner, node node)
    {
        GameManager.instance.musicLauncher.Losing(owner);
        float smallest = float.MaxValue;
        int index = 200;
        for (int q = 0; q < 2; q++)
        {
            for (int i = 0; i < cornerDraw.Count - 2; i++)
            {
                var speculation = cornerDraw[i].transform.position + cornerDraw[i + 1].transform.position;
                float dist = Vector3.Distance(speculation, node.transform.position);
                if (dist < smallest)
                {
                    smallest = dist;
                    index = i;
                }
                
            }

        }


        cornerDraw.Insert(index, node);
        owner.faction.NodesList = cornerDraw;

        owner.faction.GenFrontieres();
        UnityEngine.Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }
    public void RemoveDraw(List<node> cornerDraw, Owner owner, node node)
    {
          
       
        owner.faction.NodesList.Remove(node);
         
        owner.faction.GenFrontieres();
        UnityEngine.Debug.Log("ÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉÉ");

    }



}


