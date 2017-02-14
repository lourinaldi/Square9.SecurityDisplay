using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Threading;
using RestSharp;
using Square9.SecurityDisplay.Models;

namespace Square9.SecurityDisplay.Requests
{
    public sealed class UserRequests
    {

        RestClient ApiClient;
        internal UserRequests(RestClient ApiClient)
        {
            this.ApiClient = ApiClient;
        }

        public class LicenseExpiredException : Exception
        {
            public LicenseExpiredException()
            { }
            public LicenseExpiredException(String message)
                : base(message)
            { }
        }

        public class ApiException : Exception
        {
            public ApiException()
            { }
            public ApiException(String message) : base(message)
            { }
            public ApiException(String message, Exception inner) : base(message, inner)
            { }
        }

        public List<Models.SecuredGroup> GetUsersAndGroups(String Token)
        {
            String Secured = "secured";
            //var request = new RestRequest("api/dbs/" + DatabaseID + "/archives/" + ParentArchiveID + "?token=" + Token);
            //var archiveResponse = ApiClient.Execute<ArchiveList>(request);
            var request = new RestRequest("api/userAdmin/" + Secured + "?token=" + Token);
            var usersResponse = ApiClient.Execute<List<SecuredGroup>>(request);

            if (usersResponse.StatusCode != HttpStatusCode.OK)
            {
                if (usersResponse.Content.Contains("LicenseExpired"))
                {
                    throw new LicenseExpiredException(usersResponse.Content);
                }
                else
                {
                    throw new ApiException(usersResponse.Content);
                }
            }

            var SecuredUsersAndGroups = usersResponse.Data;

            return SecuredUsersAndGroups;
        }

    public List<Models.SecurityNode> GetUsersAndGroupsTree(String Token)
    {
        String Secured = "tree";
        //var request = new RestRequest("api/dbs/" + DatabaseID + "/archives/" + ParentArchiveID + "?token=" + Token);
        //var archiveResponse = ApiClient.Execute<ArchiveList>(request);
        var request = new RestRequest("api/userAdmin/" + Secured + "?token=" + Token);
        var usersResponse = ApiClient.Execute<List<SecurityNode>>(request);

        if (usersResponse.StatusCode != HttpStatusCode.OK)
        {
            if (usersResponse.Content.Contains("LicenseExpired"))
            {
                throw new LicenseExpiredException(usersResponse.Content);
            }
            else
            {
                throw new ApiException(usersResponse.Content);
            }
        }

        var SecuredUsersAndGroups = usersResponse.Data;

        return SecuredUsersAndGroups;
    }
}

public sealed class Licenses
    {
        LicensePersist tracker;

        RestClient ApiClient;
        internal Licenses(RestClient ApiClient)
        {
            this.ApiClient = ApiClient;
            tracker = new LicensePersist(this);
        }

        /// <summary>
        /// Get a License token if one is available.
        /// </summary>
        /// <param name="KeepAlive">Periodically ping the API to ensure the license token stays valid.</param>
        /// <returns>A new License.</returns>
        public License GetLicense(Boolean KeepAlive = false)
        {
            return GetLicense("", KeepAlive);
        }
        /// <summary>
        /// Get a license token if one is available, or check to see if an existing license token is still valid, and if keepalive is true, updates the license tracking information.
        /// </summary>
        /// <param name="Token">The token to check if it's still valid, an empty string will get a new license.</param>
        /// <param name="KeepAlive">Periodically ping the API to ensure the license token stays valid.</param>
        /// <returns>If no token was passed, or token was expired, this returns a new license. Otherwise it returns the passed token's license.</returns>
        public License GetLicense(String Token, Boolean KeepAlive = false)
        {
            bool licActive = !String.IsNullOrEmpty(Token);
            var request = new RestRequest("api/licenses");
            if (licActive)
            {
                tracker.StopTracking();
                request.AddParameter("Token", Token);
            }
            var license = ApiClient.Execute<License>(request);
            if (license.StatusCode != HttpStatusCode.OK)
            {
                if (license.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ApiException("Unable to get a License: The passed user is Unauthorized.");
                }
                else if (license.StatusCode == HttpStatusCode.BadRequest)
                {
                    if (licActive)
                    {
                        //license expired, unable to get new one
                        throw new LicenseExpiredException("License Revoked, unable to get a new one: " + license.Content);
                    }
                    //fix for bz4025 GA engine throws error state when all licenses are in use
                    else if (license.Content.Equals("\"All licenses are in use.\""))
                    {
                        throw new AllLicensesInUseException("Unable to get a License: All licenses are in use.");
                    }
                    else
                    {
                        throw new ApiException("Unable to get a License: " + license.Content);
                    }
                }
                else if (license.StatusCode == HttpStatusCode.NotFound || license.StatusCode == 0)
                {
                    throw new ApiException("Unable to get a License: Unable to connect to the license server, server not found. " + ApiClient.BaseUrl);
                }
                else
                {
                    throw new ApiException("Unable to get a License: " + license.Content);
                }
            }
            else if (licActive && !license.Data.Token.Equals(Token))
            {
                OnLicenseTokenChanged(new LicenseTokenChanged() { NewLicenseToken = license.Data.Token });
            }

            if (KeepAlive)
            {
                tracker.KeepAlive(license.Data.Token);
            }
            return license.Data;
        }

