// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Runtime.Caching;

namespace CTS.SmartEngg.Framework
{
    /// <summary>
    /// Class for Cache Manager
    /// </summary>
    public class CacheManager : ICacheService
    {
        /// <summary>
        /// Method to get or create item in cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="getItemCallback"></param>
        /// <param name="cacheDuration"></param>
        /// <returns></returns>
        public T GetOrCreate<T>(string cacheKey, Func<T> getItemCallback, CacheDuration cacheDuration) where T : class
        {
            T item = MemoryCache.Default.Get(cacheKey) as T;
            if (item == null)
            {
                item = getItemCallback();
                MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes((int)cacheDuration));
            }
            return item;
        }

        /// <summary>
        /// Method to clear the cache item
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool Clear(string cacheKey)
        {
            if (MemoryCache.Default.Get(cacheKey) != null)
            {
                MemoryCache.Default.Remove(cacheKey);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method to check key is exits in caching
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool IsExists(string cacheKey)
        {
            if (MemoryCache.Default.Get(cacheKey) != null)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Caching Interface 
    /// </summary>
    internal interface ICacheService
    {
        T GetOrCreate<T>(string cacheKey, Func<T> getItemCallback, CacheDuration cacheDuration) where T : class;

        bool Clear(string cacheKey);

        bool IsExists(string cacheKey);
    }

    /// <summary>
    /// Enum to cache duration
    /// </summary>
    public enum CacheDuration
    {
        Short = 15,
        Medium = 20,
        Long = 30
    }
}