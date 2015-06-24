using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;
using Android.Net.Wifi;

namespace GPSApp
{
    public static class Utils
    {
        public enum DistanceUnit { Miles, Kilometers };
        public static double ToRadian(this double value) {
            return (Math.PI / 180) * value;
        }

        public static double HaversineDistance(LatLng coord1, LatLng coord2, DistanceUnit unit) {
            double R = (unit == DistanceUnit.Miles) ? 3960 : 6371;
            var lat = (coord2.Latitude - coord1.Latitude).ToRadian();
            var lng = (coord2.Longitude - coord1.Longitude).ToRadian();

            var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                     Math.Cos(coord1.Latitude.ToRadian()) * Math.Cos(coord2.Latitude.ToRadian()) *
                     Math.Sin(lng / 2) * Math.Sin(lng / 2);

            var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

            return R * h2;
        }

        public static void TurnOffWifi(bool isEnable) {
            var connectivityManager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
            var mobileState = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi).GetState();
            if (mobileState == NetworkInfo.State.Connected) {
                var mawifi = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
                mawifi.SetWifiEnabled(false);
            }
        }
    }
}