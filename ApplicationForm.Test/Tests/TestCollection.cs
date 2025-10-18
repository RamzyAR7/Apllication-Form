using Xunit;

namespace ApplicationForm.Test.Tests
{
    [CollectionDefinition("AppTestCollection")]
    public class AppTestCollection : ICollectionFixture<TestFixture>
    {
    }
}
