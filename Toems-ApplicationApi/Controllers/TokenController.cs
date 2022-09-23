﻿using System.Web.Http;
using Toems_Service;

namespace Toems_ApplicationApi.Controllers
{
    public class TokenController : ApiController
    {       
        public string Get(string username, string password, string baseUrl)
        {
            return new TokenServices().GetToken(username, password,baseUrl);
        }
        public string GetWithMfa(string username, string password, string baseUrl, string verificationCode)
        {
            return new TokenServices().GetToken(username, password, baseUrl, verificationCode);
        }
    }
}