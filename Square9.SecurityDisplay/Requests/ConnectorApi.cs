using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using System.Configuration;

namespace Square9.SecurityDisplay.Requests
{
    public class ConnectorApi
    {
        public Requests Requests;
        public String URL;

        public ConnectorApi(String URL, String Username, String Password)
        {
            this.URL = URL;
            Requests = new Requests(URL, new RestSharp.Authenticators.HttpBasicAuthenticator(Username, Password));
        }
    }

    public class Requests
    {
        protected RestClient ApiClient;

        //request categories
        public UserRequests UserRequests;
        public Licenses Licenses;

        private void InitRequests()
        {
            UserRequests = new UserRequests(ApiClient);
            Licenses = new Licenses(ApiClient);
        }

        internal Requests(String URL, RestSharp.Authenticators.IAuthenticator Authenticator)
        {
            ApiClient = new RestClient(URL);
            InitRequests();

            ApiClient.Authenticator = Authenticator;
        }
    }
}