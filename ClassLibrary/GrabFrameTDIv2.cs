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

public class GrabFramesTDIv2
{
    public static SapAcquisition Acq = null;
    public static SapAcqDevice AcqDevice = null;
    public static SapBuffer Buffers = null;
    public static SapTransfer Xfer = null;
    public static SapView View = null;
    public static Int16[,] tempMatrix;
    // TODO: Lista o Labview entende como .NET object, preciso organizar outro elemento para poder observar
    public static List<Int16[,]> frames = new List<Int16[,]>();
    public MyAcquisitionParams acqParams = new MyAcquisitionParams();
    private static int countFrame = 0;

    public GrabFramesTDIv2()
    {
        acqParams = new MyAcquisitionParams
        {
            ResourceIndex = 0,
            ServerName = "Xtium - CLHS_PX8_1"
        };
    }

    public bool setConfigFile(string filePath)
    {
        acqParams.ConfigFileName = filePath;

        if ((acqParams.ConfigFileName != null) && (acqParams.ServerName.Equals("Xtium - CLHS_PX8_1")))
        {
            return true;
        }

        return false;
    }

    public void StartGrabFramesTDI(int numFrames)
    {
        SapLocation loc = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);

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

        // Create buffer object
        if (!Buffers.Create())
        {
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
            return;
        }

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

        //Xfer.Grab();
        if (numFrames > 0 && numFrames < 100)
        {
            Xfer.Snap(numFrames);
        }
        else
        {
            Xfer.Snap();
        }
        Xfer.Wait(1000);
        DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
        loc.Dispose();
    }

    static void Xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
    {
        // refresh view
        SapView View = args.Context as SapView;
        View.Show();

        // save Buffer
        int width = Buffers.Width;
        int height = 1; //Buffers.Height;
        Buffers.GetAddress(out IntPtr buffAddress);

        tempMatrix = SaveFrames(width, height, buffAddress);
    }

    static void DestroysObjects(SapAcquisition acq, SapAcqDevice camera, SapBuffer buf, SapTransfer xfer, SapView view)
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
    public static Int16[,] SaveFrames(int width, int height, IntPtr buffAddress)
    {
        Int16[] imageData = new Int16[width * height];
        Int16[,] matrix = new Int16[width, height];
        Marshal.Copy(buffAddress, imageData, 0, width * height);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                matrix[j, i] = imageData[i + j];
            }
        }

        frames.Insert(countFrame, matrix);
        countFrame++;

        return matrix;   
    }
    public static void SaveImageData2File(int width, int height, IntPtr buffAddress)
    {
        Int16[] imageData = new Int16[width * height];
        Marshal.Copy(buffAddress, imageData, 0, width * height);

        string filePath = @"C:\temp\matrix" + countFrame + ".txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    writer.Write(imageData[i + j].ToString() + " ");
                }
                writer.WriteLine();
            }
        }
    }
    public static void Frames2File()
    {
        string filePath = @"C:\temp\frames.txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var mat in frames)
            {
                writer.WriteLine("Matriz:");
                for (int i = 0; i < mat.GetLength(0); i++)
                {
                    for (int j = 0; j < mat.GetLength(1); j++)
                    {
                        writer.Write(mat[i, j].ToString() + " ");
                    }
                    writer.WriteLine();
                }
            }

        }
    }
    public static List<Int16[,]> GetAllFrames()
    {
        return frames;
    }
}
