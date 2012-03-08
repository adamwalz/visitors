// AirprintPlugin
// Nathan Swenson

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class AirprintPlugin : MonoBehaviour 
{
	// Import the C function from Xcode
	[DllImport ("__Internal")]
	private static extern void _PrintARCard();
	
	// If we are running on the iOS simulator or on a device, bring up the airprint view
	public static void PrintARCard()
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer) _PrintARCard();
		else print("Can't use Airprint. Not enough iPhone.");
	}
}
