using System;
using System.Linq;
using Android;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using AndroidResource = Android.Resource;

namespace CommunicationHelper.App
{
    public static class PermissionUtils
    {
        public const Int32 RC_LOCATION_PERMISSIONS = 1000;
            
        public static readonly String[] LocationPermissions =
            {Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation};

        public static void RequestPermissionsForApp(this AppCompatActivity fragment)
        {
            var showRequestRationale =
                ActivityCompat.ShouldShowRequestPermissionRationale(fragment,
                    Manifest.Permission.AccessFineLocation) ||
                ActivityCompat.ShouldShowRequestPermissionRationale(fragment,
                    Manifest.Permission.AccessCoarseLocation);

            if (showRequestRationale)
            {
                var rootView = fragment.FindViewById(AndroidResource.Id.Content);
                Snackbar.Make(rootView, Resource.String.request_location_permissions, Snackbar.LengthIndefinite)
                    .SetAction(Resource.String.ok,
                        v => { fragment.RequestPermissions(LocationPermissions, RC_LOCATION_PERMISSIONS); })
                    .Show();
            }
            else
            {
                fragment.RequestPermissions(LocationPermissions, RC_LOCATION_PERMISSIONS);
            }
        }

        public static bool AllPermissionsGranted(this Android.Content.PM.Permission[] grantResults)
        {
            if (grantResults.Length < 1)
            {
                return false;
            }

            return !grantResults.Any(result => result == Android.Content.PM.Permission.Denied);
        }

        public static bool HasLocationPermissions(this Context context)
        {
            foreach (var perm in LocationPermissions)
            {
                if (ContextCompat.CheckSelfPermission(context, perm) != Android.Content.PM.Permission.Granted)
                {
                    return false;
                }
            }
            return true;
        }
    }
}