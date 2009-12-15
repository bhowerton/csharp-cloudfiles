///
/// See COPYING file for licensing information
///

using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Rackspace.CloudFiles
{
    /// <summary>
    /// StorageObject
    /// </summary>
    public class StorageObject
    {
        private readonly IHttpReaderWriter _readerwriter;
        private readonly IContainer _container;

        private readonly string objectName;

        private readonly string objectContentType;
        private readonly long contentLength;
        private readonly DateTime lastModified;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="objectContentType"></param>
        /// <param name="contentLength"></param>
        /// <param name="lastModified"></param>
        /// <param name="readerwriter"></param>
        /// <param name="container"></param>
        public StorageObject(IHttpReaderWriter readerwriter, IContainer container, string objectName,

                             string objectContentType,
                             long contentLength,
                             DateTime lastModified,
                             string etag)
        {
            _readerwriter = readerwriter;
            _container = container;
            this.ETag = etag;
            this.objectName = objectName;
            this.lastModified = lastModified;
            this.contentLength = contentLength;
            this.objectContentType = objectContentType;


        }
        public StorageObject(IContainer container, string objectName, string objectContentLength,long contentLength ,
            DateTime lastmodified, string etag):
            this(new HttpReaderWriter(), container, objectName, objectContentLength, contentLength,lastmodified, etag)
        {
            
        }


        /// <summary>
        /// 
        /// </summary>
        public long ContentLength
        {
            get { return contentLength; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ContentType
        {
            get { return objectContentType; }
        }



        public string ETag
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string RemoteName
        {
            get { return objectName; }
        }

        public DateTime LastModified
        {
            get { return lastModified; }
        }
        /// <summary>
        /// This method uploads a storage object to cloudfiles with meta tags
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// connection.PutStorageObject("container name", "C:\Local\File\Path\file.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="localFilePath">The complete file path of the storage object to be uploaded</param>
        /// <param name="metadata">An optional parameter containing a dictionary of meta tags to associate with the storage object</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void SendToCloud(string localFilePath, Dictionary<string, string> metadata)
        {

            Ensure.NotNullOrEmpty(localFilePath);


            var remoteName = Path.GetFileName(localFilePath);
            var localName = localFilePath.Replace("/", "\\");
            SendToCloud(File.OpenRead(localName), remoteName, metadata, (l, d) => { });

        }
        public void SendToCloud(Stream streamToRead, string remotename, Dictionary<string, string> metadata)
        {
            SendToCloud(streamToRead, remotename, metadata, (i, o) => { });
        }
        public void SendToCloud(Stream streamToRead, string remotename, Dictionary<string, string> metadata,
            Action<long, long> progressevent)
        {
            var request = _container.Connection.CreateRequest();
            request.Method = HttpVerb.PUT;


            if (metadata != null && metadata.Count > 0)
            {
                foreach (var s in metadata.Keys)
                {
                    request.Headers.Add(Constants.META_DATA_HEADER + s, metadata[s]);
                }
            }
            string fileurl = CleanUpFilePath(remotename);
            request.ContentType = _contentType(fileurl);
            request.AllowWriteStreamBuffering = false;
    
            var webResponse =
                request.SubmitStorageRequest(_container.Name.Encode() + "/" + remotename.StripSlashPrefix().Encode(),
                req => _readerwriter.WriteRequest(req, streamToRead, progressevent),
                res =>
                    {
                        //do nothing with response
                    });


            if (webResponse.Status == HttpStatusCode.BadRequest)
                throw new ContainerNotFoundException("The requested container does not exist");
            if (webResponse.Status == HttpStatusCode.PreconditionFailed)
                throw new PreconditionFailedException("Precondition Failed");
        }
        private string CleanUpFilePath(string filePath)
        {
            return filePath.StripSlashPrefix().Replace(@"file:\\\", "");
        }
        /// <summary>
        /// the content type of the storage item
        /// </summary>
        /// <returns>string representation of the storage item's content type</returns>
        private string _contentType(string fileUrl)
        {

            if (String.IsNullOrEmpty(fileUrl) || fileUrl.IndexOf(".") < 0) return "application/octet-stream";
            return MimeType(fileUrl);

        }
        private string MimeType(string filename)
        {
            var extension = Path.GetExtension(filename).ToLower();

            string mimetype = "";
            Constants.ExtensionToMimeTypeMap.TryGetValue(extension, out mimetype);
            return mimetype ?? "application/octet-stream";
        }
        /// <summary>
        /// </summary>
        /// <param name="streamToWriteTo">does not close the stream you are responsible yourself</param>
        public void SaveToDisk(Stream streamToWriteTo)
        {


            SaveToDisk(streamToWriteTo, (i, d) => { });

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filelocation"></param>
        public void SaveToDisk(string filelocation)
        {
            using (var fs = new FileStream(filelocation, FileMode.Create))
            {
                SaveToDisk(fs);
            }

        }

        public void SaveToDisk(Stream streamToWriteTo, Action<long, long> progressevent)
        {
            var request = _container.Connection.CreateRequest();
            request.Method = HttpVerb.GET;
            Action<HttpWebResponse> del = response =>
            {
                using (var responseStream = response.GetResponseStream())
                {
                    byte[] buffer = new byte[4096];

                    int amt = 0;
                    long totalread = 0;
                    while ((amt = responseStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        totalread += amt;
                        streamToWriteTo.Write(buffer, 0, amt);
                        progressevent.Invoke(totalread, responseStream.Length);
                    }

                }
            };

            var cfresponse = request.SubmitStorageRequest(_container.Name + "/" + RemoteName, req => { }, del);
        }
    }
}