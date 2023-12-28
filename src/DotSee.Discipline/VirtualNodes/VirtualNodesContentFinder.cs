﻿using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Microsoft.Extensions.Caching.Memory;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using Serilog;

namespace DotSee.Discipline.VirtualNodes
{
    public class VirtualNodesContentFinder : IContentFinder
{
        private readonly IMemoryCache _memCache;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly ILogger _logger;

        public VirtualNodesContentFinder(IMemoryCache memCache,IUmbracoContextFactory umbracoContextFactory, ILogger logger)
        {
            
            _memCache = memCache;
            _umbracoContextFactory = umbracoContextFactory;
            _logger = logger;   
        }

        public Task<bool> TryFindContent(IPublishedRequestBuilder request)
        {
           //Get a cached dictionary of urls and node ids
            var cachedVirtualNodeUrls = _memCache.Get<Dictionary<string, int>>("cachedVirtualNodes");

            //Get the request path
            string path =request.AbsolutePathDecoded;

            //If found in the cached dictionary, get the node id from there
            if (cachedVirtualNodeUrls != null && cachedVirtualNodeUrls.ContainsKey(path))
            {
                //That's all folks
                int nodeId = cachedVirtualNodeUrls[path];
                using (var _umb = _umbracoContextFactory.EnsureUmbracoContext()) 
                { 
                    request.SetPublishedContent(_umb?.UmbracoContext?.Content?.GetById(nodeId));
                }
                return Task.FromResult( true);
            }

            //If not found on the cached dictionary, traverse nodes and find the node that corresponds to the URL
            IPublishedContent item = null;
            using (var _umb= _umbracoContextFactory.EnsureUmbracoContext())
            {
                var rootNodes = _umb?.UmbracoContext?.Content?.GetAtRoot();
                try
                {
                    var nodes = rootNodes?.DescendantsOrSelf<IPublishedContent>();
                    item = nodes?.Where(x => x.Url() == (path + "/") || x.Url() == path).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, string.Format("Could not get content for URL '{0}'", request.Uri.ToString()));
                }
            }

            //If item is found, return it after adding it to the cache so we don't have to go through the same process again.
            if (cachedVirtualNodeUrls == null) { cachedVirtualNodeUrls = new Dictionary<string, int>(); }

            //If we have found a node that corresponds to the URL given
            if (item != null)
            {
                //This check is redundant, but better to be on the safe side.
                if (!cachedVirtualNodeUrls.ContainsKey(path))
                {
                    //Add the new path and id to the dictionary so that we don't have to go through the tree again next time.
                    cachedVirtualNodeUrls.Add(path, item.Id);
                }

                //Update cache
                _memCache.Set("cachedVirtualNodes",  cachedVirtualNodeUrls, new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.High                   
                });

                //That's all folks
                request.SetPublishedContent(item);
                return Task.FromResult(true);
            }

            //Abandon all hope ye who enter here. This means that we didn't find a node so we return false to let
            //the next ContentFinder (if any) take over.
            return Task.FromResult(false);
        }
    }
}



