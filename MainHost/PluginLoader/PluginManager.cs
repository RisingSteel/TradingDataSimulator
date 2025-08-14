using Core.Interfaces;
using Core.Models;
using DataFormaterContract;
using System.Reflection;
using System.Runtime.Loader;


namespace MainHost.PluginLoader
{
    public class PluginManager : IPriceChangeNotifier, IAsyncDisposable
    {
        private readonly ILogger<PluginManager> _logger;
        private List<(PluginLoadContext, List<IDataFormatter>)> _contexts; 

        public PluginManager(ILogger<PluginManager> logger)
        {
            _logger = logger;
            _contexts = new List<(PluginLoadContext, List<IDataFormatter>)>();
            LoadPlugins();
        }


        public void LoadPlugins()
        {
            string pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

            if (!Directory.Exists(pluginPath))
            {
                Directory.CreateDirectory(pluginPath);
            }
            var pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
            foreach (var pluginFile in pluginFiles)
            {
                var loadContext = new PluginLoadContext(pluginFile);
                List<IDataFormatter> plugins = new List<IDataFormatter>();

                Assembly pluginAssembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(pluginFile));
                _logger.LogDebug("Loaded plugin assembly: {AssemblyName}", pluginAssembly.GetName().Name);
                var formatterTypes = pluginAssembly.GetTypes()
                    .Where(t => typeof(IDataFormatter).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
                foreach (Type fType in formatterTypes)
                {
                    var formatter = (IDataFormatter)Activator.CreateInstance(fType)!;
                    plugins.Add(formatter);
                    
                }
                _contexts.Add((loadContext, plugins));

            }
        }

        public Task PriceNotifierAsync(PriceTick priceTick)
        {
            var snapshot = _contexts.ToList();

            foreach (var item in snapshot)
            {
                foreach (var plugin in item.Item2)
                {
                    var pluginString = plugin.FormatPrice(priceTick.Symbol, priceTick.Price, priceTick.Timestamp);
                    _logger.LogInformation("{Plugin}: {Formatted}", plugin.GetType().Name, pluginString);
                }
            }

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var (context, plugins) in _contexts)
            {
                context.Unload();
            }
            _contexts.Clear();

            await Task.Yield();
            GC.Collect(); 
            GC.WaitForPendingFinalizers(); 
            GC.Collect();
        }
    }
}
