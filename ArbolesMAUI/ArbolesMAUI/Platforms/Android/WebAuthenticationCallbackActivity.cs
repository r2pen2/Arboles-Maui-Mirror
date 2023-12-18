﻿using Android.App;
using Android.Content.PM;
using Android.Webkit;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Android.Content.Intent.ActionView },
              Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
              DataScheme = "ArbolesMAUI")]
public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity {
}