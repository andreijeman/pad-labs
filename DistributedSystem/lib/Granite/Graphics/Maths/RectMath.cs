namespace Granite.Graphics.Maths;

public static class RectMath
{
    public static bool Intersects(this Rect rect, Rect rect2)
    {
        return rect.X1 <= rect2.X2 &&
               rect.X2 >= rect2.X1 &&
               rect.Y1 <= rect2.Y2 &&
               rect.Y2 >= rect2.Y1;
    }

    public static bool TryGetIntersection(this Rect rect, Rect rect2, out Rect result)
    {
        result = new Rect();

        if (rect.Intersects(rect2))
        {
            result.X1 = Math.Max(rect.X1, rect2.X1);
            result.Y1 = Math.Max(rect.Y1, rect2.Y1);
            result.X2 = Math.Min(rect.X2, rect2.X2);
            result.Y2 = Math.Min(rect.Y2, rect2.Y2);

            return true;
        }

        return false;
    }

    public static bool TryGetUncoveredSections(this Rect rect, Rect rect2, out List<Rect> results)
    {
        results = new List<Rect>();

        if (!rect.TryGetIntersection(rect2, out Rect sect))
        {
            results.Add(rect);
            return true;
        }

        // Top section
        if (rect.Y1 < sect.Y1)
        {
            results.Add(new Rect()
            {
                X1 = rect.X1,
                Y1 = rect.Y1,
                X2 = rect.X2,
                Y2 = sect.Y1 - 1,
            });
        }

        // Bottom section
        if (rect.Y2 > sect.Y2)
        {
            results.Add(new Rect()
            {
                X1 = rect.X1,
                Y1 = sect.Y2 + 1,
                X2 = rect.X2,
                Y2 = rect.Y2,
            });
        }

        // Left section
        if (rect.X1 < sect.X1)
        {
            results.Add(new Rect()
            {
                X1 = rect.X1,
                Y1 = sect.Y1,
                X2 = sect.X1 - 1,
                Y2 = sect.Y2,
            });
        }

        // Right section
        if (rect.X2 > sect.X2)
        {
            results.Add(new Rect()
            {
                X1 = sect.X2 + 1,
                Y1 = sect.Y1,
                X2 = rect.X2,
                Y2 = sect.Y2,
            });
        }

        if (results.Count > 0) return true;
        return false;
    }
}
