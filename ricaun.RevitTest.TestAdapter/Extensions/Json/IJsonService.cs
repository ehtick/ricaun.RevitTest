namespace ricaun.RevitTest.TestAdapter.Extensions.Json
{
    internal interface IJsonService
    {
        T Deserialize<T>(string value);
        string Serialize(object value);
    }
}