using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.DynamicData;
using System.Web.Script.Serialization;

namespace TB_Stock.Api.ApiHandler
{
    public class InstagramGraphApi : IInstagramGraphApi
    {
        private const string BASE_URL = "https://graph.instagram.com/";
        private const string MEDIA_URL = "me/media?fields={0}&limit={1}&access_token={2}";
        private const string MEDIA_FIELDS = "id,caption,media_type,media_url,children{media_url}";
        private const string MEDIA_PAGE_SIZE = "25";

        private static readonly HttpClient _client = new HttpClient();

        public async Task<IEnumerable<InstagramPost>> GetInstagramPosts(string accessToken)
        {
            List<InstagramPost> posts = new List<InstagramPost>();

            string url = string.Concat(BASE_URL, string.Format(MEDIA_URL, MEDIA_FIELDS, MEDIA_PAGE_SIZE, accessToken));


            string nextUrl = url;

            do
            {
                nextUrl = await AddPosts(posts, nextUrl);
            } while (!string.IsNullOrEmpty(nextUrl));


            return posts;
        }

        public async Task<Tuple<IEnumerable<InstagramPost>,string>> GetPagedInstagramPosts(string accessToken,string nextUrl)
        {
            List<InstagramPost> posts = new List<InstagramPost>();

            if (string.IsNullOrEmpty(nextUrl)) { 
                nextUrl = string.Concat(BASE_URL, string.Format(MEDIA_URL, MEDIA_FIELDS, MEDIA_PAGE_SIZE, accessToken));
            }

            nextUrl= await AddPosts(posts, nextUrl);

            return new Tuple<IEnumerable<InstagramPost>, string>(posts, nextUrl);
        }

        private static async Task<string> AddPosts(List<InstagramPost> posts, string nextUrl)
        {
            var mediaJsonResponse = await _client.GetAsync(nextUrl);

            var mediaResponse = await mediaJsonResponse.Content.ReadAsAsync<MediaResponse>();

            posts.AddRange(mediaResponse.Data);

            nextUrl = mediaResponse.Paging.Next;
            return nextUrl;
        }
    }

    public class InstagramPost
    {
        public string Id { get; set; }
        public string Caption { get; set; }
        public string Media_Type { get; set; }
        public string Media_Url { get; set; }
        public Children Children { get; set; }
    }


    public class Children
    {
        public IEnumerable<InstagramChildPost> Data { get; set; }
    }

    public class InstagramChildPost
    {
        public string Id;
        public string Media_Url;
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