///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Request
{
    /// <summary>
    /// possible http comparison header fields to apply to this request
    /// </summary>
    public enum RequestHeaderFields
    {
        [Description("If-Match")]
        IfMatch,
        [Description("If-None-Match")]
        IfNoneMatch,
        [Description("If-Modified-Since")]
        IfModifiedSince,
        [Description("If-Unmodified-Since")]
        IfUnmodifiedSince,
        [Description("Range")]
        Range
    }

    /// <summary>
    /// GetStorageObject
    /// </summary>
    public class GetStorageObject : IAddToWebRequest
    {
        private readonly string _storageUrl;
        private readonly string _containerName;
        private readonly string _storageItemName;
        private Dictionary<RequestHeaderFields, string> _requestHeaderFields;
         

        /// <summary>
        /// GetStorageObject constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="storageItemName">the name of the storage item to add meta information too</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        /// <exception cref="InvalidContainerNameException">Thrown when the container name length exceeds the maximum container length allowed</exception>
        public GetStorageObject(string storageUrl, string containerName, string storageItemName) :
            this(storageUrl, containerName, storageItemName, (Dictionary<RequestHeaderFields, string>) null)
        {
        }

        /// <summary>
        /// GetStorageObject constructor with http comparison header fields
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="storageObjectName">the name of the storage item to add meta information too</param>
        /// <param name="requestHeaderFields">dictionary of request header fields to apply to the request</param>
        /// <exception cref="InvalidContainerNameException">Thrown when the container name is invalid</exception>
        /// <exception cref="InvalidStorageObjectNameException">Thrown when the object name is invalid</exception>
        public GetStorageObject(string storageUrl, string containerName, string storageObjectName, 
                                Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {
            _storageUrl = storageUrl;
            _containerName = containerName;
            _storageItemName = storageObjectName;
            _requestHeaderFields = requestHeaderFields;
        }

        private void AddRequestFieldHeadersToRequestHeaders(ICollection<KeyValuePair<RequestHeaderFields, string>> requestHeaderFields,
                                                            ICloudFilesRequest request)
        {
            if (requestHeaderFields == null || requestHeaderFields.Count == 0) return;

            foreach(KeyValuePair<RequestHeaderFields, string> item in requestHeaderFields)
            {
                if(!IsSpecialRequestHeaderField(item.Key)) request.Headers.Add(EnumHelper.GetDescription(item.Key), item.Value);
                
                if (item.Key == RequestHeaderFields.IfUnmodifiedSince)
                {
                    request.Headers.Add(EnumHelper.GetDescription(item.Key), String.Format("{0:r}", ParserDateTimeHttpHeader(item.Value)));
                    continue;
                }
                if (item.Key == RequestHeaderFields.IfModifiedSince)
                {
                    request.IfModifiedSince = ParserDateTimeHttpHeader(item.Value); ;
                    continue;
                }
                if (item.Key == RequestHeaderFields.Range)
                {
                    VerifyAndSplitRangeHeader(request,item.Value);
                    continue;
                }
                
            }
        }

        private bool IsSpecialRequestHeaderField(RequestHeaderFields key)
        {
            return key == RequestHeaderFields.IfModifiedSince ||
                   key == RequestHeaderFields.Range ||
                   key == RequestHeaderFields.IfUnmodifiedSince;
        }

        private void VerifyAndSplitRangeHeader(ICloudFilesRequest request, string value)
        {
            Regex r = new Regex("^[0-9]*[-][0-9]*$");
            if (!r.IsMatch(value))
                throw new InvalidRangeHeaderException(
                    "The range must be of the format integer-integer where either integer field is optional. ");

            string [] ranged = value.Split('-');
            if (ranged.Length >= 1 && ranged[0].Length > 0)
                request.RangeFrom = int.Parse(ranged[0]);
            if (ranged.Length == 2 && ranged[1].Length > 0)
            {
                if (ranged[0].Length == 0)
                    request.RangeTo = -int.Parse(ranged[1]);
                else
                    request.RangeTo = int.Parse(ranged[1]);
            }
        }

        private DateTime ParserDateTimeHttpHeader(string value)
        {
            try
            {
                return DateTime.Parse(value, CultureInfo.CurrentCulture);    
            }
            catch(FormatException fe)
            {
                throw new DateTimeHttpHeaderFormatException("A Datetime Http Request Header Field is in incorrect format.  Format Exception:" + fe.Message);
            }
        }

//        /// <summary>
//        /// the valueof the If-Modified-Since http comparison header field 
//        /// </summary>
//        public DateTime ModifiedSince { get; private set;}
//        
//        /// <summary>
//        /// the from value of the range http comparison header field 
//        /// </summary>
//        public int RangeFrom { get; private set; }
//        
//        /// <summary>
//        /// the to value of the range http comparison header field
//        /// </summary>
//        public int RangeTo { get; private set; }

        public Uri CreateUri()
        {
            return new Uri(string.Format("{0}/{1}/{2}",
                                         _storageUrl,
                                         _containerName.Encode(),
                                         _storageItemName.Encode()));
        }

        

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "GET";
            AddRequestFieldHeadersToRequestHeaders(_requestHeaderFields, request);
        }
    }
}