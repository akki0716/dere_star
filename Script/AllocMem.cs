using UnityEngine;
using System.Collections;
using System.Text;
using System.Linq;
using UnityEngine.Profiling;

[ExecuteInEditMode()]

public class AllocMem : MonoBehaviour
{
    //------------------------------------------------------------
    // constant
    //------------------------------------------------------------
    public const int SIZE_KB = 1024;
    public const int SIZE_MB = 1024 * 1024;

    double lower_fps = 60;

    //------------------------------------------------------------
    // Start
    //------------------------------------------------------------
    void Start()
    {
        useGUILayout = false;
    }
    //------------------------------------------------------------
    // OnGUI
    //------------------------------------------------------------
    void OnGUI()
    {
        // profile
        profileStats();
        // draw
        if (m_IsShow)
        {
            if (Application.isPlaying)
            {
                drawStats();
            }
            else {
                if (m_IsShowEditor)
                {
                    drawStats();
                }
            }
        }
    }

    //------------------------------------------------------------
    // profile stats
    //------------------------------------------------------------
    private void profileStats()
    {
        // check GC
        int collCount = System.GC.CollectionCount(0);
        if (m_LastCollectNum != collCount)
        {
            m_LastCollectNum = collCount;
            m_CollectDeltaTime = Time.realtimeSinceStartup - m_LastCollectTime;
            m_LastCollectTime = Time.realtimeSinceStartup;
            m_LastCollectDeltaTime = Time.deltaTime;
            m_CollectMemSize = m_UsedMemSize;
        }

        // check memory
        m_UsedMemSize = System.GC.GetTotalMemory(false);  // byte
        if (m_UsedMemSize > m_MaxUsedMemSize)
        {
            m_MaxUsedMemSize = m_UsedMemSize;
        }

        // calc alloc rate
        if ((Time.realtimeSinceStartup - m_LastAllocSet) > 0.3f)
        {
            long diff = m_UsedMemSize - m_LastAllocMemSize;

            m_LastAllocMemSize = m_UsedMemSize;
            m_LastAllocSet = Time.realtimeSinceStartup;

            if (diff >= 0)
            {
                m_AllocRate = diff;
            }
        }
    }
    //------------------------------------------------------------
    // draw stats
    //------------------------------------------------------------
    private void drawStats()
    {
        // make string with builder
        StringBuilder text = new StringBuilder();

        // FPS
        text.Append((1f / Time.deltaTime).ToString("0.000") + " FPS")
            .AppendLine();

        if ((1f / Time.deltaTime) <= lower_fps)
        {
            lower_fps = (1f / Time.deltaTime);
        }

        text.Append("最低FPS "+ lower_fps)
            .AppendLine();


        // current memory
        text.Append("使用メモリ                      ")
            .Append(((float)m_UsedMemSize / SIZE_MB).ToString("0.00") + " MB")
            .AppendLine();
        // max used memory
        text.Append("最大使用メモリ              ")
            .Append(((float)m_MaxUsedMemSize / SIZE_MB).ToString("0.00") + " MB")
            .AppendLine();
        // Profiler.usedHeapSize
        text.Append("Used Prog Heap                ")
            .Append(((float)Profiler.usedHeapSize / SIZE_MB).ToString("0.00") + " MB")
            .AppendLine();
        // GetMonoUsedSize (byte)
        text.Append("Mono Used                 ")
            .Append(((float)Profiler.GetMonoUsedSize() / SIZE_MB).ToString("0.00") + " MB")
            .AppendLine();
        // GetMonoHeapSize (byte)
        text.Append("Mono Heap                 ")
            .Append(((float)Profiler.GetMonoHeapSize() / SIZE_MB).ToString("0.00") + " MB")
            .AppendLine();
        // GetTotalAllocatedMemory (byte)
        text.Append("Total Alloc Mem               ")
            .Append(((float)Profiler.GetTotalAllocatedMemory() / SIZE_MB).ToString("0.00") + " MB")
            .AppendLine();
        // SystemInfo.systemMemorySize (mb)
        text.Append("システムメモリ         ")
            .Append(SystemInfo.systemMemorySize.ToString() + " MB")
            .AppendLine();

        // sub memory
        if (m_IsShowSubMemory)
        {
            // GetTotalReservedMemory (byte)
            text.Append("Total Reserved Mem        ")
                .Append(((float)Profiler.GetTotalReservedMemory() / SIZE_MB).ToString("0.00") + " MB")
                .AppendLine();
            // GetTotalUnusedReserved (byte)
            text.Append("Total Unused Res-Mem  ")
                .Append(((float)Profiler.GetTotalUnusedReservedMemory() / SIZE_MB).ToString("0.00") + " MB")
                .AppendLine();
            // graphicMemorySize
            text.Append("Graphic Memory            ")
                .Append(SystemInfo.graphicsMemorySize.ToString() + " MB")
                .AppendLine();
        }
        // GC
        if (m_IsShowGC)
        {
            // alloc rate
            text.Append("Allocation rate               ")
                .Append(((float)m_AllocRate / SIZE_MB).ToString("0.00") + " MB")
                .AppendLine();
            // GC frequency
            text.Append("Collection frequency      ")
                .Append(m_CollectDeltaTime.ToString("0.00") + " s")
                .AppendLine();
            // time.deltaTime when last GC ?
            text.Append("Last collect delta            ")
                .Append(m_LastCollectDeltaTime.ToString("0.000") + " s (")
                .Append((1f / m_LastCollectDeltaTime).ToString("0.0") + " fps)")
                .AppendLine();
        }

        // draw
        int lineCount = text.ToString().ToList().Where(c => c.Equals('\n')).Count();

        GUI.Box(new Rect(5, 5, 310, (int)(m_FontSizeBase * lineCount) + 5), "");
        GUI.Label(new Rect(10, 5, Screen.width, Screen.height), text.ToString());
    }

    //------------------------------------------------------------
    // member
    //------------------------------------------------------------
    public bool m_IsShow = true;
    public bool m_IsShowSubMemory = false;
    public bool m_IsShowGC = false;
    public bool m_IsShowEditor = false;

    public float m_FontSizeBase = 15.175f;

    private long m_UsedMemSize;
    private long m_MaxUsedMemSize;

    private long m_CollectMemSize;
    private float m_LastCollectTime;
    private float m_LastCollectNum;
    private float m_CollectDeltaTime;
    private float m_LastCollectDeltaTime;
    private long m_AllocRate;
    private long m_LastAllocMemSize;
    private float m_LastAllocSet = -9999.0f;
}