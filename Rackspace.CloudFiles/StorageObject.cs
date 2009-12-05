///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.Request;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles
{
    /// <summary>
    /// StorageObject
    /// </summary>
    public class StorageObject : IDisposable
    {
        public event ProgressCallback Progress;
        private readonly IAuthenticatedRequestFactory _authenticatedRequestFactory;
        private readonly string _containerName;
        private readonly string objectName;
        private readonly Dictionary<string, string> metadata;
        private readonly string objectContentType;
        private readonly Stream objectStream;
        private readonly long contentLength;
        private readonly DateTime lastModified;
        public event OperationCompleteCallback OperationComplete;
        private List<ProgressCallback> callbackFuncs;
        public delegate void OperationCompleteCallback();
        public void AddProgressWatcher(ProgressCallback progressCallback)
        {
            callbackFuncs.Add(progressCallback);
        }
        public delegate void ProgressCallback(int bytesWritten);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="objectName"></param>
        /// <param name="metadata"></param>
        /// <param name="objectContentType"></param>
        /// <param name="contentLength"></param>
        /// <param name="lastModified"></param>
        /// <param name="authenticatedRequestFactory"></param>
        public StorageObject(IAuthenticatedRequestFactory authenticatedRequestFactory, string containerName,string objectName, Dictionary<string, string> metadata, string objectContentType, long contentLength, DateTime lastModified):
            this(authenticatedRequestFactory, containerName, objectName, metadata, objectContentType, null, contentLength, lastModified)
        {
          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="objectName"></param>
        /// <param name="metadata"></param>
        /// <param name="objectContentType"></param>
        /// <param name="contentStream"></param>
        /// <param name="contentLength"></param>
        /// <param name="lastModified"></param>
        /// <param name="authenticatedRequestFactory"></param>
        public StorageObject(IAuthenticatedRequestFactory authenticatedRequestFactory, string containerName, string objectName, Dictionary<string, string> metadata, string objectContentType, Stream contentStream, long contentLength, DateTime lastModified)
        {
            _authenticatedRequestFactory = authenticatedRequestFactory;
            _containerName = containerName;
            this.objectName = objectName;
            this.lastModified = lastModified;
            this.contentLength = contentLength;
            this.objectContentType = objectContentType;
            this.metadata = metadata;
            if(contentStream!=null)
                objectStream = contentStream;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (objectStream != null)
                objectStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public long FileLength
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

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Metadata
        {
            get { return metadata; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Stream ObjectStream
        {
            get { return objectStream; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ObjectName
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
        /// IConnection connection = new Connection(userCredentials);
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
        public void Send(string localFilePath, Dictionary<string, string> metadata)
        {
            Ensure.NotNullOrEmpty(localFilePath);
            

            try
            {
                var remoteName = Path.GetFileName(localFilePath);
                var localName = localFilePath.Replace("/", "\\");
                var request = _authenticatedRequestFactory.CreateRequest();
                request.Method = HttpVerb.PUT;


                if (metadata != null && metadata.Count > 0)
                {
                    foreach (var s in metadata.Keys)
                    {
                        request.Headers.Add(Constants.META_DATA_HEADER + s, metadata[s]);
                    }
                }

                request.AllowWriteStreamBuffering = false;
                string fileurl = CleanUpFilePath(remoteName);
                request.ContentType = _contentType(fileurl);
                //dirty hack FIXME and refactory
                var filetosend = File.OpenRead(localName);
                request.SetContent(filetosend, Progress);
                foreach (var callback in callbackFuncs)
                {
                    Progress += callback;
                }
                request.SubmitStorageRequest(_containerName.Encode() + "/" + remoteName.StripSlashPrefix().Encode());
            }
            catch (WebException webException)
            {


                var webResponse = (HttpWebResponse)webException.Response;
                if (webResponse == null) throw;
                if (webResponse.StatusCode == HttpStatusCode.BadRequest)
                    throw new ContainerNotFoundException("The requested container does not exist");
                if (webResponse.StatusCode == HttpStatusCode.PreconditionFailed)
                    throw new PreconditionFailedException(webException.Message);

                throw;
            }
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
        /// This method uploads a storage object to cloudfiles
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.PutStorageObject("container name", "C:\Local\File\Path\file.txt");
        /// </code>
        /// </example>
      
        /// <param name="localFilePath">The complete file path of the storage object to be uploaded</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void PutStorageObject(string localFilePath)
        {

            throw new NotImplementedException();
           // PutStorageObject( localFilePath, new Dictionary<string, string>());
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles with an alternate name
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// FileInfo file = new FileInfo("C:\Local\File\Path\file.txt");
        /// connection.PutStorageObject("container name", file.Open(FileMode.Open), "RemoteFileName.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="remoteStorageItemName">The alternate name as it will be called on cloudfiles</param>
        /// <param name="storageStream">The stream representing the storage item to upload</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void PutStorageObject( Stream storageStream, string remoteStorageItemName)
        {
            PutStorageObject( storageStream, remoteStorageItemName, new Dictionary<string, string>());
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles asychronously
        /// </summary>
        /// <example>
        /// <code>
        /// private void transferComplete()
        /// {
        ///     if (InvokeRequired)
        ///     {
        ///         Invoke(new CloseCallback(Close), new object[]{});
        ///     }
        ///     else
        ///     {
        ///         if (!IsDisposed)
        ///             Close();
        ///     }
        /// }
        /// 
        /// private void fileTransferProgress(int bytesTransferred)
        /// {
        ///    if (InvokeRequired)
        ///    {
        ///        Invoke(new FileProgressCallback(fileTransferProgress), new object[] {bytesTransferred});
        ///    }
        ///    else
        ///    {
        ///        System.Console.WriteLine(totalTransferred.ToString());
        ///        totalTransferred += bytesTransferred;
        ///        bytesTransferredLabel.Text = totalTransferred.ToString();
        ///        var progress = (int) ((totalTransferred/filesize)*100.0f);
        ///        if(progress > 100)
        ///            progress = 100;
        ///        transferProgressBar.Value = progress ;
        ///    }
        /// }
        /// 
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.AddProgressWatcher(fileTransferProgress);
        /// connection.OperationComplete += transferComplete;
        /// connection.PutStorageObjectAsync("container name", "RemoteStorageItem.txt", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="remoteStorageItemName">The alternate name as it will be called on cloudfiles</param>
        /// <param name="storageStream">The stream representing the storage item to upload</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void PutStorageObjectAsync(string containerName, Stream storageStream, string remoteStorageItemName)
        {
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        PutStorageObject( storageStream, remoteStorageItemName);
                    }
                        finally  //Always fire the completed event
                    {
                        if (OperationComplete != null)
                        {
                            //Fire the operation complete event if there are any listeners
                            OperationComplete();
                        }
                    }
                }
                );
            thread.Start();
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles asychronously
        /// </summary>
        /// <example>
        /// <code>
        /// private void transferComplete()
        /// {
        ///     if (InvokeRequired)
        ///     {
        ///         Invoke(new CloseCallback(Close), new object[]{});
        ///     }
        ///     else
        ///     {
        ///         if (!IsDisposed)
        ///             Close();
        ///     }
        /// }
        /// 
        /// private void fileTransferProgress(int bytesTransferred)
        /// {
        ///    if (InvokeRequired)
        ///    {
        ///        Invoke(new FileProgressCallback(fileTransferProgress), new object[] {bytesTransferred});
        ///    }
        ///    else
        ///    {
        ///        System.Console.WriteLine(totalTransferred.ToString());
        ///        totalTransferred += bytesTransferred;
        ///        bytesTransferredLabel.Text = totalTransferred.ToString();
        ///        var progress = (int) ((totalTransferred/filesize)*100.0f);
        ///        if(progress > 100)
        ///            progress = 100;
        ///        transferProgressBar.Value = progress ;
        ///    }
        /// }
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// connection.PutStorageObjectAsync("container name", "LocalFileName.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="localStorageItemName">The name of the file locally </param>
        /// <param name="metadata">An optional parameter containing a dictionary of meta tags to associate with the storage object</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void PutStorageItemAsync( string localStorageItemName, Dictionary<string, string> metadata)
        {
            var thread = new Thread(
                () =>
                {
                    //needs fundamentatal rewrite
                    throw new NotImplementedException();
//                    try
//                    {
//                        PutStorageObject( ,localStorageItemName, metadata);
//                    }
//                    finally //Always fire the completed event
//                    {
//                        if (OperationComplete != null)
//                        {
//                            //Fire the operation complete event if there aren't any listeners
//                            OperationComplete();
//                        }
//                    }
                }
            );
            thread.Start();
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles asychronously
        /// </summary>
        /// <example>
        /// <code>
        /// private void transferComplete()
        /// {
        ///     if (InvokeRequired)
        ///     {
        ///         Invoke(new CloseCallback(Close), new object[]{});
        ///     }
        ///     else
        ///     {
        ///         if (!IsDisposed)
        ///             Close();
        ///     }
        /// }
        /// 
        /// private void fileTransferProgress(int bytesTransferred)
        /// {
        ///    if (InvokeRequired)
        ///    {
        ///        Invoke(new FileProgressCallback(fileTransferProgress), new object[] {bytesTransferred});
        ///    }
        ///    else
        ///    {
        ///        System.Console.WriteLine(totalTransferred.ToString());
        ///        totalTransferred += bytesTransferred;
        ///        bytesTransferredLabel.Text = totalTransferred.ToString();
        ///        var progress = (int) ((totalTransferred/filesize)*100.0f);
        ///        if(progress > 100)
        ///            progress = 100;
        ///        transferProgressBar.Value = progress ;
        ///    }
        /// }
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// FileInfo file = new FileInfo("C:\Local\File\Path\file.txt");
        /// connection.PutStorageObjectAsync("container name", file.Open(FileMode.Open), "RemoteFileName.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="remoteStorageItemName">The alternate name as it will be called on cloudfiles</param>
        /// <param name="storageStream">The stream representing the storage item to upload</param>
        /// <param name="metadata">An optional parameter containing a dictionary of meta tags to associate with the storage object</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void PutStorageItemAsync(Stream storageStream, string remoteStorageItemName, Dictionary<string, string> metadata)
        {
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        PutStorageObject( storageStream, remoteStorageItemName, metadata);
                    }
                    finally
                    {
                        if (OperationComplete != null)
                        {
                            //Fire the operation complete event if there are any listeners
                            OperationComplete();
                        }
                    }
                }
                );
            thread.Start();
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles asychronously
        /// </summary>
        /// <example>
        /// <code>
        /// private void transferComplete()
        /// {
        ///     if (InvokeRequired)
        ///     {
        ///         Invoke(new CloseCallback(Close), new object[]{});
        ///     }
        ///     else
        ///     {
        ///         if (!IsDisposed)
        ///             Close();
        ///     }
        /// }
        /// 
        /// private void fileTransferProgress(int bytesTransferred)
        /// {
        ///    if (InvokeRequired)
        ///    {
        ///        Invoke(new FileProgressCallback(fileTransferProgress), new object[] {bytesTransferred});
        ///    }
        ///    else
        ///    {
        ///        System.Console.WriteLine(totalTransferred.ToString());
        ///        totalTransferred += bytesTransferred;
        ///        bytesTransferredLabel.Text = totalTransferred.ToString();
        ///        var progress = (int) ((totalTransferred/filesize)*100.0f);
        ///        if(progress > 100)
        ///            progress = 100;
        ///        transferProgressBar.Value = progress ;
        ///    }
        /// }
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.PutStorageObjectAsync("container name", "LocalFileName.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="localStorageItemName">The name of the file locally </param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void PutStorageObjectAsync(string containerName, string localStorageItemName)
        {
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        throw new NotImplementedException();
                        //needs fundamental rewrite
                     //   PutStorageObject(containerName, localStorageItemName);
                    }
                    finally //Always fire the completed event
                    {
                        if (OperationComplete != null)
                        {
                            //Fire the operation complete event if there aren't any listeners
                            OperationComplete();
                        }
                    }
                }
            );
            thread.Start();
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles with an alternate name
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// FileInfo file = new FileInfo("C:\Local\File\Path\file.txt");
        /// connection.PutStorageObject("container name", file.Open(FileMode.Open), "RemoteFileName.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="storageStream">The file stream to upload</param>
        /// <param name="metadata">An optional parameter containing a dictionary of meta tags to associate with the storage object</param>
        /// <param name="remoteStorageItemName">The name of the storage object as it will be called on cloudfiles</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void PutStorageObject(Stream storageStream, string remoteStorageItemName, Dictionary<string, string> metadata)
        {
            try
            {
               //entire storageobect save, put, etc needs complete rewrite
          //      foreach (var callback in callbackFuncs)
               // {
            //        putStorageItem.Progress += callback;
              //  }
              //  var request = _authenticatedRequestFactory.CreateRequest();
               // request.SubmitStorageRequest()
            }
            catch (WebException webException)
            {


                var webResponse = (HttpWebResponse)webException.Response;
                if (webResponse == null) throw;
                if (webResponse.StatusCode == HttpStatusCode.BadRequest)
                    throw new ContainerNotFoundException("The requested container does not exist");
                if (webResponse.StatusCode == HttpStatusCode.PreconditionFailed)
                    throw new PreconditionFailedException(webException.Message);

                throw;
                //following exception is cause when status code is 422 (unprocessable entity)
                //unfortunately, the HttpStatusCode enum does not have that value
                //throw new InvalidETagException("The ETag supplied in the request does not match the ETag calculated by the server");
            }
        }
    }
}