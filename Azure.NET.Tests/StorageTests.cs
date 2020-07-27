using Azure;
using NUnit.Framework;

namespace Azure.Data.Tests
{
    public class StorageTests
    {
        [Test]
        public void Upload()
        {
            var client = new StorageClient("http://mystorage");
            client.Upload("container", "blob", "hello world");
        }
    }
}