using System;

namespace UnityEngine.Purchasing.Security;

public class AppleStoreKitTestTangle
{
	private static byte[] data = Convert.FromBase64String("9FE31ddOy+pFvlbeR62jbx/jmqzl3MF4NeYlJCYnJoQcFx4XKCEkckdREHXL2Is+eisyCK0o0q7v8Aw6LSsvdFNIVUJsTlMWNhcoISRyIywsFy4hJHIjITQlcnQWMBcyIS4Nob7clK1yuMg6D594DfZUuBJgEJxhdFNIVUJsTlMXOTAqFRcXExcWFhBstJwt34YGGe9G/gPveThSGf20/A2hb6HQKiYmLCInF3gWNhcoISRyNaTxoF9Kvpp5lC3Hv+nRzeSpSk5AAp6MSxRCiv/lvV9vKM7r5gAq4fDDX4+Ij0qA5v1PCkHr6A4sdwvE6F+Lep0ZeHcMew6r+uXwDTD4BZPASMc8wSpP6gJJ5FEv0m0siV1TGywea+xoSeeUeJCynZ6Pem1TjMhIq8ZW/G1i1Ao+sSRxpoFjyVJQfHQXeBY2FyghJHIjJCsvdFNIVUJsTuOOLJToRSjck5vQ/KN5D8xgKtbKFjYXKCEkciMsKy90U0hVQmxOUxZEjLC9DOQN//eVxvJ9Kh48SEAPjhctIS8MISYiIiAkJBcqIS4NoW+hmufxOme5cJ4ft1BRdfWPbFdJn0AXpSRTF6Ule4ckJSYlJSYmFyohLsyQZ6YaoBpXdvbjeqqC+1hNfz35JxelJi0lpSYmJ/xYtxsLVCj8sAErL3RTSFVCbE5TFiwXLiEkciMhNOhDG0qnfhSr7REhDTY8qm7pL9hBJXJ0FjAXMiEuDaFvodAqJi4mMS8SFRQSfTAqExcXFBUQFhASFRQSfQtSwWuWNfTaxDh5Klqff/CHW8NYUxY2FyghJHIjLSsvdFNIVUJsTlOiX2/p5+I1Nk0oK4kIIu1IXUNYB6yL0riXLh+ESVzsAXctPcau9lVRBRcqIS4NoW+h0ComJiYiJySlJig7yC6vX0LpptWqVcAgPUkBNdAVliqcH7dMIePbKsKyd+q2750C03/u52APbMx95gwucQuCjS4kXBUW+886NCYm2CMiFyQmJtgXKSEkcjooJm+h0ComLiYxL3RTSFVCbE5TF6UmFj22p56YwivsG5wpY0UB3SMOkW5mglkuqp0aGupXcP/KyPL9UO+0I9AqJiYsIickpSYmJ5UnxxvWz85FIyQrL3RTSFVCbE5TFjYXKCEkciMm2CMjJCUloxcxISRyOgImJtgjKznLDLYigBpo");

	private static int[] order = new int[45]
	{
		34, 27, 43, 3, 11, 33, 6, 15, 27, 41,
		35, 26, 25, 17, 20, 24, 27, 18, 35, 40,
		30, 32, 34, 42, 30, 26, 36, 33, 30, 39,
		41, 38, 40, 35, 34, 42, 37, 41, 41, 42,
		40, 41, 43, 43, 44
	};

	private static int key = 39;

	public static readonly bool IsPopulated = true;

	public static byte[] Data()
	{
		if (!IsPopulated)
		{
			return null;
		}
		return Obfuscator.DeObfuscate(data, order, key);
	}
}
