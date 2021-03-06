﻿using System;
using System.Linq;
using Android;
using Android.Content;
using Android.Content.PM;
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

        public static void RequestPermissionsForApp(this AppCompatActivity activity)
        {
            var showRequestRationale =
                ActivityCompat.ShouldShowRequestPermissionRationale(activity,
                    Manifest.Permission.AccessFineLocation) ||
                ActivityCompat.ShouldShowRequestPermissionRationale(activity,
                    Manifest.Permission.AccessCoarseLocation);

            if (showRequestRationale)
            {
                var rootView = activity.FindViewById(AndroidResource.Id.Content);
                Snackbar.Make(rootView, Resource.String.request_location_permissions, Snackbar.LengthIndefinite)
                    .SetAction(Resource.String.ok,
                        v => { activity.RequestPermissions(LocationPermissions, RC_LOCATION_PERMISSIONS); })
                    .Show();
            }
            else
            {
                activity.RequestPermissions(LocationPermissions, RC_LOCATION_PERMISSIONS);
            }
        }

        public static Boolean AllPermissionsGranted(this Permission[] grantResults)
        {
            return grantResults.Length >= 1 && grantResults.All(result => result != Permission.Denied);
        }

        public static Boolean HasLocationPermissions(this Context context)
        {
            return LocationPermissions.All(perm =>
                ContextCompat.CheckSelfPermission(context, perm) == Permission.Granted);
        }
    }
}