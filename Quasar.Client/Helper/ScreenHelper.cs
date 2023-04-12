using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ProtoBuf.Meta;
using ProtoBuf.Serializers;
using Quasar.Client.Utilities;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Quasar.Client.Helper
{
    public static class ScreenHelper
    {
        private const int SRCCOPY = 0x00CC0020;

        private static Device _device;
        private static OutputDuplication _duplicatedOutput;

        private static int _currentAdapter;
        private static int _currentMonitor;
        private static Texture2D _screenTexture;

        public static int Width;
        public static int Height;
        private static bool _isSetup = false;

        public static void SetupMonitor(int adapterNumber, int monitorNumber)
        {
            if (_isSetup && _currentAdapter == adapterNumber && _currentMonitor == monitorNumber)
            {
                return;
            }

            _isSetup = true;

            _screenTexture?.Dispose();
            _duplicatedOutput?.Dispose();
            _device?.Dispose();

            _currentAdapter = adapterNumber;
            _currentMonitor = monitorNumber;

            Factory1 factory = new Factory1();
            Adapter1 adapter = factory.GetAdapter1(adapterNumber);

            _device = new Device(adapter);

            Output output = adapter.GetOutput(monitorNumber);
            Output1 output1 = output.QueryInterface<Output1>();

            RawRectangle bounds = output.Description.DesktopBounds;
            Width = bounds.Right - bounds.Left;
            Height = bounds.Bottom - bounds.Top;

            _duplicatedOutput = output1.DuplicateOutput(_device);

            Texture2DDescription textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = Width,
                Height = Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };

            _screenTexture = new Texture2D(_device, textureDesc);

            adapter.Dispose();
            factory.Dispose();
            output1.Dispose();
            output.Dispose();
        }

        public static Bitmap CaptureScreenSharpDX()
        {
            var bitmap = new System.Drawing.Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            bool captureDone = false;

            for (int i = 0; !captureDone; i++)
            {
                try
                {
                    SharpDX.DXGI.Resource screenResource;
                    OutputDuplicateFrameInformation duplicateFrameInformation;

                    // Try to get duplicated frame within given time
                    _duplicatedOutput.AcquireNextFrame(10000, out duplicateFrameInformation, out screenResource);

                    if (i > 0)
                    {
                        // copy resource into memory that can be accessed by the CPU
                        using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                            _device.ImmediateContext.CopyResource(screenTexture2D, _screenTexture);

                        // Get the desktop capture texture
                        var mapSource = _device.ImmediateContext.MapSubresource(_screenTexture, 0, MapMode.Read, MapFlags.None);

                        // Create Drawing.Bitmap
                        var boundsRect = new System.Drawing.Rectangle(0, 0, Width, Height);

                        // Copy pixels from screen capture Texture to GDI bitmap
                        var mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                        var sourcePtr = mapSource.DataPointer;
                        var destPtr = mapDest.Scan0;
                        for (int y = 0; y < Height; y++)
                        {
                            // Copy a single line 
                            SharpDX.Utilities.CopyMemory(destPtr, sourcePtr, Width * 4);

                            // Advance pointers
                            sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                            destPtr = IntPtr.Add(destPtr, mapDest.Stride);
                        }

                        // Release source and dest locks
                        bitmap.UnlockBits(mapDest);
                        _device.ImmediateContext.UnmapSubresource(_screenTexture, 0);

                        // Capture done
                        captureDone = true;
                    }

                    screenResource.Dispose();
                    _duplicatedOutput.ReleaseFrame();

                }
                catch (SharpDXException e)
                {
                    if (e.ResultCode.Code != SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
                    {
                        throw e;
                    }
                }
            }

            return bitmap;
        }


        public static Bitmap CaptureScreen(int screenNumber)
        {
            Rectangle bounds = GetBounds(screenNumber);
            Bitmap screen = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(screen))
            {
                IntPtr destDeviceContext = g.GetHdc();
                IntPtr srcDeviceContext = NativeMethods.CreateDC("DISPLAY", null, null, IntPtr.Zero);

                NativeMethods.BitBlt(destDeviceContext, 0, 0, bounds.Width, bounds.Height, srcDeviceContext, bounds.X,
                    bounds.Y, SRCCOPY);

                NativeMethods.DeleteDC(srcDeviceContext);
                g.ReleaseHdc(destDeviceContext);
            }

            return screen;
        }

        public static Rectangle GetBounds(int screenNumber)
        {
            return Screen.AllScreens[screenNumber].Bounds;
        }
    }
}
