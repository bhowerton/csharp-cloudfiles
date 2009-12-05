///
/// See COPYING file for licensing information
///

using System;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.domain
{
    /// <summary>
    /// UserCredentials
    /// </summary>
    public class UserCredentials
    {
        private readonly Uri authUrl;
        private readonly string username;
        private readonly string api_access_key;
        private readonly string cloudversion;
        private readonly string accountName;
        private readonly HttpProxy httpProxy;

        /// <summary>
        /// Constructor - defaults Auth Url to https://api.mosso.com/auth without proxy credentials
        /// </summary>
        /// <param name="username">client username to use during authentication</param>
        /// <param name="api_access_key">client api access key to use during authentication</param>
        public UserCredentials(string username, string api_access_key) :
            this(new Uri(Constants.AUTH_URL), username, api_access_key, null, null)
        {
        }

        /// <summary>
        /// Constructor - defaults Auth Url to https://api.mosso.com/auth with proxy credentials
        /// </summary>
        /// <param name="username">client username to use during authentication</param>
        /// <param name="api_access_key">client api access key to use during authentication</param>
        /// <param name="httpProxy">credentials to use to obtain access via proxy</param>
        public UserCredentials(string username, string api_access_key, HttpProxy httpProxy) :
            this(new Uri(Constants.AUTH_URL), username, api_access_key, null, null, httpProxy)
        {
        }

        /// <summary>
        /// UserCredential constructor
        /// </summary>
        /// <param name="authUrl">url to authenticate against</param>
        /// <param name="username">client username to use during authentication</param>
        /// <param name="api_access_key">client api access key to use during authentication</param>
        /// <param name="cloudversion">version of the cloudfiles system to access</param>
        /// <param name="accountName">client account name</param>
        /// <param name="httpProxy">credentials to use to obtain access via proxy</param>
        /// <returns>An instance of UserCredentials</returns>
        public UserCredentials(Uri authUrl, string username, string api_access_key, string cloudversion, string accountName, HttpProxy httpProxy)
        {
            this.authUrl = authUrl;
            this.username = username;
            this.api_access_key = api_access_key;
            this.accountName = accountName;
            this.cloudversion = cloudversion;
            this.httpProxy = httpProxy;
        }

        /// <summary>
        /// UserCredential constructor
        /// </summary>
        /// <param name="authUrl">url to authenticate against</param>
        /// <param name="username">client username to use during authentication</param>
        /// <param name="api_access_key">client api access key to use during authentication</param>
        /// <param name="cloudversion">version of the cloudfiles system to access</param>
        /// <param name="accountname">client account name</param>
        /// <returns>An instance of UserCredentials</returns>
        public UserCredentials(Uri authUrl, string username, string api_access_key, string cloudversion, string accountname)
            : this(authUrl, username, api_access_key, cloudversion, accountname, null)
        {
        }

        /// <summary>
        /// Proxy Credentials
        /// </summary>
        /// <returns>An instance of the local proxy credentials</returns>
        public HttpProxy HttpProxy
        {
            get { return httpProxy; }
        }


        /// <summary>
        /// api access key to use for authentication
        /// </summary>
        /// <returns>a string representation of the api access key used to authenticate against the user's account</returns>
        public string Api_access_key
        {
            get { return api_access_key; }
        }


        /// <summary>
        /// username to use for authentication
        /// </summary>
        /// <returns>a string representation of the account name used to authenticate against the user's account</returns>
        public string AccountName
        {
            get { return accountName; }
        }

        /// <summary>
        /// the url to authenticate against
        /// </summary>
        /// <returns>a Uri instance having the url for authentication</returns>
        public Uri AuthUrl
        {
            get { return authUrl; }
        }

        /// <summary>
        /// version of the cloudfiles system
        /// </summary>
        /// <returns>a string representation of the cloudfiles system version</returns>
        public string Cloudversion
        {
            get { return cloudversion; }
        }

        /// <summary>
        /// username to use for authentication
        /// </summary>
        /// <returns>a string representation of the username used to authenticate against the user's account</returns>
        public string Username
        {
            get { return username; }
        }
    }
}