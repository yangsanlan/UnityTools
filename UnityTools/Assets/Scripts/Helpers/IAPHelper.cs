//
//  IAPHelper.cs
//  UnityTools
//
//  Created by JYMain on 6/23/2016.
//

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;

public class IAPHelper : MonoBehaviour
{
    public static readonly string[] Ids = new string[]
   {
        "20tools"
   };

    public static readonly string[] AndroidIds = new string[]
    {
        "com.jymain.unitytools.20"
    };

    public static readonly string[] iOSIds = new string[]
    {
        "com.jymain.unitytools.20"
    };

    public static readonly string[] WSAIds = new string[]
    {
        "com.jymain.unitytools.20"
    };

    public static readonly string[] AmazonIds = new string[]
    {
        "com.jymain.unitytools.20"
    };

    public static IAPHelper Instance;
    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        MarkProduct[] Products = new MarkProduct[AndroidIds.Length];
        for (int i = 0; i < Ids.Length; i++)
        {
            MarkProduct mp = new MarkProduct()
            {
                Id = Ids[i],
                Type = ProductType.Consumable,
                GooglePlayStoreProductId = AndroidIds[i],
                AmazonStoreProductId = AmazonIds[i],
                IosStoreProductId = iOSIds[i],
                WindowsStoreProductId = WSAIds[i]
            };
            Products[i] = mp;
        }

        IAPManager.Instance.Init(new IAPSetting()
        {
            //todo:jymain editor google play base 64 key
            GooglePublicKey = "",
            Products = Products,
            SandboxEnable = false
        });
        IAPManager.Instance.OnProcessPurchase += OnProcessPurchased;
        IAPManager.Instance.OnPurchaseFail += OnPurchaseFail;
        IAPManager.Instance.OnPurchaseDisable += OnPurchaseDisable;
    }

    private void OnPurchaseDisable()
    {
        // 支付不能用
        Debug.Log("Purchase Disable");
    }

    private void OnPurchaseFail(PurchaseFailureReason reason)
    {
        //支付失败原因
        Debug.Log("Purchase Fail:" + reason.ToString());
        if (reason == PurchaseFailureReason.UserCancelled)
        {
            //用户取消支付
        }
    }

    private void OnProcessPurchased(PurchaseEventArgs p)
    {
        ////支付完成后调用
        Debug.Log(p.purchasedProduct.metadata.localizedTitle);
        Debug.Log(p.purchasedProduct.metadata.isoCurrencyCode);
        Debug.Log(p.purchasedProduct.metadata.localizedDescription);
        Debug.Log(p.purchasedProduct.metadata.localizedPrice);
        Debug.Log(p.purchasedProduct.metadata.localizedPriceString);

        Debug.Log(p.purchasedProduct.definition.id);
        Debug.Log(p.purchasedProduct.definition.storeSpecificId);
        Debug.Log(p.purchasedProduct.definition.type.ToString());
    }

    public void OnClick()
    {
        //test purchase
        //IAPManager.Instance.OnPurchase(Ids[0], "");
    }
}
