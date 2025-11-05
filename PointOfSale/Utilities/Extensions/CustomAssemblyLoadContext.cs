using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace PointOfSale.Utilities.Extensions
{
    public class CustomAssemblyLoadContext: AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            Console.WriteLine($"Attempting to load native DLL: {absolutePath}");
            // Add this BEFORE loading the DLL
            Console.WriteLine($"Is 64-bit OS: {Environment.Is64BitOperatingSystem}");
            Console.WriteLine($"Is 64-bit Process: {Environment.Is64BitProcess}");
            Console.WriteLine($"Processor Architecture: {RuntimeInformation.ProcessArchitecture}");
            return LoadUnmanagedDll(absolutePath);
        }
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }
        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}
