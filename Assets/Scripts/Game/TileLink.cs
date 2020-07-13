using System.Collections.Generic;

public class TileLink
{
    public TileIndex a;
    public TileIndex b;

    public TileLink(TileIndex a, TileIndex b)
    {
        this.a = a;
        this.b = b;
    }

    public override string ToString()
    {
        return a + " : " + b;
    }

    public override bool Equals(object obj)
    {
        TileLink link = obj as TileLink;
        return link!=null && a == link.a && b==link.b;
    }

    public override int GetHashCode()
    {
        var hashCode = 2118541809;
        hashCode = hashCode * -1521134295 + EqualityComparer<TileIndex>.Default.GetHashCode(a);
        hashCode = hashCode * -1521134295 + EqualityComparer<TileIndex>.Default.GetHashCode(b);
        return hashCode;
    }

}
