using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TB_Stock.Api.ApiHandler
{
    public interface IInstagramGraphApi
    {
        Task<IEnumerable<InstagramPost>> GetInstagramPosts(string accessToken);

        Task<Tuple<IEnumerable<InstagramPost>, string>> GetPagedInstagramPosts(string accessToken, string nextUrl);
    }
}