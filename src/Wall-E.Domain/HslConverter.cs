namespace Wall_E.Domain;

public static class HslConverter
{
    public static (int r, int g, int b) HslToRgb(double h, double s, double l)
    {
        h = ((h % 360) + 360) % 360;
        s = Math.Clamp(s, 0, 100) / 100.0;
        l = Math.Clamp(l, 0, 100) / 100.0;

        double c = (1 - Math.Abs(2 * l - 1)) * s;
        double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        double m = l - c / 2;

        double r1, g1, b1;
        if (h < 60)       { r1 = c; g1 = x; b1 = 0; }
        else if (h < 120) { r1 = x; g1 = c; b1 = 0; }
        else if (h < 180) { r1 = 0; g1 = c; b1 = x; }
        else if (h < 240) { r1 = 0; g1 = x; b1 = c; }
        else if (h < 300) { r1 = x; g1 = 0; b1 = c; }
        else              { r1 = c; g1 = 0; b1 = x; }

        int r = (int)Math.Round((r1 + m) * 255);
        int g = (int)Math.Round((g1 + m) * 255);
        int b = (int)Math.Round((b1 + m) * 255);
        return (Math.Clamp(r, 0, 255), Math.Clamp(g, 0, 255), Math.Clamp(b, 0, 255));
    }

    public static string ToHex(double h, double s, double l)
    {
        var (r, g, b) = HslToRgb(h, s, l);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    public static void RgbToHsl(int r, int g, int b, out double h, out double s, out double l)
    {
        double rd = r / 255.0, gd = g / 255.0, bd = b / 255.0;
        double max = Math.Max(rd, Math.Max(gd, bd));
        double min = Math.Min(rd, Math.Min(gd, bd));
        double d = max - min;

        l = (max + min) / 2.0;

        if (d < 1e-9)
        {
            h = 0; s = 0;
        }
        else
        {
            s = l > 0.5 ? d / (2.0 - max - min) : d / (max + min);
            if (max == rd) h = 60 * ((gd - bd) / d % 6);
            else if (max == gd) h = 60 * ((bd - rd) / d + 2);
            else h = 60 * ((rd - gd) / d + 4);
            if (h < 0) h += 360;
        }
    }

    public static string Lighten(string hex, double amount)
    {
        var (r, g, b) = ParseHex(hex);
        RgbToHsl(r, g, b, out double h, out double s, out double l);
        l = Math.Min(1, l + amount / 100.0);
        return ToHex(h, s * 100, l * 100);
    }

    public static string Darken(string hex, double amount)
    {
        var (r, g, b) = ParseHex(hex);
        RgbToHsl(r, g, b, out double h, out double s, out double l);
        l = Math.Max(0, l - amount / 100.0);
        return ToHex(h, s * 100, l * 100);
    }

    public static string Mix(string hex1, string hex2, double ratio = 0.5)
    {
        var (r1, g1, b1) = ParseHex(hex1);
        var (r2, g2, b2) = ParseHex(hex2);
        int r = (int)Math.Round(r1 * (1 - ratio) + r2 * ratio);
        int g = (int)Math.Round(g1 * (1 - ratio) + g2 * ratio);
        int b = (int)Math.Round(b1 * (1 - ratio) + b2 * ratio);
        return $"#{Math.Clamp(r, 0, 255):X2}{Math.Clamp(g, 0, 255):X2}{Math.Clamp(b, 0, 255):X2}";
    }

    public static string Complement(string hex)
    {
        var (r, g, b) = ParseHex(hex);
        RgbToHsl(r, g, b, out double h, out double s, out double l);
        h = (h + 180) % 360;
        return ToHex(h, s * 100, l);
    }

    private static (int r, int g, int b) ParseHex(string hex)
    {
        string h = hex.TrimStart('#');
        if (h.Length == 3) h = $"{h[0]}{h[0]}{h[1]}{h[1]}{h[2]}{h[2]}";
        if (h.Length == 4) h = $"{h[0]}{h[0]}{h[1]}{h[1]}{h[2]}{h[2]}{h[3]}{h[3]}";
        if (h.Length == 8) h = h[..6];
        int r = Convert.ToInt32(h[..2], 16);
        int g = Convert.ToInt32(h[2..4], 16);
        int b = Convert.ToInt32(h[4..6], 16);
        return (r, g, b);
    }
}
