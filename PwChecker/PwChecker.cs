using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PwCheckerLib
{
    public class PwChecker
    {
        private const string BaseUrl = "https://haveibeenpwned.com/api/v2/pwnedpassword";
        private static readonly HttpClient Client = new HttpClient();

        static PwChecker()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("PwChecker/1.0");
        }

        public async static Task<bool> IsCompromisedAsync(string password)
        {
            var data = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Password", password )
            });
            var response = await Client.PostAsync(BaseUrl, data);

            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
