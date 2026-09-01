using Wall_E.Domain;
namespace Wall_E.Application.DSL;

/// <summary>Represents a single lexical token produced by the WALL-E lexer.</summary>
public class Token
{
    /// <summary>The token category.</summary>
    public TokenType Type { get; set; }
    /// <summary>The raw text or value carried by the token.</summary>
    public string Value { get; set; }
    /// <summary>The source location where the token appeared.</summary>
    public Location TokenLocation { get; protected set; }

    /// <summary>Enumerates all token types recognized by the WALL-E lexer.</summary>
    public enum TokenType { sin, cos, sqrt, exp, tan, atan, abs, floor, ceil, phi, sqrt2, seed, print, rgb, rgba, hsl, polygon, ellipse, repeat, for_token, animate, label, grosor, dashed, dotted, dashdot, solid_k, fill, unfill, linear, radial, lighten, darken, mix, complement, layer, hide, show, snap, randoms, points, color_value, samples, log, PI, E, draw, sum, substraction, multiplication, division, power, module, keyword, conditional, symbol, left_bracket, right_bracket, left_key, right_key, low_hyphen, undefined, not, concatenate, Or, And, minor, major, equal_minor, equal_major, equal, diferent, identifier, text, number, point, line, sequence, segment, ray, circle, point_sequence, line_sequence, arc, measure, intersect, count, not_id, EOL, EOF }

    /// <summary>Creates a token with the given type, text, and source position.</summary>
    public Token(TokenType type, string value, string file, string line, string column)
    {
        this.Type = type;
        this.Value = value;
        TokenLocation = new Location(file, line, column);
    }

    /// <summary>Formats the token as "Type [Value]".</summary>
    public override string ToString() => string.Format("{0} [{1}]", Type, Value);
}