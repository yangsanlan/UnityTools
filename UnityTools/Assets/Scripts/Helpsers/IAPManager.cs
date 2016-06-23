//
//  IAPManager.cs
//  UnityTools
//
//  Created by DefaultCompany on 6/22/2016.
//

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : IStoreListener
{
    // Use this for initialization
    private ConfigurationBuilder builder;
    // public StoreListener StoreListener;

    private static IAPManager _instance;

    public static IAPManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IAPManager();
            }
            return _instance;
        }
    }

    private IStoreController StoreController;
    private IExtensionProvider ExtensionProvider;
    private IAPSetting IAPSetting;
    public Action<PurchaseEventArgs> OnProcessPurchase;
    public Action OnPurchaseDisable;
    public Action<PurchaseFailureReason> OnPurchaseFail;

    public void Init(IAPSetting iapSetting)
    {
        IAPSetting = iapSetting;
        var module = StandardPurchasingModule.Instance();
        builder = ConfigurationBuilder.Instance(module);
        builder.Configure<IGooglePlayConfiguration>().SetPublicKey(IAPSetting.GooglePublicKey);
        //StoreListener = new StoreListener(this);

        for (var i = 0; i < IAPSetting.Products.Length; i++)
        {
            var ids = new IDs();
            if (!string.IsNullOrEmpty(IAPSetting.Products[i].GooglePlayStoreProductId))
            {
                ids.Add(IAPSetting.Products[i].GooglePlayStoreProductId, GooglePlay.Name);
            }
            if (!string.IsNullOrEmpty(IAPSetting.Products[i].IosStoreProductId))
            {
                ids.Add(IAPSetting.Products[i].IosStoreProductId, AppleAppStore.Name);
            }
            if (!string.IsNullOrEmpty(IAPSetting.Products[i].MacStoreProductId))
            {
                ids.Add(IAPSetting.Products[i].MacStoreProductId, MacAppStore.Name);
            }
            if (!string.IsNullOrEmpty(IAPSetting.Products[i].WindowsStoreProductId))
            {
                ids.Add(IAPSetting.Products[i].WindowsStoreProductId, WindowsStore.Name);
            }
            builder.AddProduct(IAPSetting.Products[i].Id, IAPSetting.Products[i].Type, ids);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// 获取所有商品信息
    /// </summary>
    /// <returns></returns>
    public Product[] GetProducts()
    {
        return StoreController.products.all;
    }

    /// <summary>
    /// 获取商品详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Product GetProduct(string id)
    {
        return StoreController.products.WithID(id);
    }

    /// <summary>
    /// 支付
    /// </summary>
    /// <param name="id"></param>
    /// <param name="payload"></param>
    public void OnPurchase(string id, string payload = "")
    {
        StoreController.InitiatePurchase(id, payload);
    }

    /// <summary>
    ///     Called when Unity IAP encounters an unrecoverable initialization error.
    /// </summary>
    /// <param name="error"></param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log(string.Format("IAPManager initialize failed by {0}", error));
        switch (error)
        {
            case InitializationFailureReason.PurchasingUnavailable:
                Debug.Log("Purchase Disable!");
                if (OnPurchaseDisable != null) OnPurchaseDisable();
                break;
            case InitializationFailureReason.NoProductsAvailable:
                Debug.Log("No products available for purchase!");
                break;
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
        }
    }

    /// <summary>
    ///     Called when a purchase completes.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log(string.Format("Purchase Success:{0}", e.purchasedProduct.definition.id));
        if (OnProcessPurchase == null)
        {
            Debug.LogError("OnProcessPurchase is null");
            return PurchaseProcessingResult.Pending;
        }
        if (e.purchasedProduct.hasReceipt)
        {
            var dic = SimpleJson.SimpleJson.DeserializeObject<Dictionary<string, object>>(e.purchasedProduct.receipt);
            var store = "";
#if UNITY_ANDROID
            store = dic["Store"].ToString(); 
#elif UNITY_WSA
            store = "WinRT"
#endif
            switch (store)
            {
                case GooglePlay.Name:
                    var payload = dic["Payload"];
                    var payloadDict =
                        SimpleJson.SimpleJson.DeserializeObject<Dictionary<string, object>>(payload.ToString());
                    var json = payloadDict["json"].ToString();
                    Debugger.Log(string.Format("Json = {0}", json));
                    var signature = payloadDict["signature"].ToString();
                    Debugger.Log(string.Format("signature = {0}", signature));
                    if (VerifyGooglePlay(json, signature, IAPSetting.GooglePublicKey))
                    {
                        OnProcessPurchase(e);
                    }
                    else
                    {
                        OnPurchaseFailed(e.purchasedProduct, PurchaseFailureReason.SignatureInvalid);
                    }
                    break;
                default:
                    OnProcessPurchase(e);
                    break;
            }
        }
        else
        {
            OnProcessPurchase(e);
        }
        Debug.Log(string.Format("IAPManager {0} purchase processed", e.purchasedProduct.metadata.localizedTitle));
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    ///     Called when a purchase fails.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="p"></param>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log(string.Format("Purchase {1} Failed by {0}", p, i.metadata.localizedTitle));
        if (OnPurchaseFail != null)
            OnPurchaseFail(p);
    }

    /// <summary>
    ///     Called when Unity IAP is ready to make purchases.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        StoreController = controller;
        ExtensionProvider = extensions;
    }

#if UNITY_IOS || UNITY_IPHONE
    public void ReStorePurchase()
    {
        ExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(result =>
        {
            if (result)
            {
                //restore success
            }
            else
            {

            }
        });
    } 
#endif


    //#if UNITY_ANDROID
    private bool VerifyGooglePlay(string purchaseInfo, string signature, string publicKey)
    {
        var purchaseInfoBuffer = Encoding.UTF8.GetBytes(purchaseInfo);
        var signatureBuffer = Convert.FromBase64String(signature);
        var publickKeyBuffer = Convert.FromBase64String(publicKey);

        var rsaParameters = new RSAParameters();
        var modulusOffset = 33;
        var modulusBytes = 256;
        var exponentOffset = 291;
        var exponentBytes = 3;

        var modulus = new byte[modulusBytes];
        for (var i = 0; i < modulusBytes; ++i)
        {
            modulus[i] = publickKeyBuffer[modulusOffset + i];
        }
        var exponent = new byte[exponentBytes];
        for (var i = 0; i < exponentBytes; ++i)
        {
            exponent[i] = publickKeyBuffer[exponentOffset + i];
        }

        rsaParameters.Modulus = modulus;
        rsaParameters.Exponent = exponent;
        using (var sha1 = SHA1.Create())
        {
            var hash = sha1.ComputeHash(purchaseInfoBuffer);

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportParameters(rsaParameters);
                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA1");
                return rsaDeformatter.VerifySignature(hash, signatureBuffer);
            }
        }
    }
    //#endif
}

public class IAPSetting
{
    public string GooglePublicKey;
    public MarkProduct[] Products;
}

public class MarkProduct
{
    public string GooglePlayStoreProductId;
    public string Id;
    public string IosStoreProductId;
    public string MacStoreProductId;
    public ProductType Type;
    public string WindowsStoreProductId;
}