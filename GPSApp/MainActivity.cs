using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.Content;

namespace GPSApp
{
    [Activity(Label = "GPSApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener
    {
        Location _currentLocation;
        LocationManager _locationManager;

        TextView _locationText;
        TextView _addressText;
        TextView _remarksText;
        string _locationProvider;
        const string _sourceAddress = "The Link, Cebu IT Park, Jose Maria del Mar St,Lahug, Cebu City, 6000 Cebu";

        GPSServiceBinder binder;
        GPSServiceConnection gpsServiceConnection;
        Intent gpsServiceIntent;
        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _addressText = FindViewById<TextView>(Resource.Id.txtAddress);
            _locationText = FindViewById<TextView>(Resource.Id.txtLocation);
            _remarksText = FindViewById<TextView>(Resource.Id.txtRemarks);

            //Initialising the LocationManager to provide access to the system location services.
            //The LocationManager class will listen for GPS updates from the device and notify the application by way of events. 
            _locationManager = (LocationManager)GetSystemService(LocationService);

            //Define a Criteria for the best location provider
            Criteria criteriaForLocationService = new Criteria {
                //A constant indicating an approximate accuracy
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };


            _locationProvider = _locationManager.GetBestProvider(criteriaForLocationService, true);


            //If using a Service
            //gpsServiceConnection = new GPSServiceConnection(binder);
            //gpsServiceIntent = new Intent(Android.App.Application.Context, typeof(GPSService));
            //BindService(gpsServiceIntent, gpsServiceConnection, Bind.AutoCreate);


            //IntentFilter filter = new IntentFilter(GPSServiceReciever.LOCATION_UPDATED);
            //GPSServiceReciever receiver = new GPSServiceReciever();
            //RegisterReceiver(receiver, filter);
           
        }

        //ILocationListener methods
        public void OnLocationChanged(Location location) {
            try {
                _currentLocation = location;

                if (_currentLocation == null)
                    _locationText.Text = "Unable to determine your location.";
                else {
                    _locationText.Text = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);

                    Geocoder geocoder = new Geocoder(this);

                    //The Geocoder class retrieves a list of address from Google over the internet
                    IList<Address> addressList = geocoder.GetFromLocation(_currentLocation.Latitude, _currentLocation.Longitude, 10);

                    Address addressCurrent = addressList.FirstOrDefault();

                    if (addressCurrent != null) {
                        StringBuilder deviceAddress = new StringBuilder();

                        for (int i = 0; i < addressCurrent.MaxAddressLineIndex; i++)
                            deviceAddress.Append(addressCurrent.GetAddressLine(i))
                                .AppendLine(",");

                        _addressText.Text = deviceAddress.ToString();
                    }
                    else
                        _addressText.Text = "Unable to determine the address.";

                    IList<Address> source = geocoder.GetFromLocationName(_sourceAddress, 1);
                    Address addressOrigin = source.FirstOrDefault();

                    var coord1 = new LatLng(addressOrigin.Latitude, addressOrigin.Longitude);
                    var coord2 = new LatLng(addressCurrent.Latitude, addressCurrent.Longitude);

                    var distance = Utils.HaversineDistance(coord1, coord2, Utils.DistanceUnit.Miles);

                    _remarksText.Text = string.Format("Your are {0} miles away from your original location.", distance);
                }
            }
            catch {
                _addressText.Text = "Unable to determine the address.";
            }
        }

        protected override void OnResume() {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        }

        protected override void OnPause() {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
   
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) {
        }

        public void OnProviderDisabled(string provider) {

        }

        public void OnProviderEnabled(string provider) {

        }

        

    }

    
}

