using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using System;
using System.IO;

using System.Text;

namespace Azure
{
    public class StorageClient
    {
        BlobServiceClient _client;
        ObjectSerializer _serializer = new JsonObjectSerializer();

        public StorageClient(string uri)
        {
            _client = new Storage.Blobs.BlobServiceClient(new Uri(uri), new DefaultAzureCredential());
        }

        public void Upload(string containerName, string blobName, string text)
            => Upload(containerName, blobName, Encoding.UTF8.GetBytes(text));

        public void Upload(string containerName, string blobName, byte[] bytes)
            => _client.GetBlobContainerClient(containerName).UploadBlob(blobName, new MemoryStream(bytes));

        public void Upload(string containerName, string blobName, Stream bytes)
            => _client.GetBlobContainerClient(containerName).UploadBlob(blobName, bytes);

        public void Upload<T>(string containerName, string blobName, T jsonSerliazable)
        {
            Stream stream = new MemoryStream();
            _serializer.Serialize(stream, jsonSerliazable, typeof(T), default);
            Upload(containerName, blobName, stream);
        }
    }
}