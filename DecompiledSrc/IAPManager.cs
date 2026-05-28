using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPManager : MonoBehaviour
{
	private enum BundleState
	{
		Idle,
		Downloading,
		Installing,
		Unzipping,
		Completed
	}

	private StoreController storeController;

	private CrossPlatformValidator validator;

	public const string neoCosmosId = "com.7thbeat.adofai.neocosmos";

	public string neoCosmosPrice = "";

	public string neoCosmosDownloadMessage = "";

	public Action<IAPState> StateNotifier;

	private Action OnCompleteAction;

	private TMP_Text downloadText;

	private IAPState state;

	private BundleState bundleState;

	private const int neoCosmosSpaceRequired = 500;

	public IAPState GetState()
	{
		return state;
	}

	public bool IsDownloading()
	{
		return bundleState != BundleState.Idle;
	}

	private void Awake()
	{
		if (ADOBase.isMobile)
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			StartCoroutine(CheckInternectConnection());
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (bundleState == BundleState.Installing)
		{
			bundleState = BundleState.Unzipping;
			SetDownloadTextValue(RDString.Get("editor.dialog.installing"));
		}
		else if (bundleState == BundleState.Completed)
		{
			bundleState = BundleState.Idle;
			Complete();
		}
	}

	private void InitializeValidator()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		try
		{
			if (CanPlatformValidateLocal())
			{
				validator = new CrossPlatformValidator(GooglePlayTangle.Data(), Application.identifier);
			}
		}
		catch (NotImplementedException arg)
		{
			Debug.Log($"Cross Platform Validator Not Implemented: {arg}");
		}
	}

	public async void Initialize()
	{
		if (state == IAPState.Uninitialized)
		{
			SetState(IAPState.Loading);
			InitializeValidator();
			storeController = UnityIAPServices.StoreController((string)null);
			storeController.OnPurchasePending += OnPurchasePending;
			storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
			storeController.OnPurchaseFailed += OnPurchaseFailed;
			storeController.OnPurchaseDeferred += OnPurchaseDeferred;
			storeController.OnStoreDisconnected += OnStoreDisconnected;
			await storeController.Connect();
			FetchProducts();
		}
	}

	private void OnStoreDisconnected(StoreConnectionFailureDescription description)
	{
		neoCosmosDownloadMessage = $"In-App Purchasing Store disconnected: {description}";
		SetState(IAPState.Failed);
		Debug.Log($"In-App Purchasing Store disconnected: {description}");
	}

	private void FetchProducts()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		storeController.OnProductsFetchFailed += OnProductsFetchFailed;
		storeController.OnProductsFetched += OnProductsFetched;
		List<ProductDefinition> list = new List<ProductDefinition>
		{
			new ProductDefinition("com.7thbeat.adofai.neocosmos", (ProductType)1)
		};
		storeController.FetchProducts(list, (IRetryPolicy)null);
	}

	private void OnProductsFetchFailed(ProductFetchFailed failure)
	{
		neoCosmosDownloadMessage = "In-App Purchasing products fetch failed: " + failure.FailureReason;
		SetState(IAPState.Failed);
		Debug.Log("In-App Purchasing products fetch failed: " + failure.FailureReason);
	}

	private void OnProductsFetched(List<Product> products)
	{
		storeController.OnPurchasesFetched += OnPurchasesFetched;
		storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
		if (products == null || products.Count < 1)
		{
			neoCosmosDownloadMessage = "In-App Purchasing products fetch failed: No products founded";
			SetState(IAPState.Failed);
			Debug.Log("In-App Purchasing products fetch failed: No products founded");
		}
		neoCosmosPrice = products.FirstOrDefault((Product product) => product.definition.id == "com.7thbeat.adofai.neocosmos").metadata.localizedPriceString;
		storeController.FetchPurchases();
	}

	private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log($"In-App Purchasing purchases fetch failed: {failure.FailureReason}");
		OnInitialized();
	}

	private void OnPurchasesFetched(Orders orders)
	{
		OnInitialized(orders);
	}

	private void OnInitialized(Orders orders = null)
	{
		if (HasNeoCosmos())
		{
			UnlockNeoCosmos();
		}
		SetState(IAPState.Successfully);
	}

	public void RestorePurchases(Action OnCompleteAction = null)
	{
		this.OnCompleteAction = OnCompleteAction;
		storeController.RestoreTransactions((Action<bool, string>)OnRestoreTransactions);
	}

	private void OnRestoreTransactions(bool success, string errorMessage)
	{
		neoCosmosDownloadMessage = "";
		if (success)
		{
			if (HasNeoCosmos())
			{
				UnlockNeoCosmos();
			}
		}
		else
		{
			neoCosmosDownloadMessage = RDString.Get("error.restorePurchases") + " " + errorMessage;
			Debug.Log("[IAP] Restore failed with error: " + errorMessage);
		}
		Complete();
	}

	public void OnPurchaseClicked(string productId, Action OnCompleteAction = null)
	{
		this.OnCompleteAction = OnCompleteAction;
		storeController.PurchaseProduct(productId);
	}

	private void OnPurchasePending(PendingOrder order)
	{
		if (GetFirstProductInOrder((Order)(object)order) == null)
		{
			neoCosmosDownloadMessage = "Error: Could not find product in order. Could not validate order.";
			Debug.Log("Could not find product in order. Could not validate order.");
			Complete();
		}
		else
		{
			StartCoroutine(BackEndValidation(order));
		}
	}

	private void OnPurchaseConfirmed(Order order)
	{
		if (neoCosmosDownloadMessage != null || neoCosmosDownloadMessage != "")
		{
			Complete();
		}
		Product firstProductInOrder = GetFirstProductInOrder(order);
		if (firstProductInOrder == null)
		{
			Debug.Log("Could not find product in purchase confirmation.");
		}
		if (!(order is ConfirmedOrder))
		{
			if (order is FailedOrder)
			{
				neoCosmosDownloadMessage = "Confirmation failed - Product: " + GetIdFromProduct(firstProductInOrder);
				Debug.Log("Confirmation failed - Product: " + GetIdFromProduct(firstProductInOrder));
			}
			else
			{
				neoCosmosDownloadMessage = "Confirmation failed - Product: " + GetIdFromProduct(firstProductInOrder);
			}
		}
		else
		{
			if (firstProductInOrder.definition.id == "com.7thbeat.adofai.neocosmos")
			{
				UnlockNeoCosmos();
			}
			Debug.Log("Order confirmed - Product: " + GetIdFromProduct(firstProductInOrder));
		}
		Complete();
	}

	private void OnPurchaseFailed(FailedOrder order)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (GetFirstProductInOrder((Order)(object)order) == null)
		{
			Debug.Log("Could not find product in failed order.");
		}
		neoCosmosDownloadMessage = RDString.Get("error.purchaseFailed") + ". " + RDString.Get("error.purchaseFailed." + ((object)order.FailureReason/*cast due to constrained. prefix*/).ToString());
		Debug.Log($"Purchase failed, PurchaseFailureReason: {order.FailureReason}");
		Complete();
	}

	private void OnPurchaseDeferred(DeferredOrder order)
	{
		Debug.Log("Purchase deferred - Product: " + GetIdFromProduct(GetFirstProductInOrder((Order)(object)order)));
	}

	private IEnumerator BackEndValidation(PendingOrder order)
	{
		neoCosmosDownloadMessage = "";
		if (!IsPurchaseValid((Order)(object)order))
		{
			neoCosmosDownloadMessage = RDString.Get("error.receiptInvalid");
			storeController.ConfirmPurchase(order);
			yield break;
		}
		string receipt = ((Order)order).Info.Receipt;
		if (receipt.IsNullOrEmpty())
		{
			neoCosmosDownloadMessage = RDString.Get("error.receiptMissing");
			Debug.Log("The receipt is null or empty");
			Complete();
			yield break;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(receipt);
		UnityWebRequest request = new UnityWebRequest($"https://7thbe.at/iap/adofai/neo-cosmos?platform={ADOBase.platform}&version={GCNS.buildCommit}&check=1", "POST", (DownloadHandler)new DownloadHandlerBuffer(), (UploadHandler)new UploadHandlerRaw(bytes)
		{
			contentType = "application/json"
		});
		yield return request.SendWebRequest();
		if ((int)request.result == 2)
		{
			neoCosmosDownloadMessage = RDString.Get("error.connection");
			Debug.Log(request.error);
		}
		else if (request.responseCode == 403)
		{
			neoCosmosDownloadMessage = RDString.Get("error.receiptInvalid");
			Debug.Log("The receipt is invalid");
		}
		else if (request.responseCode == 409)
		{
			neoCosmosDownloadMessage = RDString.Get("error.receiptBanned");
			Debug.Log("The receipt is banned (piracy)");
		}
		else if (request.responseCode == 429)
		{
			neoCosmosDownloadMessage = RDString.Get("error.IAP.TooManyRequests");
			Debug.Log("Too many requests");
		}
		else if (request.responseCode != 204)
		{
			neoCosmosDownloadMessage = RDString.Get("error.server");
			Debug.Log("Server error, try again later");
		}
		else
		{
			storeController.ConfirmPurchase(order);
		}
	}

	public IEnumerator GetNeoCosmosURL(Action OnCompleteAction = null)
	{
		this.OnCompleteAction = OnCompleteAction;
		neoCosmosDownloadMessage = "";
		string receipt = storeController.GetPurchases().FirstOrDefault((Order order) => order.CartOrdered.Items().Any((CartItem item) => item.Product.definition.id == "com.7thbeat.adofai.neocosmos")).Info.Receipt;
		if (receipt.IsNullOrEmpty())
		{
			neoCosmosDownloadMessage = RDString.Get("error.receiptMissing");
			Debug.Log("The receipt is null or empty");
			Complete();
			yield break;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(receipt);
		UnityWebRequest request = new UnityWebRequest($"https://7thbe.at/iap/adofai/neo-cosmos?platform={ADOBase.platform}&version={GCNS.buildCommit}", "POST", (DownloadHandler)new DownloadHandlerBuffer(), (UploadHandler)new UploadHandlerRaw(bytes)
		{
			contentType = "application/json"
		});
		yield return request.SendWebRequest();
		if ((int)request.result == 2)
		{
			neoCosmosDownloadMessage = RDString.Get("error.connection");
			Debug.Log(request.error);
		}
		else if (request.responseCode == 403)
		{
			neoCosmosDownloadMessage = RDString.Get("error.receiptInvalid");
			Debug.Log("The receipt is invalid");
		}
		else if (request.responseCode == 409)
		{
			neoCosmosDownloadMessage = RDString.Get("error.receiptBanned");
			Debug.Log("The receipt is banned (piracy)");
		}
		else if (request.responseCode == 410)
		{
			neoCosmosDownloadMessage = RDString.Get("error.preRelease");
			Debug.Log("The version doesn't exist anymore)");
		}
		else if (request.responseCode != 200)
		{
			neoCosmosDownloadMessage = RDString.Get("error.server");
			Debug.Log("Server error, try again later");
		}
		else
		{
			neoCosmosDownloadMessage = request.downloadHandler.text;
		}
		Complete();
	}

	public void SetDownloadValues(TMP_Text downloadText, Action OnCompleteAction)
	{
		this.downloadText = downloadText;
		this.OnCompleteAction = OnCompleteAction;
	}

	public void DownloadNeoCosmos(string url, TMP_Text downloadText = null, Action OnCompleteAction = null)
	{
		this.downloadText = downloadText;
		this.OnCompleteAction = OnCompleteAction;
		UninstallNeoCosmosBundle();
		double num = -1.0;
		num = GameServices.Instance.GetAvailableDiskSpace();
		if (num != -1.0 && num - 500.0 < 0.0)
		{
			neoCosmosDownloadMessage = RDString.Get("error.noSpace.NeoCosmos", new Dictionary<string, object> { { "space", 500 } });
			Debug.Log("Download failed: Not enough space available for Neo Cosmos bundle");
			bundleState = BundleState.Completed;
		}
	}

	private void Unzip(string zipPath, string zipFilePath, string directoryPath, string scenesPath, string shadersPath)
	{
		bundleState = BundleState.Installing;
		bool flag = false;
		string text = "";
		try
		{
			ZipUtils.Unzip(zipFilePath, directoryPath);
			flag = true;
		}
		catch (Exception ex)
		{
			text = ex.Message;
			Debug.Log("Unzip failed: " + ex.ToString());
		}
		if (RDFile.Exists(zipFilePath))
		{
			RDFile.Delete(zipFilePath);
		}
		if (RDFile.Exists(zipPath))
		{
			RDFile.Delete(zipPath);
		}
		if (!flag)
		{
			neoCosmosDownloadMessage = RDString.Get("editor.notification.unzipFailed") + ": " + text;
			bundleState = BundleState.Completed;
			return;
		}
		if (RDFile.Exists(scenesPath) && RDFile.Exists(shadersPath))
		{
			string text2 = RDUtils.GenerateHash(scenesPath);
			string text3 = RDUtils.GenerateHash(shadersPath);
			string[] neoCosmosScenesHashes = GCNS.neoCosmosScenesHashes;
			if (text2 == neoCosmosScenesHashes[neoCosmosScenesHashes.Length - 1])
			{
				string[] neoCosmosShadersHashes = GCNS.neoCosmosShadersHashes;
				if (text3 == neoCosmosShadersHashes[neoCosmosShadersHashes.Length - 1])
				{
					NeoCosmosManager.instance.installed = true;
					goto IL_00f3;
				}
			}
			neoCosmosDownloadMessage = RDString.Get("error.corrupted.NeoCosmos");
		}
		else
		{
			neoCosmosDownloadMessage = RDString.Get("editor.notification.unzipFailed");
		}
		goto IL_00f3;
		IL_00f3:
		bundleState = BundleState.Completed;
	}

	public void UninstallNeoCosmosBundle()
	{
		if (RDFile.Exists(GCNS.neoCosmosBundleAssetsPath))
		{
			RDFile.Delete(GCNS.neoCosmosBundleAssetsPath);
		}
		if (RDFile.Exists(GCNS.neoCosmosBundleScenesPath))
		{
			RDFile.Delete(GCNS.neoCosmosBundleScenesPath);
		}
		if (RDFile.Exists(GCNS.bundleShadersPath))
		{
			RDFile.Delete(GCNS.bundleShadersPath);
		}
		NeoCosmosManager.instance.installed = false;
	}

	private IEnumerator CheckInternectConnection()
	{
		yield return new WaitForSeconds(10f);
		if (state == IAPState.Loading)
		{
			SetState(IAPState.NoInternet);
		}
	}

	private void SetState(IAPState state)
	{
		this.state = state;
		StateNotifier?.Invoke(this.state);
	}

	private void UnlockNeoCosmos()
	{
		NeoCosmosManager.instance.own = true;
	}

	private bool HasNeoCosmos()
	{
		return storeController.GetPurchases().SelectMany((Order order) => order.CartOrdered.Items()).Any((CartItem item) => item.Product.definition.id == "com.7thbeat.adofai.neocosmos");
	}

	private void Complete()
	{
		Action onCompleteAction = OnCompleteAction;
		OnCompleteAction = null;
		onCompleteAction?.Invoke();
	}

	private void SetDownloadTextValue(string text)
	{
		if (downloadText != null)
		{
			downloadText.text = text;
		}
	}

	private bool IsPurchaseValid(Order order)
	{
		//IL_0024: Expected O, but got Unknown
		if (CanPlatformValidateLocal())
		{
			try
			{
				validator.Validate(order.Info.Receipt);
				return true;
			}
			catch (IAPSecurityException ex)
			{
				IAPSecurityException arg = ex;
				Debug.Log($"Invalid receipt: {arg}");
				return false;
			}
		}
		return true;
	}

	private bool CanPlatformValidateLocal()
	{
		return IsGooglePlay();
	}

	private bool IsGooglePlay()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return DefaultStoreHelper.GetDefaultStoreName() == "GooglePlay";
		}
		return false;
	}

	private Product GetFirstProductInOrder(Order order)
	{
		CartItem obj = order.CartOrdered.Items().FirstOrDefault();
		if (obj == null)
		{
			return null;
		}
		return obj.Product;
	}

	private string GetIdFromProduct(Product product)
	{
		return ((product != null) ? product.definition.id : null) ?? "no product found";
	}

	private string GetDataPathFromURL(string url)
	{
		string[] array = url.Split('?', StringSplitOptions.None)[0].Split('/', StringSplitOptions.None);
		string path = array[array.Length - 1];
		string bundlesLoadPath = GCNS.BundlesLoadPath;
		if (!Directory.Exists(bundlesLoadPath))
		{
			RDDirectory.CreateDirectory(bundlesLoadPath);
		}
		return Path.Combine(bundlesLoadPath, path);
	}
}
