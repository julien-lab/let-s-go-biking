using System;

public class Position
{
    public float latitude { get; set; }
    public float longitude { get; set; }

    public override string ToString()
    {
        return "{ 'latitude': " + latitude + " , 'longitude': " + longitude + " }";
    }
}