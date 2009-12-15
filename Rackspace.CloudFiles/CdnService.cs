using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles
{
    public class CdnService
    {
        private readonly IAccount _account;

        public CdnService(IAccount account)
        {
            _account = account;
        }

        public IList<PublicContainer> GetContainers()
        {
            throw new NotImplementedException();
        }
        public Container MakeContainerPrivate(PublicContainer publicContainer)
        {
            throw new NotImplementedException();
        }

        public PublicContainer MakeContainerPublic(Container privateContainer, 
            int ttl, bool isLoggingEnabled, 
            string userAcl, string referrerAcl)
        {
            throw new NotImplementedException();
        }

        public PublicContainer MakeContainerPublic(Container privatecontainer)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// needs error checking or code restructure to make sure this is only called on public container
        /// </summary>
        /// <param containerName="loggingenabled"></param>
        /// <param containerName="ttl">valid values are </param>
        /// <param containerName="referreracl"></param>
        /// <param containerName="useragentacl"></param>
        public void SetDetailsOnPublicContainer(string containerName,
            bool loggingenabled,
            int ttl,
            string referreracl, 
            string useragentacl)
        {

            try
            {

                var request = _account.Connection.CreateRequest();
                request.Method = HttpVerb.POST;
                request.Headers.Add(Constants.X_LOG_RETENTION, loggingenabled.Capitalize());
                if (ttl > -1) request.Headers.Add(Constants.X_CDN_TTL, ttl.ToString());
                if (useragentacl != null) request.Headers.Add(Constants.X_USER_AGENT_ACL, useragentacl);
                if (useragentacl != null) request.Headers.Add(Constants.X_REFERRER_ACL, referreracl);
                request.SubmitCdnRequest(containerName.Encode());
            }
            catch (WebException we)
            {


                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Your access credentials are invalid or have expired. ");
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new PublicContainerNotFoundException("The specified container does not exist.");
                throw;
            }



        }
    }
}