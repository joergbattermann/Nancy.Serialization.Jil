using System;
using System.IO;
using Jil;
using Nancy.ModelBinding;

namespace Nancy.Serialization.Jil
{
    /// <summary>
    /// A <see cref="IBodyDeserializer"/> implementation based on <see cref="Jil"/>.
    /// </summary>
    public class JilBodyDeserializer : IBodyDeserializer
    {
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
            throw new NotImplementedException("Waiting for Jil Issue #60 to be fixed / added as feature (see https://github.com/kevin-montrose/Jil/issues/60).");
        }

        #endregion
    }
}