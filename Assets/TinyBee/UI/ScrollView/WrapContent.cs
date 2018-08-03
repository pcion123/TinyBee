namespace TinyBee.UI
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class WrapContent : UIWrapContent
	{
		public bool isRepeat = true;
		public GameObject Direction1 = null;
		public GameObject Direction2 = null;
		public Action OnMoving = null;

		// Use this for initialization
		protected override void Start()
		{
			SortAlphabetically();
			if (mScroll != null)
				mScroll.GetComponent<UIPanel>().onClipMove = OnMove;
			mFirstTime = false;
		}

		//事件 移動
		protected override void OnMove(UIPanel panel)
		{ 
			FixWrapContent();

			if (OnMoving != null)
				OnMoving();
		}

		//修正版
		public void FixWrapContent()
		{
			float extents = itemSize * mChildren.Count * 0.5f;
			Vector3[] corners = mPanel.worldCorners;

			for (int i = 0; i < 4; ++i)
			{
				Vector3 v = corners[i];
				v = mTrans.InverseTransformPoint(v);
				corners[i] = v;
			}

			Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);
			bool allWithinRange = true;
			float ext2 = extents * 2f;

			if (mHorizontal)
			{
				float min = corners[0].x - itemSize;
				float max = corners[2].x + itemSize;

				for (int i = 0, imax = mChildren.Count; i < imax; ++i)
				{
					Transform t = mChildren[i];
					float distance = t.localPosition.x - center.x;

					if (distance < -extents)
					{
						Vector3 pos = t.localPosition;
						pos.x += ext2;
						distance = pos.x - center.x;
						int realIndex = Mathf.RoundToInt(pos.x / itemSize);

						if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
						{
							t.localPosition = pos;
							UpdateItem(t, i);
						}
						else allWithinRange = false;
					}
					else if (distance > extents)
					{
						Vector3 pos = t.localPosition;
						pos.x -= ext2;
						distance = pos.x - center.x;
						int realIndex = Mathf.RoundToInt(pos.x / itemSize);

						if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
						{
							t.localPosition = pos;
							UpdateItem(t, i);
						}
						else allWithinRange = false;
					}
					else if (mFirstTime) UpdateItem(t, i);

					if (cullContent)
					{
						distance += mPanel.clipOffset.x - mTrans.localPosition.x;
						if (!UICamera.IsPressed(t.gameObject))
							NGUITools.SetActive(t.gameObject, (distance > min && distance < max), false);
					}
				}
			}
			else
			{
				float min = corners[0].y - itemSize;
				float max = corners[2].y + itemSize;

				for (int i = 0, imax = mChildren.Count; i < imax; ++i)
				{
					Transform t = mChildren[i];
					float distance = t.localPosition.y - center.y;

					if (distance < -extents)
					{
						Vector3 pos = t.localPosition;
						pos.y += ext2;
						distance = pos.y - center.y;
						int realIndex = Mathf.RoundToInt(-pos.y / itemSize);

						if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
						{
							t.localPosition = pos;
							UpdateItem(t, i);
						}
						else 
						{
							allWithinRange = false;

							if (Direction1 != null)
								Direction1.SetActive(false);
						}
					}
					else if (distance > extents)
					{
						Vector3 pos = t.localPosition;
						pos.y -= ext2;
						distance = pos.y - center.y;
						int realIndex = Mathf.RoundToInt(-pos.y / itemSize);

						if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
						{
							t.localPosition = pos;
							UpdateItem(t, i);
						}
						else 
						{
							allWithinRange = false;

							if (Direction2 != null)
								Direction2.SetActive(false);
						}
					}
					else if (mFirstTime) UpdateItem(t, i);

					if (cullContent)
					{
						distance += mPanel.clipOffset.y - mTrans.localPosition.y;
						if (!UICamera.IsPressed(t.gameObject))
							NGUITools.SetActive(t.gameObject, (distance > min && distance < max), false);
					}
				}
			}
			mScroll.restrictWithinPanel = !allWithinRange;

			if (allWithinRange == true)
			{
				if (Direction1 != null)
					Direction1.SetActive(true);

				if (Direction2 != null)
					Direction2.SetActive(true);
			}
		}

		//更新Item資料
		protected override void UpdateItem(Transform item, int index)
		{
			if (onInitializeItem != null)
			{
				int realIndex = (mScroll.movement == UIScrollView.Movement.Vertical) ? Mathf.RoundToInt(-item.localPosition.y / itemSize) :	Mathf.RoundToInt(item.localPosition.x / itemSize);
				onInitializeItem(item.gameObject, index, realIndex);
			}
		}
	}
}