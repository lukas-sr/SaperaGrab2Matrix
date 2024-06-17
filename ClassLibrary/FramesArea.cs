using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALSA.SaperaLT.SapClassBasic;

public class FramesArea : FramesTDI
{
    public FramesArea()
    {
        acqParams = new MyAcquisitionParams
        {
            ResourceIndex = 0,
            ServerName = "Xtium-CLHS_PX8_1"
        };
    }

    public override void StartSnap()
    {
        // Create buffer object
        if (!Buffers.Create())
        {
            DestroysObjects();
            return;
        }

        // For TDI Case the heigth of the buffer must be equal 1
        sizeArr = Buffers.Width * Buffers.Height;
        InitializeFrameArray(numFrames, sizeArr);

        // Create buffer object
        if (!Xfer.Create())
        {
            DestroysObjects();
            return;
        }

        // Create buffer object
        if (!View.Create())
        {
            DestroysObjects();
            return;
        }
        Xfer.Snap(numFrames);

        Xfer.Wait(numFrames * 500);
        DestroysObjects();
        loc.Dispose();
    }
}
