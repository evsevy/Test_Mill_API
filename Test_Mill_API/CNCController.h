using System;

namespace CNCControllerApp
{
    public interface ICNCController
    {
        short READ_information(out short Axes, out string CncType, out short MaxAxes, out string Series, out string Nc_Ver, out string[] AxisName);
        short READ_status(out string MainProg, out string CurProg, out int CurSeq, out string Mode, out string Status, out string Alarm, out string EMG);
    }
}
