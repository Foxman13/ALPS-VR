/************************************************************************
	ALPSWP8 is an interface with the Windows Phone system
	
    Copyright (C) 2014  ALPS VR. - Jasaon Fox, Peter Daukintis

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

************************************************************************/

using UnityEngine;
using System;

public class ALPSWP8 : MonoBehaviour
{
    /// <summary>
    /// Initializes Android ALPS Activity.
    /// </summary>
    public static void Init()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    /// <summary>
    /// Vibrates constantly for 8 milliseconds.
    /// </summary>
    public static void Vibrate()
    {
        Vibrate(8);
    }

    /// <summary>
    /// Vibrates constantly for the specified period of time.
    /// </summary>
    /// <param name="_milliseconds">The number of milliseconds to vibrate.</param>
    public static void Vibrate(int _milliseconds)
    {
#if NETFX_CORE
            var vibrationDevice = Windows.Phone.Devices.Notification.VibrationDevice.GetDefault();
            vibrationDevice.Vibrate(TimeSpan.FromMilliseconds(_milliseconds));
#endif
    }

    /// <summary>
    /// The absolute width of the display in pixels.
    /// </summary>
    public static int WidthPixels()
    {
        return Screen.currentResolution.width;
    }

    /// <summary>
    /// The absolute height of the display in pixels.
    /// </summary>
    public static int HeightPixels()
    {
        return Screen.currentResolution.height;
    }

    /// <summary>
    /// The device orientation.
    /// </summary>
    public static Quaternion DeviceOrientation()
    {
        //switch x and y
        float y = 0.0f;// jc.CallStatic<float>("getGameRotationX");
        float x = 0.0f;//jc.CallStatic<float>("getGameRotationY");
        float z = 0.0f;//jc.CallStatic<float>("getGameRotationZ");
        float w = 0.0f;//jc.CallStatic<float>("getGameRotationW");
        return new Quaternion(x, -y, z, w);
    }
}
