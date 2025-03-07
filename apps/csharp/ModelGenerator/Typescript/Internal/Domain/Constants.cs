namespace Typescript.Internal.Domain;

public class Constants
{
    internal static IDictionary<string, string> ConvertedTypes = new Dictionary<string, string>
    {
        ["System.String"] = "string",
        ["String"] = "string",
        ["System.Char"] = "string",
        ["Char"] = "string",
        ["System.Byte"] = "number",
        ["Byte"] = "number",
        ["System.SByte"] = "number",
        ["SByte"] = "number",
        ["System.Int16"] = "number",
        ["Int16"] = "number",
        ["System.UInt16"] = "number",
        ["UInt16"] = "number",
        ["System.Int32"] = "number",
        ["Int32"] = "number",
        ["System.UInt32"] = "number",
        ["UInt32"] = "number",
        ["System.Int64"] = "number",
        ["Int64"] = "number",
        ["System.UInt64"] = "number",
        ["UInt64"] = "number",
        ["System.Single"] = "number",
        ["Single"] = "number",
        ["System.Double"] = "number",
        ["Double"] = "number",
        ["System.Decimal"] = "number",
        ["Decimal"] = "number",
        ["System.Boolean"] = "boolean",
        ["Boolean"] = "boolean",
        ["System.Void"] = "void",
        ["Void"] = "void",
        ["System.Guid"] = "string",
        ["Guid"] = "string",
        ["System.DateTime"] = "Date",
        ["DateTime"] = "Date",
    };
}