using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using DALSA.SaperaLT.SapClassBasic;

public class MyAcquisitionParams
{
    public MyAcquisitionParams()
    {
        m_ServerName = "";
        m_ResourceIndex = 0;
        m_ConfigFileName = "";
    }

    public MyAcquisitionParams(string ServerName, int ResourceIndex)
    {
        m_ServerName = ServerName;
        m_ResourceIndex = ResourceIndex;
        m_ConfigFileName = "";
    }

    public MyAcquisitionParams(string ServerName, int ResourceIndex, string ConfigFileName)
    {
        m_ServerName = ServerName;
        m_ResourceIndex = ResourceIndex;
        m_ConfigFileName = ConfigFileName;
    }

    public string ConfigFileName
    {
        get { return m_ConfigFileName; }
        set { m_ConfigFileName = value; }
    }

    public string ServerName
    {
        get { return m_ServerName; }
        set { m_ServerName = value; }
    }

    public int ResourceIndex
    {
        get { return m_ResourceIndex; }
        set { m_ResourceIndex = value; }
    }

    protected string m_ServerName;
    protected int m_ResourceIndex;
    protected string m_ConfigFileName;
}
