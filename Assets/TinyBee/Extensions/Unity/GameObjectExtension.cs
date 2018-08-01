namespace TinyBee
{
	using UnityEngine;

	public static class GameObjectExtension : object
	{
		public static GameObject Show(this GameObject self)
		{
			self.SetActive(true);
			return self;
		}

		public static T Show<T>(this T self) where T : Component
		{
			self.gameObject.Show();
			return self;
		}

		public static GameObject Hide(this GameObject self)
		{
			self.SetActive(false);
			return self;
		}

		public static T Hide<T>(this T self) where T : Component
		{
			self.gameObject.Hide();
			return self;
		}

		public static GameObject DestroyAllChild(this GameObject self)
		{
			var childCount = self.transform.childCount;

			for (var i = 0; i < childCount; i++)
				self.transform.GetChild(i).DestroyGameObjGracefully();

			return self;
		}

		public static void DestroyGameObj<T>(this T self) where T : Component
		{
			self.gameObject.DestroySelf();
		}

		public static void DestroyGameObjGracefully<T>(this T self) where T : Component
		{
			if (self && self.gameObject)
			{
				self.gameObject.DestroySelfGracefully();
			}
		}

		public static T DestroyGameObjAfterDelay<T>(this T self, float delay) where T : Component
		{
			self.gameObject.DestroySelfAfterDelay(delay);
			return self;
		}

		public static T DestroyGameObjAfterDelayGracefully<T>(this T self, float delay) where T : Component
		{
			if (self && self.gameObject)
			{
				self.gameObject.DestroySelfAfterDelay(delay);
			}
			return self;
		}

		public static GameObject Layer(this GameObject self, int layer)
		{
			self.layer = layer;
			return self;
		}

		public static T Layer<T>(this T self, int layer) where T : Component
		{
			self.gameObject.layer = layer;
			return self;
		}

		public static GameObject Layer(this GameObject self, string layerName)
		{
			self.layer = LayerMask.NameToLayer(layerName);
			return self;
		}

		public static T Layer<T>(this T self, string layerName) where T : Component
		{
			self.gameObject.layer = LayerMask.NameToLayer(layerName);
			return self;
		}
	}
}