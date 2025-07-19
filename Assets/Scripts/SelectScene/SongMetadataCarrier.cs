using UnityEngine;

public class ChartMetadataCarrier : MonoBehaviour
{
    public static ChartMetadataCarrier Instance { get; private set; }

    private ChartMetadata chartMetadata;

    /// <summary>
    /// 创建并赋值一个携带 ChartMetadata 的 GameObject
    /// </summary>
    public static void Create(ChartMetadata chartMetadata)
    {
        GameObject carrierGO = new("ChartMetadataCarrier");
        ChartMetadataCarrier chartMetadataCarrier = carrierGO.AddComponent<ChartMetadataCarrier>();
        chartMetadataCarrier.SetChartMetadata(chartMetadata);
        DontDestroyOnLoad(carrierGO);
    }

    /// <summary>
    /// 尝试在场景中获取 ChartMetadata 并销毁 Carrier, 失败则返回 null
    /// </summary>
    public static ChartMetadata TryRetrieve()
    {
        return Instance?.RetrieveAndDestroy();
    }

    void Awake()
    {
        // 确保只有一个实例存在
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 初始化并赋值字典
    /// </summary>
    void SetChartMetadata(ChartMetadata chartMetadata)
    {
        this.chartMetadata = chartMetadata;
    }

    /// <summary>
    /// 获取字典并销毁自身
    /// </summary>
    public ChartMetadata RetrieveAndDestroy()
    {
        ChartMetadata data = chartMetadata;
        chartMetadata = null;

        // 延迟销毁以避免立即访问空引用
        Destroy(gameObject);
        Instance = null;

        return data;
    }
}
