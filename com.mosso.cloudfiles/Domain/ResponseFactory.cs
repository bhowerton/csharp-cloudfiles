///
/// See COPYING file for licensing information
///

using System;
using System.Net;
using System.Text;
using Rackspace.CloudFiles.domain.request;
using Rackspace.CloudFiles.domain.response;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.utils;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.domain.response.Interfaces;

namespace Rackspace.CloudFiles.domain
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResponseFactory
    {
        ICloudFilesResponse Create(ICloudFilesRequest request);
    }

    /// <summary>
    /// ResponseFactory
    /// </summary>
    public class ResponseFactory : IResponseFactory 
    {
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ICloudFilesResponse Create(ICloudFilesRequest request)
        {
            var response = request.GetResponse();
         
            
            response.Close();
            return response;
                      
        }

    

     
    }
}