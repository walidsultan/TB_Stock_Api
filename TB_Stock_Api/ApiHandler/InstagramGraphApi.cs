using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace TB_Stock.Api.ApiHandler
{
    public class InstagramGraphApi : IInstagramGraphApi
    {
        private const string BASE_URL = "https://graph.instagram.com/";
        private const string MEDIA_URL = "me/media?fields={0}&limit={1}&access_token={2}";
        private const string MEDIA_FIELDS = "id,caption,media_type,media_url";
        private const string MEDIA_PAGE_SIZE = "25";

        private static readonly HttpClient _client = new HttpClient();

        public async Task<IEnumerable<InstagramPost>> GetInstagramPosts(string accessToken)
        {
            List<InstagramPost> posts = new List<InstagramPost>();

            string url = string.Concat(BASE_URL, string.Format(MEDIA_URL, MEDIA_FIELDS, 25, accessToken));


            string nextUrl = url;

            do
            {
                var mediaJsonResponse = await _client.GetAsync(nextUrl);

                var mediaResponse = await mediaJsonResponse.Content.ReadAsAsync<MediaResponse>();

                posts.AddRange(mediaResponse.Data);

                nextUrl = mediaResponse.Paging.Next;

            } while (!string.IsNullOrEmpty(nextUrl));


            return posts;
        }

    }

    public class InstagramPost
    {
        public string Id { get; set; }
        public string Caption { get; set; }
        public string Media_Type { get; set; }
        public string Media_Url { get; set; }
    }

    public class MediaResponse
    {
        public IEnumerable<InstagramPost> Data { get; set; }
        public Paging Paging { get; set; }
    }

    public class Paging
    {
        public string Next { get; set; }
    }
}