using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DO NOT TOUCH THIS, this has to correspond exactly with the JSON structure
/// </summary>
[Serializable]
public class HouseInfo
{
    
    public Floor Floor;
    public List<Wall> Walls;
    public Attic Attic;
    public List<Item> Items;

    public GPS GPS;

    public static HouseInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<HouseInfo>(jsonString);
    }
}

[Serializable]
public class Position
{
    public float X;
    public float Y;
    public float Z;
    public float R;
}

[Serializable]
public class GPS
{
    public float X;
    public float Z;
}

[Serializable]
public class Dimension
{
    public float X;
    public float Y;
    public float Z;
}

[Serializable]
public class Orientation
{
    public float W;
    public float X;
    public float Y;
    public float Z;
}

[Serializable]
public class BuildingBlock : IEquatable<BuildingBlock>, IComparable<BuildingBlock>
{
    public Position Position;
    public Dimension Dimension;

    public int CompareTo(BuildingBlock other)
    {
        if (other == null)
            return 1;
        else
        {
            return this.Position.X.CompareTo(other.Position.X);
        }
    }

    public bool Equals(BuildingBlock other)
    {
        if (other == null) return false;
        return (this.Position.X.Equals(other.Position.X));
    }
}

[Serializable]
public class Door : BuildingBlock
{
}

[Serializable]
public class Window : BuildingBlock
{
}

[Serializable]
public class AtticSegment
{
    public Position Position;
    public Dimension Dimension;
}

[Serializable]
public class Roof
{
    public Position Position;
    public Dimension Dimension;
    public float Pitch;
}

[Serializable]
public class Floor
{
    public Position Position;
    public Dimension Dimension;
}

[Serializable]
public class Wall : BuildingBlock
{
    public List<BuildingBlock> BuildingBlocks;
    public List<Door> Doors;
    public List<Window> Windows;

    public void sortDoorsAndWindows()
    {
        BuildingBlocks.Sort();
    }
}

[Serializable]
public class Attic
{
    public Floor Floor;
    public List<AtticSegment> AtticSegments;
    public Roof Roof;
}

[Serializable]
public class Item
{
    public int Id;  // ID from Alen's database, multiple instances of the same furniture model  will have the same ID
    public string Type; // category of furniture
    public string Name;
    public string ModelUrl;
    public Position Position;
    public Orientation Orientation;
}