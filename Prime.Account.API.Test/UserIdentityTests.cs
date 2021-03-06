﻿using System;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Account.Data.Core;

namespace Prime.Account.API.Test
{
    [TestClass]
    public class UserIdentityTests
    {
        [TestMethod]
        public void GetUserIdentityTests()
        {
            try
            {
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("username", "admin"));
                postData.Add(new KeyValuePair<string, string>("password", "system"));
                postData.Add(new KeyValuePair<string, string>("grant_type", "password")); // static value: password
                postData.Add(new KeyValuePair<string, string>("client_id", "clientId101"));
                postData.Add(new KeyValuePair<string, string>("client_secret", "clientSecret101"));

                var content = new FormUrlEncodedContent(postData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage tokenResponse;
                using(var client = new HttpClient())
                {
                    // Call hosted web API
                    tokenResponse = client.PostAsync("https://www.rvluzuriaga.somee.com/oauth2/token", content).Result;

                    // Call deployed web API in local environment
                    // tokenResponse = client.PostAsync("https://localhost/primeapi/oauth2/token", content).Result;

                    // Debug mode
                    // tokenResponse = client.PostAsync("https://localhost:44377/oauth2/token", content).Result;
                }

                var tokenResult = tokenResponse.Content.ReadAsStringAsync().Result;

                // Assertion
                Assert.IsTrue(tokenResponse.IsSuccessStatusCode, tokenResult);
                Assert.IsNotNull(tokenResult, "Token is null.");

                // Get Token
                var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenResult);
                var accessToken = dictResult.Where(x => x.Key == "access_token")
                                            .Select(v => v.Value)
                                            .FirstOrDefault();

                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Call the hosted web API
                    response = client.GetAsync("https://www.rvluzuriaga.somee.com/api/User/GetUserIdentity").Result;

                    // Call the deployed web API in local environment
                    // response = client.GetAsync("https://localhost/primeapi/api/User/GetUserIdentity").Result;

                    // Debug mode
                    //response = client.GetAsync("https://localhost:44377/api/User/GetUserIdentity").Result;
                }

                var result = response.Content.ReadAsStringAsync().Result;

                Assert.IsTrue(response.IsSuccessStatusCode, result);

                var businessEntity = JsonConvert.DeserializeObject<BusinessEntity>(result);

                Assert.IsNotNull(businessEntity, "Business Entity is null.");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
