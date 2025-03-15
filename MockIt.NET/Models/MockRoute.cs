namespace JsonMockServer.Models
{

    public class MockCondition
    {
        public Dictionary<string, string>? PathParams { get; set; }
        public Dictionary<string, string>? QueryParams { get; set; }
    }

    public class MockResponse
    {
        public MockCondition? Conditions { get; set; }
        public object? Response { get; set; }
        public int StatusCode { get; set; } = 200;
    }

    public class MockRoute
    {
        public string RouteName { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = "GET";
        public List<MockResponse> Responses { get; set; } = new List<MockResponse>();
    }
}
