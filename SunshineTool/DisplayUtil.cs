using System.Runtime.InteropServices;

public static class DisplayUtil
{

    private const uint SDC_APPLY = 0x00000080;

    private const uint SDC_TOPOLOGY_INTERNAL = 0x00000001;

    private const uint SDC_TOPOLOGY_CLONE = 0x00000002;

    private const uint SDC_TOPOLOGY_EXTERNAL = 0x00000008;

    private const uint SDC_TOPOLOGY_EXTEND = 0x00000004;

    // 控制改变屏幕分辨率的常量
    private const int ENUM_CURRENT_SETTINGS = -1;
    private const int CDS_UPDATEREGISTRY = 0x01;
    private const int CDS_TEST = 0x02;
    private const int DISP_CHANGE_SUCCESSFUL = 0;
    private const int DISP_CHANGE_RESTART = 1;
    private const int DISP_CHANGE_FAILED = -1;

    // 控制改变方向的常量定义
    private const int DMDO_DEFAULT = 0;
    private const int DMDO_90 = 1;
    private const int DMDO_180 = 2;
    private const int DMDO_270 = 3;


    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern long SetDisplayConfig(uint numPathArrayElements, IntPtr pathArray, uint numModeArrayElements, IntPtr modeArray, uint flags);
    // 平台调用的申明
    [DllImport("user32.dll")]
    private static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
    [DllImport("user32.dll")]
    private static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);


    /// <summary>
    /// 设置屏幕的显示模式,模式(0 - 主屏  1 - 双屏复制  2 - 双屏扩展  3 - 第二屏幕)
    /// </summary>
    /// <param name="type">模式(0 - 主屏  1 - 双屏复制  2 - 双屏扩展  3 - 第二屏幕)</param>
    /// <returns></returns>
    public static void SwitchDisplayMode(int type)
    {
        uint smode;

        switch (type)
        {
            case 0:
                smode = SDC_APPLY | SDC_TOPOLOGY_INTERNAL;
                break;
            case 1:
                smode = SDC_APPLY | SDC_TOPOLOGY_CLONE;
                break;
            case 2:
                smode = SDC_APPLY | SDC_TOPOLOGY_EXTEND;
                break;
            case 3:
                smode = SDC_APPLY | SDC_TOPOLOGY_EXTERNAL;
                break;
            default:
                smode = SDC_APPLY | SDC_TOPOLOGY_INTERNAL;
                break;
        }
        SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, smode);
    }

    // 改变分辨率
    public static void ChangeResolution(int width, int height, int fps)
    {
        // 初始化 DEVMODE结构
        DEVMODE devmode = new DEVMODE();
        devmode.dmDeviceName = new String(new char[32]);
        devmode.dmFormName = new String(new char[32]);
        devmode.dmSize = (short)Marshal.SizeOf(devmode);

        if (0 != EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devmode))
        {
            devmode.dmPelsWidth = width;
            devmode.dmPelsHeight = height;
            devmode.dmDisplayFrequency = fps;

            // 改变屏幕分辨率
            int iRet = ChangeDisplaySettings(ref devmode, CDS_TEST);

            if (iRet == DISP_CHANGE_FAILED)
            {
                Util.Log($"改变屏幕分辨率失败, 错误码: {iRet}");
            }
            else
            {
                iRet = ChangeDisplaySettings(ref devmode, CDS_UPDATEREGISTRY);
                switch (iRet)
                {
                    // 成功改变
                    case DISP_CHANGE_SUCCESSFUL:
                        {
                            break;
                        }
                    case DISP_CHANGE_RESTART:
                        {
                            Util.Log($"你需要重新启动电脑设置才能生效, 错误码: {iRet}");
                            break;
                        }
                    default:
                        {
                            Util.Log($"改变屏幕分辨率失败, 错误码: {iRet}");
                            break;
                        }
                }
            }
        }
    }

    public static (int, int, int) GetCurResolution()
    {
        // 初始化 DEVMODE结构
        DEVMODE devmode = new DEVMODE();
        devmode.dmDeviceName = new String(new char[32]);
        devmode.dmFormName = new String(new char[32]);
        devmode.dmSize = (short)Marshal.SizeOf(devmode);
        if (0 != EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devmode))
        {
            return (devmode.dmPelsWidth, devmode.dmPelsHeight, devmode.dmDisplayFrequency);
        }
        return (1920, 1080, 60);
    }
}

// 定义DEVMODE结构
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
internal struct DEVMODE
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string dmDeviceName;
    public short dmSpecVersion;
    public short dmDriverVersion;
    public short dmSize;
    public short dmDriverExtra;
    public int dmFields;
    public int dmPositionX;
    public int dmPositionY;
    public int dmDisplayOrientation;
    public int dmDisplayFixedOutput;
    public short dmColor;
    public short dmDuplex;
    public short dmYResolution;
    public short dmTTOption;
    public short dmCollate;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string dmFormName;
    public short dmLogPixels;
    public short dmBitsPerPel;
    public int dmPelsWidth;
    public int dmPelsHeight;
    public int dmDisplayFlags;
    public int dmDisplayFrequency;
    public int dmICMMethod;
    public int dmICMIntent;
    public int dmMediaType;
    public int dmDitherType;
    public int dmReserved1;
    public int dmReserved2;
    public int dmPanningWidth;
    public int dmPanningHeight;
};