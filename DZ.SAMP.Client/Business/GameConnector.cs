using DZ.SAMP.Client.Domain;
using DZ.SAMP.Client.Models;
using DZ.SAMP.Client.Resources.Language;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;

namespace DZ.SAMP.Client.Business
{
    public class GameConnector
    {
        public void Connect(Server server, string name, string password, string rconpassword, string singleplayerlocation)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 3)
            {
                MessageBox.Show(LauncherStrings.InvalidName, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(singleplayerlocation + "\\gta_sa.exe"))
            {
                MessageBox.Show(singleplayerlocation + "\\gta_sa.exe" + LauncherStrings.CouldNotBeFound, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(singleplayerlocation + "\\samp.dll"))
            {
                MessageBox.Show(singleplayerlocation + "\\samp.dll " + LauncherStrings.CouldNotBeFound, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (server != null)
            {
                Console.WriteLine("1");
                var processes = Process.GetProcessesByName("gta_sa");
                Console.WriteLine("2");
                if (processes != null)
                {
                    Console.WriteLine("3");
                    foreach (var process in processes)
                    {
                        try
                        {
                            Console.WriteLine("4");
                            process.Kill();
                        }
                        catch
                        {
                            MessageBox.Show(LauncherStrings.GameIsStillRunning, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                Console.WriteLine("5");
                var mh = Kernel32.GetModuleHandle("kernel32.dll");
                if (mh != IntPtr.Zero)
                {
                    Console.WriteLine("6");
                    var load_library_w = Kernel32.GetProcAddress(mh, "LoadLibraryW");
                    Console.WriteLine("7");
                    if (load_library_w != IntPtr.Zero)
                    {
                        Console.WriteLine("8");
                        Kernel32.STARTUPINFO startup_info = new Kernel32.STARTUPINFO();
                        if (Kernel32.CreateProcess(singleplayerlocation + "\\gta_sa.exe", "-c " + rconpassword + " -h " +
                            server.IPAddress + " -p " + server.Port + " -n " + name + (string.IsNullOrEmpty(password) ? "" : (" -z " + password)),
                            IntPtr.Zero, IntPtr.Zero, false, 0x8 | 0x4, IntPtr.Zero,
                            singleplayerlocation, ref startup_info, out Kernel32.PROCESS_INFORMATION process_info))
                        {
                            Console.WriteLine("9");
                            InjectPlugin(singleplayerlocation + "\\samp.dll", process_info.hProcess, load_library_w);
                            Console.WriteLine("10");

                            Kernel32.ResumeThread(process_info.hThread);
                            Kernel32.CloseHandle(process_info.hProcess);
                            Console.WriteLine("11");
                        }
                    }
                }
                else
                    Console.WriteLine("mh ist zero");
            }
        }

        private static void InjectPlugin(string pluginPath, IntPtr processHandle, IntPtr loadLibraryW)
        {
            if (File.Exists(pluginPath))
            {
                Console.WriteLine("12");
                var ptr = Kernel32.VirtualAllocEx(processHandle, IntPtr.Zero, (uint)(pluginPath.Length + 1) * 2U,
                    Kernel32.AllocationType.Reserve | Kernel32.AllocationType.Commit, Kernel32.MemoryProtection.ReadWrite);
                Console.WriteLine("13");
                if (ptr != IntPtr.Zero)
                {
                    Console.WriteLine("14");
                    var p = Encoding.Unicode.GetBytes(pluginPath);
                    var nt = Encoding.Unicode.GetBytes("\0");
                    if (Kernel32.WriteProcessMemory(processHandle, ptr, p, (uint)p.Length, out _) &&
                        Kernel32.WriteProcessMemory(processHandle, new IntPtr(ptr.ToInt64() + p.LongLength), nt, (uint)nt.Length, out _))
                    {
                        Console.WriteLine("15");
                        var rt = Kernel32.CreateRemoteThread(processHandle, IntPtr.Zero, 0U, loadLibraryW, ptr, 0x4, out _);
                        Console.WriteLine("16");
                        if (rt != IntPtr.Zero)
                        {
                            Console.WriteLine("17");
                            Kernel32.ResumeThread(rt);
                            unchecked
                            {
                                Kernel32.WaitForSingleObject(rt, (uint)(Timeout.Infinite));
                                Console.WriteLine("18");
                            }
                            Console.WriteLine("19");
                        }
                    }
                    Kernel32.VirtualFreeEx(processHandle, ptr, 0, Kernel32.AllocationType.Release);
                    Console.WriteLine("20re");
                }
            }
        }
    }
}
