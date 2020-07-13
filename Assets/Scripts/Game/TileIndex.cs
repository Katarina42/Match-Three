public class TileIndex
{
    public int x;
    public int y;

    public TileIndex(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Valid()
    {
        if (x >= 0 && y >= 0)
            return true;

        return false;
    }

    public override string ToString()
    {
        return "("+x + " , " + y+")";
    }

    public override bool Equals(object obj)
    {
        var index = obj as TileIndex;
        return index != null &&
               x == index.x &&
               y == index.y;
    }

    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
}

