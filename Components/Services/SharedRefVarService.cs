


namespace BlazorMudBlazorCACApp1.Components.Services
{
    public class SharedRefVarService
    {
        public bool loggedInStatus { get; set; } = false;
        public bool notLoggedInStatus { get; set; } = true;

        public string username { get; set; } = "";
        public string shortname { get; set; } = "";
        public int age { get; set; } = 0;
        public string email { get; set; } = "";
        public string password { get; set; } = "";
        public string accountType { get; set; } = "";
        public bool verified { get; set; } = false;
        public string profileUrl { get; set; } = "";
        public HttpClient sharedHttpClient = new HttpClient();
    }
}
