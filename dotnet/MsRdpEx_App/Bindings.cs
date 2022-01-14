using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MsRdpEx
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("13F6E86F-EE7D-44D1-AA94-1136B784441D")]
    public interface IMsRdpExCoreApi
    {
        void Load();
        void Unload();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetLogEnabled([MarshalAs(UnmanagedType.U1)] bool logEnabled);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetLogFilePath([MarshalAs(UnmanagedType.LPStr)] string logFilePath);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.U1)]
        bool QueryInstanceByWindowHandle(IntPtr hWnd, [MarshalAs(UnmanagedType.IUnknown)] out object rdpInstance);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("338784B3-3363-45A2-8ECD-80A65DBAF636")]
    public interface IMsRdpExProcess
    {
        void Start([MarshalAs(UnmanagedType.LPStr)] string filename,
            int argc, ref IntPtr[] argv);
        void Stop(UInt32 exitCode);
        void Wait(UInt32 milliseconds);
        void GetExitCode(out UInt32 exitCode);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("94CDA65A-EFDF-4453-B8B2-2493A12D31C7")]
    public interface IMsRdpExInstance
    {
        void GetRdpClient([MarshalAs(UnmanagedType.IUnknown)] out object rdpClient);

        void GetOutputMirror(out IntPtr outputMirror);

        void SetOutputMirror(IntPtr outputMirror);

        void GetCorePropsRawPtr(out IntPtr pCorePropsRaw);

        void SetCorePropsRawPtr(IntPtr pCorePropsRaw);

        void AttachOutputWindow(IntPtr hOutputWnd, IntPtr pUserData);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.U1)]
        bool GetShadowBitmap(ref IntPtr phDC, ref IntPtr phBitmap, ref UInt32 pWidth, ref UInt32 pHeight);
    }

    public static class Bindings
    {
        private static Guid IID_IMsRdpExCoreApi = new Guid(0x13F6E86F, 0xEE7D, 0x44D1,0xAA, 0x94, 0x11, 0x36, 0xB7, 0x84, 0x44, 0x1D);
        private static Guid IID_IMsRdpExProcess = new Guid(0x338784B3, 0x3363, 0x45A2, 0x8E, 0xCD, 0x80, 0xA6, 0x5D, 0xBA, 0xF6, 0x36);
        private static Guid IID_IMsRdpExInstance = new Guid(0x94CDA65A, 0xEFDF, 0x4453, 0xB8, 0xB2, 0x24, 0x93, 0xA1, 0x2D, 0x31, 0xC7);

        public static IMsRdpExCoreApi GetCoreApi()
        {
            object instance = null;
            MsRdpEx_QueryInterface(ref IID_IMsRdpExCoreApi, out instance);
            return (IMsRdpExCoreApi)instance;
        }

        public static IMsRdpExProcess StartProcess(string appName, string[] args)
        {
            object instance = null;
            MsRdpEx_QueryInterface(ref IID_IMsRdpExProcess, out instance);

            IMsRdpExProcess process = (IMsRdpExProcess) instance;

            int argc = args.Length;
            IntPtr[] argv = new IntPtr[argc];

            for (int i = 0; i < argc; i++)
            {
                argv[i] = Marshal.StringToCoTaskMemUTF8(args[i]);
            }

            process.Start(appName, argc, ref argv);

            for (int i = 0; i < argv.Length; i++)
            {
                Marshal.FreeCoTaskMem(argv[i]);
            }

            return process;
        }

        [DllImport("MsRdpEx.dll")]
        public static extern uint MsRdpEx_QueryInterface(ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);

        public static bool LaunchProcess(string appName, string[] args)
        {
            int argc = args.Length;
            IntPtr[] argv = new IntPtr[argc];

            for (int i = 0; i < argc; i++)
            {
                argv[i] = Marshal.StringToCoTaskMemUTF8(args[i]);
            }

            for (int i = 0; i < argv.Length; i++)
            {
                Marshal.FreeCoTaskMem(argv[i]);
            }

            return true;
        }
    }
}
