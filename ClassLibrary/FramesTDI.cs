using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using DALSA.SaperaLT.SapClassBasic;
using System.Net.Sockets;
using System.Diagnostics;

public class FramesTDI
{
    public static SapAcquisition Acq = null;
    public static SapAcqDevice AcqDevice = null;
    public static SapBuffer Buffers = null;
    public static SapTransfer Xfer = null;
    public static SapView View = null;
    public static SapLocation loc = null;
    public static int numFrames = 3;
    public static int sizeArr = 0;
    public static Int16[,] framesArr = null;
    public MyAcquisitionParams acqParams;
    public static int countFrame = 0;
    public FramesTDI()
    {
        acqParams = new MyAcquisitionParams
        {
            ResourceIndex = 0,
            ServerName = "Xtium-CLHS_PX8_1"
        };
    }
    public bool setConfigFile(string filePath)
    {
        acqParams.ConfigFileName = filePath;

        if ((acqParams.ConfigFileName != null) && (acqParams.ServerName.Equals("Xtium-CLHS_PX8_1")))
        {
            return true;
        }

        return false;
    }
    public void StartGrabFramesTDI()
    {
        loc = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);

        if (SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.Acq) > 0)
        {
            Acq = new SapAcquisition(loc, acqParams.ConfigFileName);
            Buffers = new SapBufferWithTrash(2, Acq, SapBuffer.MemoryType.ScatterGather);
            Xfer = new SapAcqToBuf(Acq, Buffers);

            // Create acquisition object
            if (!Acq.Create())
            {
                DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                return;
            }
            Acq.EnableEvent(SapAcquisition.AcqEventType.StartOfFrame);
        }

        else if (SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.AcqDevice) > 0)
        {
            AcqDevice = new SapAcqDevice(loc, acqParams.ConfigFileName);
            Buffers = new SapBufferWithTrash(2, AcqDevice, SapBuffer.MemoryType.ScatterGather);
            Xfer = new SapAcqDeviceToBuf(AcqDevice, Buffers);

            // Create acquisition object
            if (!AcqDevice.Create())
            {
                DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                return;
            }
        }

        View = new SapView(Buffers);
        // End of frame event
        Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
        Xfer.XferNotify += new SapXferNotifyHandler(Xfer_XferNotify);
        Xfer.XferNotifyContext = View;
    }
    public void StartSnap()
    {
        // Create buffer object
        if (!Buffers.Create())
        {
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
            return;
        }

        // For TDI Case the heigth of the buffer must be equal 1
        sizeArr = Buffers.Width * 1;
        InitializeFrameArray(numFrames, sizeArr);

        // Create buffer object
        if (!Xfer.Create())
        {
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
            return;
        }

        // Create buffer object
        if (!View.Create())
        {
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
            return;
        }
        Xfer.Snap(numFrames);

        Xfer.Wait(numFrames * 500);
        DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
        loc.Dispose();
    }
    public virtual void Xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
    {
        // refresh view
        SapView View = args.Context as SapView;
        View.Show();

        // save Buffer
        Buffers.GetAddress(out IntPtr buffAddress);
        SaveFrameArray(sizeArr, buffAddress);
    }
    public void WaitForTransferCompletion(int numFrames)
    {
        var tcs = new TaskCompletionSource<bool>();

        Xfer.XferNotify += (sender, args) =>
        {
            Buffers.GetAddress(out IntPtr buffAddress);
            SaveFrameArray(sizeArr, buffAddress);

            // Refresh view
            SapView view = args.Context as SapView;
            view.Show();

            // If all frames have been transferred, complete the task
            if (--numFrames == 0)
                tcs.TrySetResult(true);
        };

        Task.WhenAny(tcs.Task, Task.Delay(numFrames * 500)).Wait();
    }
    public static void SaveFrameArray(int size, IntPtr buffAddress)
    {
        Int16[] imageData = new Int16[size];
        Marshal.Copy(buffAddress, imageData, 0, size);

        for (int i = 0; i < size; i++)
        {
            framesArr[countFrame, i] = imageData[i];
        }
        countFrame++;
    }
    public static void InitializeFrameArray(int dim1, int dim2)
    {
        framesArr = (Int16[,])Array.CreateInstance(typeof(Int16), dim1, dim2);
    }
    public static void DestroysObjects(SapAcquisition acq, SapAcqDevice camera, SapBuffer buf, SapTransfer xfer, SapView view)
    {
        if (xfer != null)
        {
            xfer.Destroy();
            xfer.Dispose();
        }

        if (camera != null)
        {
            camera.Destroy();
            camera.Dispose();
        }

        if (acq != null)
        {
            acq.Destroy();
            acq.Dispose();
        }

        if (buf != null)
        {
            buf.Destroy();
            buf.Dispose();
        }

        if (view != null)
        {
            view.Destroy();
            view.Dispose();
        }
    }
}
