using System;

public class Station
{
    public string contractName { get; set; }

    public string name { get; set; }

    public int number { get; set; }

    public Stands totalStands { get; set; }

    public Position position { get; set; }

    public override string ToString()
    {
        string str = "Clotest station to starting point : Contract_Name : " + this.contractName + " Name : " + this.name + " Number : " + this.number + " Position : " + this.position.ToString() + " ";
        return str;
    }
}