        public Int32 getLicenseType(String DatabaseName, String User)
        {
            if (!String.IsNullOrEmpty(DatabaseName) && !String.IsNullOrEmpty(User))
            {
                var request = new RestRequest("api/licenses?DatabaseName=" + DatabaseName + "&User=" + User);
                var response = ApiClient.Execute<Int32>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Square9.SecurityDisplay.Models.ApiException("Unable to get users license type.");
                }
                return Convert.ToInt32(response.Content);
            }
            return 0;
        }

        /// <summary>
        /// Release a license token from the API.
        /// </summary>
        /// <param name="Token">The license to be released.</param>
        public void ReleaseLicense(String Token)
        {
            tracker.StopTracking();
            var request = new RestRequest("api/licenses/" + Token);
            var response = ApiClient.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ApiException("Unable to release license token. ", response.ErrorException);
            }
            else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new ServerNotFoundException("Unable to release license token: Server unavailable");
            }
            else if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApiException("Unable to release license token. " + response.Content);
            }
        }

        public Int32 GetFeatureLicenseCount(String feature, bool licenseCount = false)
        {
            var request = new RestRequest("api/admin?feature=" + feature + "&licenseCount=" + licenseCount);
            var response = ApiClient.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ApiException("Unable to return License Count: ", response.ErrorException);
            }
            else if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApiException("Unable to return License Count: " + response.Content);
            }
            return Convert.ToInt32(response.Content);
        }

        /// <summary>
        /// Validate if the provided token exists.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ValidateLicense(String token)
        {
            var request = new RestRequest("api/licenses");
            request.AddParameter("validate", token);
            var response = ApiClient.Execute(request);
            if (String.IsNullOrEmpty(response.Content))
            {
                return false;
            }
            return bool.Parse(response.Content);
        }

        /// <summary>
        /// A KeepAlive request will notify if the license is revoked and it retrieves a new one.
        /// </summary>
        public event EventHandler<LicenseTokenChanged> LicenseTokenChanged;
        /// <summary>
        /// A KeepAlive request will notify if the license is revoked and it's unable to retrieve a new token.
        /// </summary>
        public event EventHandler UnableToGetNewLicense;
        internal void OnLicenseTokenChanged(LicenseTokenChanged e)
        {
            EventHandler<LicenseTokenChanged> handler = LicenseTokenChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        internal void OnUnableToGetNewLicense(EventArgs e)
        {
            EventHandler handler = UnableToGetNewLicense;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    internal class LicensePersist
    {
        Timer t;
        TimeSpan timerInterval;
        String token = "";
        Licenses Api;

        public LicensePersist(Licenses ApiConnector)
        {
            Api = ApiConnector;
        }

        /// <summary>
        /// Set up a license token to be kept alive by periodic checks to the API.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Interval">Integer in minutes of license check interval</param>
        public void KeepAlive(String Token, Int32 Interval = 0)
        {
            //default to 1 minute ping
            timerInterval = (Interval > 0) ? TimeSpan.FromMinutes(Interval) : TimeSpan.FromMinutes(1);
            token = Token;
            t = new Timer(CheckLicense, null, timerInterval, timerInterval);
        }

        public void StopTracking()
        {
            if (t != null)
            {
                t.Change(Timeout.Infinite, Timeout.Infinite);
            }
            token = "";
        }

        private void CheckLicense(Object TimerState)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {
                    //fix for bz3175 automatically renewed licenses don't refresh -- pass true
                    License lic = Api.GetLicense(token, true);
                }
                catch (Exception ex)
                {
                    //raise event license is expired and new one can't be obtained
                    Api.OnUnableToGetNewLicense(EventArgs.Empty);
                }
            }
        }
    }

    public class LicenseTokenChanged : EventArgs
    {
        public String NewLicenseToken { get; set; }
    }
}