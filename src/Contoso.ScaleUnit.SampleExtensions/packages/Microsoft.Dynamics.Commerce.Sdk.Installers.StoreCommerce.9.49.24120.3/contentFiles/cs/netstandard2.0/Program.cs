namespace Microsoft.Dynamics.Commerce.Installers.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;

    /// <summary>
    /// File Program.cs is not built as part of the Microsoft.Dynamics.Commerce.Deployment.Installer.Framework project.
    /// This file will be compiled into the actual installer.
    /// This class is an entry point of the actual installer, which is provided by the installer framework.
    /// </summary>
    public static class Program
    {
        private const string DllExtension = ".dll";
        private static readonly string TempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        private static readonly Dictionary<string, Assembly> AssemblyDictionary = new Dictionary<string, Assembly>();

        /// <summary>
        /// The entry point method for the installer.
        /// </summary>
        /// <returns>The exit code. -1 if an error occurred, or 0 otherwise.</returns>
        public static int Main()
        {
            // Resolve the embedded assembly.
            AppDomain.CurrentDomain.AssemblyResolve +=
                (object sender, ResolveEventArgs eventArgs) => ResolveEmbeddedAssembly(eventArgs);

            ExtractUnmanagedLibrariesToTempFolder();

            try
            {
                // NOTE: the InstallerFramework needs to be called in a wrapping method that is not directly Main.!--
                // This is required because .NET will trigger type/ assembly resolution on method entry, and we use Main
                // to register the custom assembly resolution.
                return RunInstallerFramework();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return -1;
            }
            finally
            {
                // Removing temp directory if one exists. Any issues deleting it are ignored.
                try
                {
                    if (Directory.Exists(TempFolder))
                    {
                        Directory.Delete(TempFolder, recursive: true);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"An attempt to delete the temp folder '{TempFolder}' resulted in an event which can be safely ignored: '{ex}'.");
                }
            }
        }

        /// <summary>
        /// Runs the installer framework.
        /// </summary>
        /// <returns>The return code.</returns>
        /// <remarks>Installer framework references must be in methods called after the assembly resolve handler is set because all types referenced in a method must be resolved before it is executed.</remarks>
        private static int RunInstallerFramework()
        {
#pragma warning disable CA2000 // Dispose objects before losing scope. JUSTIFICATION: Needs refactoring
            return new InstallerFramework()
                .Run()
                .ConfigureAwait(continueOnCapturedContext: false)
                .GetAwaiter()
                .GetResult();
#pragma warning restore CA2000 // Dispose objects before losing scope
        }

        private static Assembly ResolveEmbeddedAssembly(ResolveEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var executingAssembly = Assembly.GetEntryAssembly();

            // Get .dll name that application tries to lookup
            string dllName = args.Name.Split(',').FirstOrDefault();

#pragma warning disable CA1307 // Specify StringComparison. JUSTIFICATION: The overload exists in .NET Core only but not in .NET 4.6.1, the project using this file compiles against both. Once we fullly switch to .NET Core this suppression should be removed.
            dllName = dllName.Replace(DllExtension, newValue: string.Empty) + DllExtension;
#pragma warning restore CA1307 // Specify StringComparison

            // Try fetch embedded resource name of the .dll which is typically <Root namespace>.<Dll name>
            string foundEmbeddedResourceName = executingAssembly.GetManifestResourceNames().FirstOrDefault(e => e.Equals(dllName, StringComparison.OrdinalIgnoreCase));

            Assembly result = null;
            if (!string.IsNullOrEmpty(foundEmbeddedResourceName))
            {
                // Check if the assembly has been loaded previously. If so, use the loaded assembly directly
                if (AssemblyDictionary.ContainsKey(foundEmbeddedResourceName))
                {
                    return AssemblyDictionary[foundEmbeddedResourceName];
                }

                // ******************* Begin special handling for DacFx assemblies ********************
                // Special processing for DacFx assembly due to the bug https://stackoverflow.microsoft.com/questions/216658.
                // Need to load the assembly from the File System to workaround the bug with null Assembly.Location.
                string[] assembliesToLoadFromDisk =
                {
                    "Microsoft.Data.Tools.Schema.Sql.dll",
                    "Microsoft.SqlServer.Dac.dll",
                };

                if (assembliesToLoadFromDisk.Contains(foundEmbeddedResourceName, StringComparer.OrdinalIgnoreCase))
                {
                    Trace.WriteLine($"Begin special handling for the request to load the assembly '{foundEmbeddedResourceName}'. Temp folder is: '{TempFolder}'.");

                    EnsureTempFolderExists();

                    Assembly assemblyFromDisk = PersistAssemblyToFileAndLoad(executingAssembly, foundEmbeddedResourceName);
                    AssemblyDictionary.Add(foundEmbeddedResourceName, assemblyFromDisk);

                    Trace.WriteLine($"End special handling for the request to load the assembly '{foundEmbeddedResourceName}'.");
                    return assemblyFromDisk;
                }

                //// ******************* End special handling for DacFx assemblies ********************

                Trace.WriteLine($"Trying to load the assembly '{foundEmbeddedResourceName}' from resources.");
                result = LoadAssemblyFromResource(executingAssembly, foundEmbeddedResourceName, out byte[] dummyAssemblyBytes);
                AssemblyDictionary.Add(foundEmbeddedResourceName, result);
            }

            return result;
        }

        private static void EnsureTempFolderExists()
        {
            if (!Directory.Exists(TempFolder))
            {
                Trace.WriteLine($"Creating temp folder '{TempFolder}' to store DacFx assemblies.");
                Directory.CreateDirectory(TempFolder);
            }
            else
            {
                Trace.WriteLine($"Skipped creating temp folder '{TempFolder}' because it already exists.");
            }
        }

        private static Assembly LoadAssemblyFromResource(Assembly executingAssembly, string assemblyName, out byte[] assemblyBytes)
        {
            assemblyBytes = GetAssemblyBytesFromResource(executingAssembly, assemblyName);
            Assembly result = Assembly.Load(assemblyBytes);
            Trace.WriteLine($"Loaded the assembly '{result}' directly from the resources.");
            return result;
        }

        private static Assembly PersistAssemblyToFileAndLoad(Assembly executingAssembly, string assemblyName)
        {
            byte[] assemblyBytes = GetAssemblyBytesFromResource(executingAssembly, assemblyName);
            string assemblyFullFileName = Path.Combine(TempFolder, assemblyName);
            File.WriteAllBytes(assemblyFullFileName, assemblyBytes);
            Assembly assembly = Assembly.LoadFrom(assemblyFullFileName);
            Trace.WriteLine($"Loaded assembly '{assemblyName}' from resources, persisted it as '{assemblyFullFileName}' and then loaded into AppDomain from the file system.");
            return assembly;
        }

        private static byte[] GetAssemblyBytesFromResource(Assembly executingAssembly, string assemblyName)
        {
            using (Stream embeddedResource = executingAssembly.GetManifestResourceStream(assemblyName))
            {
                byte[] assemblyBytes = new byte[embeddedResource.Length];
                embeddedResource.Read(assemblyBytes, 0, assemblyBytes.Length);
                return assemblyBytes;
            }
        }

        /// <summary>
        /// This method is needed to force the .Net runtime to automatically consume embedded native libraries.
        /// </summary>
        private static void ExtractUnmanagedLibrariesToTempFolder()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            EnsureTempFolderExists();

            // Add the TempFolder path to the PATH environment variable (at the head!)
            // This way native libraries will be automatically consumed by the .Net runtime
            const string PathVariableName = "PATH";
            string path = Environment.GetEnvironmentVariable(PathVariableName);
            Environment.SetEnvironmentVariable(PathVariableName, TempFolder + ";" + path);

            foreach (var resource in entryAssembly.GetManifestResourceNames()
                         .Where(e => e.EndsWith(DllExtension, StringComparison.OrdinalIgnoreCase)
                                     && !IsManagedAssembly(entryAssembly, e)))
            {
                Trace.WriteLine($"Extracting unmanaged assembly: {resource}");
                using (Stream embeddedResource = entryAssembly.GetManifestResourceStream(resource))
                {
                    byte[] assemblyBytes = new byte[embeddedResource.Length];
                    embeddedResource.Read(assemblyBytes, 0, assemblyBytes.Length);
                    var filePath = Path.Combine(TempFolder, resource);
                    File.WriteAllBytes(filePath, assemblyBytes);
                }
            }
        }

        private static bool IsManagedAssembly(Assembly entryAssembly, string fileName)
        {
            try
            {
                using (Stream stream = entryAssembly.GetManifestResourceStream(fileName))

                // Try to read CLI metadata from the PE file.
                using (var peReader = new PEReader(stream))
                {
                    if (!peReader.HasMetadata)
                    {
                        return false; // File does not have CLI metadata.
                    }

                    // Check that file has an assembly manifest.
                    MetadataReader reader = peReader.GetMetadataReader();
                    return reader.IsAssembly;
                }
            }
            catch (BadImageFormatException)
            {
                return false;
            }
        }
    }
}