using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Jil;
using Nancy.Extensions;
using Nancy.ModelBinding;

namespace Nancy.Serialization.Jil
{
    /// <summary>
    /// A <see cref="IBodyDeserializer"/> implementation based on <see cref="Jil"/>.
    /// </summary>
    public class JilBodyDeserializer : IBodyDeserializer
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyInfoCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Gets the de-serialization options used for the actual Jil based JSON De-Serialization, set to Options.Default by default.
        /// </summary>
        /// <value>
        /// The Jil De-Serialization <see cref="Options"/>.
        /// </value>
        public static volatile Options Options = Options.Default;
        
        #region Implementation of IBodyDeserializer

        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param><param name="context">Current <see cref="T:Nancy.ModelBinding.BindingContext"/>.</param>
        /// <returns>
        /// True if supported, false otherwise
        /// </returns>
        public bool CanDeserialize(string contentType, BindingContext context)
        {
            return Helpers.IsJsonType(contentType);
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param><param name="bodyStream">Request body stream</param><param name="context">Current <see cref="T:Nancy.ModelBinding.BindingContext"/>.</param>
        /// <returns>
        /// Model instance
        /// </returns>
        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            object deserializedObject;
            using (var inputStream = new StreamReader(bodyStream))
            {
                // deserialize json
                deserializedObject = JSON.Deserialize(inputStream, context.DestinationType);

                // .. then, due to NancyFx's support for blacklisted properties, we need to get the propertyInfo first (read from cache if possible)
                PropertyInfo[] propertyInfo;
                if (PropertyInfoCache.TryGetValue(context.DestinationType, out propertyInfo) == false)
                {
                    propertyInfo = context.DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    // the following is somewhat dirty but
                    SpinWait.SpinUntil(() => PropertyInfoCache.ContainsKey(context.DestinationType) || PropertyInfoCache.TryAdd(context.DestinationType, propertyInfo));
                }

                // ... and then compare whether there's anything blacklisted
                if (propertyInfo.Except(context.ValidModelProperties).Any())
                {
                    // .. if so, take object and basically eradicated value(s) for the blacklisted properties.
                    // this is inspired by https://raw.githubusercontent.com/NancyFx/Nancy.Serialization.JsonNet/master/src/Nancy.Serialization.JsonNet/JsonNetBodyDeserializer.cs
                    // but again.. only *inspired*.
                    // The main difference is, that the instance Jil returned from the JSON.Deserialize() call will be wiped clean, no second/new instance will be created.
                    return CleanBlacklistedProperties(context, deserializedObject, propertyInfo);
                }

                return deserializedObject;
            }
        }

        #endregion

        /// <summary>
        /// Cleans the blacklisted properties from the <see cref="deserializedObject"/>.
        /// If it is a an <see cref="IEnumerable"/>, the blacklisted properties from each element are cleaned.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deserializedObject">The deserialized object.</param>
        /// <param name="cachedPropertyInfo">The cached property information.</param>
        /// <returns></returns>
        private static object CleanBlacklistedProperties(BindingContext context, object deserializedObject, PropertyInfo[] cachedPropertyInfo)
        {
            if (context.DestinationType.IsEnumerable())
            {
                foreach (var enumerableElement in (IEnumerable)deserializedObject)
                {
                    CleanPropertyValues(context, enumerableElement, cachedPropertyInfo);
                }
            }
            else
            {
                CleanPropertyValues(context, deserializedObject, cachedPropertyInfo);
            }

            return deserializedObject;
        }

        /// <summary>
        /// Cleans the property values for the provided <see cref="deserializedObject"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deserializedObject">The deserialized object.</param>
        /// <param name="cachedPropertyInfo">The cached property information.</param>
        /// <param name="targetValue">The target value.</param>
        private static void CleanPropertyValues(BindingContext context, object deserializedObject, IEnumerable<PropertyInfo> cachedPropertyInfo, object targetValue = null)
        {
            foreach (var blacklistedProperty in cachedPropertyInfo.Except(context.ValidModelProperties))
            {
                blacklistedProperty.SetValue(deserializedObject, targetValue);
            }
        }
    }
}