using DataFormaterContract;
using MainHost.PluginLoader;
using Microsoft.Extensions.Logging.Abstractions;

namespace Plugins.LoaderTests
{
    public class PluginManagerTest
    {
        [Fact]
        public void Plugin_Load_And_Execute_Test()
        {
            var logger = NullLogger<PluginManager>.Instance;
            var pluginManager = new PluginManager(logger);
            pluginManager.LoadPlugins();

            Assert.NotEmpty(pluginManager.Contexts);
            foreach (var context in pluginManager.Contexts)
            {
                Assert.NotEmpty(context.Item2);
                foreach (var plugin in context.Item2)
                {
                    Assert.NotNull(plugin);
                    Assert.IsAssignableFrom<IDataFormatter>(plugin);
                    Assert.True(plugin.FormatPrice("AAPL", 150.00m, DateTime.UtcNow).Length > 0);

                }
            }

        }

        [Fact]
        public async Task Plugin_Dispose_Test()
        {
            var logger = NullLogger<PluginManager>.Instance;
            var pluginManager = new PluginManager(logger);
            pluginManager.LoadPlugins();

            Assert.NotEmpty(pluginManager.Contexts);

            await pluginManager.DisposeAsync();

            Assert.Empty(pluginManager.Contexts);
        }
    }
}