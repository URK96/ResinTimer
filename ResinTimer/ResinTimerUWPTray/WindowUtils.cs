using System.Runtime.InteropServices;

namespace ResinTimerUWPTray
{
    internal static class WindowUtils
    {
        #region WIN32API

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(HandleRef hWnd, in WindowCompositionAttributeData data);

        private enum WindowCompositionAttribute
        {
            ACCENT_POLICY = 19
        }

        private enum ACCENT
        {
            DISABLED = 0,
            ENABLE_GRADIENT = 1,
            ENABLE_TRANSPARENTGRADIENT = 2,
            ENABLE_BLURBEHIND = 3,
            ENABLE_ACRYLICBLURBEHIND = 4,
            INVALID_STATE = 5
        }

        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int DataLength;
        }

        private struct AccentPolicy
        {
            public ACCENT AccentState;
            public uint AccentFlags;
            public uint GradientColor;
            public uint AnimationId;
        }

        #endregion

        public static void EnableAcrylic(IWin32Window window, Color blurColor)
        {
            if (window is null)
            {
                return;
            }

            var accentPolicy = new AccentPolicy
            {
                AccentState = ACCENT.ENABLE_ACRYLICBLURBEHIND,
                GradientColor = ToABGR(blurColor)
            };

            IntPtr policyPtr = Marshal.AllocHGlobal(Marshal.SizeOf(accentPolicy));

            Marshal.StructureToPtr(accentPolicy, policyPtr, false);

            SetWindowCompositionAttribute(new HandleRef(window, window.Handle),
                                          new WindowCompositionAttributeData
                                          {
                                              Attribute = WindowCompositionAttribute.ACCENT_POLICY,
                                              Data = policyPtr,
                                              DataLength = Marshal.SizeOf<AccentPolicy>()
                                          });

            Marshal.FreeHGlobal(policyPtr);
        }

        private static uint ToABGR(Color color) =>
            ((uint)color.A << 24) | 
            ((uint)color.B << 16) | 
            ((uint)color.G << 8) | 
            color.R;
    }
}