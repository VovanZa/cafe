﻿using System.IO;
using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
using cafe.CommandLine.Options;
using DasMulli.Win32.ServiceUtils;
using NLog;
using NLog.Config;

namespace cafe.Updater
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Program).FullName);

        public static int Main(string[] args)
        {
            ConfigureLogging();
            const string application = "cafe.Updater";

            var processExecutor = new ProcessExecutor(() => new ProcessBoundary());

            IWin32Service ServiceFactory() => new CafeUpdaterWindowsService(new CafeInstaller(
                new FileSystemCommandsBoundary(), processExecutor,
                UpdaterSettings.Instance.CafeApplicationDirectory));

            var runner = new OptionGroup()
                .WithGroup("server", serverGroup =>
                {
                    serverGroup.WithDefaultOption(new ServerInteractiveOption(application, ServiceFactory));
                    serverGroup.WithOption(new ServerWindowsServiceOption(application, ServiceFactory),
                        "--run-as-service");
                })
                .WithGroup("service",
                    serviceGroup =>
                    {
                        var fileSystem = new FileSystem(new EnvironmentBoundary(), new FileSystemCommandsBoundary());
                        var serviceStatusWaiter = new ServiceStatusWaiter("waiting for service status",
                            new AutoResetEventBoundary(), new TimerFactory(),
                            new ServiceStatusProvider(processExecutor,
                                fileSystem), application);
                        ServiceOptionInitializer.AddServiceOptionsTo(serviceGroup, serviceStatusWaiter, processExecutor,
                            fileSystem, application, "cafe Updater", "updates cafe");
                    });

            var arguments = runner.ParseArguments(args);
            if (arguments != null)
            {
                var returnValue = runner.RunProgram(arguments);
                Logger.Debug($"Finishing {application} run");
                return returnValue;
            }
            else
            {
                Presenter.ShowError(
                    $"No options match the supplied arguments. Run {application} help to view all options",
                    Logger);
                return -2;
            }
        }

        private static void ConfigureLogging()
        {
            const string file = "nlog.config";
            LogManager.Configuration = new XmlLoggingConfiguration(Path.GetFullPath(file), false);
            Logger.Info($"Logging set up based on {file}");
        }
    }
}