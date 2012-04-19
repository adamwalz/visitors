// UtilityPlugin
// Nathan Swenson

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class UtilityPlugin : MonoBehaviour 
{
	// Import the C function from Xcode
	[DllImport ("__Internal")]
	private static extern void _PrintARCard();
	[DllImport ("__Internal")]
	private static extern int _ContentScaleFactor();
	
	// If we are running on the iOS simulator or on a device, bring up the airprint dialog
	public static void PrintARCard()
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer) _PrintARCard();
		else print("Can't use Airprint. Not enough iPhone.");
	}
	
	// This method returns two if we're on a retina display, otherwise it returns one.
	public static int ContentScaleFactor()
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer) return _ContentScaleFactor();
		else return 2;
	}
	
	// Returns the device's name
	public static string GetDeviceName()
	{
		return SystemInfo.deviceName;
	}
}
