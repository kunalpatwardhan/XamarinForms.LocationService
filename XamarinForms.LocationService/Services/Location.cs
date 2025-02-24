﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinForms.LocationService.Messages;

namespace XamarinForms.LocationService.Services
{
    public class Location
    {
		readonly bool stopping = false;
		public Location()
		{
		}

		public async Task Run(CancellationToken token)
		{
			await Task.Run(async () => {
				while (!stopping)
				{
					token.ThrowIfCancellationRequested();
					try
					{
						await Task.Delay(60000);

						//var request = new GeolocationRequest(GeolocationAccuracy.Lowest);
						//var location = await Geolocation.GetLocationAsync(request);
						//if (location != null)
						{
							var message = new LocationMessage
							{
								//Latitude = location.Latitude,
								//Longitude = location.Longitude
								Latitude = DateTime.Now.Hour,
								Longitude = DateTime.Now.Minute
							};

							Device.BeginInvokeOnMainThread(() =>
							{
								MessagingCenter.Send<LocationMessage>(message, "Location");
							});
						}
					}
					catch (Exception ex)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							var errormessage = new LocationErrorMessage();
							MessagingCenter.Send<LocationErrorMessage>(errormessage, "LocationError");
						});
					}
				}
				return;
			}, token);
		}
	}
}
