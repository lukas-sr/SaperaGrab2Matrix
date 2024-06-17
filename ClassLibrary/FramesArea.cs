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
            ServerName = "Xtium - CLHS_PX8_1"
        };
    }
    public override void Xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
    {
        // save Buffer

        Buffers.GetAddress(out IntPtr buffAddress);
        
        SaveFrameArray(sizeArr, buffAddress);

        // refresh view
        SapView View = args.Context as SapView;
        View.Show();
    }
}
