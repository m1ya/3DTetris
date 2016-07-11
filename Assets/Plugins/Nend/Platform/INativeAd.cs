namespace NendUnityPlugin.Platform
{
	internal interface INativeAd
	{
		string ShortText { 
			get;
		}

		string LongText { 
			get; 
		}

		string PromotionUrl { 
			get;
		}

		string PromotionName {
			get;
		}

		string ActionButtonText {
			get;
		}

		string AdImageUrl {
			get;
		}

		int AdImageWidth {
			get;
		}

		int AdImageHeight {
			get;
		}

		string LogoImageUrl {
			get;
		}

		int LogoImageWidth {
			get;
		}

		int LogoImageHeight {
			get;
		}

		void SendImpression ();

		void PerformAdClick ();

		void PerformInformationClick ();

		string GetAdvertisingExplicitlyText (int advertisingExplicitly);
	}
}