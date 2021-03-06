﻿using Microsoft.Extensions.Caching.Memory;
using Orchard.Settings;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using System;
using System.Threading.Tasks;
using Orchard.ContentManagement;

namespace Orchard.Dashboard
{
    public class AdminThemeService : IAdminThemeService
    {
        private const string CacheKey = "AdminThemeName";

        private readonly IExtensionManager _extensionManager;
        private readonly ISiteService _siteService;
        private readonly IMemoryCache _memoryCache;

        public AdminThemeService(
            ISiteService siteService,
            IExtensionManager extensionManager,
            IMemoryCache memoryCache
            )
        {
            _siteService = siteService;
            _extensionManager = extensionManager;
            _memoryCache = memoryCache;
        }

        public async Task<ExtensionDescriptor> GetAdminThemeAsync()
        {
            string currentThemeName = await GetAdminThemeNameAsync();
            if (String.IsNullOrEmpty(currentThemeName))
            {
                return null;
            }

            return _extensionManager.GetExtension(currentThemeName);
        }

        public async Task SetAdminThemeAsync(string themeName)
        {
            var site = await _siteService.GetSiteSettingsAsync();
            (site as IContent).ContentItem.Content.CurrentAdminThemeName = themeName;
            _memoryCache.Set(CacheKey, themeName);
            await _siteService.UpdateSiteSettingsAsync(site);
        }

        public async Task<string> GetAdminThemeNameAsync()
        {
            string themeName;
            if(!_memoryCache.TryGetValue(CacheKey, out themeName))
            {
                var site = await _siteService.GetSiteSettingsAsync();
                themeName = (string)(site as IContent).ContentItem.Content.CurrentAdminThemeName;
                _memoryCache.Set(CacheKey, themeName);
            }

            return themeName;
        }
    }
}
