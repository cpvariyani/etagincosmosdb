﻿using Cosmos_CRUD.DataAccess.Utility;
using Cosmos_CRUD.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cosmos_CRUD.DataAccess
{
    public class CosmosDataAdapter : ICosmosDataAdapter
    {
        private readonly DocumentClient _client;
        private readonly string _accountUrl;
        private readonly string _primarykey;

        public CosmosDataAdapter(
         ICosmosConnection connection,
         IConfiguration config)
        {

            _accountUrl = config.GetValue<string>("Cosmos:AccountURL");
            _primarykey = config.GetValue<string>("Cosmos:AuthKey");
            _client = new DocumentClient(new Uri(_accountUrl), _primarykey);
        }


        public async Task<bool> CreateDatabase(string name)
        {
            try
            {
                await _client.CreateDatabaseIfNotExistsAsync(new Database { Id = name });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateCollection(string dbName, string name)
        {
            try
            {
                await _client.CreateDocumentCollectionIfNotExistsAsync
                 (UriFactory.CreateDatabaseUri(dbName), new DocumentCollection { Id = name });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateDocument(string dbName, string name, UserInfo userInfo)
        {
            try
            {
                userInfo.UniqueNumber = userInfo.UniqueNumber + 1;
                userInfo.id = "b184eb7d-7967-4bb1-9749-f0496eb65047";

                var requestOptions = new RequestOptions
                {
                    AccessCondition = new AccessCondition
                    {
                        Condition = userInfo._etag,
                        Type = AccessConditionType.IfMatch
                    }
                };

                // await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbName, name), userInfo);

                await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbName, name), userInfo, requestOptions);
                return true;
                //Assembly Polly, Version=5.8.0.0, 
                // await Policy
                //.Handle<DocumentClientException>(ex =>
                //{
                //     // If a precondition failed, the etag has expired. Retry the update.
                //     return ex.StatusCode == HttpStatusCode.PreconditionFailed;
                //})
                //.RetryAsync(10)
                //.ExecuteAsync(async () =>
                //{
                // userInfo.UniqueNumber = userInfo.UniqueNumber + 1;
                //  await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbName, name), userInfo, requestOptions);

                //});

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> PlaceOrder(string dbName, string name, Order order)
        {
            try
            {
                await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbName, name), order);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<dynamic> GetData(string dbName, string name)
        {
            try
            {

                var result = await _client.ReadDocumentFeedAsync(UriFactory.CreateDocumentCollectionUri(dbName, name),
                    new FeedOptions { MaxItemCount = 10 });

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<UserInfo> UpsertUserAsync(UserInfo user)
        {
            ResourceResponse<Document> response = null;
            try
            {
                response = null;//await _client.UpsertDocumentAsync(_collectionUri, user);
            }
            catch (DocumentClientException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

            return (dynamic)response.Resource;
        }

        public async Task<UserInfo> DeleteUserAsync(string dbName, string name, string id)
        {
            try
            {
                var collectionUri = UriFactory.CreateDocumentUri(dbName, name, id);

                var result = await _client.DeleteDocumentAsync(collectionUri);

                return (dynamic)result.Resource;
            }
            catch (DocumentClientException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
