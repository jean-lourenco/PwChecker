using System;
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
        private static DateTime LastExecution = DateTime.MinValue;
        private static readonly TimeSpan ApiThrottle = TimeSpan.FromMilliseconds(1600);

        static PwChecker()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("PwChecker/1.0");
        }

        public async static Task<bool> IsCompromisedAsync(string password)
        {
            var data = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Password", password )
            });

            await WaitForThrottle();

            LastExecution = DateTime.Now;
            var response = await Client.PostAsync(BaseUrl, data);

            if ((int)response.StatusCode == 429)
                throw new HttpRequestException("Too Many Requests (429) on 'haveibeenpwned.com' api");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode} on 'haveibeenpwned.com' api");

            return response.StatusCode == HttpStatusCode.OK;
        }

        private static async Task WaitForThrottle()
        {
            var now = DateTime.Now;
            var nextPossibleExecution = now + ApiThrottle;

            if (nextPossibleExecution > now)
                await Task.Delay(nextPossibleExecution - now);
        }
    }
}
