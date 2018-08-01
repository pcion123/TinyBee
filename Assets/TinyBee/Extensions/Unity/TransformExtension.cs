namespace TinyBee
{
	using UnityEngine;

	public static class TransformExtension : object
	{
		private static Vector3 mLocalPos;
		private static Vector3 mScale;
		private static Vector3 mPos;

		public static Vector3 GetLocalPosition<T>(this T self) where T : Component
		{
			return self.transform.localPosition;
		}

		public static Quaternion GetLocalRotation<T>(this T self) where T : Component
		{
			return self.transform.localRotation;
		}

		public static Vector3 GetLocalScale<T>(this T self) where T : Component
		{
			return self.transform.localScale;
		}

		public static Vector3 GetPosition<T>(this T self) where T : Component
		{
			return self.transform.position;
		}

		public static Quaternion GetRotation<T>(this T self) where T : Component
		{
			return self.transform.rotation;
		}

		public static Vector3 GetScale<T>(this T self) where T : Component
		{
			return self.transform.lossyScale;
		}

		public static T Parent<T>(this T self, Transform parent) where T : Component
		{
			self.transform.SetParent(parent);
			return self;
		}
		public static T LocalIdentity<T>(this T self) where T : Component
		{
			self.transform.localPosition = Vector3.zero;
			self.transform.localRotation = Quaternion.identity;
			self.transform.localScale = Vector3.one;
			return self;
		}

		public static T LocalPosition<T>(this T self, Vector3 position) where T : Component
		{
			self.transform.localPosition = position;
			return self;
		}

		public static T LocalPosition<T>(this T self, float x, float y, float z) where T : Component
		{
			self.transform.localPosition = new Vector3(x, y, z);
			return self;
		}

		public static T LocalPosition<T>(this T self, float x, float y) where T : Component
		{
			mLocalPos = self.transform.localPosition;
			mLocalPos.x = x;
			mLocalPos.y = y;
			self.transform.localPosition = mLocalPos;
			return self;
		}

		public static T LocalPositionX<T>(this T self, float x) where T : Component
		{
			mLocalPos = self.transform.localPosition;
			mLocalPos.x = x;
			self.transform.localPosition = mLocalPos;
			return self;
		}

		public static T LocalPositionY<T>(this T self, float y) where T : Component
		{
			mLocalPos = self.transform.localPosition;
			mLocalPos.y = y;
			self.transform.localPosition = mLocalPos;
			return self;
		}

		public static T LocalPositionZ<T>(this T self, float z) where T : Component
		{
			mLocalPos = self.transform.localPosition;
			mLocalPos.z = z;
			self.transform.localPosition = mLocalPos;
			return self;
		}

		public static T LocalPositionIdentity<T>(this T self) where T : Component
		{
			self.transform.localPosition = Vector3.zero;
			return self;
		}

		public static T LocalRotation<T>(this T self, Quaternion rotation) where T : Component
		{
			self.transform.localRotation = rotation;
			return self;
		}

		public static T LocalRotationIdentity<T>(this T self) where T : Component
		{
			self.transform.localRotation = Quaternion.identity;
			return self;
		}

		public static T LocalScale<T>(this T self, Vector3 scale) where T : Component
		{
			self.transform.localScale = scale;
			return self;
		}

		public static T LocalScale<T>(this T self, float xyz) where T : Component
		{
			self.transform.localScale = Vector3.one * xyz;
			return self;
		}

		public static T LocalScale<T>(this T self, float x, float y, float z) where T : Component
		{
			mScale = self.transform.localScale;
			mScale.x = x;
			mScale.y = y;
			mScale.z = z;
			self.transform.localScale = mScale;
			return self;
		}

		public static T LocalScale<T>(this T self, float x, float y) where T : Component
		{
			mScale = self.transform.localScale;
			mScale.x = x;
			mScale.y = y;
			self.transform.localScale = mScale;
			return self;
		}

		public static T LocalScaleX<T>(this T self, float x) where T : Component
		{
			mScale = self.transform.localScale;
			mScale.x = x;
			self.transform.localScale = mScale;
			return self;
		}

		public static T LocalScaleY<T>(this T self, float y) where T : Component
		{
			mScale = self.transform.localScale;
			mScale.y = y;
			self.transform.localScale = mScale;
			return self;
		}

		public static T LocalScaleZ<T>(this T self, float z) where T : Component
		{
			mScale = self.transform.localScale;
			mScale.z = z;
			self.transform.localScale = mScale;
			return self;
		}

		public static T LocalScaleIdentity<T>(this T self) where T : Component
		{
			self.transform.localScale = Vector3.one;
			return self;
		}

		public static T Identity<T>(this T self) where T : Component
		{
			self.transform.position = Vector3.zero;
			self.transform.rotation = Quaternion.identity;
			self.transform.localScale = Vector3.one;
			return self;
		}

		public static T Position<T>(this T self, Vector3 position) where T : Component
		{
			self.transform.position = position;
			return self;
		}

		public static T Position<T>(this T self, float x, float y, float z) where T : Component
		{
			self.transform.position = new Vector3(x, y, z);
			return self;
		}

		public static T Position<T>(this T self, float x, float y) where T : Component
		{
			mPos = self.transform.position;
			mPos.x = x;
			mPos.y = y;
			self.transform.position = mPos;
			return self;
		}

		public static T PositionIdentity<T>(this T self) where T : Component
		{
			self.transform.position = Vector3.zero;
			return self;
		}

		public static T PositionX<T>(this T self, float x) where T : Component
		{
			mPos = self.transform.position;
			mPos.x = x;
			self.transform.position = mPos;
			return self;
		}

		public static T PositionX<T>(this T self, System.Func<float,float> xSetter) where T : Component
		{
			mPos = self.transform.position;
			mPos.x = xSetter(mPos.x);
			self.transform.position = mPos;
			return self;
		}

		public static T PositionY<T>(this T self, float y) where T : Component
		{
			mPos = self.transform.position;
			mPos.y = y;
			self.transform.position = mPos;
			return self;
		}

		public static T PositionY<T>(this T self, System.Func<float,float> ySetter) where T : Component
		{
			mPos = self.transform.position;
			mPos.y = ySetter(mPos.y);
			self.transform.position = mPos;
			return self;
		}

		public static T PositionZ<T>(this T self, float z) where T : Component
		{
			mPos = self.transform.position;
			mPos.z = z;
			self.transform.position = mPos;
			return self;
		}

		public static T PositionZ<T>(this T self, System.Func<float,float> zSetter) where T : Component
		{
			mPos = self.transform.position;
			mPos.z = zSetter(mPos.z);
			self.transform.position = mPos;
			return self;
		}

		public static T RotationIdentity<T>(this T self) where T : Component
		{
			self.transform.rotation = Quaternion.identity;
			return self;
		}

		public static T Rotation<T>(this T self, Quaternion rotation) where T : Component
		{
			self.transform.rotation = rotation;
			return self;
		}

		public static T DestroyAllChild<T>(this T self) where T : Component
		{
			var childCount = self.transform.childCount;

			for (var i = 0; i < childCount; i++)
				self.transform.GetChild(i).DestroyGameObjGracefully();

			return self;
		}

		public static Transform FindByPath(this Transform self, string path)
		{
			return self.Find(path.Replace(".", "/"));
		}

		public static Transform SeekTrans(this Transform self, string uniqueName)
		{
			var childTrans = self.Find(uniqueName);

			if (null != childTrans)
				return childTrans;

			foreach (Transform trans in self)
			{
				childTrans = trans.SeekTrans(uniqueName);

				if (null != childTrans)
					return childTrans;
			}

			return null;
		}

		public static T ShowChildTransByPath<T>(this T self, string transformPath) where T : Component
		{
			self.transform.Find(transformPath).gameObject.Show();
			return self;
		}

		public static T HideChildTransByPath<T>(this T self, string transformPath) where T : Component
		{
			self.transform.Find(transformPath).Hide();
			return self;
		}

		public static void CopyDataFromTransform(this Transform self, Transform from)
		{
			self.SetParent(from.parent);
			self.localPosition = from.localPosition;
			self.localRotation = from.localRotation;
			self.localScale = from.localScale;
		}

		public static void ActionRecursion(this Transform self, System.Action<Transform> action)
		{
			action(self);
			foreach (Transform child in self)
				child.ActionRecursion(action);
		}

		public static Transform FindChildRecursion(this Transform self, string name, System.StringComparison stringComparison = System.StringComparison.Ordinal)
		{
			if (self.name.Equals(name, stringComparison))
				return self;

			foreach (Transform child in self)
			{
				Transform final = null;
				final = child.FindChildRecursion(name, stringComparison);
				if (final)
					return final;
			}

			return null;
		}

		public static Transform FindChildRecursion(this Transform self, System.Func<Transform,bool> predicate)
		{
			if (predicate(self))
				return self;

			foreach (Transform child in self)
			{
				Transform final = null;
				final = child.FindChildRecursion(predicate);
				if (final)
					return final;
			}

			return null;
		}

		public static string GetPath(this Transform self)
		{
			var sb = new System.Text.StringBuilder();
			var t = self;
			while (true)
			{
				sb.Insert(0, t.name);
				t = t.parent;
				if (t)
				{
					sb.Insert(0, "/");
				}
				else
				{
					return sb.ToString();
				}
			}
		}
	}
}