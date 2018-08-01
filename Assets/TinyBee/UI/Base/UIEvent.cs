namespace TinyBee.UI
{
    using System;
    using UnityEngine;

    public enum eEventParam
    {
        None = 0,
        Click = 1,
        Double = 2,
        Press = 4,
        Slide = 8,
        Drag = 16
    }

    public class UIEvent : UIEventListener
    {
        protected eEventParam mEventType;
        protected object mParameter;

        public eEventParam EventType { get { return mEventType; } set { mEventType = value; } }
        public object Parameter { get { return mParameter; } set { mParameter = value; } }

        public static void Register(GameObject target, eEventParam eventType, object parameter, params object[] events)
        {
            if (target == null)
                return;

            if (eEventParam.None == eventType)
                return;

            if (events == null || events.Length <= 0)
                return;

            if ((eventType & eEventParam.Press) == eEventParam.Press)
            {
                UIPress press = target.GetComponent<UIPress>();
                if (press == null)
                    press = target.AddComponent<UIPress>();

                press.EventType = eventType;
                press.Parameter = parameter;
                press.onClick = (Action<object>)events[0];
                press.onShow = (Action<object>)events[1];
                press.onHide = (Action<object>)events[2];
                press.onCheck = (Action<object>)events[3];
            }
            else if ((eventType & eEventParam.Click) == eEventParam.Click)
            {
                UIClick click = target.GetComponent<UIClick>();
                if (click == null)
                    click = target.AddComponent<UIClick>();

                click.EventType = eventType;
                click.Parameter = parameter;
                click.onClick = (Action<object>)events[0];
            }
            else if ((eventType & eEventParam.Double) == eEventParam.Double)
            {
                UIDoubleClick click = target.GetComponent<UIDoubleClick>();
                if (click == null)
                    click = target.AddComponent<UIDoubleClick>();

                click.EventType = eventType;
                click.Parameter = parameter;
                click.onDoubleClick = (Action<object>)events[0];
            }
            else if ((eventType & eEventParam.Slide) == eEventParam.Slide)
            {
                UISlider slider = target.GetComponent<UISlider>();
                if (slider == null)
                    slider = target.AddComponent<UISlider>();

                slider.EventType = eventType;
                slider.Parameter = parameter;
                slider.onSlider = (Action<object>)events[0];
                slider.Direction = (eDirection)events[1];
                slider.Amount = (int)events[2];
            }
            else if ((eventType & eEventParam.Drag) == eEventParam.Drag)
            {
				//TODO:未實作
            }
        }
    }
}
