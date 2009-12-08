using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Request;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles
{
    public class TemporaryDumpingGround
    {
        /// <summary>
        /// This method downloads a storage object from cloudfiles
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageObjectName">The name of the storage object to retrieve</param>
        /// <returns>An instance of StorageObject with the stream containing the bytes representing the desired storage object</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public StorageObject GetStorageObject(string storageObjectName)
        {
            Ensure.NotNullOrEmpty(storageObjectName);
            throw new  NotImplementedException();//my commenting and exception: Ryan; this is a temporary spot for refactoring
          //  return GetStorageObject(storageObjectName, new Dictionary<RequestHeaderFields, string>());
        }

        /// <summary>
        /// An alternate method for downloading storage objects from cloudfiles directly to a file name specified in the method
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteStorageItem.txt", "C:\Local\File\Path\file.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageObjectName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The file name to save the storage object into on disk</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void GetStorageObject(string storageObjectName, string localFileName)
        {
            Ensure.NotNullOrEmpty(storageObjectName, localFileName);

            Ensure.ValidStorageObjectName(storageObjectName);

            GetStorageObject(storageObjectName, localFileName, new Dictionary<RequestHeaderFields, string>());
        }

        /// <summary>
        /// An alternate method for downloading storage objects from cloudfiles directly to a file name specified in the method
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// Dictionary{RequestHeaderFields, string} requestHeaderFields = Dictionary{RequestHeaderFields, string}();
        /// string dummy_etag = "5c66108b7543c6f16145e25df9849f7f";
        /// requestHeaderFields.Add(RequestHeaderFields.IfMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfNoneMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfModifiedSince, DateTime.Now.AddDays(6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.IfUnmodifiedSince, DateTime.Now.AddDays(-6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.Range, "0-5");
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteFileName.txt", "C:\Local\File\Path\file.txt", requestHeaderFields);
        /// </code>
        /// </example>
        /// <param name="storageObjectName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The file name to save the storage object into on disk</param>
        /// <param name="requestHeaderFields">A dictionary containing the special headers and their values</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void GetStorageObject(string storageObjectName, string localFileName, Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {

            Ensure.NotNullOrEmpty(storageObjectName, localFileName);
            Ensure.ValidStorageObjectName(storageObjectName);
            throw new AbandonedMutexException();
            //this will become the core class for file transfer, sync calls will call this as well.
//
//            var getStorageItem = new GetStorageObject(StorageUrl, containerName, storageObjectName, requestHeaderFields);
//
//            try
//            {
//                var getStorageItemResponse = _requestfactory.Submit(getStorageItem, AuthToken, _usercreds.ProxyCredentials);
//                foreach (var callback in callbackFuncs)
//                {
//                    getStorageItemResponse.Progress += callback;
//                }
//                var stream = getStorageItemResponse.GetResponseStream();
//
//                StoreFile(localFileName, stream);
//            }
//            catch (WebException we)
//            {
//
//                var response = (HttpWebResponse)we.Response;
//                response.Close();
//                if (response.StatusCode == HttpStatusCode.NotFound)
//                    throw new StorageItemNotFoundException("The requested storage object does not exist");
//
//                throw;
//            }
        
        
        }
        
        /// <summary>
        /// This method retrieves meta information and size, in bytes, of a requested storage object
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// StorageObject storageItem = connection.GetStorageItemInformation("container name", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="storageObjectName">The name of the storage object</param>
        /// <returns>An instance of StorageObject containing the byte size and meta information associated with the container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public StorageObjectInformation GetStorageItemInformation(string storageObjectName)
        {
            //May not need this one, moving it over, but it will likely be replaced
            throw new NotImplementedException();
//            Ensure.NotNullOrEmpty(containerName, storageObjectName);
//            try
//            {
//                var getStorageItemInformation = new GetStorageItemInformation(StorageUrl, containerName, storageObjectName);
//                var getStorageItemInformationResponse = _requestfactory.Submit(getStorageItemInformation, AuthToken, _usercreds.ProxyCredentials);
//
//
//                var storageItemInformation = new StorageObjectInformation(getStorageItemInformationResponse.Headers);
//
//                return storageItemInformation;
//            }
//            catch (WebException we)
//            {
//
//                var response = (HttpWebResponse)we.Response;
//                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
//                    throw new StorageItemNotFoundException("The requested storage object does not exist");
//
//                throw;
//            }
        }
        /// <summary>
        /// This method downloads a storage object from cloudfiles asychronously
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
        /// Dictionary{RequestHeaderFields, string} requestHeaderFields = Dictionary{RequestHeaderFields, string}();
        /// string dummy_etag = "5c66108b7543c6f16145e25df9849f7f";
        /// requestHeaderFields.Add(RequestHeaderFields.IfMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfNoneMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfModifiedSince, DateTime.Now.AddDays(6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.IfUnmodifiedSince, DateTime.Now.AddDays(-6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.Range, "0-5");
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// connection.AddProgressWatcher(fileTransferProgress);
        /// connection.OperationComplete += transferComplete;
        /// connection.GetStorageObjectAsync("container name", "RemoteStorageItem.txt", "RemoteStorageItem.txt", requestHeaderFields);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageItemName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The name to write the file to on your hard drive. </param>
        /// <param name="requestHeaderFields">A dictionary containing the special headers and their values</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void GetStorageObjectAsync(string containerName, string storageItemName, string localFileName, Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {
            //will decide if I use this or not
            /*
            var thread = new Thread(
                 () =>
                 {
                     try
                     {
                         GetStorageObject(containerName, storageItemName, localFileName, requestHeaderFields);
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
             * */
        }

        /// <summary>
        /// This method downloads a storage object from cloudfiles asychronously
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
        /// IConnection connection = new Account(userCredentials);
        /// connection.AddProgressWatcher(fileTransferProgress);
        /// connection.OperationComplete += transferComplete;
        /// connection.GetStorageObjectAsync("container name", "RemoteStorageItem.txt", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageItemName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The name to write the file to on your hard drive. </param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void GetStorageObjectAsync(string containerName, string storageItemName, string localFileName)
        {
            //will likely not use
//            var thread = new Thread(
//                 () =>
//                 {
//                     try
//                     {
//                         GetStorageObject(containerName, storageItemName, localFileName);
//                     }
//                    finally //Always fire the completed event
//                     {
//                         if (OperationComplete != null)
//                         {
//                             //Fire the operation complete event if there aren't any listeners
//                             OperationComplete();
//                         }
//                     }
//                 }
//             );
//            thread.Start();
        }

        /// <summary>
        /// An alternate method for downloading storage objects. This one allows specification of special HTTP 1.1 compliant GET headers
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials); 
        /// Dictionary{RequestHeaderFields, string} requestHeaderFields = Dictionary{RequestHeaderFields, string}();
        /// string dummy_etag = "5c66108b7543c6f16145e25df9849f7f";
        /// requestHeaderFields.Add(RequestHeaderFields.IfMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfNoneMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfModifiedSince, DateTime.Now.AddDays(6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.IfUnmodifiedSince, DateTime.Now.AddDays(-6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.Range, "0-5");
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteStorageItem.txt", requestHeaderFields);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object</param>
        /// <param name="storageObjectName">The name of the storage object</param>
        /// <param name="requestHeaderFields">A dictionary containing the special headers and their values</param>
        /// <returns>An instance of StorageObject with the stream containing the bytes representing the desired storage object</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public StorageObject GetStorageObject(string storageObjectName, Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {
            //will get this working but may implement differently
            
            Ensure.NotNullOrEmpty(storageObjectName);
            Ensure.ValidStorageObjectName(storageObjectName);

//            try
//            {
//                var getStorageItem = new GetStorageObject(StorageUrl, containerName, storageObjectName, requestHeaderFields);
//                var getStorageItemResponse = _requestfactory.Submit(getStorageItem, AuthToken, _usercreds.ProxyCredentials);
//
//
//                var metadata = GetMetadata(getStorageItemResponse);
//                var storageItem = new StorageObject(TODO, containerName, storageObjectName, metadata, getStorageItemResponse.ContentType, getStorageItemResponse.GetResponseStream(), getStorageItemResponse.ContentLength, getStorageItemResponse.LastModified);
//                //                getStorageItemResponse.Dispose();
//                return storageItem;
//            }
//            catch (WebException we)
//            {
//
//
//                var response = (HttpWebResponse)we.Response;
//                response.Close();
//                if (response.StatusCode == HttpStatusCode.NotFound)
//                    throw new StorageItemNotFoundException("The requested storage object does not exist");
//
//                throw;
//            }
            throw new AbandonedMutexException();
        }

    }
}